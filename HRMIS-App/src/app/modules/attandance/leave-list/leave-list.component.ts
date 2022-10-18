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

import { Leavelist } from "./Leave-list.class";

@Component({
  selector: 'app-leave-list',
  templateUrl: './leave-list.component.html',
  styleUrls: ['./leave-list.component.scss']
})
export class LeaveListComponent implements OnInit {

  public kGrid: KGridHelper = new KGridHelper();

  public dropDowns: DropDownsHR = new DropDownsHR();
  public leaveList: Leavelist;
  public loading = true;
  private subscription: Subscription;

  //Pagination variables
  public pageSizes = [5, 10, 25, 50];
  public totalRecords = 0;
  public pageSize = 50;
  public skip = 0;

  //gridview
  public leave: any[] = [];
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
  public leaves : any[] = [];

  constructor(
    private _rootService: RootService,
    private _attandanceService: AttandanceService,
    private _authenticationService: AuthenticationService,
    private ngZone: NgZone,
    public _notificationService: NotificationService
  ) { }

  ngOnInit() {
   this.leaveList = new Leavelist();
   
    //this.loadDropdownValues();
    this.currentUser = this._authenticationService.getUser();
    this.hfmisCode = this.currentUser.HfmisCode;
    this.loadLeaves();
    this.windowWidth = window.innerWidth;
  }
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
    data: orderBy(this.leave, this.sort),
    total: this.totalRecords
  };
}
//
public pageChange(event: PageChangeEvent): void {
  this.skip = event.skip;
  this.loadLeaves();
}
public changePagesize(value: any) {
  this.pageSize = +value;
  this.skip = 0;
  this.loadLeaves();
}

public onSearch() {
  this.loading = true;
  this.skip = 0;
  this.loadLeaves();
}

private loadLeaves(){

  //debugger;
 // console.log(this.leaveList.LeaveFrom);
 // console.log(this.leaveList.LeaveTo);

  if (this.leaveList.LeaveFrom && this.leaveList.LeaveTo) {
   
    this._attandanceService.getLeaveList(this.skip, this.pageSize, this.leaveList.LeaveFrom.toDateString(), this.leaveList.LeaveTo.toDateString(), this.leaveList.EmployeeName).subscribe(
      response => this.handleResponse(response),
      err => this.handleError(err)
    ); 

  }
  else{
    
    this._attandanceService.getLeaveList(this.skip, this.pageSize, this.leaveList.LeaveFrom, this.leaveList.LeaveTo, this.leaveList.EmployeeName).subscribe(
      response => this.handleResponse(response),
      err => this.handleError(err)
    ); 

  }

 
}

/* private loadLeaves() {
  this.loading = true;
  console.log();
  this._attandanceService
    .getLeaveList(this.skip, this.pageSize)
    .subscribe(
      response => this.handleResponse(response),
      err => this.handleError(err)
    );
}
 */

get showInitialPageSize() {
  return (
    this.pageSizes.filter(num => num === Number(this.pageSize)).length === 0
  );
}

private handleResponse(response: any) {
  //debugger;
  console.log(response);
  this.leaves = [];
  this.leaves = response.List;
  //  console.log(this.hftypes);
  this.totalRecords = response.Count;
  this.gridView = { data: this.leaves, total: this.totalRecords };
  this.loading = false;
}

private handleError(err: any) {
  this._notificationService.notify("danger", "Error!");
  this.loading = false;
  if (err.status == 403) {
    this._authenticationService.logout();
  }
}


}
