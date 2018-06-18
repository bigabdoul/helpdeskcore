import { Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';

import '@app/rxjs-operators';
import { Observable } from 'rxjs/Rx';

import { CategoryDetail, CategorySnapshot } from '@app/admin/models';
import { ConfigService, DataService, UserDetail, UserSnapshot } from "@app/shared";

@Injectable()
export class AdminService extends DataService {

  constructor(http: Http, configService: ConfigService) {
    configService.controller = 'admin';
    super(http, configService);
  }

  getUser(id: string) {
    return super.get<UserDetail>('userdetail', id);
  }

  getUsers(sortBy?: number, page: number = 0, size: number = 20) {
    return super.getPage<UserSnapshot[]>('users', { sortBy, page, size });
  }

  getCategory(id: number) {
    return super.get<CategoryDetail>('categorydetail', id);
  }

  getCategories(sortBy?: number, page: number = 0, size: number = 20) {
    return super.getPage<CategorySnapshot[]>('categories', { sortBy, page, size });
  }

}
