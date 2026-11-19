import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { Activity, Boxes, Check, ChevronRight, Code2, Menu, Plug, UtensilsCrossed, Webhook, X } from 'lucide-react';
import { errorMessage, request, setWorkspaceConnection } from './api';
import { DemoContext, type Seed } from './context';
import { operations } from './reference';
import { clearLookupCache, LookupPicker, referenceLookup } from './Lookup';
import { Workspace } from './Workspace';
import { SetupScreen } from './Setup';
import { CoverageScreen, HistoryScreen, WebhooksScreen } from './Screens';
import type { Connection } from './types';

const emptyConnection: Connection = { configured: false, name: 'Cửa hàng thử nghiệm', baseUrl: '', hasApiKey: false, hasWebhookSecret: false };

export default function App() {
  const [page, setPage] = useState(() => location.hash.replace(/^#\/?/, '') || 'connection');
  const [connection, setConnection] = useState(emptyConnection);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [branchId, setBranchId] = useState('');
  const [seed, setSeed] = useState<Seed>();
  const [dataVersion, setDataVersion] = useState(0);
  const [sidebar, setSidebar] = useState(false);
  const [toast, setToast] = useState<{ message: string; tone: string }>();
  const toastTimer = useRef<ReturnType<typeof setTimeout> | undefined>(undefined);
  const notify = useCallback((message: string, tone = 'success') => {
    setToast({ message, tone }); clearTimeout(toastTimer.current);
    toastTimer.current = setTimeout(() => setToast(undefined), 6500);
  }, []);
  const selectBranch = useCallback((id: string) => setBranchId(id), []);
  const refreshData = useCallback(() => { clearLookupCache(); setDataVersion(n => n + 1); }, []);
  const refreshConnection = useCallback(async () => {
    try {
      const next = await request<Connection>('/api/connection');
      setWorkspaceConnection(next.connectionId); setConnection(next); setError('');
    } catch (e) { setError(errorMessage(e)); throw e; }
    finally { setLoading(false); }
  }, []);
  useEffect(() => {
    const change = () => { setPage(location.hash.replace(/^#\/?/, '') || 'connection'); setSidebar(false); window.scrollTo(0, 0); };
    const reconnect = () => { void refreshConnection().catch(() => {}); };
    window.addEventListener('hashchange', change);
    window.addEventListener('connection-required', reconnect);
    void refreshConnection().catch(() => {});
    return () => { window.removeEventListener('hashchange', change); window.removeEventListener('connection-required', reconnect); clearTimeout(toastTimer.current); };
  }, [refreshConnection]);
  useEffect(() => { clearLookupCache(); setBranchId(''); setSeed(undefined); }, [connection.connectionId]);
  const openOperation = useCallback((id: string, preset: Omit<Seed, 'version' | 'operation'> = {}) => {
    const op = operations.find(o => o.id === id); if (!op) return;
    setSeed({ version: performance.now(), operation: id, ...preset });
    location.hash = '/sdk/' + op.group;
  }, []);
  const groups = useMemo(() => [...new Map(operations.map(op => [op.group, { id: op.group, label: op.groupLabel }])).values()], []);
  const route = page.split('?')[0];
  const requestedGroup = route.startsWith('sdk/') ? route.slice(4) : '';
  const group = groups.some(g => g.id === requestedGroup) ? requestedGroup : groups[0].id;
  const title = route === 'connection' ? 'Kết nối POS' : route === 'history' ? 'Lịch sử gọi SDK' : route === 'webhooks' ? 'Webhook inbox' : route === 'coverage' ? 'Độ phủ chức năng' : groups.find(g => g.id === group)?.label;
  const context = { connection, branchId, selectBranch, dataVersion, refreshData, refreshConnection, notify, openOperation, operations };
  if (loading || error) return <main className="setup-shell"><section className="panel panel-body"><h1>{error ? 'Chưa kết nối được backend' : 'Đang mở SDK Lab…'}</h1>{error && <><p>{error}</p><button className="btn" onClick={() => void refreshConnection().catch(() => {})}>Thử lại</button></>}</section></main>;
  return <DemoContext.Provider value={context}>
    {!connection.configured ? <main className="setup-shell"><SetupScreen /></main> : <div className="app-shell">
      {sidebar && <div className="sidebar-backdrop" onClick={() => setSidebar(false)} />}
      <aside className={'sidebar ' + (sidebar ? 'open' : '')}>
        <a href="#/sdk/Lookups" className="brand"><span className="brand-mark">S<span>•</span></span><span>SofiPOS<span className="brand-sub">DEVELOPER WORKSPACE</span></span></a>
        <div className="workspace-selector"><span className="workspace-avatar"><UtensilsCrossed size={18} /></span><div><strong>FnB SDK Lab</strong><small>Form gọi API qua SDK</small></div></div>
        <div className="nav-scroll"><div className="nav-caption">KẾT NỐI</div>
          <a href="#/connection" className={'nav-link ' + (route === 'connection' ? 'active' : '')}><Plug size={17} />Kết nối POS</a>
          <div className="nav-caption">NHÓM API <span>{groups.length}</span></div>
          {groups.map(g => <a key={g.id} href={'#/sdk/' + g.id} className={'nav-link ' + (route.startsWith('sdk/') && group === g.id ? 'active' : '')}><Boxes size={17} /><span>{g.label}</span></a>)}
          <div className="nav-caption">QUAN SÁT</div>
          {[{ id: 'history', label: 'Lịch sử gọi SDK', Icon: Activity }, { id: 'webhooks', label: 'Webhook inbox', Icon: Webhook }, { id: 'coverage', label: 'Độ phủ chức năng', Icon: Code2 }].map(({ id, label, Icon }) =>
            <a key={id} href={'#/' + id} className={'nav-link ' + (route === id ? 'active' : '')}><Icon size={17} /><span>{label}</span></a>)}
        </div>
        <div className="sidebar-footer"><Plug size={18} /><div><strong>{connection.name}</strong><small>Chỉ lưu kết nối trong phiên backend</small></div></div>
      </aside>
      <div className="app-main">
        <header className="topbar"><div className="breadcrumb"><button className="icon-btn mobile-menu" onClick={() => setSidebar(!sidebar)}><Menu size={20} /></button><span>SDK Lab</span><ChevronRight size={13} /><strong>{title}</strong></div>
          {(group.startsWith('Fnb') || group === 'QrOrderCodes') && route.startsWith('sdk/') && <div className="header-branch"><LookupPicker spec={referenceLookup('branches', {}, true)} value={branchId} onChange={value => selectBranch(value || '')} label="Chi nhánh FnB" optional /></div>}
        </header>
        <main className="content" key={connection.connectionId}>{route === 'connection' ? <SetupScreen /> : route === 'history' ? <HistoryScreen key={page} /> : route === 'webhooks' ? <WebhooksScreen /> : route === 'coverage' ? <CoverageScreen /> : <Workspace key={group + connection.connectionId} group={group} seed={seed} />}</main>
        <footer className="page-footer"><span>SofiPOS FnB SDK Lab</span><span>SDK thật · Request / response · Webhook</span></footer>
      </div>
    </div>}
    {toast && <div className={'toast ' + toast.tone}><Check size={18} /><span>{toast.message}</span><button className="icon-btn" onClick={() => setToast(undefined)}><X size={15} /></button></div>}
  </DemoContext.Provider>;
}
