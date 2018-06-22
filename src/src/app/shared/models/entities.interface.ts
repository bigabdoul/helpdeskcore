export interface EntityListItem {
  id: number;
  name: string;
}

export enum SysEventType {
  loginFailure = 1,
  loginSuccess,
  logout,
  issueCreated,
  issueAssigned,
  issueUpdated,
  issueClosed,
  issueReopened,
  issueDeleted,
  userRegistered,
  userCreated,
  userUpdated,
  userDeleted,
  userImported,
  categoryCreated,
  categoryUpdated,
  categoryDeleted,
  emailConfigUpdated,
  emailSendFailed,
}

export enum SysEventCategory {
  /** Unknown category */
  unknown = -1,

  /** Login/logout category */
  login = 1,

  /** Issues */
  issue,

  /** Users */
  user,

  /** Issue categories */
  category,

  /** Emails */
  email,
}

export enum MessageType {
  success = 1,
  info,
  warn,
  error,
}

export interface NotificationMessage {
  type: MessageType;
  eventType: SysEventType;
  message: any;
  messageId: string;
  userId: string;
  userName: string;
}

export interface OnlineUser {
  userId: string;
  userName: string;
}

export interface ChangePasswordModel {
  userId: string;
  oldPassword?: string;
  newPassword: string;
  confirmNewPassword: string;
}
