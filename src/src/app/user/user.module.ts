import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { SharedModule } from "@app/shared";
import { AuthGuard } from "@app/auth.guard";
import { DataService } from "@app/shared";
import { routing } from './user.routing';
import { UserDetailComponent } from './user-detail/user-detail.component';
import { EditUserComponent } from './edit-user/edit-user.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    SharedModule,
    routing,
  ],
  declarations: [UserDetailComponent, EditUserComponent],
  exports: [],
  providers: [AuthGuard, DataService]
})
export class UserModule { }
