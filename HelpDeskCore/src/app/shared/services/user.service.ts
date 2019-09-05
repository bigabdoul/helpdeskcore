import { Injectable, Output, EventEmitter } from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';

// Add the RxJS Observable operators we need in this app.
import '@app/rxjs-operators';
import { Observable } from 'rxjs/Rx';
import { BehaviorSubject } from 'rxjs/Rx';

import { DataService } from './data.service';
import { ConfigService } from './config.service';
import { UserRegistration } from '@app/shared/models';

@Injectable()
export class UserService extends DataService {

  // Observable navItem source
  private _authNavStatusSource = new BehaviorSubject<boolean>(false);

  // Observable navItem stream
  authNavStatus$ = this._authNavStatusSource.asObservable();

  @Output() onConnection = new EventEmitter<boolean>();

  constructor(http: Http, configService: ConfigService) {
    super(http, configService);
    super.setHeaders(new Headers({ 'Content-Type': 'application/json' }));

    // ?? not sure if this the best way to broadcast the status but seems to resolve issue on page refresh where auth status is lost in
    // header component resulting in authed user nav links disappearing despite the fact user is still logged in
    this._authNavStatusSource.next(this.loggedIn());
  }

  register(email: string, password: string, firstName: string, lastName: string, location: string): Observable<boolean> {
    return this.use('accounts').post({ email, password, firstName, lastName, location });
  }

  login(userName, password) {
    return this.loginCore('auth', 'login', { userName, password });
  }

  facebookLogin(accessToken: string) {
    return this.loginCore('externalauth', 'facebook', { accessToken });
  }

  /**
   * Do proper logout both locally, and remotely.
   */
  logout() {
    const id = localStorage.getItem('id');
    this.clearStorage();
    // fire and forget
    this.use('auth').post('', 'logout/' + id)
      .subscribe(
      res => { },
      error => { }
    );
    this._authNavStatusSource.next(false);
    this.onConnection.emit(false);
  }

  private loginCore(controller: string, action: string, data: any) {
    return this.use(controller).postGet<any>(data, action)
      .map(res => {
        localStorage.setItem('auth_token', res.auth_token);
        localStorage.setItem('role', res.role);
        localStorage.setItem('id', res.id);
        this._authNavStatusSource.next(true);
        this.onConnection.emit(true);
        return true;
      });
  }

  private clearStorage() {
    localStorage.removeItem('auth_token');
    localStorage.removeItem('role');
    localStorage.removeItem('id');
  }
}

