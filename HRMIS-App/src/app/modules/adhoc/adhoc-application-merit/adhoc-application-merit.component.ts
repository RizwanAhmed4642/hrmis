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

import { KGridHelper } from "../../../_helpers/k-grid.class";
import { take } from "rxjs/operators/take";
import { DropDownsHR } from "../../../_helpers/dropdowns.class";

import { from } from "rxjs/observable/from";
import { delay } from "rxjs/operators/delay";
import { switchMap } from "rxjs/operators/switchMap";
import { map } from "rxjs/operators/map";
import { Subscription } from "rxjs/Subscription";
import { cstmdummyActiveStatus } from "../../../_models/cstmdummydata";
import { RootService } from "../../../_services/root.service";
import { Subject } from "rxjs/Subject";
import { debounceTime } from "rxjs/operators/debounceTime";
import { AdhocService } from "../adhoc.service";
//import { ActivatedRoute } from '@angular/router';
//import { LocalService } from '../../../_services/local.service';

@Component({
  selector: "app-adhoc-application-merit",
  templateUrl: "./adhoc-application-merit.component.html",
  styleUrls: ['./adhoc-application-merit.component.scss']
})
export class AdhocApplicationMeritComponent implements OnInit {
  // @ViewChild("grid") public grid: GridComponent;
  public kGrid: KGridHelper = new KGridHelper();
  public hfOpens: any[] = [];
  public vpProfileStatus: any[] = [];
  public hfsList: any[] = [];
  public jobHFs: any[] = [];
  public documents: any[] = [];
  public jobApplications: any[] = [];
  public user: any = {};
  public job: any = {};
  public jobHF: any = {};
  public jobDocument: any = {};
  public selectedApplicant: any = {};
  public jobDocuments: any[] = [];
  public jobApplicants: any[] = [];
  public dateNow: string = '';
  public format = "dd/MM/yyyy HH:mm";
  public hfName: string = '';
  public hfmisCode: string = '';
  public loading: boolean = false;
  public saving: boolean = false;
  public scheduled: boolean = false;
  public scheduledWindow: boolean = false;
  public showHF: boolean = true;
  public searchingHfs: boolean = false;
  public preferencesWindow: boolean = false;
  public searchEvent = new Subject<any>();
  public searchSubcription: Subscription;
  public dropDowns: DropDownsHR = new DropDownsHR();
  public jobVacancy: any = {};
  public hfs: any[] = [];
  public hfsOrigional: any[] = [];
  public designations: any[] = [];
  public designationsOrigional: any[] = [];
  constructor(
    private _adhocService: AdhocService,
    public _notificationService: NotificationService,
    public _rootService: RootService,
    private _authenticationService: AuthenticationService,
    private ngZone: NgZone
  ) { }

  public ngOnInit(): void {
    this.user = this._authenticationService.getUser();
    this.hfmisCode = this.user.HfmisCode;
    let today = new Date();
    let dd: any = today.getDate(), mm: any = today.getMonth() + 1, yyyy = today.getFullYear();
    if (dd < 10) dd = '0' + dd; if (mm < 10) mm = '0' + mm;
    this.dateNow = dd + '/' + mm + '/' + yyyy;
    this.kGrid.multiple = true;
    this.kGrid.loading = false;
    this.kGrid.pageSize = 150;
    this.kGrid.pageSizes = [50, 100, 200, 300, 500, 1000];
    this.getApprovedJobApplications();
  }

