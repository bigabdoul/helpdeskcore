import { Component } from '@angular/core';
import { ActivatedRoute } from "@angular/router";

import { TicketService } from "../services/ticket.service";
import { TicketDetails } from "../models/ticket.details.interface";
import { CommentRegistration } from "../models/ticket.registration.interface";
import { DetailView } from "../../shared/components/detail-view";

@Component({
  selector: 'app-view-ticket',
  templateUrl: './view-ticket.component.html',
  styleUrls: ['./view-ticket.component.scss']
})
export class ViewTicketComponent extends DetailView<TicketDetails> {
  
  user: any;
  submitted: boolean = false;
  commentShown: boolean = false;
  closeOpenText: string = 'Fermer le ticket';

  constructor(route: ActivatedRoute, private ticketService: TicketService) {
    super(route, ticketService);
  }

  ngOnInit() {
    super.ngOnInit();
    this.fetchDetail().subscribe(model => {
      this.model = model;
      this.setCloseOpenText();
    });
  }

  takeOver() {
    if (this.model.canTakeOver) {
      this.ticketService.takeOver(this.model.id).subscribe(result => {
        if (result) {
          this.model.canTakeOver = false;
          this.model.overTaken = true;
        }
      },
        errors => this.errors = errors);
    }
  }

  toggleStatus() {
    if (this.model.overTaken) {
      let status = this.model.statusId;
      if (status === 3) {
        status = 4; // Resolved
      } else {
        status = 3; // InProgress
      }
      this.ticketService.setStatus(this.model.id, status).subscribe(result => {
        if (result) {
          this.model.statusId = status;
          this.setCloseOpenText();
        }
      },
        errors => this.errors = errors);
    }
  }

  submitComment({ value, valid }: { value: CommentRegistration, valid: boolean }) {
    this.errors = '';
    this.submitted = true;
    this.isRequesting = true;
    if (valid) {
      this.ticketService.createComment(this.model.id
        , value.body
        , value.forTechsOnly
        , value.recipients)
        .finally(() => this.isRequesting = false)
        .subscribe(result => {
          this.commentShown = false;
          this.model.comments.unshift(result);
        },
        errors => this.errors = errors);
    }
  }

  canSetStatus() {
    return this.model.overTaken && (this.model.statusId === 3 || this.model.statusId === 4);
  }

  canComment() {
    return this.model.owned || this.model.canComment;
  }

  private setCloseOpenText() {
    this.closeOpenText = this.model.statusId === 4 ? 'Rouvrir le ticket' : 'Fermer le ticket';
  }
}
