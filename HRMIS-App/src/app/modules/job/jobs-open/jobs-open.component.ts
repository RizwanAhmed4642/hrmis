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
  selector: "app-jobs-open",
  templateUrl: "./jobs-open.component.html",
  styleUrls: ['./jobs-open.component.scss']
})
export class JobsOpenComponent implements OnInit {
  // @ViewChild("grid") public grid: GridComponent;
  public kGrid: KGridHelper = new KGridHelper();
  public hfOpens: any[] = [];
  public vpProfileStatus: any[] = [];
  public hfsList: any[] = [];
  public jobHFs: any[] = [];
  public documents: any[] = [];
  public jobs: any[] = [];
  public job: any = {};
  public jobHF: any = {};
  public dataItem: any = {};
  public jobDocument: any = {};
  public jobDocuments: any[] = [];
  public hfName: string = '';
  public loading: boolean = false;
  public saveDialogOpened: boolean = false;
  public saving: boolean = false;
  public showHF: boolean = true;
  public searchingHfs: boolean = false;
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
    // this.getDocuments();
    // this.getDesignations();
    //this.handleSearchEvents();
    this.getJobs();
  }

  public saveJob() {
    this.saving = true;
    if (this.job.StartDate) {
      this.job.StartDate = this.job.StartDate.toDateString();
    }
    if (this.job.EndDate) {
      this.job.EndDate = this.job.EndDate.toDateString();
    }
    this.showHF = false;
    this._jobService.saveJob({ job: this.job, JobDocuments: this.jobDocuments, JobHFs: this.jobHFs }).subscribe((res: any) => {
      if (res && res.Id) {
        this._notificationService.notify('success', 'Job Saved');
        this.job = {};
        this.dataItem = {};
        this.jobHF = {};
        this.jobHFs = [];
        this.jobDocument = {};
        this.jobDocuments = [];
        this.dropDowns.selectedFiltersModel.designation = this.dropDowns.defultFiltersModel.designation;
        this.hfName = '';
      }
      this.saving = false;
      this.showHF = true;
      this.saveDialogOpened = false;
      //this.getJobs();
    },
      err => {
        console.log(err);
        this.saving = false;
      }
    );
  }

  public closeWindow() {
    if (!this.dataItem) {
      this.saveDialogOpened = false;
    }
    this.bindJobData(this.dataItem);
  }

  public openWindow() {
    this.saveDialogOpened = true;
  }
  public saveOpenHF() {
    this.jobHFs.push(this.jobHF);
    this.jobHF = {};
    this.hfName = '';
  }

  public getPHFMCVacants() {

    this._jobService.getPHFMCVacants().subscribe((res: any) => {
      if (res) {
        this.jobVacancy = res;
        this.hfsOrigional = this.jobVacancy.hfs;
        this.designationsOrigional = this.jobVacancy.designations;
        this.designations = this.designationsOrigional;
        this.designations.forEach(designation => {
          let job = this.jobs.find(x => x.Designation_Id == designation.Id);
          if (job) {
            designation.on = true;
            designation.AgeLimit = job.AgeLimit;
            designation.Experience = job.Experience;
            designation.RelevantExperience = job.RelevantExperience;
          }
        });
      }
    }, err => {
      console.log(err);
    })
  }
  public searchDesignations(query: string) {
    if (!query) {
      this.designations = this.designationsOrigional;
    }
    this.designations = this.designationsOrigional.filter(x => x.Name.toLowerCase().indexOf(query.toLowerCase()) !== -1);
  } public switchValChange(dataItem, event) {
    if (dataItem) {
      this.job.AgeLimit = dataItem.AgeLimit;
      this.job.Experience = dataItem.Experience;
      this.job.RelevantExperience = dataItem.RelevantExperience;
      this.job.Designation_Id = dataItem.Id;
      this.job.TotalPreferences = dataItem.Vacant;
      this.job.IsActive = event;
      this.saveJob();
    }
  }
  public bindJobData(dataItem) {
    this.job.AgeLimit = dataItem.AgeLimit;
    this.job.Experience = dataItem.Experience;
    this.job.RelevantExperience = dataItem.RelevantExperience;
    this.job.Designation_Id = dataItem.Id;
    this.job.TotalPreferences = dataItem.Vacant;
    this.job.IsActive = event;
    this.saveJob();
  }
  public removeOpenedHF(index: number) {
    this.jobHFs.splice(index, 1);
  }
  public removeJob(item: any) {

    console.log(item);

  }
  public addJobDocument() {
    /*  let i = 0;
     for (let index = 0; index < this.documents.length; index++) {
       const element = this.documents[index];
       if (element.Id == this.jobDocument.Id) {
         i = index;
       }
     }
     this.documents.splice(i, 1); */

    this.jobDocuments.push(this.jobDocument);
    this.jobDocument = {};
  }

  public removeJobDocument(index: number) {
    this.jobDocuments.splice(index, 1);
  }
  private getDesignations = () => {
    this.dropDowns.designations = [];
    this.dropDowns.designationsData = [];
    this._rootService.getDesignations().subscribe((res: any) => {
      if (res) {
        this.dropDowns.designations = res;
        this.dropDowns.designationsData = this.dropDowns.designations.slice();
      }
    },
      err => { console.log(err); }
    );
  }
  private getJobs = () => {
    this.loading = true;
    this.jobs = [];
    this._jobService.getJobs().subscribe((res: any) => {
      if (res) {
        this.jobs = res;
        this.getPHFMCVacants();
      }
      this.loading = false;
    },
      err => {
        console.log(err);
        this.loading = false;
      }
    );
  }
  public search(value: string, filter: string) {
    if (filter == 'hfs') {
      this.hfsList = [];
      if (value.length > 2) {
        this.searchingHfs = true;
        this._rootService.searchHealthFacilitiesAll(value).subscribe((data) => {
          this.hfsList = data as any[];
          this.searchingHfs = false;
        });
      }
    }
  }
  public searchClicked(FullName, filter) {
    if (filter == 'hfs') {
      let item = this.hfsList.find(x => x.FullName === FullName);
      if (item) {
        this.jobHF.HF_Id = item.Id;
        this.jobHF.HFMISCode = item.HFMISCode;
        this.jobHF.HealthFacility = item.FullName;
      }
    }
  }
  private getDocuments = () => {
    this.documents = [];
    this._jobService.getDocuments().subscribe((res: any) => {
      if (res) {
        this.documents = res;
      }
    },
      err => { console.log(err); }
    );
  }
  /* private getDesignations = () => {
    this.dropDowns.designations = [];
    this.dropDowns.designationsData = [];
    this._rootService.getDocu().subscribe((res: any) => {
      if (res && res.List) {
        this.dropDowns.designations = res.List;
        this.dropDowns.designationsData = this.dropDowns.designations.slice();
      }
    },
      err => { console.log(err); }
    );
  } */
  public dropdownValueChanged = (value, filter) => {
    if (!value) {
      return;
    }
    if (filter == 'designation') {
      this.job.Designation_Id = value.Id;
    }
  }
  public handleSearchEvents() {
    this.searchSubcription = this.searchEvent.pipe(
      debounceTime(400)).subscribe((x: any) => {
        this.search(x.event, x.filter);
      });
  }


  public removeVpProfileStatus(status_Id: number) {
    if (confirm('Are you sure?')) {
      this._jobService.removeVpProfileStatus(status_Id).subscribe((res: any) => {
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
