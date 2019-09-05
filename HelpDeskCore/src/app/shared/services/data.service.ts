import { Injectable } from "@angular/core";
import { Http, Headers, RequestOptions } from "@angular/http";

import "@app/rxjs-operators";
import { Observable } from "rxjs/Observable";

import { BaseService } from "./base.service";
import { ConfigService } from "./config.service";
import { Pagination, QueryStringOptions } from "@app/shared/models";

@Injectable()
export class DataService extends BaseService {
  private requestHeaders: Headers;
  protected _baseUrl = "";
  protected _controller: string;
  protected _action: string;

  constructor(protected http: Http, protected configService: ConfigService) {
    super();
    this._baseUrl = configService.getApiURI();
    this._controller = configService.controller || "";
    this._action = configService.action || "";
  }

  post(data: any, action: string = ""): Observable<boolean> {
    return this.postGetCore(data, action, true);
  }

  postGet<T>(data: any, action: string = ""): Observable<T> {
    return <Observable<T>>this.postGetCore(data, action, false);
  }

  get<T>(action?: string, id?: number | string): Observable<T> {
    const headers = this.getHeaders();
    action = action || this._action;
    if (action) {
      action = "/" + action;
    } else {
      action = "";
    }
    if (id === undefined || id === null) {
      id = "";
    }
    return this.http
      .get(this._baseUrl + `/${this._controller}${action}/${id}`, { headers })
      .map(response => response.json())
      .catch(this.handleError);
  }

  getPage<T>(
    action?: string,
    qso?: QueryStringOptions
  ): Observable<Pagination<T>> {
    action = action || this._action;
    qso = Object.assign(
      {
        column: "",
        page: 1,
        query: "",
        size: 10,
        sortBy: 0,
        userId: ""
      },
      qso || {}
    );

    if (action) {
      action = "/" + action;
    } else {
      action = "";
    }

    qso.column = qso.column || "";
    qso.query = qso.query || "";

    const headers = this.getHeaders();
    const page = (qso.page || 1) - 1;
    const url =
      `/${this._controller}${action}/?sortBy=${qso.sortBy}&page=${page}&size=${
        qso.size
      }` + `&column=${qso.column}&query=${qso.query}&userId=${qso.userId}`;

    return this.http
      .get(this._baseUrl + url, { headers })
      .map(response => response.json())
      .catch(this.handleError);
  }

  update(data: any, action: string = ""): Observable<boolean> {
    const options = new RequestOptions({ headers: this.getHeaders() });
    action = action || this._action;
    return this.http
      .put(
        this._baseUrl + `/${this._controller}/${action}`,
        JSON.stringify(data),
        options
      )
      .map(res => true)
      .catch(this.handleError);
  }

  delete(id: number | string, action = ""): Observable<boolean> {
    const options = new RequestOptions({ headers: this.getHeaders() });
    action = action || this._action;
    if (action) {
      action += "/";
    }
    return this.http
      .delete(this._baseUrl + `/${this._controller}/${action}${id}`, options)
      .map(res => true)
      .catch(this.handleError);
  }

  /**
   * Instructs the service to use a specific controller when sending requests.
   * @param controller The name of the controller to use.
   */
  use(controller: string) {
    this._controller = controller;
    return this;
  }

  /**
   * Sends a POST or PUT request using the specified data and action link.
   * @param method The method to use (POST, PUT, UPDATE).
   * @param data The data to send.
   * @param action The controller action name.
   */
  send(method: string, data: any, action?: string) {
    const met = (method || "").toUpperCase();
    switch (met) {
      case "POST":
        return this.post(data, action);
      case "PUT":
      case "UPDATE":
        return this.update(data, action);
      default:
        throw new Error(
          'Only methods "POST" and "UPDATE" (or "PUT") are supported.'
        );
    }
  }

  /**
   * Returns the controller being used.
   */
  getCtrl() {
    return this._controller;
  }

  protected getHeaders() {
    if (this.requestHeaders) {
      return this.requestHeaders;
    }
    const headers = new Headers();
    headers.append("Content-Type", "application/json");
    const authToken = localStorage.getItem("auth_token");
    headers.append("Authorization", `Bearer ${authToken}`);
    return headers;
  }

  protected setHeaders(headers: Headers) {
    this.requestHeaders = headers;
    return this;
  }

  private postGetCore(
    data: any,
    action: string = "",
    isBool = false
  ): Observable<any> {
    const options = new RequestOptions({ headers: this.getHeaders() });
    const url = this._baseUrl + `/${this._controller}/${action}`;
    console.log(url);
    action = action || this._action;
    return this.http
      .post(url, JSON.stringify(data || {}), options)
      .map(res => {
        if (isBool) {
          return true;
        } else {
          return res.json();
        }
      })
      .catch(this.handleError);
  }
}
