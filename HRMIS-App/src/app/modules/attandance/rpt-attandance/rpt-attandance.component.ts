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

import { RptAttandance } from "./rptAttandance.class";
@Component({
  selector: 'app-rpt-attandance',
  templateUrl: './rpt-attandance.component.html',
  styles: []
})
export class RptAttandanceComponent implements OnInit {

  public kGrid: KGridHelper = new KGridHelper();
  public dateNow: string = '';

  public dropDowns: DropDownsHR = new DropDownsHR();
  public rptAttandance: RptAttandance;
  public loading = true;
  public printing = false;
  private subscription: Subscription;
  public savingStatus = false;

  //Pagination variables
  public pageSizes = [5, 10, 25, 50, 100];
  public totalRecords = 0;
  public pageSize = 100;
  public skip = 0;

  //gridview
  public att: any[] = [];
  // public times: any[] = [];
  public currentUser: any;
  public hfmisCode: string;
  public windowWidth = 100;
  //
  //sorting variable
  public multiple = false;
  public allowUnsort = true;
  public sort: SortDescriptor[] = [];

  public gridView: GridDataResult;
  public rptlist: any[] = [];
  public rptlistPrint: any[] = [];
  public EmpDialogOpened: boolean = false;
  public EmpEditOpened: boolean = false;
  public selectedEmp: any;
  public dayn: string;
  public EmpName: string;

  constructor(
    private _rootService: RootService,
    private _attandanceService: AttandanceService,
    private _authenticationService: AuthenticationService,
    private ngZone: NgZone,
    public _notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.rptAttandance = new RptAttandance();

    let today = new Date();
    let dd: any = today.getDate(), mm: any = today.getMonth() + 1, yyyy = today.getFullYear();
    if (dd < 10) dd = '0' + dd; if (mm < 10) mm = '0' + mm;
    this.dateNow = dd + '/' + mm + '/' + yyyy;

    var day = today.getDay();

    var weekday = new Array(7);
    weekday[0] = "Sunday";
    weekday[1] = "Monday";
    weekday[2] = "Tuesday";
    weekday[3] = "Wednesday";
    weekday[4] = "Thursday";
    weekday[5] = "Friday";
    weekday[6] = "Saturday";



    let yesterday = new Date(today);
    //this.rptAttandance.From =  yesterday.setDate(yesterday.getDate() - 1);
    this.rptAttandance.From = today;
    this.rptAttandance.To = today;
    if (this.rptAttandance.From == this.rptAttandance.To) {
      this.dayn = weekday[day];
    }
    else {
      this.dayn = "";
    }

    this.loadDropdownValues();
    this.currentUser = this._authenticationService.getUser();
    this.hfmisCode = this.currentUser.HfmisCode;
    this.loadAttandance();
    this.windowWidth = window.innerWidth;
  }

