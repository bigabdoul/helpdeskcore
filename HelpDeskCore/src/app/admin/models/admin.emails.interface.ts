
/** Data contract for incoming e-mail settings. */
export interface IncomingMailSettings {
  /** New tickets go to default category */
  newTicketsDefaultCategoryId: number;

  /** Accept emails from unregistered users.
  *
  * When ON, HelpDesk will convert all incoming emails into new tickets. If an email-address
  * is not present in the system yet, HelpDesk will create a new user with a new password.
  * The user can later retrieve his password via the "forgot password" link. When OFF the app
  * will ignore the incoming email optionally sending a "bounce" autoreply.
  */
  acceptEmailsFromUnregisteredUsers: boolean;

  /** Add all emails from CC and TO fields to ticket-subscribers.
  *
  * By default Helpdesk will add all recipients from TO and CC fields to ticket subscribers.
  * Subscribers will receive all updates from that ticket. If this setting is off, only the
  * original ticket sender will be added to subscribers.
  */
  addEmailsFromCcAndToForTicketSubscribers: boolean;

  /** Extract the original sender from forwarded emails and create a ticket on their behalf.
  *
  * You can forward any email to your support mailbox and Helpdesk will try to extract the original
  * sender and create a ticket on his behalf. This is very useful when someone sends an email to
  * your personal mailbox, instead of the support address.
  *
  * Make sure that:
  *
  * Your email (the one that will send the forward) is registered in the helpdesk app and its
  * user-account is assigned the "technician" role.
  *
  * The email has a "Fwd:" or "Fw:" in the subject line, so the app detect a forward and tries to
  * extract the original sender.
  */
  extractOriginalSenderFromForwardedEmailsCreateTicket: boolean;
}

/** Data contract for e-mail notification settings. */
export interface EmailNotificationSettings {
  /** Email notifications enabled? */
  enabled: boolean;

  /** The one users get after submitting a new ticket */
  ticketConfirmationNotification: boolean;

  /** "Ticket closed" notification */
  ticketClosedNotification: boolean;

  /** Notify all administrators of new tickets? */
  notifyAllAdmins: boolean;

  /** Notify technicians of new tickets in their categories? */
  notifyTechs: boolean;

  /**
  * Notify ALL technicians in a category when a customer updates
  * a ticket (not just the ticket-technician and ticket-subscribers)
  */
  notifyAllTechsOnCustomerUpdate: boolean;

  /** Notify ALL technicians in a category when another technician TAKES a ticket */
  notifyAllTechsOnTechTakeOver: boolean;

  /** Include attachments into outgoing notifications? */
  includeAttachmentsInOutgoing: boolean;

  /** Send 'autologin' links in email notifications?
  *
  * By default we send "autologin" links - links users can click and login instantly
  * without the need to enter a password - to ticket submitters in outgoing notifications.
  * We send them only to regular users, not to techs or admins. These links are also
  * time-limited to three days. This is very convenient, but can lead to a potential
  * confusion. When a user forwards his helpdesk notification to someone else, the
  * recipient of the forwarded email can click the autologin link to login under the
  * original user credentials.
  */
  sendAutoLoginLinks: boolean;
}

/** Data contract for outgoing e-mail settings. */
export interface OutgoingEmailSettings {
  /** "From" address. */
  from: string;

  /** "From" name.
  *
  * Example "MyCompany Support Team". Used for all email notifications, except
  * human replies (in this case, the "From" name will be the name of the user or
  * technician who wrote the message)
  */
  fromName: string;

  /** Use 'From Name' for ALL outgoing notifications? */
  useFromNameForAll: boolean;

  /** It is recommended to set the 'Reply-To' address to one of email addressed
  * that is being checked by Helpdesk (see "Incoming mail settings" above). */
  replyTo: string;

  //smtp: SmtpSettings;
}

/** Data contract for SMTP settings. */
export interface SmtpSettings {
  /** SMTP server address (eg. "smtp.server.com") */
  serverAddress: string;

  /** SMTP server port (eg. "25") */
  serverPort: number;

  /** SMTP server requires authentication? */
  requiresAuth: boolean;

  /** SMTP username */
  userName: string;

  /** SMTP password */
  password: string;

  /** Use SSL\TLS to connect to the SMTP server? */
  useSsl: boolean;
}

/** Data contract for e-mail templates. */
export interface EmailTemplate {
  /** The subject of the e-mail. */
  subject: string;

  /** The body of the e-mail. */
  body: string;
}

export interface MimeMessage extends EmailTemplate {
  from: string;
  to: string;
}

/** Data contract for e-mail template settings. */
export interface EmailTemplateSettings {
  /** "New ticket" email template.
  *
  * Sent to technicians when a new ticket arrives. All technicians
  * that have permissions to the category get one of these.
  */
  newTicket: EmailTemplate;

  /** "Ticket-updated" email template
  *
  * Sent to both technicians and ticket-submitter (and all ticket-subscribers if any)
  * when a new reply is added to the ticket.
  */
  ticketUpdated: EmailTemplate;

  /** "Ticket closed" email template
  *
  * Sent to subscribers when a ticket is closed. Note that
  * "Disable ticket closed notifications" setting has to be off.
  */
  ticketClosed: EmailTemplate;

  /** "Ticket confirmation" email template.
  *
  * Sent to the ticket-submitter after the app received his ticket.
  */
  ticketConfirmation: EmailTemplate;

  /** "Welcome to Helpdesk" email template.
  *
  * Sent to new users when they register a new account.
  */
  welcome: EmailTemplate;
}

/** Data contract for e-mail settings. */
export interface EmailSettings {
  /** Incoming mail settings. */
  incoming: IncomingMailSettings;

  /** E-mail notification settings. */
  notifications: EmailNotificationSettings;

  /** Outgoing e-mail settings. */
  outgoing: OutgoingEmailSettings;

  /** SMTP settings. */
  smtp: SmtpSettings;

  /** E-mail template settings. */
  templates: EmailTemplateSettings;
}
