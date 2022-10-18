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

import { Leavelist } from "../leave-list/Leave-list.class";

@Component({
  selector: 'app-rpt-leave',
  templateUrl: './rpt-leave.component.html',
  styleUrls: ['./rpt-leave.component.scss']
})
export class RptLeaveComponent implements OnInit {

  public kGrid: KGridHelper = new KGridHelper();
  public dropDowns: DropDownsHR = new DropDownsHR();
  public leaveList: Leavelist;
  public loading = true;
  private subscription: Subscription;

  //Pagination variables
  public pageSizes = [5, 10, 25, 50, 100];
  public totalRecords = 0;
  public pageSize = 100;
  public skip = 0;
  public printing = false;
  public rptlistPrint: any[] = [];
  //gridview
  public leave: any[] = [];
  public times: any[] = [];
  public currentUser: any;
  public hfmisCode: string;
  public windowWidth = 100;
  public EmpDialogOpened: boolean = false;
  public selectedEmp: any;
  public rptlist: any[] = [];
  public EmpName: string;

  //sorting variable
  public multiple = false;
  public allowUnsort = true;
  public sort: SortDescriptor[] = [];

  public gridView: GridDataResult;
  public leaves: any[] = [];

  public yearItems: any[] = [
    // { text: 'Select Year', value: 0 },
    { text: '2020', value: '2020' }
  ]
  public loadingDetail = false;

  /*   public monthItems: any[] = [
      { text: 'January', value: '1' },
      { text: 'February', value: '2' }
    ]
    public defaultItem: { text: string, value: number } =   { text: 'January', value: 1 }; */
  constructor(
    private _rootService: RootService,
    private _attandanceService: AttandanceService,
    private _authenticationService: AuthenticationService,
    private ngZone: NgZone,
    public _notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.leaveList = new Leavelist();
    //debugger;
    this.leaveList.Month = 1;
    this.leaveList.MonthName = "January";
    this.leaveList.Year = 2020;
    this.getMonths();


    //console.log(this.leaveList.Month);
    //console.log(this.leaveList.Year);
    // this.leaveList.Month = 1;
    // this.loadDropdownValues();
    this.currentUser = this._authenticationService.getUser();
    this.hfmisCode = this.currentUser.HfmisCode;
    this.loadLeaves();
    this.windowWidth = window.innerWidth;
  }

  private getMonths = () => {
    debugger;
    this.dropDowns.months = [];
    this.dropDowns.monthsData = [];
    this._attandanceService.getMonths(this.leaveList.Year).subscribe(
      (res: any) => {
        this.dropDowns.months = res;
        this.dropDowns.monthsData = this.dropDowns.months.slice();
      },
      err => {
        this.handleError(err);
      }
    );
  };

  public dropdownValueChanged = (value, filter) => {
    debugger;
    if (filter == 'month') {

      if (value == 1) {
        this.leaveList.MonthName = "January";
      }
      else if (value == 2) {
        this.leaveList.MonthName = "Feburary";
      }
      else if (value == 3) {
        this.leaveList.MonthName = "March";
      }
      else if (value == 4) {
        this.leaveList.MonthName = "April";
      }
      else if (value == 5) {
        this.leaveList.MonthName = "May";
      }
      else if (value == 6) {
        this.leaveList.MonthName = "June";
      }
      else if (value == 7) {
        this.leaveList.MonthName = "July";
      }
      else if (value == 8) {
        this.leaveList.MonthName = "August";
      }
      else if (value == 9) {
        this.leaveList.MonthName = "September";
      }
      else if (value == 10) {
        this.leaveList.MonthName = "October";
      }
      else if (value == 11) {
        this.leaveList.MonthName = "November";
      }
      else if (value == 12) {
        this.leaveList.MonthName = "December";
      }
      else if (value == 0) {
        this.leaveList.MonthName = "";
      }
    }
    //debugger;
    //console.log(this.leaveList.MonthName);
  }

  private loadDropdownValues = () => {
    this.getYear();
  };

  private getYear = () => {
    // debugger;
    this.dropDowns.years = [];
    this.dropDowns.yearsData = [];
    this._attandanceService.getEmpYear().subscribe(
      (res: any) => {

        // debugger;
        // console.log(res);
        this.dropDowns.years = res;
        //  console.log(this.dropDowns.yearsData);
        this.dropDowns.yearsData = this.dropDowns.years.slice();
        // console.log(this.dropDowns.yearsData);
      },
      err => {
        this.handleError(err);
      }
    );
  };

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

  public loadAttandanceRec(parm: string) {
    //debugger;

    this._attandanceService.getAttendanceRec(this.selectedEmp.HrId, this.selectedEmp.Year, this.selectedEmp.Month, this.selectedEmp.IsLate, parm).subscribe(
      (response: any) => {
        if (response && response.List) {
          this.rptlist = response.List;
        }
        this.loadingDetail = false;
      }
    );
  }

  public loadLeaves() {
    //debugger;
    //console.log(response);
    this.loading = true;
    this._attandanceService.getLeaveReport(this.skip, this.pageSize, this.leaveList.EmployeeName, this.leaveList.Year, this.leaveList.Month).subscribe(
      response => this.handleResponse(response),
      err => this.handleError(err)
    );
  }

