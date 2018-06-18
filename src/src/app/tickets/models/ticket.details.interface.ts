
export interface TicketComment {
  id: number;
  body: string;
  date: string;
  since: string;
  author: string;
  authorId: string;
  authorEmail: string;
  authorPicture: string;
  authorIsTech: boolean;
  system: boolean;
  forTechs: boolean;
  issueId: number;
  issueSubject: string;
}

export interface TicketSnapshot {
  id: number;
  subject: string;
  body: string;
  category: string,
  issueDate: string;
  issuedSince: string;
  lastUpdated: string;
  updatedSince: string;
  priority: number;
  status: string;
  statusId: number;
  from: string;
  userId: string;
}

export interface TicketDetails extends TicketSnapshot {
  dueOn: string;
  dueSince: string;
  startedOn: string;
  startedSince: string;
  resolvedOn: string;
  resolvedSince: string;
  assignee: string;
  assigneeId: number;
  timeSpent: number;
  owned: boolean;
  canTakeOver: boolean;
  overTaken: boolean;
  canComment: boolean;
  comments: TicketComment[]
}

export interface TicketHistory {
  tickets: TicketSnapshot[];
  comments: TicketComment[];
}
