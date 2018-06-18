import { Component, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from "@angular/router";

import { DetailEditView } from "../../shared/components/detail-edit-view";
import { AdminService } from "../services/admin.service";
import { UserRegistration } from "../../shared/models/user.entities.interface";

@Component({
  selector: 'app-user-create',
  templateUrl: './user-create.component.html',
  styleUrls: ['./user-create.component.scss']
})
export class UserCreateComponent extends DetailEditView<UserRegistration> implements OnDestroy {

  submitted = false;
  private _ctlr: string;

  constructor(route: ActivatedRoute, dataService: AdminService, private router: Router) {
    super(route, dataService);
    this.action = undefined;
    this._ctlr = dataService.getCtrl();
    dataService.use('accounts');
  }
  
  create({ value, valid }: { value: UserRegistration, valid: boolean }) {
    this.submitted = true;
    if (valid) {
      value.mode = 'admin';
      this.createModel(() => this.router.navigate(['/admin/users']), value);
    }
  }

  ngOnDestroy() {
    this.dataService.use(this._ctlr);
  }
}
