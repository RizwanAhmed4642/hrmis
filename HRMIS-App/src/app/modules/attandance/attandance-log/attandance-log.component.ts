import {
  Component,
  OnInit,
  Inject,
  ViewChild,
  NgZone,
  OnDestroy
} from "@angular/core";
import { Observable } from "rxjs/Observable";
import { Subscription } from "rxjs/Subscription";
import { NotificationService } from "../../../_services/notification.service";

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
import { KGridHelper } from "../../../_helpers/k-grid.class";
import { DropDownsHR } from "../../../_helpers/dropdowns.class";

import { RootService } from "../../../_services/root.service";
import { AttandanceService } from "../attandance.service";

import { Validators, FormBuilder, FormGroup } from "@angular/forms";
import { AuthenticationService } from "../../../_services/authentication.service";
import { take } from "rxjs/operators/take";
import { DBColumn } from "../../database/database.class";
import { from } from "rxjs/observable/from";
import { delay } from "rxjs/operators/delay";
import { switchMap } from "rxjs/operators/switchMap";
import { map } from "rxjs/operators/map";

import { AttandanceLog } from "./attandance-log.class";

@Component({
  selector: 'app-attandance-log',
  templateUrl: './attandance-log.component.html',
  styles: []
})
export class AttandanceLogComponent implements OnInit {
  public kGrid: KGridHelper = new KGridHelper();

  public dropDowns: DropDownsHR = new DropDownsHR();
  public attandanceLog: AttandanceLog;
  public loading = true;
  private subscription: Subscription;

  //Pagination variables
  public pageSizes = [5, 10, 25, 50];
  public totalRecords = 0;
  public pageSize = 50;
  public skip = 0;

  //gridview
  public attandance: any[] = [];
  public times: any[] = [];
  public currentUser: any;
  public hfmisCode: string;
  public windowWidth = 100;
  //
  //sorting variable
  public multiple = false;
  public allowUnsort = true;
  public sort: SortDescriptor[] = [];

  public gridView: GridDataResult;

  constructor(
    private _rootService: RootService,
    private _attandanceService: AttandanceService,
    private _authenticationService: AuthenticationService,
    private ngZone: NgZone,
    public _notificationService: NotificationService

  ) { }

  // grid
  public sortChange(sort: SortDescriptor[]): void {
    if (sort[0].field == "asd") {
      return;
    }
    this.sort = sort;
    this.sortData();
  }

  private sortData() {
    this.gridView = {
      data: orderBy(this.attandance, this.sort),
      total: this.totalRecords
    };
  }
  //
  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadAttandance();
  }
  public changePagesize(value: any) {
    this.pageSize = +value;
    this.skip = 0;
    this.loadAttandance();
  }

  private loadAttandance() {
    this.loading = true;
    console.log();
    this._attandanceService
      .getAttandanceLog(this.skip, this.pageSize)
      .subscribe(
        response => this.handleResponse(response),
        err => this.handleError(err)
      );
  }


  get showInitialPageSize() {
    return (
      this.pageSizes.filter(num => num === Number(this.pageSize)).length === 0
    );
  }


  private handleResponse(response: any) {
    //debugger;
    console.log(response);
    this.attandance = [];
    this.attandance = response.AttandanceList.List;
    this.times = response.AttandanceTime;
    //  console.log(this.hftypes);
    this.attandance.forEach(attandance => {
      var time = this.times.filter(x => x.Id == attandance.Id && x.date_att == attandance.date_att);
      if (time) {
        attandance.times = time;
        this.calculateTime(attandance);
      }
    });
    this.totalRecords = response.AttandanceList.Count;
    this.gridView = { data: this.attandance, total: this.totalRecords };
    this.loading = false;
  }

  private handleError(err: any) {
    this._notificationService.notify("danger", "Error!");
    this.loading = false;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }
  public calculateTime(item: any) {
    if (item && item.times) {
      let length = item.times.length;
      let c: number = 0;

      for (let index = 0; index < item.times.length; index += 2) {
        const element = item.times[index];
        if (element && item.times[c + 1]) {
          console.log('===================================');
          console.log(index);
          console.log(c);
          console.log(element.DateTime);
          console.log(item.times[c + 1].DateTime);

          item.hourss = this.diff_hours(element.DateTime, item.times[c + 1].DateTime);
        /*   console.log(h);
          item.hourss += +h; */
          console.log(item.hourss);
          console.log('===================================');
          /*   if (item.Name == 'TAYYABA ADNAN') {
              debugger;
            }
            if (h >= 0) {
            } */
        } else {
          break;
        }

        c++;
      }
      /*       if (item.times[0] && item.times[1]) {
              item.hourss = this.diff_hours(item.times[1].DateTime, item.times[0].DateTime);
            } */
    }
  }
  public diff_hours(dt2, dt1) {
    if (!dt1) return;
    if (!dt2) {
      dt2 = new Date();
    } else {
      dt2 = new Date(dt2);
    }
    dt1 = new Date(dt1);
    var diff = (dt2.getTime() - dt1.getTime()) / 1000;
    diff /= (60 * 60);
    return Math.abs(Math.round(diff));

  }
  ngOnInit() {
    this.attandanceLog = new AttandanceLog();
    this.currentUser = this._authenticationService.getUser();
    this.hfmisCode = this.currentUser.HfmisCode;
    this.loadAttandance();
    this.windowWidth = window.innerWidth;
  }

}
