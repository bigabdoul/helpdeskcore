import { Component, OnInit } from '@angular/core';
import { Router } from "@angular/router";

import { EntityListItem } from "@app/shared";
import { TicketService } from "@app/tickets/services";
import { TicketRegistration } from "@app/tickets/models";

@Component({
  selector: 'app-open-ticket',
  templateUrl: './open-ticket.component.html',
  styleUrls: ['./open-ticket.component.scss']
})
export class OpenTicketComponent implements OnInit {

  errors: string;
  isRequesting: boolean;
  submitted: boolean = false;
  categories: EntityListItem[];

  constructor(private ticketService: TicketService, private router: Router) { }

  ngOnInit() {
    this.ticketService.getCategories()
      .subscribe((items: EntityListItem[]) => this.categories = items);
  }

  create({ value, valid }: { value: TicketRegistration, valid: boolean }) {
    this.submitted = true;
    this.isRequesting = true;
    this.errors = '';
    if (valid) {
      this.ticketService.post(value)
        .finally(() => this.isRequesting = false)
        .subscribe(success => {
          if (success) {
            this.router.navigate(['/tickets/home']);
          }
        },
        errors => this.errors = errors);
    }
  }

}
