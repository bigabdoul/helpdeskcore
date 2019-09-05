import { Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';

import { BaseService } from './base.service';
import { ConfigService } from './config.service';

@Injectable()
export class FileUploadService extends BaseService {

  private _baseUrl: string = '';
  private _controller: string;
  private _action: string;

  constructor(private http: Http, configService: ConfigService) {
    super();
    this._baseUrl = configService.getApiURI();
    this._controller = configService.controller || '';
    this._action = configService.action || '';
  }

  config(controller: string, action: string) {
    this._controller = controller;
    this._action = action;
    return this;
  }

  upload(files: FormData) {
    const options = new RequestOptions({ headers: this.getHeaders() });
    return this.http.post(this._baseUrl + `/${this._controller}/${this._action}`, files, options)
      .map(res => true)
      .catch(this.handleError);
  }

  private getHeaders() {
    const headers = new Headers();
    const authToken = localStorage.getItem('auth_token');
    headers.append('Authorization', `Bearer ${authToken}`);
    return headers;
  }
}
