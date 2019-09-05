import { ModuleWithProviders } from '@angular/core';
import { RouterModule }        from '@angular/router';

import { AdminAuthGuard } from "@app/auth.admin.guard";
import { RootComponent }    from '@app/shared';

import { UsersComponent } from './users/users.component';
import { UserCreateComponent } from "./user-create/user-create.component";
import { CategoriesComponent } from "./categories/categories.component";
import { CategoryViewComponent } from "./category-view/category-view.component";
import { CategoryEditComponent } from "./category-edit/category-edit.component";
import { EmailSettingsComponent } from "./email-settings/email-settings.component";

export const routing: ModuleWithProviders = RouterModule.forChild([
  {
      path: 'admin',
      component: RootComponent,
      canActivate: [AdminAuthGuard],

      children: [      
        { path: 'users', component: UsersComponent },
        { path: 'create/user', component: UserCreateComponent },
        { path: 'categories', component: CategoriesComponent },
        { path: 'view/category/:id', component: CategoryViewComponent },
        { path: 'edit/category/:id', component: CategoryEditComponent },
        { path: 'email-settings', component: EmailSettingsComponent },
      ]       
    }  
]);

