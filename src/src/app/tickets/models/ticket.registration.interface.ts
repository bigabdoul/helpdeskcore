export interface TicketRegistration {
  email: string;
  subject: string;
  body: string;
  categoryId: number;
  priority: number;
}

export interface TicketModification {
  id?: number;
  subject?: string;
  body?: string;
}


export interface CommentRegistration {
  issueId: number;
  body: string;
  recipients: string;
  forTechsOnly: boolean;
}

export interface CommentModification {
  id?: number;
  body?: string;
  forTechsOnly?: boolean;
  issueId?: number;
}
