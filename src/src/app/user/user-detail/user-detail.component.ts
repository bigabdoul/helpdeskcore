import { Component } from '@angular/core';
import { ActivatedRoute, Router } from "@angular/router";

import { DetailView, UserDetail, DataService } from "@app/shared";
import { TicketHistory, TicketComment, TicketSnapshot } from "@app/tickets/models";

@Component({
  selector: 'app-user-detail',
  templateUrl: './user-detail.component.html',
  styleUrls: ['./user-detail.component.scss']
})
export class UserDetailComponent extends DetailView<UserDetail> {

  history: TicketHistory;

  constructor(route: ActivatedRoute, dataService: DataService, private router: Router) {
    super(route, dataService);
    dataService.use('admin');
  }

  ngOnInit() {
    super.ngOnInit();
    this.fetchDetail('userdetail')
      .finally(() => this.isRequesting = false)
      .subscribe(model => {
        // only admins and the users themselves can edit their details
        if (this.admin() || this.userId() === model.id) {
          this.model = model;
          this.isRequesting = true;
          this.dataService.use('tickets').get<TicketHistory>('history', this.model.id)
            .finally(() => this.isRequesting = false)
            .subscribe(history => {
              this.history = history;
            }, error => this.errors = error);
        } else {
          this.router.navigate(['/dashboard/home']);
        }
      });
  }

}