  private getApprovedJobApplications = () => {
    this.kGrid.loading = false;
    this.jobApplications = [];
    let obj = {
      Skip: this.kGrid.skip,
      PageSize: this.kGrid.pageSize,
      applicantId: 0,
      hfmisCode: this.hfmisCode
    };
    this._adhocService.getApprovedJobApplications(obj).subscribe((res: any) => {
      if (res) {
        this.jobApplicants = res.applicants;
        this.jobApplications = res.applications;
        this.kGrid.data = [];

        this.jobApplicants.forEach(jobApplicant => {
          jobApplicant.applications = this.jobApplications.filter(x => x.Applicant_Id == jobApplicant.Id);
        });
        console.log(this.jobApplicants);

        this.kGrid.data = [];
        this.kGrid.data = this.jobApplicants;
        this.kGrid.totalRecords = res.totalRecords;
        this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords };
        console.log('grid: ', this.kGrid.data);

        this.kGrid.loading = false;
      }
    },
      err => {
        console.log(err);
        this.loading = false;
      }
    );
  }

  private getJobApplicants = () => {
    this.kGrid.loading = false;
    this.jobApplicants = [];
    this.jobApplications = [];
    let obj = {
      Skip: this.kGrid.skip,
      PageSize: this.kGrid.pageSize,
      applicantId: 0,
      hfmisCode: this.hfmisCode
    };
    this._adhocService.getAdhocApplicants(obj).subscribe((res: any) => {
      if (res) {
        this.jobApplicants = res.List;
        this.kGrid.data = [];
        this.kGrid.data = res.List;
        this.kGrid.totalRecords = res.Count;
        this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords };
        console.log(this.kGrid.gridView);

        this.kGrid.loading = false;
      }
    },
      err => {
        console.log(err);
        this.loading = false;
      }
    );
  }
  public sortChange(sort: SortDescriptor[]): void {
    if (sort[0].field == 'asd') { return; }
    this.kGrid.sort = sort;
    this.sortData();
  }
  private sortData() {
    this.kGrid.gridView = {
      data: orderBy(this.kGrid.data, this.kGrid.sort),
      total: this.kGrid.data.length
    };
  }
  public pageChange(event: PageChangeEvent): void {
    this.kGrid.skip = event.skip;
    this.getApprovedJobApplications();
  }
  public changePagesize(value: any) {
    this.kGrid.pageSize = +value;
    this.kGrid.skip = 0;
    this.getApprovedJobApplications();
  }
  public scheduledInt() {
    this.scheduled = true;
    this.scheduledWindow = false;
  }
  public openPreferencesWindow(dataItem: any) {
    this.preferencesWindow = true;
    this.selectedApplicant = dataItem;
    this._adhocService.getApplicantDocuments(this.selectedApplicant.Id).subscribe((x: any) => {
      if (x) {
        this.selectedApplicant.ApplicantDocuments = x;
      }
    }, err => {
      console.log(err);
    });
    this._adhocService.getExperiences(this.selectedApplicant.Id).subscribe((x: any) => {
      if (x) {
        this.selectedApplicant.ApplicantExperiences = x;
      }
    }, err => {
      console.log(err);
    });
    this._adhocService.getAdhocApplication(this.selectedApplicant.Id).subscribe((x: any) => {
      if (x) {

        this.selectedApplicant.Applications = x.recentApplication;
        this.selectedApplicant.Preferences = x.recentPreference;
        console.log(this.selectedApplicant.Applications);
      }
    }, err => {
      console.log(err);
    });
    /* this._adhocService.getJobApplicants(obj).subscribe((res: any) => {
      if (res) {
        this.jobApplicants = res;
        this.kGrid.data = [];
        this.kGrid.data = this.jobApplicants;
        this.kGrid.totalRecords = res.totalRecords;
        this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords };
        this.kGrid.loading = false;
      }

    },
      err => {
        console.log(err);
        this.loading = false;
      }
    ); */
  }

  public closePreferencesWindow() {
    this.preferencesWindow = false;
    this.selectedApplicant = {};
  }

  public dashifyCNIC(cnic: string) {
    if (cnic) {
      return cnic[0] +
        cnic[1] +
        cnic[2] +
        cnic[3] +
        cnic[4] +
        '-' +
        cnic[5] +
        cnic[6] +
        cnic[7] +
        cnic[8] +
        cnic[9] +
        cnic[10] +
        cnic[11] +
        '-' +
        cnic[12];
    }
  }

  printApplication() {
    let html = document.getElementById('formPrint').innerHTML;
    var mywindow = window.open('', 'PRINT', 'height=600,width=900');
    if (mywindow) {
      mywindow.document.write(`<html><head>
     
    <style>
   
    body {
      padding: 30px 50px 0 50px;
      font-family: -apple-system,system-ui;
  }
  table.file-table, table.file-table th, table.file-table td {
    border: 1px solid black;
    border-collapse: collapse;
  }
  table.file-table td {
    padding: 0.5rem;
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
    <title>Application</title>`);
      mywindow.document.write('</head><body >');
      mywindow.document.write('<button onclick="printFunc()" class="print">Print</button>');
      mywindow.document.write(html);
      mywindow.document.write(`
      <script>
  function printFunc() {
  window.print();
  }
  </script>
      `);
      mywindow.document.write('</body></html>');
      /* this.dropDowns.selectedFiltersModel.inboxOfficers = { Name: 'Select Office', Id: 0 };
      this.dropdownValueChanged(this.dropDowns.selectedFiltersModel.inboxOfficers, 'office'); */
      //this.getFileMovements();
      /*     mywindow.document.close(); // necessary for IE >= 10
          mywindow.focus(); // necessary for IE >= 10
          mywindow.print();
          mywindow.close(); */
    }
  }
}

