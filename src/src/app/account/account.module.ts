import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
 
import { DialogModule } from 'primeng/primeng';
import { UserService, SharedModule }  from '@app/shared';
import { EmailValidator } from '@app/directives';

import { routing } from "./account.routing";
import { LoginFormComponent } from "./login-form/login-form.component";
import { FacebookLoginComponent } from "./facebook-login/facebook-login.component";
import { RegistrationFormComponent } from "./registration-form/registration-form.component";

@NgModule({
  imports: [
    CommonModule,DialogModule,FormsModule,SharedModule,routing
  ],
  declarations: [RegistrationFormComponent,EmailValidator, LoginFormComponent, FacebookLoginComponent],
  providers:    [ UserService ]
})
export class AccountModule { }
