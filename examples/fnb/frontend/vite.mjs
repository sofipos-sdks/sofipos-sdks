// Disable terminal color codes before Vite loads its logger.
process.env.NO_COLOR = '1';
delete process.env.FORCE_COLOR;

await import('./node_modules/vite/bin/vite.js');
