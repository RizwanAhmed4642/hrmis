import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import { Notification } from '../_models/notification.class';
import { NotificationService } from '../_services/notification.service';

@Component({
  selector: 'app-notification',
  template: `
  <div id="note" *ngIf="notification?.show" [ngClass]="notification.type">
  <i [ngClass]="'fa ' + notification.icon"></i> &nbsp;{{notification.content ? notification.content : ''}}
</div>`
})
export class NotificationComponent implements OnInit, OnDestroy {
  public notification: Notification;
  private subscription: Subscription;
  constructor(private _notificationService: NotificationService) { }
  ngOnInit() {
    this.subscription = this._notificationService.getMessage().subscribe(
      (x: any) => {
        this.notification = new Notification(x.type, x.content, x.icon);
        setTimeout(() => { this.notification = null; }, 4500);
      });
  }
  ngOnDestroy() {
    this.subscription.unsubscribe();
  }
}
