import type { Operation, SdkResponse, Values } from './types';

export class ApiError extends Error {
  constructor(public status: number, public detail: any) {
    super(detail?.message || detail?.title || `HTTP ${status}`);
  }
}

export async function request<T = any>(path: string, method = 'GET', body?: unknown, signal?: AbortSignal): Promise<T> {
  let response: Response;
  try {
    response = await fetch(path, { method, headers: body === undefined ? {} : { 'Content-Type': 'application/json' }, body: body === undefined ? undefined : JSON.stringify(body), signal });
  } catch (error) {
    if (error instanceof DOMException && error.name === 'AbortError') throw error;
    throw new ApiError(0, { message: 'Không kết nối được backend demo. Kiểm tra backend ở cổng 5080.' });
  }
  const text = await response.text();
  let data: any;
  try { data = text ? JSON.parse(text) : null; } catch { data = { message: text || response.statusText }; }
  if (!response.ok) throw new ApiError(response.status, data);
  return data as T;
}

export function operationPath(op: Operation, path: Values, query: Values): string {
  let url = '/api/pos/' + op.path.replace(/\{(\w+)\}/g, (_, key: string) => encodeURIComponent(String(path[key] ?? '')));
  const params = new URLSearchParams();
  for (const [key, value] of Object.entries(query)) {
    if (value === undefined || value === null || value === '') continue;
    if (Array.isArray(value)) value.forEach(v => params.append(key, String(v)));
    else params.set(key, String(value));
  }
  if (params.size) url += '?' + params;
  return url;
}

export function runOperation(op: Operation, path: Values, query: Values, body: unknown): Promise<SdkResponse> {
  return request(operationPath(op, path, query), op.method, op.method === 'GET' ? undefined : body ?? {});
}

export async function allPages(path: string): Promise<any[]> {
  const rows: any[] = [];
  for (let page = 1; page <= 1000; page++) {
    const separator = path.includes('?') ? '&' : '?';
    const response = await request<SdkResponse>(`${path}${separator}pageNumber=${page}&pageSize=100`);
    if (Array.isArray(response.data)) return response.data;
    const items = response.data?.items || [];
    rows.push(...items);
    if (!response.data?.metaData?.hasNext || items.length === 0) return rows;
  }
  throw new Error('Danh mục quá lớn để tải vào bộ chọn. Hãy dùng tìm kiếm danh sách hoặc nhập ID trực tiếp.');
}

export function recordId(row: any): string { return String(row?.id ?? row?.productId ?? row?.orderId ?? row?.saleChannelId ?? ''); }
export function recordName(row: any): string { return String(row?.name ?? row?.title ?? row?.customerName ?? row?.rankName ?? row?.code ?? row?.productCode ?? row?.saleChannelId ?? recordId(row)); }
export function niceDate(value?: string): string { return value ? new Date(value).toLocaleString('vi-VN') : '—'; }
export function localNow(): string { return new Date(Date.now() + 7 * 3600_000).toISOString().slice(0, 19); }
export function errorMessage(error: unknown): string { return error instanceof Error ? error.message : 'Có lỗi khi xử lý.'; }
