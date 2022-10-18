import { Component, OnInit, Input, ViewChild, OnDestroy } from '@angular/core';
import { DashboardService } from '../dashboard.service';
import { usp_HFTypeWiseReport_Result } from './hrmis-dashboard.class';
import { AuthenticationService } from '../../../_services/authentication.service';
import { aggregateBy } from '@progress/kendo-data-query';

import { Router } from '@angular/router';
import { RootService } from '../../../_services/root.service';
import { KGridHelper } from '../../../_helpers/k-grid.class';
import { PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subscription } from 'rxjs/Subscription';
import { Subject } from 'rxjs/Subject';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { NotificationService } from '../../../_services/notification.service';
import { Config } from '../../../_helpers/config.class';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { FirebaseHisduService } from '../../../_services/firebase-hisdu.service';

@Component({
  selector: 'app-hrmis-dashboardhrmis',
  templateUrl: './hrmis-dashboard.component.html',
  styleUrls: ['./hrmis-dashboard.component.scss']
})
export class HrmisDashboardComponent implements OnInit, OnDestroy {

  @Input() public currentUser: any;
  @ViewChild('popup', { static: false }) calendarpopup;
  public showTrackingDetail: boolean = false;
  public loadingPSO: boolean = false;
  public liveReporting: boolean = false;
  public poolDialogOpened: boolean = false;
  public showVacancy: boolean = false;
  public searchingProfile: boolean = false;
  public profileExist: boolean = false;
  public profileNotExist: boolean = false;
  public addingToPool: boolean = false;
  public removingFromPool: boolean = false;
  public searchingTrack: boolean = false;
  public trackingNumber: number = 0;
  public dropDowns: DropDownsHR = new DropDownsHR();
  public program: string = '';
  public activeTab: string = "Health Facility";
  public cnicMask: string = "00000-0000000-0";
  public queryUserActivity: string = '';
  public cnic: string = '';
  public profile: any;
  public records: usp_HFTypeWiseReport_Result[] = [];
  public dashboardFcAppFwdCounts: any[] = [];
  public totalSums: any;
  public vacancyReport: any[] = [];
  public adhocReport: any[] = [];
  public adhocApplicantCounts: any = {};
  public total: any;
  public vacancyTotal: any;
  public aggregates: any[] = [];
  public vacancyAggregates: any[] = [];
  public kGrid: KGridHelper = new KGridHelper();
  public searchEvent = new Subject<any>();
  public searchSubcription: Subscription = null;
  public appsRealtimeSubscription: Subscription = null;
  public vacancyLoaded: boolean = false;
  public searchingActivity: boolean = false;
  public loading: boolean = false;
  public userActivities: any[] = [];
  public range = { start: null, end: null };
  constructor(private _notificationService: NotificationService,
    private _authenticationService: AuthenticationService, 
    private _firebaseHisduService: FirebaseHisduService, private _rootService: RootService, public _authService: AuthenticationService, private _dashboardService: DashboardService, private router: Router) { }

  ngOnInit() {
    this._authenticationService.saveIP('ussr');
    this.getFacilityData();
    this.getVacancyReport(this.currentUser.HfmisCode);
    this.getAdhocCounts();
    this.getAdhocApplicantCounts();
    this.initializeProps();
    if (this.currentUser.RoleName == 'SDP'|| this.currentUser.RoleName == 'Senior Data Processor'  || this.currentUser.RoleName == 'PHFMC Admin') {
      this.handleSearchEvents();
      this.getUserAcitiy();
    }
    if (this.currentUser.UserName == 'dpd') {
      this.getDashboardFcAppFwdCounts();
    }
  }
  private initializeProps() {
    this.kGrid.loading = false;
    this.kGrid.pageSize = 10;
    this.kGrid.pageSizes = [10, 50, 100, 200, 300, 500];
    if (this.currentUser.UserName == 'dpd') {
      this.subscribeLiveApp();
    }
  }
  private subscribeLiveApp() {
    /*  this.appsRealtimeSubscription = this._firebaseHisduService.getAppsChanged().subscribe((apps: any) => {
       if (apps && this.liveReporting) {
         this.getDashboardFcAppFwdCounts();
       }
     }); */
  }
  public handleSearchEvents() {
    this.searchSubcription = this.searchEvent.pipe(
      debounceTime(400)).subscribe((x: any) => {
        this.search(x);
      });
  }
  public search(value: string) {
    this.queryUserActivity = value;
    this.searchingActivity = true;
    this.getUserAcitiy();
  }
  public searchApplication(trackingNumber: any) {
    this.searchingTrack = true;
    this.showTrackingDetail = false;
    if (+trackingNumber) {
      this.trackingNumber = trackingNumber;
      this.showTrackingDetail = true;
    } else {
      this.showTrackingDetail = false;
    }
    this.searchingTrack = false;
  }

