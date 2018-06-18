import { Injectable } from '@angular/core';
import { Http, Response, Headers } from '@angular/http';

import { HomeDetails } from '@app/dashboard/models';
import { ConfigService, DataService } from '@app/shared';

@Injectable()
export class DashboardService extends DataService {

  constructor(http: Http, configService: ConfigService) {
    configService.controller = 'dashboard';
     super(http, configService);
  }

  getHomeDetails() {
    return this.get<HomeDetails>('home');
  }  
}
