import { execFileSync } from 'node:child_process';
import { existsSync, readFileSync, statSync } from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

const root = fileURLToPath(new URL('../', import.meta.url));
const files = [...new Set(execFileSync('git', [
  'ls-files', '--cached', '--others', '--exclude-standard', '-z',
], { cwd: root, encoding: 'utf8' }).split('\0').filter(Boolean))];
const textFile = /\.(?:cs|csproj|sln|ts|tsx|js|mjs|json|md|ya?ml|css|svg|html|ps1)$|(?:^|\/)(?:LICENSE|\.editorconfig|\.gitattributes|\.gitignore|\.env\.example)$/;
const decoder = new TextDecoder('utf-8', { fatal: true });
const problems = [];
let checked = 0;

function checkPath(file, target, kind) {
  const resolved = path.resolve(root, path.dirname(file), target.replaceAll('\\', '/'));
  const relative = path.relative(root, resolved);
  if (relative === '..' || relative.startsWith('..' + path.sep) || path.isAbsolute(relative)) {
    problems.push(`${file}: ${kind} nằm ngoài repository: ${target}`);
  } else if (!existsSync(resolved)) {
    problems.push(`${file}: không tìm thấy ${kind}: ${target}`);
  }
}

for (const file of files) {
  const absolute = path.join(root, file);
  if (!textFile.test(file) || !existsSync(absolute) || !statSync(absolute).isFile()) continue;
  checked++;
  const bytes = readFileSync(absolute);
  if (bytes[0] === 0xef && bytes[1] === 0xbb && bytes[2] === 0xbf) {
    problems.push(`${file}: dùng UTF-8 không BOM`);
  }

  let content;
  try {
    content = decoder.decode(bytes);
  } catch {
    problems.push(`${file}: nội dung không phải UTF-8 hợp lệ`);
    continue;
  }

  if (file.endsWith('.json')) {
    try {
      JSON.parse(content);
    } catch (error) {
      problems.push(`${file}: JSON không hợp lệ: ${error.message}`);
    }
  }

  if (file.endsWith('.md')) {
    const markdown = content.replace(/^```[^\n]*\n[\s\S]*?^```\s*$/gm, '');
    for (const match of markdown.matchAll(/!?\[[^\]]*\]\((<[^>]+>|[^\s)]+)(?:\s+"[^"]*")?\)/g)) {
      const destination = match[1].replace(/^<|>$/g, '');
      if (/^[a-z][a-z0-9+.-]*:/i.test(destination) || destination.startsWith('#')) continue;
      try {
        const target = decodeURIComponent(destination.split(/[?#]/)[0]);
        if (target) checkPath(file, target, 'liên kết');
      } catch {
        problems.push(`${file}: liên kết có encoding không hợp lệ`);
      }
    }
  }

  if (file.endsWith('.csproj')) {
    for (const match of content.matchAll(/<ProjectReference\b[^>]*\bInclude=["']([^"']+)["']/g)) {
      if (!match[1].includes('$(')) checkPath(file, match[1], 'ProjectReference');
    }
  }

  if (file.endsWith('.sln')) {
    for (const match of content.matchAll(/^Project\([^\n]+?=\s*"[^"]+",\s*"([^"]+\.csproj)"/gm)) {
      checkPath(file, match[1], 'project trong solution');
    }
  }
}

if (problems.length) {
  for (const problem of problems) console.error(problem);
  process.exitCode = 1;
} else {
  console.log(`Đã kiểm tra ${checked} file: UTF-8, JSON, liên kết nội bộ và tham chiếu project hợp lệ.`);
}