  public loadLeavesRec(parm: string) {
    debugger;

    this._attandanceService.loadLeavesRec(this.selectedEmp.HrId, this.selectedEmp.Year, this.selectedEmp.Month, parm).subscribe(
      (response: any) => {
        // console.log(response1);
        /*  if(response && response.List){
           this.rptlist = response.List;
         } */
        if (response) {
          this.rptlist = response;
        }
        console.log(this.rptlist);
        this.loadingDetail = false;
      }
    );

    /* this.loading = true;
     this._attandanceService.loadLeavesRec(this.selectedEmp.HrId, this.selectedEmp.Year,this.selectedEmp.Month).subscribe(
         response => this.handleResponse(response),
         err => this.handleError(err)
       ); */
  }


  public onSearch() {
    this.loading = true;
    this.skip = 0;
    this.loadLeaves();
  }

  public loadAttandanceRpt() {
    debugger;
    console.log(this.leaveList.Month);
    /*   if(this.leaveList.Month = 1){
        this.leaveList.MonthName = "January";
      }
      else if(this.leaveList.Month = 2){
        this.leaveList.MonthName = "Feburary";
      }
      else if(this.leaveList.Month = 3){
        this.leaveList.MonthName = "March";
      }
      else if(this.leaveList.Month = 4){
        this.leaveList.MonthName = "April";
      }
      else if(this.leaveList.Month = 5){
        this.leaveList.MonthName = "May";
      }
      else if(this.leaveList.Month = 6){
        this.leaveList.MonthName = "June";
      }
      else if(this.leaveList.Month = 7){
        this.leaveList.MonthName = "July";
      }
      else if(this.leaveList.Month = 8){
        this.leaveList.MonthName = "August";
      }
      else if(this.leaveList.Month = 9){
        this.leaveList.MonthName = "September";
      }
      else if(this.leaveList.Month = 10){
        this.leaveList.MonthName = "October";
      }
      else if(this.leaveList.Month = 11){
        this.leaveList.MonthName = "November";
      }
      else if(this.leaveList.Month = 12){
        this.leaveList.MonthName = "December";
      }
     */

    this.printing = true;

    if (this.leaveList.EmployeeName) {
      this.EmpName = "Employee Name : "
    }
    else {
      this.EmpName = "";
    }
    this._attandanceService.getLeaveRpt(this.leaveList.EmployeeName, this.leaveList.Year, this.leaveList.Month).subscribe(
      (response: any) => {
        debugger;
        if (response) {
          this.rptlistPrint = response;
          setTimeout(() => {
            this.printing = false;
            this.printApplication();
          }, 500);
          // this.leaveList.MonthName = response.MonthName;
          //  this.leaveList.Year = response.Year;
        }
      },
      err => this.handleError(err)
    );
  }

  printApplication() {
    let html = document.getElementById('formPrint').innerHTML;
    var mywindow = window.open('', 'PRINT', 'height=600,width=900');
    if (mywindow) {
      mywindow.document.write(`<html><head>
     
    <style>
   
    body {
      font-family: arial;
  }
  p {
    margin-top: 0;
    margin-bottom: 1rem !important;
}.mt-2 {
  margin-top: 0.5rem !important;
}.mb-0 {
  margin-bottom: 0 !important;
}
.ml-1 {
  margin-left: 0.25rem !important;
}
.mb-2 {
  margin-bottom: 0.5rem !important;
}
button.print {
  padding: 10px 40px;
  font-size: 21px;
  position: absolute;
  margin-left: 40%;
  background: #46a23f;
  background-image: linear-gradient(rgba(63, 162, 79, 0), rgba(63, 162, 79, 0.2));
  cursor: pointer;
  border: none;
  color: #ffffff;
  z-index: 9999;
}
@media print {
  button.print {
    display: none;
  }
}
    </style>
    <title></title>`);
      mywindow.document.write('</head><body >');

      mywindow.document.write(html);
      mywindow.document.write(`
      <script>
function printFunc() {
  window.print();
}
</script>
      `);
      mywindow.document.write('<button onclick="printFunc()" class="print">Print</button>');
      mywindow.document.write('</body></html>');
      //   this.skip =0;

    }
  }

  get showInitialPageSize() {
    return (
      this.pageSizes.filter(num => num === Number(this.pageSize)).length === 0
    );
  }

  private handleResponse(response: any) {

    console.log(response);
    this.leaves = [];
    //this.leaves = response.List;
    this.leaves = response;
    console.log(this.leaves);
    console.log(this.totalRecords);
    // this.totalRecords = response.Count;
    this.totalRecords = response.length;
    this.gridView = { data: this.leaves, total: this.totalRecords };
    this.loading = false;
  }

  public openEmpWindow(at: any, parm: string) {
    debugger;
    this.loadingDetail = true;
    this.EmpDialogOpened = true;
    this.selectedEmp = at;
    if (parm == "Late") {
      this.selectedEmp.IsLate = 1;
      this.loadAttandanceRec(parm);
    }
    if (parm == "Absent") {
      this.loadLeavesRec(parm);
    }
    if (parm == "Casual") {
      this.loadLeavesRec(parm);
    }
    if (parm == "Sick") {
      this.loadLeavesRec(parm);
    }
    if (parm == "Short") {
      this.loadLeavesRec(parm);
    }

  }

  public closeEmpWindow() {
    this.selectedEmp = null;
    this.rptlist = [];
    this.loadingDetail = false;
    this.EmpDialogOpened = false;
  }

  private handleError(err: any) {
    this._notificationService.notify("danger", "Error!");
    this.loading = false;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }

}
