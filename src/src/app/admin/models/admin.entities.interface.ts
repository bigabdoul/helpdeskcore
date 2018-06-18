
export interface CategorySnapshot {
  id: number;
  name: string;
}

export interface CategoryDetail extends CategorySnapshot {
  notes: string;
  sectionId: number;
  sectionName: string;
  fromAddress: string;
  fromName: string;
  kbOnly: boolean;
  orderByNumber: number;
  mode: string;
  differentFrom?: boolean;
  fromAddressInReplyTo?: boolean;
}
