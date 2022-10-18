import { Injectable, EventEmitter } from '@angular/core';
import { Notification } from '../_models/notification.class';
import { Subject } from 'rxjs/Subject';
import { Observable } from 'rxjs/Observable';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private subject = new Subject<any>();
  constructor() { }
  notify(type: string, content?: string, icon?: string) {
    this.subject.next({ type, content, icon });
  }
  getMessage(): Observable<any> {
    return this.subject.asObservable();
  }
}
