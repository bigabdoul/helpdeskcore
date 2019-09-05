import { Injectable } from '@angular/core';
import { Http } from '@angular/http';

import { ConfigService, DataService, EntityListItem } from "@app/shared";
import { TicketModification, TicketComment, TicketSnapshot  } from '@app/tickets/models';

@Injectable()
export class TicketService extends DataService {

  constructor(http: Http, configService: ConfigService) {
    configService.controller = 'tickets';
    super(http, configService);
  }
  
  takeOver(id: number) {
    return this.post({ id }, `takeover/${id}`);
  }

  setStatus(id: number, status: string | number) {
    return this.post({ issueId: id, status }, 'setStatus');
  }

  createComment(issueId: number, body: string, forTechsOnly: boolean, recipients: string) {
    return this.postGet<TicketComment>({ issueId, body, forTechsOnly, recipients }, 'comment');
  }
  
  forEdit(id: number) {
    return this.get<TicketModification>('edit', id);
  }
  
  getList(sortBy?: number) {
    return this.get<TicketSnapshot[]>('home', sortBy || 1);
  }

  getCategories() {
    return this.get<EntityListItem[]>('categories');
  }
  
  getSections() {
    return this.get<EntityListItem[]>('sections');
  }
}
