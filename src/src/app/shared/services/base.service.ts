import { Observable } from 'rxjs/Rx';

export abstract class BaseService {

  constructor() {
  }

  loggedIn() {
    return !!localStorage.getItem('auth_token');
  }

  admin() {
    return this.role() === 'admin';
  }

  tech() {
    return this.role() === 'tech';
  }

  role() {
    return localStorage.getItem('role');
  }

  userId() {
    return localStorage.getItem('id');
  }

  protected handleError(error: any) {
    const applicationError = error.headers.get('Application-Error');

    // either applicationError in header or model error in body
    if (applicationError) {
      return Observable.throw(applicationError);
    }

    let modelStateErrors: string = '';
    const serverError = error.json();

    if (!serverError.type) {
      for (const key in serverError) {
        if (serverError[key])
          modelStateErrors += serverError[key] + '\n';
      }
    }

    modelStateErrors = modelStateErrors = '' ? null : modelStateErrors;
    return Observable.throw(modelStateErrors || 'Server error');
  }
}
