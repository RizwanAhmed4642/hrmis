import { Injectable, EventEmitter } from '@angular/core';
import { Notification } from '../_models/notification.class';
import { Subject } from 'rxjs/Subject';
import { Observable } from 'rxjs/Observable';

@Injectable({
  providedIn: 'root'
})
export class DialogService {
  private subject = new Subject<any>();
  constructor() { }
  openDialog(dialogType: any) {
    this.subject.next(dialogType);
  }
  getDialog(): Observable<any> {
    return this.subject.asObservable();
  }

}
