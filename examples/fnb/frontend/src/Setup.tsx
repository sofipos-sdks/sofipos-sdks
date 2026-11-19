import { useEffect, useState } from 'react';
import { LoaderCircle, Plug, RefreshCw, Save } from 'lucide-react';
import { ApiError, errorMessage, request } from './api';
import { useDemo } from './context';
import { JsonView } from './Workspace';

export function SetupScreen() {
  const { connection, refreshConnection, notify } = useDemo();
  const initial = () => ({ name: connection.name, baseUrl: connection.baseUrl, apiKey: '', webhookSecret: '', clearWebhookSecret: false });
  const [form, setForm] = useState(initial);
  const [error, setError] = useState('');
  const [trace, setTrace] = useState<any>();
  const [busy, setBusy] = useState(false);
  useEffect(() => { setForm(initial()); setTrace(undefined); }, [connection.updatedAt]);
  function change(key: string, value: string | boolean) { setForm(old => ({ ...old, [key]: value })); setTrace(undefined); }
  async function submit(save: boolean) {
    setBusy(true); setError('');
    try {
      const result = await request(save ? '/api/connection' : '/api/connection/check', save ? 'PUT' : 'POST', form);
      if (save) { await refreshConnection(); notify('Đã kết nối qua SDK. Chọn nhóm API để thực hiện thao tác.'); location.hash = '/sdk/Lookups'; }
      else { setTrace(result.trace); notify('SDK đã kiểm tra kết nối thành công.'); }
    } catch (e) { setError(errorMessage(e)); if (e instanceof ApiError) setTrace(e.detail?.trace); await refreshConnection().catch(() => {}); }
    finally { setBusy(false); }
  }
  return <>
    <div className="page-heading"><div><div className="eyebrow">SOFIPOS · SDK LAB</div><h1>Kết nối POS</h1><p>Nhập thông tin kết nối để gọi API bằng SDK.</p></div><Plug size={30} /></div>
    <section className="panel"><form onSubmit={e => { e.preventDefault(); void submit(true); }}>
      <fieldset disabled={busy} className="setup-fields panel-body form-grid">
        <div className="field"><label>Tên kết nối</label><input value={form.name} onChange={e => change('name', e.target.value)} required /></div>
        <div className="field"><label>API Base URL</label><input type="url" value={form.baseUrl} onChange={e => change('baseUrl', e.target.value)} placeholder="https://api.example.com" required /></div>
        <div className="field"><label>Partner Key</label><input type="password" autoComplete="new-password" value={form.apiKey} onChange={e => change('apiKey', e.target.value)} placeholder={connection.hasApiKey ? 'Để trống để dùng key đã lưu cùng URL' : 'tp_...'} required={!connection.hasApiKey} /></div>
        <div className="field"><label>Secret webhook · tùy chọn</label><input type="password" autoComplete="new-password" value={form.webhookSecret} onChange={e => change('webhookSecret', e.target.value)} placeholder={connection.hasWebhookSecret ? 'Để trống để giữ secret cùng kết nối' : 'whsec_...'} /></div>
        {connection.hasWebhookSecret && <label className="check-label"><input type="checkbox" checked={form.clearWebhookSecret} onChange={e => change('clearWebhookSecret', e.target.checked)} />Xóa secret đã lưu trong phiên</label>}
      </fieldset>
      <div className="notice subtle">Thông tin xác thực, trace và webhook chỉ giữ trong RAM backend. Khởi động lại backend sẽ kết thúc phiên. Đổi URL hoặc Partner Key hợp lệ sẽ xóa lịch sử của kết nối cũ.</div>
      {error && <div className="notice error">{error}</div>}
      <div className="form-footer"><button type="button" className="btn" disabled={busy || !form.baseUrl} onClick={() => void submit(false)}><RefreshCw size={17} />Kiểm tra qua SDK</button><button className="btn btn-primary" disabled={busy}>{busy ? <LoaderCircle size={17} className="spin" /> : <Save size={17} />}Lưu kết nối</button></div>
    </form></section>
    {trace && <details className="panel setup-trace" open><summary>Trace kiểm tra kết nối</summary><JsonView value={trace} label="Request / response · key đã che" /></details>}
  </>;
}
