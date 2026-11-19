import { useEffect, useRef, useState } from 'react';
import { ChevronDown, ChevronLeft, ChevronRight, RefreshCw, X } from 'lucide-react';
import { errorMessage, recordId, recordName, request } from './api';
import { useDemo } from './context';
import type { Field, Operation, Values } from './types';

export interface LookupSpec {
  path: string;
  params?: Values;
  search?: string;
  paged?: boolean;
  detail?: string;
  rows?: (data: any) => any[];
  waiting?: string;
  key?: string;
  allowText?: boolean;
}
const cache = new Map<string, any>();
const names = new Map<string, string>();
let epoch = 0;
export function clearLookupCache() { epoch++; cache.clear(); names.clear(); }

const catalogs: Record<string, [string, string?]> = {
  Products: ['products', 'keyword'], Customers: ['customers', 'key'], Combos: ['combos', 'key'],
  PricingPolicies: ['pricing-policies', 'key'], FlashSales: ['flashsales', 'key'], Vouchers: ['vouchers', 'title'],
  SaleChannels: ['sale-channels', 'name'], QrOrderCodes: ['qr-order-codes', 'keyword'],
  OrderAdjustmentPrograms: ['order-adjustment-programs', 'key'], FnbOrders: ['fnb/orders', 'orderCode'], FnbPayments: ['fnb/orders', 'orderCode'],
};
export function referenceLookup(resource: string, scope: Values = {}, fnb = false): LookupSpec {
  const branch = scope.branchId;
  const product = scope.productId;
  const combo = scope.comboId;
  const order = scope.orderId || scope.sourceOrderId;
  const scoped = (path: string): LookupSpec => ({ path, params: { branchId: branch }, waiting: branch ? undefined : 'Chọn chi nhánh trước.' });
  if (catalogs[resource]) {
    const [path, search] = catalogs[resource];
    return { path, search, paged: true, detail: path + '/{id}', ...(path === 'fnb/orders' ? scoped(path) : {}) };
  }
  if (resource === 'branches') return { path: fnb ? 'qr-order-codes/branches' : 'lookups/branches', rows: data => data.filter((row: any) => row.isActive !== false) };
  if (resource === 'categories') return { path: 'lookups/product-categories' };
  if (resource === 'ranks') return { path: 'lookups/membership-ranks' };
  if (resource === 'channelOptions') return { path: 'sale-channels/options', rows: data => data.filter((row: any) => row.isActive) };
  if (resource === 'tables') return { ...scoped('fnb/areas'), rows: data => data.flatMap((area: any) => (area.tables || []).map((table: any) => ({ ...table, name: `${area.name || area.title || ''} · ${table.name || table.title}` }))) };
  if (resource === 'providers') return { ...scoped('fnb/payment-options'), rows: data => data.filter((p: any) => p.isEnabled && p.provider).map((p: any) => ({ ...p, id: p.provider, name: p.name || p.provider })) };
  if (resource === 'units') return { path: `products/${encodeURIComponent(product || '')}/units`, waiting: product ? undefined : 'Chọn sản phẩm trước.', rows: data => data.map((u: any) => ({ ...u, name: `${u.unit || u.unitName || 'Đơn vị'} · ${Number(u.price || 0).toLocaleString('vi-VN')} đ` })) };
  if (resource === 'addons') return { ...scoped(`fnb/products/${encodeURIComponent(product || '')}/addons`), waiting: !branch ? 'Chọn chi nhánh trước.' : !product ? 'Chọn món chính trước.' : undefined };
  if (resource === 'attributes' || resource === 'attributeValues') {
    const spec = product ? scoped(`fnb/products/${encodeURIComponent(product)}/attributes`) : { path: 'qr-order-codes/order-attributes' };
    if (resource === 'attributes') return spec;
    return { ...spec, key: String(scope.attributeId || ''), allowText: true,
      waiting: !scope.attributeId ? 'Chọn thuộc tính trước.' : spec.waiting,
      rows: data => (data.find((a: any) => String(a.id) === String(scope.attributeId))?.values || []).map((value: any) => typeof value === 'string' ? { id: value, name: value } : value) };
  }
  if (resource === 'comboGroups' || resource === 'comboProducts') return {
    path: `combos/${encodeURIComponent(combo || '')}`, key: resource + '/' + (scope.groupId || ''),
    waiting: !combo ? 'Chọn combo trước.' : resource === 'comboProducts' && !scope.groupId ? 'Chọn nhóm combo trước.' : undefined,
    rows: data => resource === 'comboGroups' ? data.groups || [] : (data.groups?.find((g: any) => g.id === scope.groupId)?.products || []).map((p: any) => ({ ...p, id: p.productId || p.product?.id || p.id, name: p.name || p.productName || p.product?.name }))
  };
  if (resource === 'lines' || resource === 'children') return {
    ...scoped(`fnb/orders/${encodeURIComponent(order || '')}`), key: `${resource}/${scope.lineKind || ''}/${scope.parentLineId || ''}`,
    waiting: !branch ? 'Chọn chi nhánh trước.' : !order ? 'Chọn đơn hàng trước.' : resource === 'children' && !scope.parentLineId ? 'Chọn dòng món trước.' : undefined,
    rows: data => {
      const lines = scope.lineKind === 'combo' ? data.combos || [] : data.products || [];
      const rows = resource === 'children' ? lines.find((line: any) => line.id === scope.parentLineId)?.childs || [] : lines;
      return rows.map((line: any, index: number) => ({ ...line, name: `${line.name || line.title || line.product?.name || line.combo?.title || 'Dòng ' + (index + 1)} · SL ${line.quantity ?? 1}` }));
    }
  };
  if (resource === 'flashSaleProducts') return { ...scoped('fnb/menu'), params: { branchId: branch, saleChannelId: scope.saleChannelId }, key: product || '',
    rows: data => (data.products || []).filter((p: any) => p.flashSaleProductId && (!product || p.id === product)).map((p: any) => ({ ...p, id: p.flashSaleProductId })) };
  return { path: '', waiting: 'Chưa có API lấy danh sách cho trường này.' };
}