  public getFacilityData() {
    this._dashboardService.getHrmisDashboard().subscribe((data: usp_HFTypeWiseReport_Result[]) => {
      debugger;
      this.records = data;
      this.setHFTypesAggregates();
    },
      err => {
        console.log(err);
      }
    );
  }

  public getAdhocCounts() {
    this._rootService.getAdhocCounts().subscribe((data: usp_HFTypeWiseReport_Result[]) => {
      this.adhocReport = data;
      this.setTotalSumAggregates();
    },
      err => {
        console.log(err);
      }
    );
  }
  public getAdhocApplicantCounts() {
    this._rootService.getAdhocApplicantCounts(this.currentUser.HfmisCode).subscribe((data: usp_HFTypeWiseReport_Result[]) => {
      this.adhocApplicantCounts = data;
    },
      err => {
        console.log(err);
      }
    );
  }
  public getVacancyReport = (code: string) => {
    /* try {
      this.vacancyReport = await this._rootService.getVacancyReport(code).toPromise();
    } catch (error) {
      this.handleError(error);
    } */
    this.vacancyLoaded = false;
    this._rootService.getVacancyReport(code).subscribe((res: any) => {
      this.vacancyReport = res;
      this.setVacancyAggregates();
    },
      err => { this.handleError(err); }
    );
  }
  cellCLickedFc(item, statusId) {
    console.log(item);
    //this.router.navigate(['/application/']);
    this.router.navigate(['/application/list/' + item.OfficerDesignation + '/null/' + statusId]);
  }
  colCLicked(field) {
    // this.router.navigate(['/application/']);
    this.router.navigate(['/application/list/' + field]);
    console.log(field);
  }
  public dropdownValueChanged(value, filter) {
    if (filter == 'program') {
      this.program = value == 'Select Program' ? '' : value;
      this.getDashboardFcAppFwdCounts();
    }
  }
  public setTotalAggregates() {
    this.aggregates = [
      { field: 'Total', aggregate: 'sum' },
      { field: 'No_Process_Initiated', aggregate: 'sum' },
      { field: 'Section_Pendency', aggregate: 'sum' },
      { field: 'Approved', aggregate: 'sum' },
      { field: 'Rejected', aggregate: 'sum' },
      { field: 'No_Further_Action', aggregate: 'sum' },
      { field: 'UnderProcess', aggregate: 'sum' },
      { field: 'FileReturned', aggregate: 'sum' },
      { field: 'AReturn', aggregate: 'sum' },
      { field: 'RReturn', aggregate: 'sum' },
      { field: 'Waiting_Documents', aggregate: 'sum' },
      { field: 'FileRequested', aggregate: 'sum' }
    ];
    this.totalSums = aggregateBy(this.dashboardFcAppFwdCounts, this.aggregates);
    if (this.calendarpopup) {
      this.calendarpopup.toggle(false);
    }
    this.loadingPSO = false;
  }

