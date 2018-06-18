// auth.guard.ts
import { Injectable } from '@angular/core';
import { Router, CanActivate } from '@angular/router';
import { UserService } from '@app/shared';
import { AuthGuard } from "@app/auth.guard";

@Injectable()
export class AdminAuthGuard extends AuthGuard {
  constructor(user: UserService, router: Router) {
    super(user, router);
  }

  canActivate() {
    let success = super.canActivate();
    if (success) {
      if (!(success = this.user.admin())) {
        this.router.navigate(['/dashboard/home']);
      }
    }
    return success;
  }
}
