import { useEffect, useRef, useState } from 'react';
import { ArrowRight, ArrowUpRight, CheckCircle2, ChevronLeft, ChevronRight, Clipboard, Code2, Copy, Download, Eye, FilePenLine, Info, LoaderCircle, Play, Plus, RefreshCw, RotateCcw, ScanQrCode, ShoppingBag, Trash2, XCircle } from 'lucide-react';
import { ApiError, errorMessage, niceDate, operationPath, recordId, recordName, runOperation } from './api';
import { useDemo, type Seed } from './context';
import { defaultObject, defaultValue, FieldEditor, ObjectEditor, validateFields } from './Form';
import { schemas } from './reference';
import { getLookupData } from './Lookup';
import type { Field, Operation, SdkResponse, Values } from './types';

const money = (v: unknown) => Number(v || 0).toLocaleString('vi-VN') + ' đ';
const columns: Record<string, [string, string][]> = {
  Products: [['productCode', 'Mã sản phẩm'], ['name', 'Tên sản phẩm'], ['price', 'Giá bán'], ['unit', 'Đơn vị'], ['updatedAt', 'Cập nhật']],
  Customers: [['customerName', 'Khách hàng'], ['customerPhone', 'Điện thoại'], ['customerAddress', 'Địa chỉ'], ['currentPoint', 'Điểm']],
  FnbOrders: [['code', 'Mã đơn'], ['createDate', 'Ngày tạo'], ['customerName', 'Khách hàng'], ['totalPrice', 'Tổng tiền'], ['isPaid', 'Đã thanh toán']],
  SaleChannels: [['id', 'Kênh'], ['name', 'Tên kênh'], ['priority', 'Ưu tiên'], ['isActive', 'Trạng thái']],
  QrOrderCodes: [['title', 'Tên QR'], ['saleChannelId', 'Kênh'], ['isActive', 'Trạng thái'], ['updateDate', 'Cập nhật']],
};

export function JsonView({ value, label = 'Dữ liệu JSON' }: { value: any; label?: string }) {
  const { notify } = useDemo();
  const text = typeof value === 'string' ? (() => { try { return JSON.stringify(JSON.parse(value), null, 2); } catch { return value; } })() : JSON.stringify(value, null, 2) ?? 'null';
  return <div className="json-view"><div className="json-heading"><span><Code2 size={14} />{label}</span><button className="icon-btn" title="Sao chép" onClick={() => void navigator.clipboard.writeText(text).then(() => notify('Đã sao chép JSON.')).catch(() => notify('Không sao chép được từ trình duyệt.', 'error'))}><Copy size={14} /></button></div><pre>{text}</pre></div>;
}

export function EmptyState({ title, description, icon = true }: { title: string; description?: string; icon?: boolean }) {
  return <div className="empty-state">{icon && <div className="empty-icon"><ShoppingBag size={25} /></div>}<h3>{title}</h3>{description && <p>{description}</p>}</div>;
}

export function DataTable({ rows, group = '', onRow, onEdit }: { rows: any[]; group?: string; onRow?: (row: any) => void; onEdit?: (row: any) => void }) {
  const spec = columns[group] || (rows[0] ? Object.keys(rows[0]).filter(k => !['id', 'image', 'content', 'brief'].includes(k) && (rows[0][k] == null || typeof rows[0][k] !== 'object')).slice(0, 5).map(k => [k, ({ title: 'Tên', name: 'Tên', code: 'Mã', price: 'Giá', isActive: 'Trạng thái', customerName: 'Khách hàng', unitName: 'Đơn vị', quantity: 'Số lượng' } as Record<string, string>)[k] || k]) : []);
  if (!rows.length) return <EmptyState title="Chưa có dữ liệu" description="Điều chỉnh bộ lọc hoặc tạo bản ghi đầu tiên." />;
  return <div className="table-scroll"><table><thead><tr><th className="row-index">#</th>{spec.map(([key, title]) => <th key={key}>{title}</th>)}{onRow && <th className="table-actions">Thao tác</th>}</tr></thead><tbody>{rows.map((row, index) => <tr key={recordId(row) || index}>
    <td className="muted row-index">{String(index + 1).padStart(2, '0')}</td>
    {spec.map(([key]) => <td key={key}>{typeof row[key] === 'boolean' ? <span className={'badge ' + (row[key] ? 'success' : 'neutral')}>{row[key] ? 'Hoạt động' : 'Không'}</span> : /price|amount|cost/i.test(key) && typeof row[key] === 'number' ? <strong className="money">{money(row[key])}</strong> : /date|updatedAt|createdAt/i.test(key) && row[key] ? niceDate(row[key]) : <span className={key.endsWith('Id') || key === 'productCode' || key === 'code' ? 'mono compact' : ''}>{String(row[key] ?? '—')}</span>}</td>)}
    {onRow && <td className="table-actions"><button className="icon-btn" title="Xem chi tiết" onClick={() => onRow(row)}><Eye size={16} /></button>{onEdit && <button className="icon-btn" title="Sửa bản ghi" onClick={() => onEdit(row)}><FilePenLine size={16} /></button>}</td>}
  </tr>)}</tbody></table></div>;
}

