import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { AuthGuard } from '@app/auth.guard';
import { SharedModule } from '@app/shared';
import { routing } from './tickets.routing';
import { TicketService } from '@app/tickets/services';

import { TicketsHomeComponent } from './home/home.component';
import { EditCommentComponent } from './edit-comment/edit-comment.component';
import { EditTicketComponent } from './edit-ticket/edit-ticket.component';
import { OpenTicketComponent } from './open-ticket/open-ticket.component';
import { ViewTicketComponent } from './view-ticket/view-ticket.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    routing,
    SharedModule
  ],
  declarations: [
    TicketsHomeComponent, ViewTicketComponent, OpenTicketComponent,
    EditTicketComponent, EditCommentComponent
  ],
  exports: [],
  providers: [AuthGuard, TicketService]
})
export class TicketsModule { }
