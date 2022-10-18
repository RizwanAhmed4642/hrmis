import { Component, OnInit } from '@angular/core';
import { KGridHelper } from '../../../_helpers/k-grid.class';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { Subject } from 'rxjs/Subject';
import { NotificationService } from '../../../_services/notification.service';
import { RootService } from '../../../_services/root.service';
import { FileTrackingSystemService } from '../../../file-tracking-system/file-tracking-system.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { orderBy, SortDescriptor } from '@progress/kendo-data-query';
import { PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subscription } from 'rxjs/Subscription';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { DomSanitizer } from '@angular/platform-browser';
import { PublicPortalService } from '../public-portal.service';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styles: [`
  .k-grid-content{
    overflow-y: auto !important;
  }
  `]
})
export class OrdersComponent implements OnInit {

  public kGrid: KGridHelper = new KGridHelper();
  public dropDowns: DropDownsHR = new DropDownsHR();
  public inputChange: Subject<any>;
  public skipChange: Subject<any>;
  public searchQuery: string = '';
  public totalPreferences: number = 0;
  public ActiveDesignationId: number = 68;
  public designationId: number = 1320;
  public selectedJobId: number = 0;
  public currentUser: any;
  public filters: any = {};
  public user: any = {};
  public meritPosting: any = {};
  public esr: any = {};
  public profile: any = {};
  public selectedApplicant: any;
  public selectedApplicantPreferences: any[] = [];
  public hfOpened: any[] = [];
  public selectedHF: any = {};
  public actualPostingHF_Id: number = 0;
  public actualPostingHFMISCode: string = '0';
  public selectedVacancy: any[] = [];
  public jobBatches: any[] = [];
  public designations: any[] = [];
  public designationsData: any[] = [];
  private inputChangeSubscription: Subscription;
  private skipChangeSubscription: Subscription;

  public showFilters: boolean = true;
  public loadingPreferences: boolean = false;
  public showOrder: boolean = false;
  public loadingVacancy: boolean = false;
  public noVacancy: boolean = false;
  public savingOrder: boolean = false;
  public showVacancy: boolean = false;
  public preferencesWindow: boolean = false;
  public searchingHfs: boolean = false;
  public checkingVacancy: boolean = false;
  public downloadingAcceptanceLetter: boolean = false;
  public acceptanceLetterDownloaded: boolean = false;
  public downloadingOfferLetter: boolean = false;
  public specialQuota: boolean = false;
  public view: string = 'grid';
  public dateNow: string = '';
  public joiningDate: string = '';
  public offerDate: string = '';
  public imgSrc: string = '';
  public offerMonth: string = '';
  public searchEvent = new Subject<any>();
  public searchSubcription: Subscription = null;

  constructor(private sanitized: DomSanitizer, public _notificationService: NotificationService,
    private _rootService: RootService,
    private _publicPortalService: PublicPortalService,
    private _authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.user = this._authenticationService.getUser();
    this.kGrid.pageSizes = [5, 10, 25, 50];
    this.kGrid.totalRecords = 0;
    this.kGrid.pageSize = 100;
    this.filters.Skip = 0;
    this.filters.PageSize = this.kGrid.pageSize;
    this.getDesignations();
    
    this.filters.JobId = 65;
    this.filters.BatchNo = 'MS-16-09-2021';
    this.getJobApplicants();
  }
  public dropdownValueChanged(value: any, filter: any) {
    if (filter == 'designation') {
      this.getJobBatches(value.Id);
    }
    if (filter == 'batch') {
      this.selectedJobId = value.JobId;
      this.filters.JobId = this.selectedJobId;
      this.filters.BatchNo = value.BatchNo;
      this.getJobApplicants();
    }
  }
  public handleFilter = (value, filter) => {
    if (filter == 'designation') {
      this.designationsData = this.designations.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
  }
  public getJobApplicants() {
    this.kGrid.loading = true;
    this._publicPortalService.getJobApplicants(this.filters).subscribe((res: any) => {
      if (res) {
        this.kGrid.data = res.List;
        this.kGrid.gridView = { data: res.List, total: res.Count };
      }
      this.showFilters = false;
      this.kGrid.loading = false;
    }, err => {
      console.log(err);
    });
  }
  public getDesignations() {
    this._rootService.getDesignations().subscribe((res: any) => {
      if (res) {
        this.designations = res;
        this.designationsData = this.designations;
      }
    }, err => {
      console.log(err);
    });
  }
  public getJobBatches(designationId: number) {
    this._publicPortalService.getJobBatches(designationId).subscribe((res: any) => {
      if (res) {
        this.jobBatches = res;
      }
    }, err => {
      console.log(err);
    });
  }
  public getJobApplicantQualifications(applicantId: number) {
    this._publicPortalService.getJobApplicantQualifications(applicantId).subscribe((res: any) => {
      if (res) {
        this.selectedApplicant.Qualifications = res;
      }
    }, err => {
      console.log(err);
    });
  }
  public getJobApplicantExperiences(applicantId: number) {
    this._publicPortalService.getJobApplicantExperiences(applicantId).subscribe((res: any) => {
      if (res) {
        this.selectedApplicant.Experiences = res;
      }
    }, err => {
      console.log(err);
    });
  }
  public getJobApplicantPreferences(applicantId: number, jobId: number) {
    this._publicPortalService.getJobApplicantPreferences(applicantId, jobId).subscribe((res: any) => {
      if (res) {
        this.selectedApplicant.Preferences = res;
      }
    }, err => {
      console.log(err);
    });
  }
  public selectApplicant(id) {
    this._publicPortalService.getJobApplicant(id).subscribe((res: any) => {
      if (res) {
        this.selectedApplicant = res;
        this.getJobApplicantQualifications(this.selectedApplicant.Id);
        this.getJobApplicantExperiences(this.selectedApplicant.Id);
        this.getJobApplicantPreferences(this.selectedApplicant.Id, this.filters.JobId);
      }
    }, err => {
      console.log(err);
    });
  }
  public openInNewTab(link: string) {
    window.open('' + link, '_blank');
  }
  public dashifyCNIC(cnic: string) {
    if (!cnic) return '';
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
