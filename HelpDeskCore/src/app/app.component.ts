import { Component, OnInit, OnDestroy } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";

import { Message } from "primeng/api";
import { HubConnection, HubConnectionBuilder } from "@aspnet/signalr";

import { environment } from "@env/environment";
import { UserService } from "@app/shared/services";
import {
  NotificationMessage,
  MessageType,
  SysEventType,
  OnlineUser
} from "@app/shared/models";
import { Subscription } from "rxjs/Subscription";

@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.scss"]
})
export class AppComponent implements OnInit, OnDestroy {
  private subs: any;
  private navSubs: Subscription;
  private connection: HubConnection;
  private hubStarted: boolean;

  log: Message[] = [];
  messages: Message[] = [];
  onlineUsers: OnlineUser[] = [];
  changingPwd: boolean;
  showPasswordReset: boolean;

  constructor(
    private svc: UserService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.subs = this.svc.onConnection.subscribe(connected => {
      if (connected) {
        if (!this.hubStarted) {
          this.startConnection();
        }
      } else if (this.hubStarted) {
        this.connection.stop().then(() => (this.hubStarted = false));
      }
    });

    if (!this.hubStarted && this.svc.loggedIn()) {
      this.startConnection();
    }

    // check if a password reset is requested
    this.navSubs = this.route.queryParamMap
      .filter(params => +params.get("pwdreset") === 1)
      .subscribe(params => {
        const obj = {
          userName: params.get("username"),
          resetToken: params.get("token")
        };
        localStorage.setItem("passwordReset", JSON.stringify(obj));
        this.showPasswordReset = true;
      });
  }

  ngOnDestroy() {
    if (this.hubStarted) {
      this.connection.stop();
    }

    this.subs.unsubscribe();
    this.navSubs.unsubscribe();
  }

  private startConnection() {
    const users = this.onlineUsers;
    const token = localStorage.getItem("auth_token");

    this.connection = new HubConnectionBuilder()
      .withUrl(`${environment.baseUrl}/notify?access_token=${token}`)
      .build();

    this.connection.on("BroadcastMessage", (m: NotificationMessage) => {
      const { eventType, userId, userName } = m;

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
      } else {
        // no specific user, assume message's for everybody
        this.spreadMessages(msg);
        this.log.unshift(msg);
      }
    });

    this.connection
      .start()
      .then(() => {
        this.hubStarted = true;
        console.log("SignalR connection started!");
      })
      .catch(err =>
        console.error("Error while establishing SignalR connection: " + err)
      );
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
