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

import { JobService } from "../job.service";
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
//import { ActivatedRoute } from '@angular/router';
//import { LocalService } from '../../../_services/local.service';

@Component({
  selector: "app-job-application",
  templateUrl: "./job-application.component.html",
  styleUrls: ['./job-application.component.scss']
})
export class JobApplicationComponent implements OnInit {
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
  public hfName: string = '';
  public hfmisCode: string = '';
  public loading: boolean = false;
  public saving: boolean = false;
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
    private _jobService: JobService,
    public _notificationService: NotificationService,
    public _rootService: RootService,
    private _authenticationService: AuthenticationService,
    private ngZone: NgZone
  ) { }

  public ngOnInit(): void {
    this.user = this._authenticationService.getUser();
    this.hfmisCode = this.user.HfmisCode;

    this.kGrid.multiple = true;
    this.kGrid.loading = false;
    this.kGrid.pageSize = 150;
    this.kGrid.pageSizes = [50, 100, 200, 300, 500, 1000];
    this.getJobApplicants();
  }

  private getJobApplication = () => {
    this.kGrid.loading = false;
    this.jobApplicants = [];
    this.jobApplications = [];
    let obj = {
      Skip: this.kGrid.skip,
      PageSize: this.kGrid.pageSize,
      applicantId: 0,
      hfmisCode: this.hfmisCode
    };
    this._jobService.getJobApplications(obj).subscribe((res: any) => {
      if (res) {
        this.jobApplicants = res.applicants;
        this.jobApplications = res.applications;

        this.jobApplicants.forEach(jobApplicant => {
          jobApplicant.applications = this.jobApplications.filter(x => x.Applicant_Id == jobApplicant.Id);
        });
        console.log(this.jobApplicants);

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
    this._jobService.getJobApplicants(obj).subscribe((res: any) => {
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
    this.getJobApplication();
  }
  public changePagesize(value: any) {
    this.kGrid.pageSize = +value;
    this.kGrid.skip = 0;
    this.getJobApplication();
  }


  public openPreferencesWindow(dataItem: any) {
    this.preferencesWindow = true;
    this.selectedApplicant = dataItem;

    /* this._jobService.getJobApplicants(obj).subscribe((res: any) => {
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

}
