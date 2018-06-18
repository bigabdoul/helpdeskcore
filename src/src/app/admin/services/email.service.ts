import { Injectable } from '@angular/core';
import { Http } from '@angular/http';

import { ConfigService, DataService } from "@app/shared";
import { MimeMessage, SmtpSettings } from "@app/admin/models";

@Injectable()
export class EmailService extends DataService {

  constructor(http: Http, configService: ConfigService) {
    configService.action = 'send';
    configService.controller = 'email';
    super(http, configService);
  }

  sendEmail(data: MimeMessage) {
    return this.post(data);
  }

  testEmail(data: { message: MimeMessage, config: SmtpSettings }) {
    return this.post(data, 'test');
  }

}