  public setTotalSumAggregates() {
    this.aggregates = [{ field: 'Applications', aggregate: 'sum' },
    { field: 'ApplicationsApproved', aggregate: 'sum' },
    { field: 'ApplicationsRejected', aggregate: 'sum' },
    { field: 'ApplicationsPending', aggregate: 'sum' }
    ];
    this.totalSums = aggregateBy(this.adhocReport, this.aggregates);

    this.loading = false;
  }
  public setHFTypesAggregates() {
    this.aggregates = [
      { field: 'District_Headquarter_Hospital', aggregate: 'sum' },
      { field: 'Tehsil_Headquarter_Hospital', aggregate: 'sum' },
      { field: 'Tehsil_Headquarter_Level_Hospital', aggregate: 'sum' },
      { field: 'Rural_Health_Center', aggregate: 'sum' },
      { field: 'Basic_Health_Unit', aggregate: 'sum' },
      { field: 'Dispensary', aggregate: 'sum' },
      { field: 'Specialized_Hospital', aggregate: 'sum' },
      { field: 'MCH_Center', aggregate: 'sum' },
      { field: 'Trauma_Center', aggregate: 'sum' },
      { field: 'Mobile_Dispensary', aggregate: 'sum' },
      { field: 'Town_Hospital', aggregate: 'sum' },
      { field: 'Eye_Hospital', aggregate: 'sum' },
      { field: 'TB_Clinic', aggregate: 'sum' },
      { field: 'Institute', aggregate: 'sum' },
      { field: 'Flying_Squad', aggregate: 'sum' },
      { field: 'Police_Hospital', aggregate: 'sum' },
      { field: 'Civil_Hospital', aggregate: 'sum' },
      { field: 'Filter_Clinic', aggregate: 'sum' },
      { field: 'District_Health_Developement_Center', aggregate: 'sum' },
      { field: 'Mother_and_Child_Emergency_Unit', aggregate: 'sum' },
      { field: 'School_of_Paramedics', aggregate: 'sum' },
      { field: 'Maternity_Hospital', aggregate: 'sum' },
      { field: 'Infectious_Diseases_Hospital_', aggregate: 'sum' },
      { field: 'Dental_Clinic', aggregate: 'sum' },
      { field: 'City_Medical_Center', aggregate: 'sum' },
      { field: 'Sub_Health___Center', aggregate: 'sum' },
      { field: 'Administrative_Office_District__CEO_', aggregate: 'sum' },
      { field: 'Administrative_Office_District__DOH_P__', aggregate: 'sum' },
      { field: 'Administrative_Office_District__DOH_MS__', aggregate: 'sum' },
      { field: 'Administrative_Office_District__DC_IRMNCH_', aggregate: 'sum' },
      { field: 'Administrative_Office_District__DM_PHFMC_', aggregate: 'sum' },
      { field: 'Administrative_Office_District__DOH_HR_MIS__', aggregate: 'sum' },
      { field: 'Primary___Secondary_Health_Care_Department', aggregate: 'sum' },
      { field: 'Directorate_General_of_Health_Services', aggregate: 'sum' },
      { field: 'Vertical_Program', aggregate: 'sum' },
      { field: 'Administrative_Office_Tehsil__DDOH_', aggregate: 'sum' },
      { field: 'Administrative_Office_Division__DHS_', aggregate: 'sum' },
      { field: 'Administrative_Office__District_', aggregate: 'sum' },
    ];
    this.total = aggregateBy(this.records, this.aggregates);

  }
  public setVacancyAggregates() {
    this.aggregates = [
      { field: 'District_Headquarter_Hospital', aggregate: 'sum' },
      { field: 'Tehsil_Headquarter_Hospital', aggregate: 'sum' },
      { field: 'Tehsil_Headquarter_Level_Hospital', aggregate: 'sum' },
      { field: 'Rural_Health_Center', aggregate: 'sum' },
      { field: 'Basic_Health_Unit', aggregate: 'sum' },
      { field: 'Dispensary', aggregate: 'sum' },
      { field: 'Specialized_Hospital', aggregate: 'sum' },
      { field: 'MCH_Center', aggregate: 'sum' },
      { field: 'Trauma_Center', aggregate: 'sum' },
      { field: 'Mobile_Dispensary', aggregate: 'sum' },
      { field: 'Town_Hospital', aggregate: 'sum' },
      { field: 'Eye_Hospital', aggregate: 'sum' },
      { field: 'TB_Clinic', aggregate: 'sum' },
      { field: 'Institute', aggregate: 'sum' },
      { field: 'Flying_Squad', aggregate: 'sum' },
      { field: 'Police_Hospital', aggregate: 'sum' },
      { field: 'Civil_Hospital', aggregate: 'sum' },
      { field: 'Filter_Clinic', aggregate: 'sum' },
      { field: 'District_Health_Developement_Center', aggregate: 'sum' },
      { field: 'Mother_and_Child_Emergency_Unit', aggregate: 'sum' },
      { field: 'School_of_Paramedics', aggregate: 'sum' },
      { field: 'Maternity_Hospital', aggregate: 'sum' },
      { field: 'Infectious_Diseases_Hospital_', aggregate: 'sum' },
      { field: 'Dental_Clinic', aggregate: 'sum' },
      { field: 'City_Medical_Center', aggregate: 'sum' },
      { field: 'Sub_Health___Center', aggregate: 'sum' },
      { field: 'Administrative_Office_District__CEO_', aggregate: 'sum' },
      { field: 'Administrative_Office_District__DOH_P__', aggregate: 'sum' },
      { field: 'Administrative_Office_District__DOH_MS__', aggregate: 'sum' },
      { field: 'Administrative_Office_District__DC_IRMNCH_', aggregate: 'sum' },
      { field: 'Administrative_Office_District__DM_PHFMC_', aggregate: 'sum' },
      { field: 'Administrative_Office_District__DOH_HR_MIS__', aggregate: 'sum' },
      { field: 'Primary___Secondary_Health_Care_Department', aggregate: 'sum' },
      { field: 'Directorate_General_of_Health_Services', aggregate: 'sum' },
      { field: 'Vertical_Program', aggregate: 'sum' },
      { field: 'Administrative_Office_Tehsil__DDOH_', aggregate: 'sum' },
      { field: 'Administrative_Office_Division__DHS_', aggregate: 'sum' },
      { field: 'Administrative_Office__District_', aggregate: 'sum' },
    ];
    this.total = aggregateBy(this.records, this.aggregates);
    this.vacancyAggregates = [
      { field: 'TotalSanctioned', aggregate: 'sum' },
      { field: 'TotalWorking', aggregate: 'sum' },
      { field: 'TotalVacant', aggregate: 'sum' },
      { field: 'TotalProfile', aggregate: 'sum' }

    ];
    this.vacancyTotal = aggregateBy(this.vacancyReport, this.vacancyAggregates);
    this.vacancyLoaded = true;
  }
  public getDashboardFcAppFwdCounts() {
    this.loadingPSO = true;
    this._dashboardService.getDashboardFcAppFwdCounts(this.range.start, this.range.end, this.program).subscribe((data: any) => {
      this.dashboardFcAppFwdCounts = data.d;
      this.setTotalAggregates();
    }, err => {
      this.handleError(err);
    });
  }
  public cellCLicked(item, typeCode: string) {
    console.log(item);
    this.router.navigate(['health-facility'], { queryParams: { 'h': item.AreaCode, 't': typeCode } });
  }
  public getUserAcitiy() {
    this.kGrid.loading = true;
    if (this.currentUser.RoleName == 'PHFMC Admin') {
      this._dashboardService.getUserVpProfileActivity(this.kGrid.skip, this.kGrid.pageSize, this.queryUserActivity).subscribe((data: any) => {
        if (data) {
          this.kGrid.data = data.List;
          this.kGrid.totalRecords = data.Count;
          this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords };
        }
        this.kGrid.loading = false;
      }, err => {
        this.handleError(err);
      });
    } else {
      this._dashboardService.getUserActivity(this.kGrid.skip, this.kGrid.pageSize, this.queryUserActivity).subscribe((data: any) => {
        if (data) {
          this.kGrid.data = data.List;
          this.kGrid.totalRecords = data.Count;
          this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords };
        }
        this.kGrid.loading = false;
      }, err => {
        this.handleError(err);
      });
    }


  }
  public pageChange(event: PageChangeEvent): void {
    this.kGrid.skip = event.skip;
    this.getUserAcitiy();
  }
  public changePagesize(value: any) {
    this.kGrid.pageSize = +value;
    this.kGrid.skip = 0;
    this.getUserAcitiy();
  }
  public numberWithCommas(x) {
    if (x) {
      return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    } else return 0;
  }
  public cnicValueChange() {
    this.profileExist = false;
    this.profileNotExist = false;
    this.profile = null;
  }
  public getAnyProfile() {
    this.searchingProfile = true;
    this._rootService.getAnyProfile(this.cnic).subscribe((profile: any) => {
      if (profile) {
        this.profile = profile;
        this.searchingProfile = false;
        this.profileExist = true;
      } else {
        this.searchingProfile = false;
        this.profileExist = false;
        this.profileNotExist = true;
      }
      console.log(profile);
    }, err => {
      this.handleError(err);
    });
  }
  public addProfileToPool() {
    this.addingToPool = true;
    this._dashboardService.addToPool(this.profile.Id).subscribe((res: any) => {
      if (res) {
        this.addingToPool = false;
        this._notificationService.notify('success', this.profile.EmployeeName + ' (CNIC:' + Config.dashifyCNIC(this.profile.CNIC) + ') - Profile Added to Pool.')
        this.closeWindow();
      }
    }, err => {
      this.handleError(err);
    });
  }
  public removeProfileToPool() {
    this.removingFromPool = true;
    this._dashboardService.removeFromPool(this.profile.Id).subscribe((res: any) => {
      if (res) {
        this.removingFromPool = false;
        this._notificationService.notify('success', this.profile.EmployeeName + ' (CNIC:' + Config.dashifyCNIC(this.profile.CNIC) + ') - Profile Removed from Pool.')
        this.closeWindow();
      }
    }, err => {
      this.handleError(err);
    });
  }
  public closeWindow() {
    this.poolDialogOpened = false;
    this.profileExist = false;
    this.profileNotExist = false;
    this.cnic = '';
    this.profile = null;
  }
  public onTabSelect(event) {
    this.activeTab = event.heading;
  }
  public nothing() {
    this.poolDialogOpened = true;
  }
  public openWindow() {
    this.poolDialogOpened = true;
  }
  private handleError(err: any) {
    this.addingToPool = false;
    this.searchingProfile = false;
    this.removingFromPool = false;
    if (err.status == 403) {
      this._authService.logout();
    }
  }
  ngOnDestroy() {
    this.searchSubcription ? this.searchSubcription.unsubscribe() : '';
    this.appsRealtimeSubscription ? this.appsRealtimeSubscription.unsubscribe() : '';
  }
}
