import { ModuleWithProviders } from '@angular/core';
import { RouterModule }        from '@angular/router';

import { AuthGuard } from '@app/auth.guard';
import { RootComponent }    from '@app/shared';
import { DashboardHomeComponent } from './home/home.component';

export const routing: ModuleWithProviders = RouterModule.forChild([
  {
      path: 'dashboard',
      component: RootComponent,
      canActivate: [AuthGuard],

      children: [      
       { path: '', component: DashboardHomeComponent },
       { path: 'home',  component: DashboardHomeComponent },
      ]       
    }  
]);
