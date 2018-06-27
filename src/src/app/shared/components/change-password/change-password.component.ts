import { Component, OnInit, OnDestroy, EventEmitter, Input, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { DataService } from '@app/shared/services';
import { ChangePasswordModel } from '@app/shared/models';

/**
 * Provides mechanisms to change or reset a user's password.
 */
@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss']
})
export class ChangePasswordComponent implements OnInit, OnDestroy {
  errors: string;
  isRequesting: boolean;

  @Input() open: boolean;
  @Input() reset: boolean;
  @Output() passwordChanged = new EventEmitter<boolean>();
  @Output() passwordError = new EventEmitter<string>();

  constructor(private route: ActivatedRoute, private dataService: DataService, private router: Router) {
  }

  ngOnInit() {
    if (this.reset) {
      this.route.queryParamMap
        .filter(params => +params.get('pwdreset') === 1)
        .subscribe(params => {
          const obj = { userName: params.get('username'), resetToken: params.get('token') };
          localStorage.setItem('passwordReset', JSON.stringify(obj));
        });
    }
  }

  showDialog() {
    this.open = true;
  }

  closeDialog() {
    this.open = false;
  }

  changePassword({ value, valid }: { value: ChangePasswordModel, valid: boolean }) {
    if (valid) {
      this.isRequesting = true;
      let action = undefined;

      if (this.reset) {
        action = 'resetPassword';
        this.dataService.use('accounts');
        const pwd = JSON.parse(localStorage.getItem('passwordReset'));
        value.userName = pwd.userName;
        value.token = pwd.resetToken;
      } else {
        action = 'changePassword';
        this.dataService.use('admin');
        value.userId = this.dataService.userId();
      }
      
      this.dataService.post(value, action)
        .finally(() => this.isRequesting = false)
        .subscribe(success => {
          if (success) {
            this.errors = '';
            this.open = false;
            if (this.reset) {
              this.cleanLocalStorage();
              this.router.navigate(['/login']);
            }
          }
          this.passwordChanged.next(success);
        },
        error => {
          this.errors = error;
          this.passwordError.next(error);
        });
    }
  }

  admin() {
    return this.dataService.admin();
  }

  ngOnDestroy() {
    this.cleanLocalStorage();
  }

  private cleanLocalStorage() {
    localStorage.removeItem('passwordReset');
  }

}
