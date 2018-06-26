import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AuthGuard } from '@app/auth.guard';
import { RootComponent, SharedModule } from '@app/shared';

import { routing } from './dashboard.routing';
import { DashboardHomeComponent } from './home/home.component';
import { DashboardService } from '@app/dashboard/services';

@NgModule({
  imports: [
    CommonModule,
    routing,
    SharedModule
  ],
  declarations: [RootComponent, DashboardHomeComponent],
  exports: [],
  providers:    [AuthGuard,DashboardService]
})
export class DashboardModule { }
