import { ModuleWithProviders }  from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { TicketsHomeComponent } from '@app/tickets';
import { HomeComponent }  from '@app/home/home.component';
import { RootComponent } from "@app/shared";
import { UsersComponent, CategoriesComponent, EmailSettingsComponent } from "@app/admin";

const appRoutes: Routes = [
  { path: '', component: HomeComponent, pathMatch: 'full' },
  { path: 'dashboard', component: RootComponent },
  { path: 'tickets', component: TicketsHomeComponent },
  { path: 'tickets/home', component: TicketsHomeComponent },
  { path: 'admin/users', component: UsersComponent },
  { path: 'admin/categories', component: CategoriesComponent },
  { path: 'admin/email-settings', component: EmailSettingsComponent },
];

export const routing: ModuleWithProviders = RouterModule.forRoot(appRoutes);
