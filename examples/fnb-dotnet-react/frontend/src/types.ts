export interface Option { value: string | number; label: string }
export interface Field {
  name: string;
  label: string;
  type: string;
  kind: 'string' | 'guid' | 'number' | 'boolean' | 'datetime' | 'date' | 'time' | 'enum' | 'object' | 'array' | 'image';
  optional?: boolean;
  description?: string;
  default?: unknown;
  ref?: string;
  item?: Field;
  options?: Option[];
}
export interface Operation {
  id: string;
  group: string;
  groupLabel: string;
  label: string;
  method: string;
  path: string;
  pathFields: Field[];
  queryFields: Field[];
  body: Field | null;
}
export interface Connection {
  configured: boolean;
  connectionId?: string;
  name: string;
  baseUrl: string;
  hasApiKey: boolean;
  hasWebhookSecret: boolean;
  defaultBranchId?: string;
  defaultSaleChannelId: string;
  updatedAt?: string;
}
export type Values = Record<string, any>;
export interface LookupData {
  branches: Option[];
  products: Option[];
  customers: Option[];
  combos: Option[];
  channels: Option[];
  ranks: Option[];
  programs: Option[];
  tables: Option[];
  paymentProviders: Option[];
  units: Record<string, Option[]>;
  attributes: Record<string, any[]>;
  addonProducts: Record<string, Option[]>;
  comboGroups: Record<string, Option[]>;
  productRecords: any[];
  comboRecords: any[];
}
export interface SdkResponse<T = any> { data: T; callId: string; syncWarning?: string }
