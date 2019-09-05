import { Component, OnInit } from '@angular/core';

import { HomeDetails } from '@app/dashboard/models';
import { DashboardService } from '@app/dashboard/services';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class DashboardHomeComponent implements OnInit {

  model: HomeDetails;

  constructor(private dashboardService: DashboardService) { }

  ngOnInit() {

    this.dashboardService.get<HomeDetails>('index')
    .subscribe((homeDetails: HomeDetails) => {
      this.model = homeDetails;
    },
    error => {
      //this.notificationService.printErrorMessage(error);
    });
    
  }

}
