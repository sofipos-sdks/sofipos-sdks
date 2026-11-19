import { useEffect, useState } from 'react';
import { ArrowUpRight, CheckCircle2, ChevronLeft, ChevronRight, Code2, Eye, FlaskConical, Plus, RefreshCw, Webhook, X } from 'lucide-react';
import { errorMessage, niceDate, request } from './api';
import { useDemo } from './context';
import { ObjectEditor } from './Form';
import { EmptyState, JsonView } from './Workspace';
import type { Field } from './types';

function Drawer({ title, children, onClose }: { title: string; children: React.ReactNode; onClose: () => void }) {
  return <div className="drawer-overlay" onMouseDown={e => { if (e.target === e.currentTarget) onClose(); }}><section className="drawer"><div className="panel-heading"><h2>{title}</h2><button className="icon-btn" onClick={onClose}><X size={18} /></button></div><div className="drawer-body">{children}</div></section></div>;
}
function Pager({ page, total, onPage }: { page: number; total: number; onPage: (value: number) => void }) {
  return <div className="pagination"><span>{total} bản ghi · Trang {page}/{Math.max(1, Math.ceil(total / 20))}</span><div><button className="btn btn-small" disabled={page <= 1} onClick={() => onPage(page - 1)}><ChevronLeft size={15} />Trước</button><button className="btn btn-small" disabled={page * 20 >= total} onClick={() => onPage(page + 1)}>Sau<ChevronRight size={15} /></button></div></div>;
}
function useRefresh(url: string) {
  const [data, setData] = useState<any>({ items: [], total: 0 });
  const [error, setError] = useState('');
  const [revision, setRevision] = useState(0);
  useEffect(() => {
    const controller = new AbortController();
    const load = () => request(url, 'GET', undefined, controller.signal).then(result => { if (!controller.signal.aborted) { setData(result); setError(''); } }).catch(e => { if (!controller.signal.aborted) setError(errorMessage(e)); });
    void load(); const timer = setInterval(() => void load(), 4000);
    return () => { controller.abort(); clearInterval(timer); };
  }, [url, revision]);
  return { data, error, refresh: () => setRevision(n => n + 1) };
}

export function HistoryScreen() {
  const [operation, setOperation] = useState('');
  const [errorsOnly, setErrorsOnly] = useState(false);
  const [page, setPage] = useState(1);
  const [selected, setSelected] = useState<any>();
  const callId = new URLSearchParams(location.hash.split('?')[1] || '').get('callId') || '';
  const { data, error, refresh } = useRefresh(`/api/calls?operation=${encodeURIComponent(operation)}&errorsOnly=${errorsOnly}&page=${page}&callId=${encodeURIComponent(callId)}`);
  return <>
    <div className="page-heading"><div><div className="eyebrow">TRACE / SDK</div><h1>Lịch sử gọi SDK</h1><p>Tối đa 200 HTTP request gần nhất trong RAM, gồm các lần GET retry. Key và secret được che.</p></div><button className="btn" onClick={refresh}><RefreshCw size={16} />Làm mới</button></div>
    {callId && <div className="notice subtle">Đang xem trace của thao tác đã chọn. <a href="#/history">Xem tất cả</a></div>}
    {error && <div className="notice error">{error}</div>}
    <section className="panel"><div className="filter-bar"><input aria-label="Tìm thao tác SDK" placeholder="Tìm tên hàm SDK…" value={operation} onChange={e => { setOperation(e.target.value); setPage(1); }} /><label className="check-label"><input type="checkbox" checked={errorsOnly} onChange={e => { setErrorsOnly(e.target.checked); setPage(1); }} />Chỉ có lỗi</label></div>
      {data.items.length ? <div className="table-scroll"><table><thead><tr><th>Thao tác</th><th>Nguồn</th><th>HTTP</th><th>Trạng thái</th><th>Thời gian</th><th>Lúc gọi</th><th /></tr></thead><tbody>{data.items.map((call: any) => <tr key={call.id}><td className="mono compact">{call.operation}</td><td>{call.source}</td><td>{call.method}</td><td>{call.statusCode || 'Lỗi mạng'}</td><td>{call.durationMs} ms</td><td>{niceDate(call.createdAt)}</td><td><button className="icon-btn" title="Xem request / response" onClick={() => setSelected(call)}><Eye size={16} /></button></td></tr>)}</tbody></table></div> : <EmptyState title="Chưa có trace phù hợp" description="Gửi API hoặc mở bộ chọn để xem request thực tế từ SDK." />}
      <Pager page={page} total={data.total} onPage={setPage} />
    </section>
    {selected && <Drawer title={selected.operation} onClose={() => setSelected(undefined)}><div className="endpoint-bar"><span>{selected.method}</span><code>{selected.url}</code></div><div className="key-value"><span>Call ID</span><code>{selected.callId}</code></div><div className="key-value"><span>traceId</span><code>{selected.traceId || '—'}</code></div>{selected.error && <div className="notice error">{selected.error}</div>}<JsonView value={selected.headers} label="Header gửi đi · thông tin xác thực đã che" />{selected.requestJson && <JsonView value={selected.requestJson} label="Request body" />}<JsonView value={selected.responseJson} label="Response body" /></Drawer>}
  </>;
}

