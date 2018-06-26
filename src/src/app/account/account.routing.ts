import { ModuleWithProviders } from '@angular/core';
import { RouterModule }        from '@angular/router';

import { LoginFormComponent } from "./login-form/login-form.component";
import { FacebookLoginComponent } from "./facebook-login/facebook-login.component";
import { RegistrationFormComponent } from "./registration-form/registration-form.component";

export const routing: ModuleWithProviders = RouterModule.forChild([
  { path: 'register', component: RegistrationFormComponent},
  { path: 'login', component: LoginFormComponent },
  { path: 'facebook-login', component: FacebookLoginComponent}
]);
