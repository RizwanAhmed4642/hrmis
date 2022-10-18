import { Observable } from "rxjs/Observable";
import {
  Component,
  OnInit,
  Inject,
  ViewChild,
  NgZone,
  OnDestroy
} from "@angular/core";
import { Validators, FormBuilder, FormGroup } from "@angular/forms";
import { AuthenticationService } from "../../../_services/authentication.service";
import {
  GridDataResult,
  PageChangeEvent,
  GridComponent
} from "@progress/kendo-angular-grid";
import {
  State,
  process,
  SortDescriptor,
  orderBy
} from "@progress/kendo-data-query";
import { NotificationService } from "../../../_services/notification.service";

import { DatabaseService } from "../database.service";
import { KGridHelper } from "../../../_helpers/k-grid.class";
import { take } from "rxjs/operators/take";
import { DropDownsHR } from "../../../_helpers/dropdowns.class";

import { from } from "rxjs/observable/from";
import { delay } from "rxjs/operators/delay";
import { switchMap } from "rxjs/operators/switchMap";
import { map } from "rxjs/operators/map";
import { Subscription } from "rxjs/Subscription";
import { cstmdummyActiveStatus } from "../../../_models/cstmdummydata";
//import { ActivatedRoute } from '@angular/router';
//import { LocalService } from '../../../_services/local.service';

@Component({
  selector: "app-services",
  templateUrl: "./vp-profile-status.component.html",
  styles: []
})
export class VpProfileStatusComponent implements OnInit {
  // @ViewChild("grid", {static: false}) public grid: GridComponent;
  public kGrid: KGridHelper = new KGridHelper();
  public hrProfileStatus: any[] = [];
  public vpProfileStatus: any[] = [];
  constructor(
    private _databaseService: DatabaseService,
    public _notificationService: NotificationService,
    private _authenticationService: AuthenticationService,
    private ngZone: NgZone
  ) { }

  public ngOnInit(): void {
    this.getVpProfileStatus();
  }

  public getVpProfileStatus() {
    this.hrProfileStatus = [];
    this._databaseService.getVpProfileStatus('HrProfileStatus').subscribe((res: any) => {
      if (res) {
        this.hrProfileStatus = res;
        this._databaseService.getVpProfileStatus('VpProfileStatus').subscribe((res2: any) => {
          if (res2) {
            this.vpProfileStatus = res2;
            this.checkOnOffStatuses(); // and set data inside function
          }
        }, err => {
          console.log(err);
        });
      }
    }, err => {
      console.log(err);
    });

  }
  public addVpProfileStatus(status_Id: number) {
    if (confirm('Are you sure?')) {
      this._databaseService.addVpProfileStatus(status_Id).subscribe((res: any) => {
        if (res) {
          this._notificationService.notify('success', 'Link Created');
          // this.getVpProfileStatus();
        }
      }, err => {
        console.log(err);
      });
    }
  }
  public switchValChange(dataItem) {
    if (dataItem.IsActive) {
      this.addVpProfileStatus(dataItem.Id);
    } else {
      this.removeVpProfileStatus(dataItem.Id);
    }
  }
  private checkOnOffStatuses() {
    let filtered: any[] = [];
    console.log(this.vpProfileStatus);
    this.vpProfileStatus.forEach(elem => {
      console.log(elem);
    });
    console.log(this.hrProfileStatus);
    this.hrProfileStatus.forEach(profileStatus => {
      if (profileStatus.Id && profileStatus.Name) {
        let checkedProfileStatus = this.vpProfileStatus.find(x => x.ProfileStatus_Id == profileStatus.Id && x.IsActive == true);
        console.log(checkedProfileStatus);
        if (checkedProfileStatus) {
          profileStatus.IsActive = true;
        } else {
          profileStatus.IsActive = false;
        }
        filtered.push(profileStatus);
      }
    });
    this.hrProfileStatus = filtered;
  }
  public removeVpProfileStatus(status_Id: number) {
    if (confirm('Are you sure?')) {
      this._databaseService.removeVpProfileStatus(status_Id).subscribe((res: any) => {
        if (res) {
          this._notificationService.notify('success', 'Link Removed');
          //this.getVpProfileStatus();
        }
      }, err => {
        console.log(err);
      });
    }
  }
}
