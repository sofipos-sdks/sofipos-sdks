import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { Activity, ArrowUpRight, Boxes, Check, ChevronRight, ClipboardList, Code2, Database, ExternalLink, FlaskConical, LayoutDashboard, Menu, Plug, RefreshCw, Search, Settings2, ShoppingBag, SlidersHorizontal, UtensilsCrossed, Webhook, X } from 'lucide-react';
import { allPages, errorMessage, recordId, recordName, request } from './api';
import { DemoContext, type Seed } from './context';
import { operations } from './reference';
import { Workspace } from './Workspace';
import { ConnectionScreen, CoverageScreen, HistoryScreen, OverviewScreen, SnapshotsScreen, SyncScreen, WebhooksScreen } from './Screens';
import type { Connection, LookupData, Option, SdkResponse } from './types';

const emptyConnection: Connection = { configured: false, name: 'Cửa hàng thử nghiệm', baseUrl: '', hasApiKey: false, hasWebhookSecret: false, defaultSaleChannelId: 'TaiCho' };
const emptyLookups: LookupData = { branches: [], products: [], customers: [], combos: [], channels: [], ranks: [], programs: [], tables: [], paymentProviders: [], units: {}, attributes: {}, addonProducts: {}, comboGroups: {}, productRecords: [], comboRecords: [] };
const nav = [
  { page: 'overview', label: 'Tổng quan', icon: LayoutDashboard },
  { page: 'connection', label: 'Kết nối cửa hàng', icon: Plug },
  { page: 'sync', label: 'Đồng bộ dữ liệu', icon: RefreshCw },
];
const icons: Record<string, typeof Boxes> = { Products: ShoppingBag, Customers: ClipboardList, Fnb: UtensilsCrossed, FnbOrders: ClipboardList, FnbPayments: SlidersHorizontal };
const toOptions = (rows: any[]): Option[] => rows.map(row => ({ value: recordId(row), label: recordName(row) + (row.productCode ? ` · ${row.productCode}` : '') })).filter(o => o.value);