export interface LookupContext {
  operation?: Operation;
  mode?: 'path' | 'query' | 'body';
  schemaName?: string;
  scope: Values;
  productId?: string;
  addon?: boolean;
  lineKind?: string;
}
export function fieldLookup(field: Field, context: LookupContext): LookupSpec | undefined {
  const { operation, mode, schemaName, addon, lineKind } = context;
  const scope: Values = { ...context.scope, lineKind };
  const key = field.name.toLowerCase();
  if (key === 'branchid' || key === 'branchids') return referenceLookup('branches', scope, operation?.group.startsWith('Fnb') || operation?.group === 'QrOrderCodes');
  if (key === 'categoryid') return referenceLookup('categories');
  if (key === 'rankid' || key === 'membershiprankids') return referenceLookup('ranks');
  if (key === 'salechannelid' || key === 'salechannelids') {
    if (operation?.id === 'SaleChannels.CreateAsync') return referenceLookup('channelOptions');
    const spec = referenceLookup('SaleChannels');
    return { ...spec, params: mode === 'path' ? {} : { isActive: true } };
  }
  if (key === 'customerid') return referenceLookup('Customers');
  if (key === 'ids' && operation && catalogs[operation.group]) return referenceLookup(operation.group, scope);
  if (key === 'ids' && scope.event === 'category.changed') return referenceLookup('categories');
  if (key === 'ids' && scope.event === 'membership_rank.changed') return referenceLookup('ranks');
  if (key === 'ids' && scope.event === 'area_table.changed') return referenceLookup('tables', scope);
  if (key === 'comboid') return referenceLookup('Combos');
  if (key === 'productid' || key === 'productids' || key === 'addonproductid') {
    if (addon || key === 'addonproductid') return operation?.group.startsWith('Fnb')
      ? referenceLookup('addons', { ...scope, productId: context.productId }) : referenceLookup('Products');
    if (schemaName === 'FnbOrderComboProductRequest') return referenceLookup('comboProducts', scope);
    return referenceLookup('Products');
  }
  if (key === 'unitid' || key === 'addonunitid') {
    const productId = key === 'addonunitid' ? scope.addOnProductId : addon ? scope.productId : scope.productId || context.productId;
    return referenceLookup('units', { ...scope, productId });
  }
  if (key === 'groupid') return referenceLookup('comboGroups', scope);
  if (key === 'tableid' || key === 'targettableid') return referenceLookup('tables', scope);
  if (['orderid', 'sourceorderid', 'targetorderid'].includes(key)) return referenceLookup('FnbOrders', scope);
  if (key === 'lineid' || (key === 'id' && schemaName === 'FnbMoveItem')) return referenceLookup('lines', scope);
  if (key === 'id' && schemaName === 'FnbMoveChildItem') return referenceLookup('children', scope);
  if (key === 'attributeid' || (key === 'id' && schemaName === 'AttributeValue') || (key === 'value' && schemaName === 'AttributeValue')) {
    const productId = scope.productId || context.productId;
    if (('productId' in scope || context.productId !== undefined) && !productId)
      return { path: '', waiting: 'Chọn sản phẩm hoặc dòng món trước.' };
    return referenceLookup(key === 'value' ? 'attributeValues' : 'attributes', { ...scope, productId, attributeId: scope.id || scope.attributeId });
  }
  if (key === 'flashsaleproductid') return referenceLookup('flashSaleProducts', scope);
  if (key === 'provider' || key === 'paymentprovider') {
    const spec = referenceLookup('providers', scope);
    return { ...spec, key: operation?.id, rows: data => spec.rows!(data).filter((provider: any) =>
      operation?.id.includes('CheckQrTransaction') ? provider.supportsTransactionCheck : operation?.id.includes('CreateQr') ? provider.supportsQr : true) };
  }
  if (key === 'id' && mode === 'path' && operation && catalogs[operation.group]) return referenceLookup(operation.group, scope);
  return undefined;
}

