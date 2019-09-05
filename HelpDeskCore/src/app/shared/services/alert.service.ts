import { Injectable } from '@angular/core';
import { Router, NavigationStart } from '@angular/router';
import { Observable } from 'rxjs';
import { Subject } from 'rxjs/Subject';

import { BaseService } from "./base.service";

@Injectable()
export class AlertService extends BaseService {
    private subject = new Subject<any>();
    private keepSubject = false;

  constructor(private router: Router) {
    super();
    // clear alert message on route change
    router.events.subscribe(event => {
      if (event instanceof NavigationStart) {
        if (this.keepSubject) {
          // only keep for a single location change
          this.keepSubject = false;
        } else {
          // clear alert
          this.subject.next();
        }
      }
    });
  }

  success(message: string, keepSubject = false) {
    this.keepSubject = keepSubject;
    this.subject.next({ type: 'success', text: message });
  }

  error(message: string, keepSubject = false) {
    this.keepSubject = keepSubject;
    this.subject.next({ type: 'error', text: message });
  }

  getMessage(): Observable < any > {
    return this.subject.asObservable();
  }
}
