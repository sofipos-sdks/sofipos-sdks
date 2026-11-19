import { createContext, useContext } from 'react';
import type { Connection, LookupData, Operation, Values } from './types';

export interface Seed { version: number; operation: string; path?: Values; query?: Values; body?: any; autoRun?: boolean }
export interface DemoContextValue {
  connection: Connection;
  lookups: LookupData;
  loadingLookups: boolean;
  refreshConnection: () => Promise<void>;
  loadLookups: () => Promise<void>;
  pick: (kind: string, id: string) => void;
  notify: (message: string, tone?: 'success' | 'error' | 'info') => void;
  openOperation: (id: string, preset?: Omit<Seed, 'version' | 'operation'>) => void;
  operations: Operation[];
}
export const DemoContext = createContext<DemoContextValue>(null!);
export function useDemo() { return useContext(DemoContext); }
