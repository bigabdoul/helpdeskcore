// auth.guard.ts
import { Injectable } from '@angular/core';
import { Router, CanActivate } from '@angular/router';
import { UserService } from '@app/shared';

@Injectable()
export class AuthGuard implements CanActivate {
  constructor(protected user: UserService, protected router: Router) {}

  canActivate() {

    if(!this.user.loggedIn())
    {
       this.router.navigate(['/account/login']);
       return false;
    }

    return true;
  }
}