function project(field: Field, data: any): any {
  if (field.kind === 'image') return data?.base64data ? data : undefined;
  if (field.kind === 'object') {
    if (data == null && field.optional) return undefined;
    return Object.fromEntries((schemas[field.ref!] || []).map(f => [f.name, project(f, data?.[f.name])]));
  }
  if (field.kind === 'array') return Array.isArray(data) ? data.map(row => project(field.item!, row)) : [];
  return data ?? defaultValue(field);
}

function initialBody(op: Operation): any {
  if (!op.body) return undefined;
  const body = op.body.kind === 'object' ? defaultObject(schemas[op.body.ref!] || []) : defaultValue(op.body);
  if (op.id === 'FnbOrders.CreateAsync') body.requestId = crypto.randomUUID();
  return body;
}

export function Workspace({ group, seed }: { group: string; seed?: Seed }) {
  const { connection, branchId, selectBranch, operations, openOperation, notify, refreshData, dataVersion } = useDemo();
  const actions = operations.filter(o => o.group === group);
  const [activeId, setActiveId] = useState(actions.find(o => o.id.endsWith('.ListAsync'))?.id || actions[0]?.id);
  const op = actions.find(o => o.id === activeId) || actions[0];
  const defaults = (action: Operation) => ({ ...defaultObject(action.queryFields), ...(action.queryFields.some(f => f.name === 'branchId') ? { branchId: branchId || undefined } : {}) });
  const [path, setPath] = useState<Values>({});
  const [query, setQuery] = useState<Values>(() => defaults(op));
  const [body, setBody] = useState<any>(() => initialBody(op));
  const [response, setResponse] = useState<SdkResponse>();
  const [error, setError] = useState<any>();
  const [busy, setBusy] = useState(false);
  const [elapsed, setElapsed] = useState(0);
  const [showInspector, setShowInspector] = useState(false);
  const [poll, setPoll] = useState(false);
  const [lineContext, setLineContext] = useState<{ key: string; productId?: string; comboId?: string }>();
  const lineKey = `${query.branchId || branchId}/${path.orderId || ''}/${path.lineId || ''}`;
  const formScope: Values = { ...(lineContext?.key === lineKey ? lineContext : {}), ...(body && !Array.isArray(body) ? body : {}), ...query, ...path, branchId: query.branchId || branchId };
  if (op.path.includes('/products/{lineId}') && !formScope.productId) formScope.productId = undefined;
  if (group === 'Combos' && path.id) formScope.comboId = path.id;
  const lineKind = op.id.includes('Combo') ? 'combo' : 'product';
  const runSequence = useRef(0);
  const seenSeed = useRef(0);
  const lastRequest = useRef<any>(undefined);

  useEffect(() => {
    if (!path.orderId || !path.lineId || !formScope.branchId) return;
    const controller = new AbortController();
    void getLookupData(`fnb/orders/${encodeURIComponent(path.orderId)}`, { branchId: formScope.branchId }, controller.signal).then(order => {
      if (controller.signal.aborted) return;
      const product = (order.products || []).find((line: any) => line.id === path.lineId);
      const combo = (order.combos || []).find((line: any) => line.id === path.lineId);
      setLineContext({ key: lineKey, productId: product?.productId || product?.product?.id, comboId: combo?.comboId || combo?.combo?.id });
    }).catch(e => { if (!controller.signal.aborted) notify(errorMessage(e), 'info'); });
    return () => controller.abort();
  }, [lineKey, dataVersion]);

  // Chi nhánh mới không được giữ đơn, dòng món hoặc lựa chọn con của chi nhánh cũ.
  useEffect(() => {
    if (op.queryFields.some(f => f.name === 'branchId') && query.branchId !== branchId) {
      runSequence.current++; setBusy(false); setPoll(false); setResponse(undefined); setError(undefined);
      setQuery(old => ({ ...old, branchId: branchId || undefined }));
      setPath(defaultObject(op.pathFields)); setBody(initialBody(op));
    }
  }, [branchId]);

  function changePath(next: Values) {
    if (next.orderId !== path.orderId) { next = { ...next, lineId: undefined }; setBody(initialBody(op)); }
    setPath(next);
  }
  function changeQuery(next: Values) {
    if (next.branchId !== query.branchId) {
      setPath(defaultObject(op.pathFields)); setBody(initialBody(op)); setResponse(undefined); setPoll(false);
    }
    setQuery(next);
  }

  async function execute(action: Operation = op, p: Values = path, q: Values = query, b: any = body) {
    if (!connection.configured) { notify('Lưu cấu hình kết nối trước khi gọi SDK.', 'info'); return; }
    const errors = validateFields(action.pathFields, p, '', true);
    if (action.queryFields.some(f => f.name === 'branchId' && !f.optional) && !q.branchId) errors.unshift('Chi nhánh: chọn chi nhánh FnB trước khi gọi API.');
    if (action.body?.kind === 'object' && b) errors.push(...validateFields(schemas[action.body.ref!] || [], b, '', false));
    if (errors.length) { setError({ message: errors[0], errors }); return; }
    const sequence = ++runSequence.current;
    setBusy(true); setError(undefined);
    lastRequest.current = { operation: action.id, method: action.method, url: operationPath(action, p, q), ...(action.body ? { body: b } : {}) };
    const started = performance.now();
    try {
      const result = await runOperation(action, p, q, b);
      if (sequence !== runSequence.current) return;
      setResponse(result);
      if (action.method !== 'GET') notify('POS đã xử lý thành công qua SDK.');
      if (action.method !== 'GET') refreshData();
      if (action.id === 'FnbPayments.CheckQrTransactionAsync' && result.data === true) setPoll(false);
    } catch (caught) {
      if (sequence !== runSequence.current) return;
      setError(caught instanceof ApiError ? { ...caught.detail, status: caught.status } : { message: errorMessage(caught) });
      setResponse(undefined); setPoll(false);
    } finally { if (sequence === runSequence.current) { setBusy(false); setElapsed(Math.round(performance.now() - started)); } }
  }

  function select(action: Operation, preset?: Omit<Seed, 'version' | 'operation'>) {
    runSequence.current++; setBusy(false); setPoll(false); setError(undefined); setResponse(undefined);
    setActiveId(action.id);
    const nextPath = { ...defaultObject(action.pathFields), ...Object.fromEntries(action.pathFields.filter(f => path[f.name]).map(f => [f.name, path[f.name]])), ...preset?.path };
    const nextQuery = { ...defaults(action), ...preset?.query };
    if (nextQuery.branchId) selectBranch(String(nextQuery.branchId));
    const nextBody = preset?.body === undefined ? initialBody(action) : action.body ? project(action.body, preset.body) : undefined;
    setPath(nextPath); setQuery(nextQuery); setBody(nextBody);
    if (preset?.autoRun) void execute(action, nextPath, nextQuery, nextBody);
  }

  useEffect(() => {
    if (seed?.version && seed.version !== seenSeed.current && seed.operation.startsWith(group + '.')) {
      seenSeed.current = seed.version;
      const selected = actions.find(a => a.id === seed.operation);
      if (selected) select(selected, seed);
    }
  }, [seed?.version]);

  useEffect(() => {
    if (!poll || op.id !== 'FnbPayments.CheckQrTransactionAsync' || busy) return;
    const timer = setTimeout(() => void execute(), 4000);
    return () => clearTimeout(timer);
  }, [poll, busy, op.id, path, query]);

  function rowAction(row: any, edit = false) {
    const action = actions.find(a => a.id.endsWith(edit ? '.UpdateAsync' : '.GetAsync'));
    if (!action) return;
    const id = recordId(row);
    // Mở chi tiết/sửa giữ nguyên chi nhánh của bản ghi (đơn FnB) hoặc của bộ lọc hiện tại.
    const branchQuery = (op: Operation) => op.queryFields.some(f => f.name === 'branchId') ? { branchId: row.branchId || query.branchId } : {};
    if (edit) {
      const get = actions.find(a => a.id.endsWith('.GetAsync'));
      if (get) {
        setBusy(true);
        const read = runOperation(get, { [get.pathFields[0].name]: id }, branchQuery(get), undefined);
        void read.then(r => select(action, { path: { [action.pathFields[0].name]: id }, query: branchQuery(action), body: r.data })).catch(e => setError({ message: errorMessage(e) })).finally(() => setBusy(false));
      }
    } else select(action, { path: { [action.pathFields[0].name]: id }, query: branchQuery(action), autoRun: true });
  }

  const data = response?.data;
  const rows = Array.isArray(data) ? data : data?.items;
  const metadata = data?.metaData;
  const edit = actions.find(a => a.id.endsWith('.UpdateAsync'));
  const remove = actions.find(a => a.id.endsWith('.DeleteAsync'));
  const create = actions.find(a => a.id.endsWith('.CreateAsync'));
  const paginated = Boolean(metadata);
  const isOrder = group === 'FnbOrders' && data && !Array.isArray(data) && (data.products || data.combos);
  // Chi nhánh truyền tiếp khi mở thao tác đơn/thanh toán: ưu tiên chi nhánh của đơn, sau đó chi nhánh của request hiện tại.
  const orderBranch = (data && !Array.isArray(data) && data.branchId) || query.branchId || branchId;

  return <>
    <div className="page-heading"><div><div className="eyebrow">SDK EXPLORER <span>/ {group}</span></div><h1>{op?.groupLabel}</h1><p>{actions.length} thao tác qua SDK · Dữ liệu trực tiếp từ cửa hàng đã kết nối</p></div><div className="heading-actions"><button className="btn" disabled={!connection.configured} onClick={refreshData}><RefreshCw size={15}  />Làm mới bộ chọn</button>{create && <button className="btn btn-primary" onClick={() => select(create)}><Plus size={16} />Tạo mới</button>}</div></div>
    {!connection.configured && <div className="notice warning"><Info size={18} /><span>Chưa có kết nối POS. <a href="#/connection">Cấu hình API và Partner Key</a> để sử dụng các form bên dưới.</span></div>}
    <div className="workspace-grid">
      <section className="panel operation-panel">
        <div className="panel-heading"><div><span className="step-marker">01</span><h2>Chọn thao tác</h2></div><span className="badge neutral">{actions.length} API</span></div>
        <div className="action-list">{actions.map(action => <button key={action.id} className={'action-item ' + (op.id === action.id ? 'selected' : '')} onClick={() => select(action)}><span className={'http-badge ' + action.method.toLowerCase()}>{action.method}</span><span>{action.label}</span>{op.id === action.id && <ChevronRight size={14} />}</button>)}</div>
      </section>
      <section className="panel request-panel">
        <div className="panel-heading"><div><span className="step-marker">02</span><h2>{op.label}</h2></div><span className="mono muted compact">{op.id}</span></div>
        <div className="endpoint-bar"><span className={'http-badge ' + op.method.toLowerCase()}>{op.method}</span><code>/api/partner/v1/{op.path}</code><button className="icon-btn" title="Xem dữ liệu gửi" onClick={() => setShowInspector(!showInspector)}><Code2 size={16} /></button></div>
        <form onSubmit={e => { e.preventDefault(); void execute(); }}>
          <div className="request-body">
            {op.pathFields.length > 0 && <div className="form-section"><div className="section-label">BẢN GHI CẦN THAO TÁC</div><ObjectEditor operation={op} scope={formScope} lineKind={lineKind} fields={op.pathFields} value={path} onChange={changePath} mode="path" disabled={busy} /></div>}
            {op.queryFields.length > 0 && <div className="form-section"><div className="section-label">BỘ LỌC & NGỮ CẢNH</div><ObjectEditor operation={op} scope={formScope} lineKind={lineKind} fields={op.queryFields} value={query} onChange={changeQuery} mode="query" disabled={busy} /></div>}
            {op.body && <div className="form-section"><div className="section-label">DỮ LIỆU GỬI SDK</div>{op.body.kind === 'object' ? <ObjectEditor operation={op} scope={formScope} lineKind={lineKind} schemaName={op.body.ref} fields={schemas[op.body.ref!] || []} value={body || {}} onChange={setBody}  mode="body"  disabled={busy} /> : <FieldEditor operation={op} scope={formScope} lineKind={lineKind} addon={op.id === 'FnbOrders.UpdateProductAddOnsAsync'} field={{ ...op.body, label: 'Các dòng gửi lên', name: 'items' }} value={body} onChange={setBody}  mode="body"  disabled={busy} />}</div>}
            {!op.pathFields.length && !op.queryFields.length && !op.body && <div className="ready-request"><CheckCircle2 size={23} /><div><strong>Sẵn sàng gọi SDK</strong><p>Thao tác này không cần tham số bổ sung.</p></div></div>}
            {op.id === 'FnbOrders.CreateAsync' && <div className="notice subtle"><Info size={16} />Request ID được giữ nguyên khi gửi lại. Chọn “Đặt lại form” để tạo yêu cầu đơn mới.</div>}
            {op.id === 'FnbPayments.CheckQrTransactionAsync' && <label className="toggle toggle-box"><input type="checkbox" checked={poll} onChange={e => setPoll(e.target.checked)} /><span />Kiểm tra lại mỗi 4 giây, dừng khi đã nhận tiền hoặc có lỗi.</label>}
          </div>
          <div className="form-footer"><button type="button" className="btn btn-ghost" disabled={busy} onClick={() => select(op)}><RotateCcw size={14} />Đặt lại form</button><button type="submit" className="btn btn-primary" disabled={busy || !connection.configured}>{busy ? <LoaderCircle size={16} className="spin" /> : <Play size={15} />} {busy ? 'Đang xử lý…' : 'Gửi qua SDK'}<ArrowRight size={15} /></button></div>
        </form>
        {showInspector && <div className="inspector"><JsonView value={{ sdk: op.id, method: op.method, path, query, body }} label="Xem trước request · chỉ đọc" /></div>}
      </section>
    </div>
    <section className="panel results-panel">
      <div className="panel-heading"><div><span className="step-marker">03</span><h2>Kết quả từ SDK</h2></div><div className="row gap-sm">{elapsed > 0 && <span className="muted compact">{elapsed.toLocaleString('vi-VN')} ms</span>}{response && <span className="badge success"><CheckCircle2 size={12} />Thành công</span>}</div></div>
      {error && <div className="error-block"><div><XCircle size={19} /><strong>{error.message || error.title || 'Request chưa hợp lệ'}</strong>{error.status && <span className="badge error">HTTP {error.status}</span>}</div>{error.callId && <a className="btn" href={'#/history?callId=' + encodeURIComponent(error.callId)}>Xem trace lỗi</a>}{error.traceId && <p className="mono compact">traceId: {error.traceId}</p>}<JsonView value={error} label="Chi tiết lỗi API / validation" /></div>}
      {!response && !error && <EmptyState title="Kết quả sẽ hiển thị tại đây" description="Điền thông tin vào form và chọn “Gửi qua SDK”. Lịch sử giữ trong RAM của phiên backend." />}
      {response && <>
        {Array.isArray(rows) && <DataTable rows={rows} group={group} onRow={actions.some(a => a.id.endsWith('.GetAsync')) ? row => rowAction(row) : undefined} onEdit={edit ? row => rowAction(row, true) : undefined} />}
        {paginated && <div className="pagination"><span>{metadata.totalCount} bản ghi · Trang {metadata.currentPage}/{Math.max(1, metadata.totalPages)}</span><div><button className="btn btn-small" disabled={!metadata.hasPrevious || busy} onClick={() => { const next = { ...query, pageNumber: Math.max(1, Number(metadata.currentPage) - 1) }; setQuery(next); void execute(op, path, next); }}><ChevronLeft size={14} />Trước</button><button className="btn btn-small" disabled={!metadata.hasNext || busy} onClick={() => { const next = { ...query, pageNumber: Number(metadata.currentPage) + 1 }; setQuery(next); void execute(op, path, next); }}>Sau<ChevronRight size={14} /></button></div></div>}
        {data == null && <div className="success-message"><CheckCircle2 size={26} /><strong>Đã thực hiện thành công</strong><p>Kết quả đã được nhận từ POS qua SDK.</p></div>}
        {typeof data === 'boolean' && <div className="success-message"><CheckCircle2 size={26} /><strong>{data ? 'Đã xác nhận giao dịch' : 'Chưa ghi nhận giao dịch thanh toán'}</strong></div>}
        {data && !Array.isArray(data) && !data.items && <>
          <div className="detail-strip"><div><small>{data.code || data.productCode || 'BẢN GHI'}</small><h3>{recordName(data) || op.label}</h3><code>{recordId(data)}</code></div><div className="row">{edit && <button className="btn" onClick={() => select(edit, { path: { [edit.pathFields[0].name]: recordId(data) }, body: data })}><FilePenLine size={15} />Sửa</button>}{remove && <button className="btn danger-text" onClick={() => select(remove, { path: { [remove.pathFields[0].name]: recordId(data) } })}><Trash2 size={15} />Mở form xóa</button>}{isOrder && <button className="btn btn-primary" onClick={() => openOperation('FnbPayments.PreviewAsync', { path: { orderId: recordId(data) }, query: { branchId: orderBranch } })}><ScanQrCode size={16} />Thanh toán</button>}</div></div>
          {typeof data.totalAmount === 'number' && <div className="payment-summary">{[['subTotal', 'Tổng món / combo'], ['discountAmount', 'Chiết khấu'], ['surchargeAmount', 'Phụ thu'], ['voucherAmount', 'Voucher'], ['membershipAmount', 'Giảm từ điểm'], ['totalAmount', 'Tổng phải thu']].map(([key, label]) => <div key={key} className={key === 'totalAmount' ? 'total' : ''}><span>{label}</span><strong>{money(data[key])}</strong></div>)}<div className="row"><button className="btn" onClick={() => openOperation('FnbPayments.CreateQrAsync', { path, query: { branchId: orderBranch }, body: { ...body, customerPayment: data.totalAmount } })}>Tạo QR<ArrowUpRight size={14} /></button><button className="btn btn-primary" onClick={() => openOperation('FnbPayments.PayAsync', { path, query: { branchId: orderBranch }, body: { ...body, customerPayment: data.totalAmount } })}>Mở form ghi nhận thanh toán</button></div></div>}
          {(data.base64Data || data.qrCodeBase64) && <div className="qr-result"><img alt="QR từ API POS" src={(data.base64Data || data.qrCodeBase64).startsWith('data:') ? data.base64Data || data.qrCodeBase64 : 'data:image/png;base64,' + (data.base64Data || data.qrCodeBase64)} /><div><h3>{money(data.price)}</h3><p>{data.accountName} {data.accountNumber}</p>{data.url && <a href={data.url} target="_blank" rel="noreferrer">Mở đường dẫn QR<ArrowUpRight size={14} /></a>}{data.paymentProvider && <button className="btn" onClick={() => openOperation('FnbPayments.CheckQrTransactionAsync', { path: { ...path, provider: data.paymentProvider }, query: { branchId: orderBranch, transactionId: data.id } })}>Kiểm tra giao dịch</button>}</div></div>}
          {isOrder && <OrderLines order={data} branchId={orderBranch} onAction={(id, line, combo) => {
            openOperation(id, { path: { orderId: recordId(data), lineId: line.id }, query: { branchId: orderBranch }, body: { quantity: line.quantity, price: line.price, note: line.note, unitId: line.unitId } });
          }} />}
          {group === 'Fnb' && data.products && <div className="nested-result"><h3>Thực đơn · {data.products.length} món</h3><DataTable rows={data.products} onRow={row => { openOperation('FnbOrders.CreateAsync', { query: { branchId: query.branchId }, body: { requestId: crypto.randomUUID(), saleChannelId: query.saleChannelId || 'TaiCho', items: [{ productId: recordId(row), quantity: 1, unitId: row.unitId, addOns: [], attributes: [] }], combos: [], attributes: [] } }); }} /></div>}
        </>}
        {response.callId && <div className="store-toolbar"><a className="btn" href={'#/history?callId=' + encodeURIComponent(response.callId)}><Code2 size={16} />Xem trace gửi POS</a></div>}
        <details className="response-details" open={!rows && !isOrder && data != null && typeof data !== 'boolean'}><summary><Code2 size={15} />Dữ liệu đầy đủ · <span className="mono">{response.callId?.slice(0, 12)}</span></summary><JsonView value={data} label="Dữ liệu · chỉ đọc" /></details>
      </>}
    </section>
  </>;
}

