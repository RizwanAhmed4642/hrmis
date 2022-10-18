import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';

import { SortDescriptor, orderBy } from '@progress/kendo-data-query';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { AuthenticationService } from '../../../_services/authentication.service';
import { RootService } from '../../../_services/root.service';
import { from } from 'rxjs/observable/from';
import { delay } from 'rxjs/operators/delay';
import { switchMap } from 'rxjs/operators/switchMap';
import { map } from 'rxjs/operators/map';
import { ProfileService } from '../profile.service';
import { ProfileCompactView } from '../profile.class';
import { Subscription } from 'rxjs/Subscription';
import { ActivatedRoute, Router } from '@angular/router';
import { LocalService } from '../../../_services/local.service';
import { NotificationService } from '../../../_services/notification.service';

@Component({
  selector: 'app-my-account',
  template: ` `,
})
export class MyAccountComponent implements OnInit, OnDestroy {

  public currentUser: any;


  constructor(private router: Router, private route: ActivatedRoute, private _notificationService: NotificationService, private _authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.currentUser = this._authenticationService.getUser();
    if (this.currentUser.cnic && this.currentUser.cnic.length == 13) {
      this.router.navigate(['/profile/' + this.currentUser.cnic + '/' + this.currentUser.Id + '/edit']);
    } else {
      this._notificationService.notify('warning', 'No User Profile Exist!');
    }
  }

  ngOnDestroy() {
  }
}
