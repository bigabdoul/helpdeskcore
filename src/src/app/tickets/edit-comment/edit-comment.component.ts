import { Component } from '@angular/core';
import { Router, ActivatedRoute } from "@angular/router";

import { TicketService } from "../services/ticket.service";
import { CommentModification } from "../models/ticket.registration.interface";
import { DetailEditView } from "../../shared/components/detail-edit-view";

@Component({
  selector: 'app-edit-comment',
  templateUrl: './edit-comment.component.html',
  styleUrls: ['./edit-comment.component.scss']
})
export class EditCommentComponent extends DetailEditView<CommentModification> {

  constructor(route: ActivatedRoute, dataService: TicketService, private router: Router) {
    super(route, dataService);
  }

  ngOnInit() {
    this.action = 'edit-comment';
    super.ngOnInit();
  }

  update({ value, valid }: { value: CommentModification, valid: boolean }) {
    if (valid) {
      this.updateModel(() => this.router.navigate([`/tickets/view/${this.model.issueId}`]), this.model, 'comment');
    }
  }
}
