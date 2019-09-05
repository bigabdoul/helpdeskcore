import { Component } from '@angular/core';

import { ConfigService, ListView, EntityListItem } from "@app/shared";
import { TicketService } from "@app/tickets/services";
import { TicketSnapshot } from "@app/tickets/models";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class TicketsHomeComponent extends ListView<TicketSnapshot> {

  constructor(ticketService: TicketService, config: ConfigService) {
    config.controller = 'tickets';
    config.action = 'index';
    super(ticketService, config);
  }

}
