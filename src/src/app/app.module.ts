import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule, XHRBackend } from '@angular/http';
import { BrowserModule } from '@angular/platform-browser';
import { platformBrowserDynamic } from "@angular/platform-browser-dynamic/platform-browser-dynamic";

import { AuthenticateXHRBackend } from '@app/authenticate-xhr.backend';
import { routing } from '@app/app.routing';

import { GrowlModule } from 'primeng/primeng';

/* App Root */
import { AppComponent } from '@app/app.component';
import { HomeComponent } from '@app/home/home.component';
import { HeaderComponent } from '@app/header/header.component';

import { AccountModule } from '@app/account';
import { DashboardModule } from '@app/dashboard';
import { AdminModule } from '@app/admin';
import { TicketsModule } from '@app/tickets';
import { UserModule } from '@app/user';
import { AlertService, ConfigService } from '@app/shared';

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    HomeComponent,
  ],
  imports: [
    AccountModule,
    AdminModule,
    DashboardModule,
    TicketsModule,
    UserModule,
    BrowserModule,
    FormsModule,
    HttpModule,
    GrowlModule,
    routing
  ],
  providers: [
    AlertService,
    ConfigService,
    {
      provide: XHRBackend,
      useClass: AuthenticateXHRBackend
    }],
  bootstrap: [AppComponent]
})
export class AppModule { }

if ((module as any).hot) {
  (module as any).hot.accept();
  console.log('[HMR] Accepting module hot update.');
  const applicationTagName = 'app-root';
  tryRemoveApplicationNode(applicationTagName);
  tryBootstrapNewApplication(applicationTagName);
}

function tryRemoveApplicationNode(tagName) {
  const currentApplicationNode = document.getElementsByTagName(tagName)[0];
  if (currentApplicationNode) {
    const parent = currentApplicationNode.parentNode;
    parent.removeChild(currentApplicationNode);
  }
}

function tryBootstrapNewApplication(tagName) {
  const newNode = document.createElement(tagName);
  document.getElementsByTagName('body')[0].insertAdjacentElement('beforeend', newNode);

  //const newAppModule = require('./app.module').AppModule;
  //platformBrowserDynamic().bootstrapModule(newAppModule);
}