const events = ['ping', 'order.created', 'order.updated', 'product.changed', 'category.changed', 'combo.changed', 'flash_sale.changed', 'pricing_policy.changed', 'customer.changed', 'voucher.changed', 'order_adjustment_program.changed', 'sale_channel.changed', 'qr_order_code.changed', 'area_table.changed', 'membership_rank.changed'];
const eventGroups: Record<string, string> = { 'product.changed': 'Products', 'combo.changed': 'Combos', 'flash_sale.changed': 'FlashSales', 'pricing_policy.changed': 'PricingPolicies', 'customer.changed': 'Customers', 'voucher.changed': 'Vouchers', 'order_adjustment_program.changed': 'OrderAdjustmentPrograms', 'sale_channel.changed': 'SaleChannels', 'qr_order_code.changed': 'QrOrderCodes' };
export function WebhooksScreen() {
  const { connection, branchId, notify, operations } = useDemo();
  const [state, setState] = useState(''); const [page, setPage] = useState(1);
  const [selected, setSelected] = useState<any>();
  const [form, setForm] = useState<any>({ event: 'ping', signatureMode: 'valid', eventId: crypto.randomUUID().replace(/-/g, ''), branchId, ids: [], deletedIds: [] });
  const [busy, setBusy] = useState(false); const [error, setError] = useState('');
  const { data, error: loadError, refresh } = useRefresh(`/api/webhooks?state=${state}&page=${page}`);
  const operation = operations.find(o => o.group === (form.event.startsWith('order.') ? 'FnbOrders' : eventGroups[form.event] || 'Lookups'));
  const fields: Field[] = form.event === 'ping' ? [] : form.event.startsWith('order.') ? [
    { name: 'branchId', label: 'Chi nhánh', kind: 'guid', type: 'Guid' }, { name: 'orderId', label: 'Đơn hàng', kind: 'guid', type: 'Guid' }, { name: 'code', label: 'Mã đơn', kind: 'string', type: 'string', optional: true },
  ] : [{ name: 'ids', label: 'Bản ghi tạo / sửa', kind: 'array', type: 'List<string>', item: { name: '', label: 'Bản ghi', kind: 'string', type: 'string' } }, { name: 'deletedIds', label: 'ID bản ghi đã xóa · tùy chọn', kind: 'array', type: 'List<string>', item: { name: '', label: 'ID đã xóa', kind: 'string', type: 'string' } }];
  async function simulate() {
    setBusy(true); setError('');
    try { const result = await request('/api/webhooks/simulate', 'POST', form); notify(result.message); }
    catch (e) { setError(errorMessage(e)); }
    finally { setBusy(false); refresh(); }
  }
  return <>
    <div className="page-heading"><div><div className="eyebrow">TRACE / WEBHOOK</div><h1>Webhook inbox</h1><p>Xác minh bằng SDK và giữ tối đa 200 sự kiện trong RAM. Nhận webhook không gọi lại POS.</p></div><button className="btn" onClick={refresh}><RefreshCw size={16} />Làm mới</button></div>
    <section className="panel"><div className="panel-heading"><h2><FlaskConical size={18} />Mô phỏng webhook bằng SDK</h2><code>POST /webhooks/sofipos</code></div>
      <form onSubmit={e => { e.preventDefault(); void simulate(); }}><div className="panel-body form-grid">
        <div className="field"><label>Sự kiện</label><select value={form.event} onChange={e => setForm({ event: e.target.value, signatureMode: form.signatureMode, eventId: crypto.randomUUID().replace(/-/g, ''), branchId, ids: [], deletedIds: [] })}>{events.map(event => <option key={event}>{event}</option>)}</select></div>
        <div className="field"><label>Chế độ chữ ký</label><select value={form.signatureMode} onChange={e => setForm({ ...form, signatureMode: e.target.value })}><option value="valid">Hợp lệ</option><option value="invalid">Sai chữ ký</option><option value="expired">Timestamp cũ 10 phút</option></select></div>
        <div className="field wide"><label>Event ID · giữ nguyên để thử gửi trùng</label><div className="input-actions"><input value={form.eventId} onChange={e => setForm({ ...form, eventId: e.target.value })} /><button type="button" className="btn" onClick={() => setForm({ ...form, eventId: crypto.randomUUID().replace(/-/g, '') })}><Plus size={16} />ID mới</button></div></div>
        <div className="wide"><ObjectEditor fields={fields} value={form} scope={form} operation={operation} onChange={setForm} disabled={busy} /></div>
      </div>{!connection.hasWebhookSecret && <div className="notice warning">Nhập secret tại trang Kết nối POS để nhận hoặc mô phỏng webhook.</div>}{error && <div className="notice error">{error}</div>}<div className="form-footer"><span className="muted">Chỉ xác minh và ghi nhận sự kiện trong phiên; không đồng bộ dữ liệu.</span><button className="btn btn-primary" disabled={busy || !connection.hasWebhookSecret}><Webhook size={16} />{busy ? 'Đang xử lý…' : 'Mô phỏng webhook'}</button></div></form>
    </section>
    {loadError && <div className="notice error">{loadError}</div>}
    <section className="panel"><div className="filter-bar"><h2>Sự kiện đã nhận</h2><select value={state} onChange={e => { setState(e.target.value); setPage(1); }}><option value="">Mọi trạng thái</option><option value="received">Đã nhận</option><option value="rejected">Bị từ chối</option></select></div>
      {data.items.length ? <div className="table-scroll"><table><thead><tr><th>Sự kiện</th><th>Event ID</th><th>Chữ ký</th><th>Trạng thái</th><th>Số lần nhận</th><th>Lúc nhận</th><th /></tr></thead><tbody>{data.items.map((row: any) => <tr key={row.id}><td>{row.event}</td><td className="mono compact">{row.eventId}</td><td>{row.signatureStatus === 'valid' ? 'Hợp lệ' : 'Không hợp lệ'}</td><td>{row.state === 'received' ? 'Đã nhận' : 'Từ chối'}</td><td>{row.receivedCount}</td><td>{niceDate(row.createdAt)}</td><td><button className="icon-btn" onClick={() => setSelected(row)}><Eye size={16} /></button></td></tr>)}</tbody></table></div> : <EmptyState title="Chưa có webhook trong phiên" description="Gửi từ POS tới callback hoặc dùng form mô phỏng bên trên." />}
      <Pager page={page} total={data.total} onPage={setPage} />
    </section>
    {selected && <Drawer title={selected.event || 'Webhook bị từ chối'} onClose={() => setSelected(undefined)}><div className="key-value"><span>Nguồn</span><span>{selected.source === 'simulated' ? 'Mô phỏng' : 'POS'}</span></div>{selected.error && <div className="notice error">{selected.error}</div>}<JsonView value={selected.headers} label="Header nhận được" /><JsonView value={selected.payloadJson} label="Body nhận được" /></Drawer>}
  </>;
}

export function CoverageScreen() {
  const { operations, openOperation } = useDemo();
  const [term, setTerm] = useState('');
  const rows = operations.filter(op => (op.id + op.label + op.groupLabel + op.path).toLocaleLowerCase('vi-VN').includes(term.toLocaleLowerCase('vi-VN')));
  return <><div className="page-heading"><div><div className="eyebrow">SDK / API</div><h1>Độ phủ chức năng</h1><p>{operations.length} thao tác có form và controller gọi SDK trực tiếp.</p></div><CheckCircle2 size={24} /></div><section className="panel"><div className="filter-bar"><input placeholder="Tìm thao tác API…" value={term} onChange={e => setTerm(e.target.value)} /><span>{rows.length} thao tác</span></div><div className="table-scroll"><table><thead><tr><th>Hàm SDK</th><th>HTTP</th><th>Đường dẫn POS</th><th>Form</th></tr></thead><tbody>{rows.map(op => <tr key={op.id}><td className="mono compact">{op.id}</td><td>{op.method}</td><td className="mono compact">/{op.path}</td><td><button className="text-link" onClick={() => openOperation(op.id)}>{op.groupLabel} · {op.label}<ArrowUpRight size={14} /></button></td></tr>)}</tbody></table></div></section></>;
}
