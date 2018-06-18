import { Component, OnInit, OnDestroy } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';

import { Message } from "primeng/api";

import { environment } from "@env/environment";
import { UserService } from "@app/shared/services";
import { NotificationMessage, MessageType, SysEventType, OnlineUser } from "@app/shared/models";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, OnDestroy {
  private subs: any;
  private connection: HubConnection;
  private hubStarted: boolean;
  log: Message[] = [];
  messages: Message[] = [];
  onlineUsers: OnlineUser[] = [];

  constructor(private svc: UserService) {
  }

  ngOnInit(): void {

    this.subs = this.svc.onConnection.subscribe(connected => {
      if (connected) {
        if (!this.hubStarted) {
          this.startConnection();
        }
      } else if (this.hubStarted) {
        this.connection.stop().then(() => this.hubStarted = false);
      }
    });

    if (!this.hubStarted && this.svc.loggedIn()) {
      this.startConnection();
    }
  }

  ngOnDestroy() {
    if (this.hubStarted) {
      this.connection.stop();
    }
    if (this.subs && this.subs.unsubscribe) {
      this.subs.unsubscribe();
    }
  }

  private startConnection() {
    const users = this.onlineUsers;
    const token = localStorage.getItem('auth_token');

    this.connection = new HubConnectionBuilder()
      .withUrl(`${environment.baseUrl}/notify?access_token=${token}`)
      .build();

    this.connection.on('BroadcastMessage', (m: NotificationMessage) => {

      const eventType = m.eventType,
        userId = m.userId,
        userName = m.userName;

      switch (eventType) {
        case SysEventType.loginSuccess:
          users.push({ userId, userName });
          break;
        case SysEventType.logout:
        case SysEventType.userDeleted:
          this.removeOnline(userId);
          break;
        default:
          break;
      }

      const id = m.messageId,
        detail = m.message,
        severity = MessageType[m.type],
        msg = { id, severity, detail };

      if (userId) {
        if (userId === this.svc.userId() || this.svc.admin()) {
          // add message only if destined to the current user or if user is admin
          this.spreadMessages(msg);
          this.log.unshift(msg);
        }
      }
      else {
        // no specific user, assume message's for everybody
        this.spreadMessages(msg);
        this.log.unshift(msg);
      }
    });

    this.connection
      .start()
      .then(() => { this.hubStarted = true; console.log('SignalR connection started!'); })
      .catch(err => console.error('Error while establishing SignalR connection: ' + err));
  }

  private spreadMessages(m: Message) {
    // the spread syntax is required for primeng's growl
    // component to work (auto-close alert messages)
    this.messages = [...this.messages, m];
  }

  private removeOnline(userId: string) {
    const users = this.onlineUsers;

    for (let i = 0; i < users.length; i++) {
      if (users[i].userId === userId) {
        users.splice(i, 1);
        break;
      }
    }
  }
}