function OrderLines({ order, branchId, onAction }: { order: any; branchId?: string; onAction: (id: string, row: any, combo: boolean) => void }) {
  const { openOperation } = useDemo();
  return <div className="order-lines"><div className="order-lines-heading"><h3>Món & combo trong đơn</h3><div className="row"><button className="btn btn-small" onClick={() => openOperation('FnbOrders.AddProductAsync', { path: { orderId: recordId(order) }, query: { branchId } })}><Plus size={14} />Thêm món</button><button className="btn btn-small" onClick={() => openOperation('FnbOrders.AddComboAsync', { path: { orderId: recordId(order) }, query: { branchId } })}><Plus size={14} />Thêm combo</button></div></div>{[['products', false], ['combos', true]].map(([key, combo]) => (order[String(key)] || []).map((line: any, i: number) => <div className="order-line" key={String(key) + line.id}>
    <div className="line-icon">{combo ? 'C' : String(i + 1).padStart(2, '0')}</div><div className="line-info"><strong>{line.name || line.title || line.comboName || line.combo?.title || 'Dòng ' + (i + 1)}</strong><small>{line.unitName || (combo ? 'Combo' : 'Món')} · {line.quantity ?? 1} · <code>{line.id}</code></small></div><strong>{money(line.totalPriceIncludeVAT ?? line.totalCostIncludeVAT ?? (line.priceIncludeVAT ?? line.price) * (line.quantity || 1))}</strong><select className="line-action" value="" onChange={e => { if (e.target.value) onAction(e.target.value, line, Boolean(combo)); }}><option value="">Thao tác…</option>{(combo ? [['UpdateComboAsync', 'Sửa combo'], ['DeleteComboAsync', 'Xóa combo'], ['CancelComboAsync', 'Hủy combo']] : [['UpdateProductQuantityAsync', 'Số lượng'], ['UpdateProductUnitAsync', 'Đơn vị'], ['UpdateProductPriceAsync', 'Giá'], ['UpdateProductNoteAsync', 'Ghi chú'], ['UpdateProductAttributesAsync', 'Thuộc tính'], ['UpdateProductAddOnsAsync', 'Món kèm'], ['DeleteProductAsync', 'Xóa món'], ['CancelProductAsync', 'Hủy món']]).map(([method, label]) => <option key={method} value={'FnbOrders.' + method}>{label}</option>)}</select>
  </div>))}</div>;
}
