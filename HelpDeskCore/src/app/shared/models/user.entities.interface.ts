export interface UserRegistration {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  location: string;
  mode: string;
}

export interface UserSnapshot {
  id: string;
  userName: string;
  email: string;
  firstName: string;
  lastName: string;
  lastSeen: string;
  seenSince: string;
  sendEmail: boolean;
  twoFactor: boolean;
  disabled: boolean;
}

export interface UserDetail extends UserSnapshot {
  companyName?: string;
  companyId?: number;
  departmentId?: number;
  type: string;
  phone?: string;
  phoneExtension?: string;
  location?: string;
  greeting?: string,
  notes?: string;
  pictureUrl?: string;
  facebookId?: number;
  locale?: string;
  gender?: string;
  signature?: string;
  ipaddress?: string;
  hostName?: string;
  sendNewTicketTechEmail?: boolean;
  ticketsSubmitted?: number;
  tickestHandled?: number;
  ticketsCreatedForOthers?: number;
  hasAvatar?: boolean;
  role?: string;
}