export default function App() {
  const [page, setPage] = useState(() => location.hash.replace(/^#\/?/, '') || 'overview');
  const [connection, setConnection] = useState(emptyConnection);
  const [lookups, setLookups] = useState<LookupData>(emptyLookups);
  const [loadingLookups, setLoadingLookups] = useState(false);
  const [health, setHealth] = useState('loading');
  const [sidebar, setSidebar] = useState(false);
  const [search, setSearch] = useState('');
  const [seed, setSeed] = useState<Seed>();
  const [toast, setToast] = useState<{ message: string; tone: string }>();
  const toastTimer = useRef<ReturnType<typeof setTimeout> | undefined>(undefined);
  const currentConnection = useRef(connection);
  currentConnection.current = connection;
  const inflight = useRef(new Set<string>());

  const notify = useCallback((message: string, tone = 'success') => {
    setToast({ message, tone }); clearTimeout(toastTimer.current);
    toastTimer.current = setTimeout(() => setToast(undefined), 6500);
  }, []);

  const refreshConnection = useCallback(async () => {
    try { const value = await request<Connection>('/api/connection'); setConnection(value); }
    catch (error) { setHealth('mongo-unavailable'); throw error; }
  }, []);

  useEffect(() => {
    const change = () => { setPage(location.hash.replace(/^#\/?/, '') || 'overview'); setSidebar(false); window.scrollTo(0, 0); };
    window.addEventListener('hashchange', change);
    void refreshConnection().catch(() => {});
    const check = () => request('/api/health').then(() => setHealth('ready')).catch(() => setHealth('mongo-unavailable'));
    void check(); const interval = setInterval(check, 20_000);
    return () => { window.removeEventListener('hashchange', change); clearInterval(interval); };
  }, [refreshConnection]);

  const loadLookups = useCallback(async () => {
    if (!currentConnection.current.configured) return;
    setLoadingLookups(true);
    const connectionId = currentConnection.current.connectionId;
    const tasks: [keyof LookupData, string][] = [['branches', 'qr-order-codes/branches'], ['products', 'products'], ['customers', 'customers'], ['combos', 'combos'], ['channels', 'sale-channels'], ['ranks', 'lookups/membership-ranks'], ['programs', 'order-adjustment-programs']];
    const results = await Promise.allSettled(tasks.map(async ([key, path]) => ({ key, rows: await allPages('/api/pos/' + path) })));
    if (currentConnection.current.connectionId !== connectionId) { setLoadingLookups(false); return; }
    let failed = 0;
    for (const result of results) {
      if (result.status === 'rejected') { failed++; continue; }
      const { key, rows } = result.value;
      setLookups(old => ({ ...old, [key]: toOptions(rows), ...(key === 'products' ? { productRecords: rows } : key === 'combos' ? { comboRecords: rows } : {}) }));
    }
    const branchId = currentConnection.current.defaultBranchId;
    if (branchId) {
      try {
        const response = await request<SdkResponse>('/api/pos/fnb/areas?branchId=' + branchId);
        if (currentConnection.current.connectionId === connectionId) setLookups(old => ({ ...old, tables: toOptions((response.data || []).flatMap((a: any) => a.tables || [])) }));
      } catch { failed++; }
      try {
        const options = await request<SdkResponse>('/api/pos/fnb/payment-options?branchId=' + branchId);
        if (currentConnection.current.connectionId === connectionId) setLookups(old => ({ ...old, paymentProviders: options.data.filter((p: any) => p.isEnabled && p.provider).map((p: any) => ({ value: p.provider, label: `${p.name}${p.supportsQr ? ' · QR' : ''}${p.supportsTransactionCheck ? ' · tra cứu' : ''}` })) }));
      } catch { failed++; }
    }
    setLoadingLookups(false);
    if (failed) notify(`${failed} nhóm chưa tải được vào bộ chọn. Vẫn có thể nhập ID trực tiếp hoặc xem lỗi ở trang Lịch sử.`, 'info');
  }, [notify]);

  useEffect(() => { setLookups(emptyLookups); if (connection.configured) void loadLookups(); }, [connection.connectionId, connection.configured, loadLookups]);

  const pick = useCallback((kind: string, id: string) => {
    const connectionId = currentConnection.current.connectionId;
    const key = `${connectionId}:${kind}:${id}`;
    if (inflight.current.has(key)) return;
    inflight.current.add(key);
    void (async () => {
      try {
        if (kind === 'productId') {
          const branch = currentConnection.current.defaultBranchId;
          const suffix = branch ? '?branchId=' + branch : '';
          const results = await Promise.allSettled([
            request<SdkResponse>('/api/pos/products/' + id + '/units'),
            request<SdkResponse>('/api/pos/fnb/products/' + id + '/attributes' + suffix),
            request<SdkResponse>('/api/pos/fnb/products/' + id + '/addons' + suffix),
          ]);
          if (currentConnection.current.connectionId !== connectionId) return;
          setLookups(old => ({ ...old,
            units: { ...old.units, [id]: results[0].status === 'fulfilled' ? results[0].value.data.map((u: any) => ({ value: u.id, label: `${u.unit || u.unitName} · ${Number(u.price || 0).toLocaleString('vi-VN')} đ` })) : [] },
            attributes: { ...old.attributes, [id]: results[1].status === 'fulfilled' ? results[1].value.data : [] },
            addonProducts: { ...old.addonProducts, [id]: results[2].status === 'fulfilled' ? toOptions(results[2].value.data) : [] },
          }));
        } else if (kind === 'comboId') {
          const r = await request<SdkResponse>('/api/pos/combos/' + id);
          if (currentConnection.current.connectionId !== connectionId) return;
          setLookups(old => ({ ...old, comboGroups: { ...old.comboGroups, [id]: toOptions(r.data.groups || []) } }));
        } else if (kind === 'branchId') {
          const r = await request<SdkResponse>('/api/pos/fnb/areas?branchId=' + id);
          if (currentConnection.current.connectionId !== connectionId) return;
          setLookups(old => ({ ...old, tables: toOptions((r.data || []).flatMap((a: any) => a.tables || [])) }));
          const providers = await request<SdkResponse>('/api/pos/fnb/payment-options?branchId=' + id);
          if (currentConnection.current.connectionId === connectionId) setLookups(old => ({ ...old, paymentProviders: providers.data.filter((p: any) => p.isEnabled && p.provider).map((p: any) => ({ value: p.provider, label: p.name })) }));
        }
      } catch { /* Các form vẫn cho phép nhập ID khi lookup phụ không khả dụng. */ }
      finally { inflight.current.delete(key); }
    })();
  }, []);

  const openOperation = useCallback((id: string, preset: Omit<Seed, 'version' | 'operation'> = {}) => {
    const op = operations.find(o => o.id === id); if (!op) return;
    setSeed({ version: Date.now(), operation: id, ...preset });
    location.hash = '/sdk/' + op.group;
  }, []);

  const groups = useMemo(() => [...new Map(operations.map(o => [o.group, { group: o.group, label: o.groupLabel }])).values()], []);
  const selectedGroup = page.startsWith('sdk/') ? page.slice(4) : undefined;
  const selectedLabel = selectedGroup ? groups.find(g => g.group === selectedGroup)?.label : { overview: 'Tổng quan', connection: 'Kết nối cửa hàng', sync: 'Đồng bộ dữ liệu', snapshots: 'Dữ liệu MongoDB', webhooks: 'Webhook', history: 'Lịch sử gọi SDK', coverage: 'Độ phủ chức năng' }[page];

  return <DemoContext.Provider value={{ connection, lookups, loadingLookups, refreshConnection, loadLookups, pick, notify, openOperation, operations }}>
    <div className="app-shell">
      {sidebar && <div className="sidebar-backdrop" onClick={() => setSidebar(false)} />}
      <aside className={'sidebar ' + (sidebar ? 'open' : '')}>
        <a href="#/overview" className="brand"><span className="brand-mark">S<span>•</span></span><span>SofiPOS<span className="brand-sub">DEVELOPER WORKSPACE</span></span></a>
        <div className="workspace-selector"><span className="workspace-avatar"><UtensilsCrossed size={18} /></span><div><strong>FnB SDK Lab</strong><small>.NET · React · MongoDB</small></div><span className="version-chip">1.0</span></div>
        <div className="nav-scroll">
          <div className="nav-caption">WORKSPACE</div>
          {nav.map(({ page: p, label, icon: Icon }) => <a key={p} href={'#/' + p} className={'nav-link ' + (page === p ? 'active' : '')}><Icon size={17} />{label}{p === 'sync' && <span className="nav-dot" />}</a>)}
          <div className="nav-caption">DANH MỤC & BÁN HÀNG <span>13</span></div>
          {groups.filter(g => !search || g.label.toLowerCase().includes(search.toLowerCase())).map(g => {
            const Icon = icons[g.group] || Boxes;
            return <a key={g.group} href={'#/sdk/' + g.group} className={'nav-link ' + (selectedGroup === g.group ? 'active' : '')}><Icon size={16} /><span>{g.label}</span><small>{operations.filter(o => o.group === g.group).length}</small></a>;
          })}
          <div className="nav-caption">QUAN SÁT</div>
          {[{ p: 'snapshots', label: 'Dữ liệu MongoDB', icon: Database }, { p: 'webhooks', label: 'Webhook inbox', icon: Webhook }, { p: 'history', label: 'Lịch sử gọi SDK', icon: Activity }, { p: 'coverage', label: 'Độ phủ chức năng', icon: Code2 }].map(({ p, label, icon: Icon }) => <a key={p} href={'#/' + p} className={'nav-link ' + (page === p ? 'active' : '')}><Icon size={17} />{label}</a>)}
        </div>
        <div className="sidebar-footer"><FlaskConical size={18} /><div><strong>SDK demo workspace</strong><small>Dữ liệu từ cửa hàng đã kết nối</small></div></div>
      </aside>
      <div className="app-main">
        <header className="topbar"><div className="breadcrumb"><button className="icon-btn mobile-menu" onClick={() => setSidebar(!sidebar)}><Menu size={20} /></button><span>Workspace</span><ChevronRight size={13} /><strong>{selectedLabel || 'Tổng quan'}</strong></div><div className="topbar-right"><span className={'status-dot ' + (health === 'ready' ? 'green' : 'amber')} /><span className="topbar-status">{health === 'ready' ? 'MongoDB sẵn sàng' : health === 'loading' ? 'Đang kết nối…' : 'MongoDB chưa sẵn sàng'}</span><span className="topbar-divider" /><a className="doc-link" href="#/coverage">91 thao tác <ArrowUpRight size={13} /></a><span className="user-avatar">DEV</span></div></header>
        <main className="content">
          {selectedGroup ? <Workspace key={selectedGroup} group={selectedGroup} seed={seed} /> : page === 'connection' ? <ConnectionScreen /> : page === 'sync' ? <SyncScreen /> : page === 'snapshots' ? <SnapshotsScreen /> : page === 'history' ? <HistoryScreen /> : page === 'webhooks' ? <WebhooksScreen /> : page === 'coverage' ? <CoverageScreen /> : <OverviewScreen />}
        </main>
        <footer className="page-footer"><span>SofiPOS FnB SDK Lab</span><span><span className="tiny-dot" /> .NET SDK 1.0.0 <span className="footer-separator">/</span> API partner v1</span></footer>
      </div>
      {toast && <div className={'toast ' + toast.tone}><Check size={18} /><span>{toast.message}</span><button className="icon-btn" onClick={() => setToast(undefined)}><X size={15} /></button></div>}
    </div>
  </DemoContext.Provider>;
}
