import { useId } from 'react';
import { Plus, Trash2, ImagePlus, WandSparkles, ChevronDown, ChevronRight } from 'lucide-react';
import { schemas } from './reference';
import type { Field, Values, LookupData, Option } from './types';

export function defaultValue(field: Field): any {
  if (field.default !== undefined) return field.default;
  if (field.optional) return undefined;
  if (field.kind === 'object') return defaultObject(schemas[field.ref!] || []);
  if (field.kind === 'array') return [];
  if (field.kind === 'boolean') return false;
  if (field.kind === 'number') return field.name === 'quantity' || field.name === 'pageNumber' ? 1 : field.name === 'pageSize' ? 15 : 0;
  if (field.kind === 'enum') return field.options?.[0]?.value ?? 0;
  return '';
}

export function defaultObject(fields: Field[]): Values { return Object.fromEntries(fields.map(f => [f.name, defaultValue(f)])); }

function choices(field: Field, values: Values, lookups?: LookupData, productId?: string, addon = false): Option[] {
  if (!lookups) return [];
  const key = field.name.toLowerCase();
  if (key === 'productid' || key === 'productids') return addon && productId && lookups.addonProducts[productId]?.length ? lookups.addonProducts[productId] : lookups.products;
  if (key === 'branchid' || key === 'branchids') return lookups.branches;
  if (key === 'customerid') return lookups.customers;
  if (key === 'comboid') return lookups.combos;
  if (key === 'salechannelid' || key === 'salechannelids') return lookups.channels;
  if (key === 'membershiprankids' || key === 'rankid') return lookups.ranks;
  if (key === 'programid') return lookups.programs;
  if (key === 'tableid' || key === 'targettableid') return lookups.tables;
  if (key === 'provider' || key === 'paymentprovider') return lookups.paymentProviders;
  if (key === 'unitid') return lookups.units[values.productId || productId || ''] || [];
  if (key === 'groupid') return Object.values(lookups.comboGroups).flat();
  if (key === 'id' && field.type === 'string') return (lookups.attributes[productId || ''] || []).map(a => ({ value: a.id, label: a.title || a.id }));
  if (key === 'value') {
    const definition = (lookups.attributes[productId || ''] || []).find(a => a.id === values.id);
    return (definition?.values || []).map((v: string) => ({ value: v, label: v }));
  }
  return [];
}

interface Props {
  field: Field;
  value: any;
  onChange: (value: any) => void;
  values?: Values;
  lookups?: LookupData;
  productId?: string;
  addon?: boolean;
  mode?: 'path' | 'query' | 'body';
  disabled?: boolean;
  onPick?: (kind: string, id: string) => void;
}