function url(path: string, params: Values = {}) {
  const query = new URLSearchParams();
  for (const [key, value] of Object.entries(params)) if (value !== undefined && value !== null && value !== '') query.set(key, String(value));
  return '/api/pos/' + path + (query.size ? '?' + query : '');
}
async function read(path: string, signal: AbortSignal, force = false) {
  if (!force && cache.has(path)) return cache.get(path);
  const version = epoch;
  const data = (await request(path, 'GET', undefined, signal, 'lookup')).data;
  if (version === epoch) {
    cache.set(path, data);
    if (cache.size > 100) cache.delete(cache.keys().next().value!);
  }
  return data;
}
export function getLookupData(path: string, params: Values, signal: AbortSignal) { return read(url(path, params), signal); }
function optionName(row: any, index = 0) {
  const name = row.name || row.title || row.customerName || row.rankName || row.saleChannelName || row.code || row.productCode || `Bản ghi ${index + 1}`;
  return String(name) + (row.productCode && row.productCode !== name ? ' · ' + row.productCode : '');
}

export function LookupPicker({ spec, value, onChange, label, optional = false, disabled = false }: {
  spec: LookupSpec; value?: string; onChange: (value: string | undefined) => void; label: string; optional?: boolean; disabled?: boolean;
}) {
  const { connection, dataVersion } = useDemo();
  const [open, setOpen] = useState(false);
  const [term, setTerm] = useState('');
  const [page, setPage] = useState(1);
  const [revision, setRevision] = useState(0);
  const [result, setResult] = useState<{ rows: any[]; hasNext: boolean }>({ rows: [], hasNext: false });
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState('');
  const [selectedName, setSelectedName] = useState('');
  const root = useRef<HTMLDivElement>(null);
  const specKey = `${connection.connectionId}/${dataVersion}/` + url(spec.path, spec.params) + '/' + (spec.key || '');
  useEffect(() => {
    setOpen(false); setTerm(''); setPage(1); setError(''); setResult({ rows: [], hasNext: false });
  }, [specKey]);
  useEffect(() => {
    const close = (event: MouseEvent) => { if (root.current && !root.current.contains(event.target as Node)) setOpen(false); };
    document.addEventListener('mousedown', close);
    return () => document.removeEventListener('mousedown', close);
  }, []);
  useEffect(() => {
    if (!value || spec.waiting) { setSelectedName(''); return; }
    const nameKey = specKey + '/' + value;
    if (names.has(nameKey)) { setSelectedName(names.get(nameKey)!); return; }
    if (spec.allowText) { setSelectedName(value); return; }
    const controller = new AbortController();
    setSelectedName('Đang tải tên…');
    const path = spec.detail ? url(spec.detail.replace('{id}', encodeURIComponent(value)), spec.params) : url(spec.path, spec.params);
    void read(path, controller.signal).then(data => {
      if (controller.signal.aborted) return;
      const rows = spec.detail ? [data] : spec.rows ? spec.rows(data) : Array.isArray(data) ? data : data.items || [];
      rows.forEach((row: any, index: number) => names.set(specKey + '/' + recordId(row), optionName(row, index)));
      setSelectedName(names.get(nameKey) || 'Bản ghi đã chọn không có trong danh sách');
    }).catch(e => { if (!controller.signal.aborted) { setSelectedName('Không tải được tên bản ghi'); setError(errorMessage(e)); } });
    return () => controller.abort();
  }, [value, specKey, dataVersion, connection.connectionId, revision, spec.waiting]);
  useEffect(() => {
    if (!open || spec.waiting) return;
    const controller = new AbortController();
    setBusy(true); setError('');
    const timer = setTimeout(() => {
      const params = { ...spec.params, ...(spec.paged ? { pageNumber: page, pageSize: 20 } : {}), ...(spec.search && term ? { [spec.search]: term } : {}) };
      void read(url(spec.path, params), controller.signal, revision > 0).then(data => {
        if (controller.signal.aborted) return;
        let rows = spec.rows ? spec.rows(data) : Array.isArray(data) ? data : data.items || [];
        rows.forEach((row: any, index: number) => names.set(specKey + '/' + recordId(row), optionName(row, index)));
        if (!spec.search && term) rows = rows.filter((row: any) => optionName(row).toLocaleLowerCase('vi-VN').includes(term.toLocaleLowerCase('vi-VN')));
        const hasNext = spec.paged ? Boolean(data.metaData?.hasNext) : rows.length > page * 20;
        setResult({ rows: spec.paged ? rows : rows.slice((page - 1) * 20, page * 20), hasNext });
      }).catch(e => { if (!controller.signal.aborted) { setError(errorMessage(e)); setResult({ rows: [], hasNext: false }); } })
        .finally(() => { if (!controller.signal.aborted) setBusy(false); });
    }, term ? 250 : 0);
    return () => { clearTimeout(timer); controller.abort(); };
  }, [open, specKey, term, page, revision, dataVersion, connection.connectionId, spec.waiting]);
  return <div className="reference-picker" ref={root}>
    <div className="input-actions">
      {spec.allowText ? <input disabled={disabled || Boolean(spec.waiting)} value={value || ''} onChange={e => onChange(e.target.value || undefined)} onFocus={() => setOpen(true)} placeholder="Nhập hoặc chọn giá trị" aria-label={label} /> :
        <button type="button" className="lookup-trigger" disabled={disabled || Boolean(spec.waiting)} aria-label={label} aria-expanded={open} onClick={() => setOpen(current => !current)}>{value ? selectedName || 'Đang tải tên…' : `Chọn ${label.toLocaleLowerCase('vi-VN')}…`}<ChevronDown size={16} /></button>}
      {value && <button className="icon-btn" type="button" disabled={disabled} title="Bỏ lựa chọn" onClick={() => onChange(undefined)}><X size={15} /></button>}
    </div>
    {spec.waiting && <small className="field-help">{spec.waiting}</small>}
    {error && !open && <small className="field-help">{error} Mở bộ chọn và bấm tải lại để thử lại.</small>}
    {open && <div className="lookup-popup" role="dialog" aria-label={'Chọn ' + label} onKeyDown={e => { if (e.key === 'Escape') setOpen(false); }}>
      <div className="lookup-search"><input autoFocus placeholder={spec.search === 'orderCode' ? 'Tìm mã đơn…' : 'Tìm theo tên hoặc mã…'} value={term} onChange={e => { setTerm(e.target.value); setPage(1); }} /><button type="button" className="icon-btn" title="Tải lại qua SDK" disabled={busy} onClick={() => { setRevision(n => n + 1); setPage(1); }}><RefreshCw size={16} className={busy ? 'spin' : ''} /></button></div>
      {(optional || spec.allowText) && <button type="button" className="lookup-option" onClick={() => { onChange(undefined); setOpen(false); }}>Không chọn / không gửi</button>}
      {busy ? <p className="lookup-message">Đang gọi SDK lấy danh sách…</p> : error ? <p className="lookup-message">{error}</p> : result.rows.length ? result.rows.map((row, index) => <button type="button" className="lookup-option" key={recordId(row) + index} onClick={() => { const id = recordId(row); const name = optionName(row, index); names.set(specKey + '/' + id, name); setSelectedName(name); onChange(id); setOpen(false); }}>{optionName(row, index)}</button>) : <p className="lookup-message">Không có dữ liệu phù hợp.</p>}
      <div className="lookup-pagination"><button type="button" className="btn btn-small" disabled={busy || page === 1} onClick={() => setPage(n => n - 1)}><ChevronLeft size={15} />Trước</button><span>Trang {page}</span><button type="button" className="btn btn-small" disabled={busy || !result.hasNext} onClick={() => setPage(n => n + 1)}>Sau<ChevronRight size={15} /></button></div>
    </div>}
  </div>;
}
