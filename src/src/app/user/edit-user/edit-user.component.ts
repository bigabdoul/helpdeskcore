import { Component } from '@angular/core';
import { ActivatedRoute, Router } from "@angular/router";

import { DataService, DetailEditView, UserDetail, EntityListItem, ChangePasswordModel } from "@app/shared";

@Component({
  selector: 'app-edit-user',
  templateUrl: './edit-user.component.html',
  styleUrls: ['./edit-user.component.scss']
})
export class EditUserComponent extends DetailEditView<UserDetail> {

  departments: EntityListItem[];
  changingPwd: boolean;

  constructor(route: ActivatedRoute, dataService: DataService, private router: Router) {
    super(route, dataService);
    dataService.use('admin');
  }

  ngOnInit() {
    // setting action before ngOnInit() triggers
    // fetchDetail() in super class;
    super.ngOnInit();

    this.action = 'userdetail';

    // we want to do it ourselves to check if the current
    // user has the permission to edit the fetched user;
    // we could also do the check on the server; anyway,
    // the check is performed server-side during update
    this.fetchDetail(this.action).subscribe(model => {
      if (this.admin() || this.userId() === model.id) {
        this.model = model;
      } else {
        this.router.navigate(['/dashboard/home']);
      }
    });

    this.dataService.get<EntityListItem[]>('departments')
      .subscribe(items => this.departments = items);
  }

  update({ value, valid }: { value: UserDetail, valid: boolean }) {
    if (valid) {
      this.updateModel(() => {
        if (this.admin()) {
          this.router.navigate(['/admin/users']);
        } else {
          this.router.navigate(['/dashboard/home']);
        }
      });
    }
  }

  showChangePwd(event) {
    event.preventDefault();
    this.changingPwd = true;
  }

  changePassword({ value, valid }: { value: ChangePasswordModel, valid: boolean }) {
    if (valid) {
      value.userId = this.model.id;
      this.dataService.post(value, 'changePassword')
        .subscribe(res => {
          if (res) {
            this.changingPwd = false;
            this.errors = '';
          }
        },
        error => this.errors = error);
    }
  }

}
