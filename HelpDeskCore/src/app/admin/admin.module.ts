import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { AdminAuthGuard } from "@app/auth.admin.guard";
import { FileUploadService, SharedModule } from '@app/shared';

import { routing } from './admin.routing';
import { AdminService } from './services';
import { UsersComponent } from './users/users.component';
import { UserCreateComponent } from "./user-create/user-create.component";
import { UserImportComponent } from "./user-import/user-import.component";
import { CategoriesComponent } from "./categories/categories.component";
import { CategoryViewComponent } from "./category-view/category-view.component";
import { CategoryEditComponent } from "./category-edit/category-edit.component";
import { EmailSettingsComponent } from "./email-settings/email-settings.component";

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    routing,
    SharedModule,
  ],
  declarations: [
    UsersComponent, UserCreateComponent, EmailSettingsComponent,
    CategoriesComponent, CategoryViewComponent, CategoryEditComponent, UserImportComponent,
  ],
  providers: [AdminAuthGuard, AdminService, FileUploadService]
})
export class AdminModule { }