export function FieldEditor(props: Props) {
  const { field, value, onChange, lookups, disabled, mode = 'body', onPick } = props;
  const values = props.values || {};
  const listId = useId();
  const productId = values.productId || props.productId;
  const options = choices(field, values, lookups, props.productId, props.addon);
  const set = (next: any) => {
    onChange(next);
    if (onPick && next && ['productId', 'comboId', 'branchId'].includes(field.name)) onPick(field.name, String(next));
  };
  const label = <span>{field.label || field.name}{!field.optional && mode === 'path' ? <b className="required"> *</b> : null}<small className="field-key">{field.name}</small></span>;

  if (field.kind === 'object') {
    const enabled = value !== undefined && value !== null;
    return <section className={'object-field ' + (enabled ? '' : 'collapsed')}>
      <div className="object-heading">
        <span>{enabled ? <ChevronDown size={15} /> : <ChevronRight size={15} />}{label}</span>
        {field.optional && <label className="toggle"><input type="checkbox" disabled={disabled} checked={enabled} onChange={e => onChange(e.target.checked ? defaultObject(schemas[field.ref!] || []) : undefined)} /><span />Áp dụng</label>}
      </div>
      {field.description && <p className="field-help">{field.description}</p>}
      {enabled && <ObjectEditor fields={schemas[field.ref!] || []} value={value} onChange={onChange} lookups={lookups} productId={productId} addon={props.addon} onPick={onPick} disabled={disabled} />}
    </section>;
  }

  if (field.kind === 'array') {
    const entries: any[] = Array.isArray(value) ? value : [];
    return <section className="array-field">
      <div className="array-heading"><label>{label}<span className="count-pill">{entries.length}</span></label><button type="button" className="btn btn-small btn-ghost" disabled={disabled} onClick={() => onChange([...entries, field.item?.kind === 'object' ? defaultObject(schemas[field.item.ref!] || []) : defaultValue(field.item!)])}><Plus size={14} />Thêm dòng</button></div>
      {field.description && <p className="field-help">{field.description}</p>}
      {entries.length === 0 && <div className="empty-array">Chưa có dòng nào{field.optional ? ' · có thể bỏ trống' : ''}</div>}
      {entries.map((entry, index) => <div className="array-entry" key={index}>
        <div className="entry-toolbar"><span>Dòng {index + 1}</span><button type="button" className="icon-btn danger-text" disabled={disabled} aria-label="Xóa dòng" onClick={() => onChange(entries.filter((_, i) => i !== index))}><Trash2 size={14} /></button></div>
        {field.item?.kind === 'object' ? <ObjectEditor fields={schemas[field.item.ref!] || []} value={entry} onChange={next => onChange(entries.map((v, i) => i === index ? next : v))} lookups={lookups} productId={productId} addon={field.name === 'addOns' || props.addon} onPick={onPick} disabled={disabled} />
          : <FieldEditor {...props} field={{ ...field.item!, name: field.name, label: 'Giá trị' }} value={entry} onChange={next => onChange(entries.map((v, i) => i === index ? next : v))} />}
      </div>)}
    </section>;
  }

  if (field.kind === 'image') return <div className="field wide"><label>{label}</label><label className="upload-box"><ImagePlus size={24} /><span>{value?.fileName || 'Chọn ảnh JPG, PNG, WebP, GIF hoặc BMP'}</span><input type="file" accept=".jpg,.jpeg,.png,.webp,.gif,.bmp" disabled={disabled} onChange={e => {
    const file = e.target.files?.[0]; if (!file) return;
    const reader = new FileReader(); reader.onload = () => onChange({ fileName: file.name, base64data: String(reader.result).split(',')[1] }); reader.readAsDataURL(file);
  }} /></label>{value?.base64data && <div className="image-preview"><img src={'data:image/' + (/\.jpe?g$/i.test(value.fileName) ? 'jpeg' : String(value.fileName).split('.').pop()) + ';base64,' + value.base64data} alt="Ảnh đã chọn" /><button type="button" className="btn btn-small" onClick={() => onChange(undefined)}>Bỏ ảnh mới</button></div>}</div>;

  if (field.kind === 'boolean') return <div className="field"><label>{label}</label>{field.optional ? <select value={value === undefined ? '' : String(value)} disabled={disabled} onChange={e => set(e.target.value === '' ? undefined : e.target.value === 'true')}><option value="">Không lọc / không gửi</option><option value="true">Có</option><option value="false">Không</option></select>
    : <label className="toggle toggle-box"><input type="checkbox" checked={Boolean(value)} disabled={disabled} onChange={e => set(e.target.checked)} /><span />{value ? 'Bật / Có' : 'Tắt / Không'}</label>}{field.description && <small className="field-help">{field.description}</small>}</div>;

  if (field.kind === 'enum') return <div className="field"><label>{label}</label><select disabled={disabled} value={value === undefined ? '' : String(value)} onChange={e => set(e.target.value === '' ? undefined : Number(e.target.value))}>{field.optional && <option value="">Không chọn / không gửi</option>}{field.options?.map(o => <option key={o.value} value={o.value}>{o.label} ({o.value})</option>)}</select>{field.description && <small className="field-help">{field.description}</small>}</div>;

  const type = field.kind === 'number' ? 'number' : field.kind === 'datetime' ? 'datetime-local' : field.kind === 'date' ? 'date' : field.kind === 'time' ? 'time' : 'text';
  return <div className={'field ' + (['content', 'note', 'brief', 'reason'].includes(field.name) ? 'wide' : '')}>
    <label>{label}</label>
    <div className="input-actions">
      {['content', 'note', 'brief', 'reason'].includes(field.name) ? <textarea disabled={disabled} value={value ?? ''} rows={3} onChange={e => set(e.target.value || undefined)} /> : <input
        type={type} step={field.kind === 'number' ? 'any' : field.kind === 'datetime' || field.kind === 'time' ? 1 : undefined}
        disabled={disabled} list={options.length ? listId : undefined} value={value ?? ''}
        placeholder={field.kind === 'guid' ? 'xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx' : field.optional ? 'Không bắt buộc' : field.label}
        onChange={e => set(e.target.value === '' ? undefined : field.kind === 'number' ? Number(e.target.value) : field.kind === 'time' && e.target.value.length === 5 ? e.target.value + ':00' : e.target.value)}
      />}
      {field.kind === 'guid' && mode === 'body' && ['id', 'requestId', 'targetId'].includes(field.name) && <button type="button" className="icon-btn" title="Sinh GUID mới" disabled={disabled} onClick={() => set(crypto.randomUUID())}><WandSparkles size={16} /></button>}
    </div>
    {options.length > 0 && <><datalist id={listId}>{options.map((o, i) => <option key={String(o.value) + i} value={o.value}>{o.label}</option>)}</datalist><select className="quick-picker" disabled={disabled} value="" onChange={e => { if (e.target.value) set(e.target.value); }}><option value="">Chọn theo tên…</option>{options.map((o, i) => <option key={String(o.value) + i} value={o.value}>{o.label}</option>)}</select></>}
    {field.description && <small className="field-help">{field.description}</small>}
  </div>;
}

export function ObjectEditor({ fields, value, onChange, ...rest }: { fields: Field[]; value: Values; onChange: (value: Values) => void } & Omit<Props, 'field' | 'value' | 'onChange'>) {
  return <div className="form-grid">{fields.map(field => <FieldEditor key={field.name} {...rest} field={field} values={value || {}} value={value?.[field.name]} onChange={next => onChange({ ...value, [field.name]: next })} />)}</div>;
}

export function validateFields(fields: Field[], value: Values, prefix = '', required = true): string[] {
  const errors: string[] = [];
  for (const field of fields) {
    const item = value?.[field.name];
    const label = prefix + (field.label || field.name);
    if ((item === undefined || item === null || item === '') && !field.optional && required && field.kind !== 'array') { errors.push(`${label}: chưa nhập giá trị.`); continue; }
    if (item === undefined || item === null || item === '') continue;
    if (field.kind === 'guid' && !/^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(String(item))) errors.push(`${label}: GUID không đúng định dạng.`);
    if (field.kind === 'number' && !Number.isFinite(Number(item))) errors.push(`${label}: số không hợp lệ.`);
    if (field.kind === 'object') errors.push(...validateFields(schemas[field.ref!] || [], item, label + ' → ', required));
    if (field.kind === 'array' && Array.isArray(item)) item.forEach((row, i) => {
      if (field.item?.kind === 'object') errors.push(...validateFields(schemas[field.item.ref!] || [], row, `${label} [${i + 1}] → `, required));
      else if (field.item?.kind === 'guid' && !/^[0-9a-f-]{36}$/i.test(String(row))) errors.push(`${label} [${i + 1}]: GUID không hợp lệ.`);
    });
  }
  return errors;
}
