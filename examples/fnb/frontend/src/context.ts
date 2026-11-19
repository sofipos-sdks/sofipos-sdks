import { createContext, useContext } from 'react';
import type { Connection, Operation, Values } from './types';

export interface Seed { version: number; operation: string; path?: Values; query?: Values; body?: any; autoRun?: boolean }
export interface DemoContextValue {
  connection: Connection;
  branchId: string;
  selectBranch: (id: string) => void;
  dataVersion: number;
  refreshData: () => void;
  refreshConnection: () => Promise<void>;
  notify: (message: string, tone?: string) => void;
  openOperation: (id: string, preset?: Omit<Seed, 'version' | 'operation'>) => void;
  operations: Operation[];
}
export const DemoContext = createContext<DemoContextValue>(null!);
export function useDemo() { return useContext(DemoContext); }
