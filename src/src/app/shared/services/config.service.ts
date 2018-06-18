import { Injectable } from '@angular/core';
import { environment } from "@env/environment";

@Injectable()
export class ConfigService {
  
  _apiURI: string = environment.baseUrl;
  controller: string;
  action: string;

  constructor() {
  }

  getApiURI() {
    return this._apiURI;
  }
}
