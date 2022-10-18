import { Injectable, EventEmitter } from '@angular/core';
import { Notification } from '../_models/notification.class';
import { Subject } from 'rxjs/Subject';
import { Observable } from 'rxjs/Observable';

@Injectable({
  providedIn: 'root'
})
export class LivePreviewService {
  private subject = new Subject<any>();
  constructor() { }
  update(html) {
    //this.subject.next(html);
    localStorage.setItem('liveP', html);
  }
  updateTokenNumber(tokenNo) {
    //this.subject.next(html);
    localStorage.setItem('tokenNo', tokenNo);
  }
  getLivePreview() {
    return localStorage.getItem('liveP');
  }
  getTokenNumber() {
    return localStorage.getItem('tokenNo');
  }
}