  private loadDropdownValues = () => {
    this.getEmpStatus();
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
      data: orderBy(this.att, this.sort),
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

  public onSearch() {
    this.loading = true;
    this.skip = 0;
    this.loadAttandance();
  }
  public editEmpStatus() {
    this.rptAttandance.DateOnlyRecord = this.selectedEmp.LogDate;
    this.rptAttandance.IndRegID = this.selectedEmp.IndRegID;
    this._attandanceService
      .editEmpStatus(this.rptAttandance, this.selectedEmp.Id)
      .subscribe(
        (response: any) => {
          console.log(response);
          if (response) {
            this.savingStatus = false;
            this.closeEditWindow();
            this._notificationService.notify(
              "success",
              "Status is Updated!"
            );
            this.selectedEmp = "";
            this.loadAttandance();
          }
        },
        err => {
          console.log(err);
        }
      );
  }

  private loadAttandance() {
    if (this.rptAttandance.EmployeeName) {
      this.EmpName = "Employee Name : "
    }
    else {
      this.EmpName = "";
    }
    this._attandanceService.getAttendanceList(this.skip, this.pageSize, this.rptAttandance.From.toDateString(), this.rptAttandance.To.toDateString(), this.rptAttandance.EmployeeName).subscribe(
      response => this.handleResponse(response),
      err => this.handleError(err)
    );
  }

  public loadAttandanceRpt() {
    //debugger;

    /* if(this.rptAttandance.From ==  this.rptAttandance.To)
    {
      this.dayn.visible = "true";
    }
    else
    {
      this.dayn="";
    } */

    this.printing = true;
    if (this.rptAttandance.EmployeeName) {
      this.EmpName = "Employee Name : "
    }
    else {
      this.EmpName = "";
    }
    this._attandanceService.getAttendanceListRpt(this.rptAttandance.From.toDateString(), this.rptAttandance.To.toDateString(), this.rptAttandance.EmployeeName).subscribe(
      (response: any) => {
        if (response && response.List) {
          this.rptlistPrint = response.List;
          setTimeout(() => {
            this.printing = false;
            this.printApplication();
          }, 800);
        }
      },
      err => this.handleError(err)
    );
  }

  public dropdownValueChanged = (value, filter) => {
    debugger;
    if (!value) {
      return;
    }
    if (filter == 'LogStatus') {
      this.rptAttandance.LogStatus = value.Name;
      if (value.Name == 'Morning' || value.Name == 'Short Leave') {

      }
      else {
        this.rptAttandance.IsLate = 0;
      }

    }
  }


  private getEmpStatus = () => {
    this.dropDowns.EmpStatus = [];
    this.dropDowns.EmpStatusData = [];
    this._attandanceService.getEmpStatus().subscribe(
      (res: any) => {
        this.dropDowns.EmpStatus = res;
        this.dropDowns.EmpStatusData = this.dropDowns.EmpStatus.slice();
      },
      err => {
        this.handleError(err);
      }
    );
  };

  public profileSearch1() {
    //debugger;
    this._attandanceService.getTotalLeaves1(this.selectedEmp.HrId).subscribe(
      (lev: any) => {
        if (lev) {

          if (lev.TotalSL == 0) {
            this.rptAttandance.TotalSL = lev.TotalSL;
          }
          else if (lev.TotalSL == 0.5) {
            this.rptAttandance.TotalSL = 1;
          }
          else if (lev.TotalSL > 0.5) {
            this.rptAttandance.TotalSL = lev.TotalSL / 0.5;
          }
          else {
            this.rptAttandance.TotalSL = lev.TotalSL;
          }

          this.rptAttandance.TotalSick = lev.TotalSick;
          this.rptAttandance.TotalCasual = lev.TotalCasual;
          this.rptAttandance.TotalLev = lev.TotalLev;
          this.rptAttandance.Bal = lev.Bal;
        }
      }
    );
  }

  public openEmpWindow(at: any) {

    this.selectedEmp = at;
    console.log(this.selectedEmp);
    this.profileSearch1();
    this.EmpDialogOpened = true;
  }
  public closeEmpWindow() {
    this.selectedEmp = null;
    this.EmpDialogOpened = false;
  }

  public openEditWindow(rptAttandance: any) {
    debugger;
    this.selectedEmp = rptAttandance;
    console.log(this.selectedEmp);
    this.profileSearch1();
    this.EmpEditOpened = true;
  }
  public closeEditWindow() {
    this.selectedEmp = null;
    this.EmpEditOpened = false;
  }

  get showInitialPageSize() {
    return (
      this.pageSizes.filter(num => num === Number(this.pageSize)).length === 0
    );
  }

  private handleResponse(response: any) {
    //debugger;
    console.log(response);
    this.rptlist = [];
    this.rptlist = response.List;
    //this.rptlist = response;
    this.totalRecords = response.Count;
    this.gridView = { data: this.rptlist, total: this.totalRecords };
    this.loading = false;
  }

  private handleError(err: any) {
    this._notificationService.notify("danger", "Error!");
    this.loading = false;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
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


}
