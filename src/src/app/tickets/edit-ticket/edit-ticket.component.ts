import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from "@angular/router";

import { DetailEditView } from "@app/shared";
import { TicketService, TicketRegistration, TicketModification } from "@app/tickets";

@Component({
  selector: 'app-edit-ticket',
  templateUrl: './edit-ticket.component.html',
  styleUrls: ['./edit-ticket.component.scss']
})
export class EditTicketComponent extends DetailEditView<TicketModification> {

  constructor(route: ActivatedRoute, private ticketService: TicketService, private router: Router) {
    super(route, ticketService);
  }

  ngOnInit() {
    super.ngOnInit();
    this.fetchDetail();
  }

  update({ value, valid }: { value: TicketModification, valid: boolean }) {
    this.isRequesting = true;
    this.errors = '';
    if (valid) {
      const id = this.model.id;
      this.ticketService.update({ id, subject: value.subject, body: value.body })
        .finally(() => this.isRequesting = false)
        .subscribe(result => {
          if (result) {
            this.router.navigate([`/tickets/view/${id}`]);
          }
        },
        errors => this.errors = errors);
    }
  }

  fetchDetail(action?: string, id?: number|string) {
    const obs = this.ticketService.forEdit(+this.itemId);
    obs.subscribe(
      ticket => this.model = ticket,
      errors => this.errors = errors
    );
    return obs;
  }
}
