import { ModuleWithProviders } from '@angular/core';
import { RouterModule } from '@angular/router';

import { AuthGuard } from '@app/auth.guard';
import { RootComponent } from '@app/shared';
import { UserDetailComponent } from './user-detail/user-detail.component';
import { EditUserComponent } from './edit-user/edit-user.component';

export const routing: ModuleWithProviders = RouterModule.forChild([
  {
    path: 'user',
    component: RootComponent,
    canActivate: [AuthGuard],

    children: [
      { path: 'view/:id', component: UserDetailComponent },
      { path: 'edit/:id', component: EditUserComponent },
    ]
  }
]);
