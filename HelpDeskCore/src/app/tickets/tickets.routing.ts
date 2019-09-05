import { ModuleWithProviders } from '@angular/core';
import { RouterModule } from '@angular/router';

import { AuthGuard } from '@app/auth.guard';
import { RootComponent } from '@app/shared';

import { TicketsHomeComponent } from './home/home.component';
import { EditCommentComponent } from './edit-comment/edit-comment.component';
import { EditTicketComponent } from './edit-ticket/edit-ticket.component';
import { OpenTicketComponent } from './open-ticket/open-ticket.component';
import { ViewTicketComponent } from './view-ticket/view-ticket.component';

export const routing: ModuleWithProviders = RouterModule.forChild([
  {
    path: 'tickets',
    component: RootComponent, canActivate: [AuthGuard],

    children: [
      { path: '', component: TicketsHomeComponent },
      { path: 'home', component: TicketsHomeComponent },
      { path: 'open', component: OpenTicketComponent },
      { path: 'view/:id', component: ViewTicketComponent },
      { path: 'edit/:id', component: EditTicketComponent },
      { path: 'edit-comment/:id', component: EditCommentComponent },
    ]
  }
]);
