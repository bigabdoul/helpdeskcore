/**
 * Represents the base class for view components.
 */
export abstract class BaseView {

  constructor() {
  }

  /** Whether the current user is an administrator. */
  admin() {
    /* CAUTION:
    Persisting role-based attributes in the local storage
    could lead to security breaches in the app when the
    role for the current user changes on the server.
    */
    return localStorage.getItem('role') === 'admin';
  }

  userId() {
    return localStorage.getItem('id');
  }

  /** Whether the current user is logged-in. */
  loggedIn() {
    return !!localStorage.getItem('auth_token');
  }

}
