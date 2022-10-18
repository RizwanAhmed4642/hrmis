import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { DropDownsHR } from '../../_helpers/dropdowns.class';
import { RootService } from '../../_services/root.service';
import { AuthenticationService } from '../../_services/authentication.service';
import { DashboardService } from '../../modules/dashboard/dashboard.service';
import { Router } from '@angular/router';
import { aggregateBy, SortDescriptor, orderBy } from '@progress/kendo-data-query';
import { KGridHelper } from '../../_helpers/k-grid.class';
import { Subject } from 'rxjs/Subject';
import { PageChangeEvent } from '@progress/kendo-angular-grid';
import { DomSanitizer } from '@angular/platform-browser';
import { ApplicationLog, ApplicationLogView, ApplicationAttachment } from '../../modules/application-fts/application-fts';

@Component({
  selector: 'app-department-fts-report',
  templateUrl: 'department-fts-report.component.html',
  styles: [`
  p.hoverable-table {
    margin: 0px !important;
    width: 100% !important;
    /* font-size: 12px !important; */
    padding: 0.25em 0.5em 0.25em 0.8em !important;
    font-size: 14px;
    cursor: pointer;
    transition: 0.1s ease-in-out;
  }
  
  p.hoverable-table:hover {
    background: rgba(0, 0, 0, 0.08);
  }
  
  .k-grid td {
    border-color: transparent !important;
  }
  .tab-content{
    padding: 0px !important;
  }
  `]
})
export class DepartmentFTSReportComponent implements OnInit {
  @ViewChild('popup', { static: true }) calendarpopup;
  @Input() public currentUser: any;
  public showFilters: boolean = false;
  public applicatonDialogOpened: boolean = false;
  public loadingApplication: boolean = false;
  public reportView: boolean = true;
  public loadingLogs: boolean = false;
  public trackingDialogOpened: boolean = false;
  public showB: boolean = false;
  public kGrid: KGridHelper = new KGridHelper();
  public kGridApplications: KGridHelper = new KGridHelper();
  public dropDowns: DropDownsHR = new DropDownsHR();
  public currentOfficer: any;
  public rangeDept = { start: null, end: null };
  public data: any[] = [];
  public aggregatesDept: any[] = [];
  public tabs: any[] = [];
  public totalSumsDept: any;
  public minDate = new Date(2017, 8, 9);
  public maxDate = new Date();
  public program: string = '';
  public currentTab: string = 'Report';
  public inputChange: Subject<any>;
  public searchQuery: string = '';
  public type: string = '';
  public typeId: number = 0;
  public statusId: number = 0;
  public officer: string = '';
  public officerId: number = 0;
  public pandSOfficers: [] = [];
  public pandSOfficer: any;
  public status: any;
  public application: any;
  public personAppeared: any;
  public fileRequisitions: any[] = [];
  public applicationLog: ApplicationLog = new ApplicationLog();
  public applicationLogs: ApplicationLogView[] = [];
  public applicationOlds: any[] = [];
  public applicationAttachments: ApplicationAttachment[] = [];
  public ddsFile: any;
  public barcodeImgSrc: string = '';
  constructor(private sanitizer: DomSanitizer, private _rootService: RootService,
    public _authService: AuthenticationService,
    private _dashboardService: DashboardService,
    private router: Router) { }

  ngOnInit() {
    this.kGrid.multiple = true;
    this.kGridApplications.loading = false;
    this.kGridApplications.pageSize = 150;
    this.kGridApplications.pageSizes = [50, 100, 200, 300, 500, 1000];
    this.rangeDept.start = new Date(2017, 8, 9);
    this.rangeDept.end = new Date();
    this.maxDate.setDate(this.maxDate.getDate() + 1);
    this.getPandSOfficers('all');
    this.getApplicationStatus();
    this.getDepartmentReport();
    this.getDashboardPendency3();
  }

  public getDepartmentReport() {
    this.kGrid.loading = true;
    this._dashboardService.getDashboardFcAppFwdCounts(this.rangeDept.start, this.rangeDept.end, this.program).subscribe((data: any) => {
      if (data) {
        this.kGrid.data = [];
        this.kGrid.data = data.dept;
        this.kGrid.totalRecords = this.kGrid.data.length;
        this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords };
        this.setTotalAggregatesDept();
      }

    }, err => {
      console.log(err);
    });
  }
  public getDashboardPendency3() {
    this.showB = false;
    this._dashboardService.getDashboardPendency3(this.rangeDept.start, this.rangeDept.end, this.program).subscribe((data: any) => {
      if (data) {
        let p = data.pendancy;
        this.kGrid.data.forEach(report => {
          p.forEach(pend => {
            if (report.OfficerDesignation == pend.OfficerDesignation) {
              report.TodayUnderProcess = pend.TodayUnderProcess;
              report.TodayDispose = pend.TodayDispose;
              report.UnderProcessGT7Days = pend.UnderProcessGT7Days;
              report.UnderProcessGT15Days = pend.UnderProcessGT15Days;
              report.UnderProcessGT30Days = pend.UnderProcessGT30Days;
              report.UnderProcessUntilToday = pend.UnderProcessUntilToday;
            }
          });
        });
        console.log('pendency: ', data.pendancy);
        this.showB = true;
        //this.setTotalAggregatesDept();
      }

    }, err => {
      console.log(err);
    });
  }
  public setTotalAggregatesDept() {
    this.aggregatesDept = [
      { field: 'Total', aggregate: 'sum' },
      { field: 'NoProcessInitiated', aggregate: 'sum' },
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
    this.totalSumsDept = aggregateBy(this.kGrid.data, this.aggregatesDept);
    if (this.calendarpopup) {
      this.calendarpopup.toggle(false);
    }
    this.kGrid.loading = false;
    this.showFilters = false;
  }

  public getApplications() {
    this._dashboardService.getApplications(this.kGridApplications.skip, this.kGridApplications.pageSize,
      this.searchQuery, this.typeId, this.statusId, this.officerId,
      this.rangeDept.start, this.rangeDept.end, this.program).subscribe(
        (response: any) => {
          this.kGridApplications.data = [];
          this.kGridApplications.data = response.List;
          let temp: any[] = [];
          this.kGridApplications.data.forEach(element => {
            temp.push(element.Id);
          });
          console.log(temp);
          this.kGridApplications.totalRecords = response.Count;
          this.kGridApplications.gridView = { data: this.kGridApplications.data, total: this.kGridApplications.totalRecords };
          this.kGridApplications.loading = false;
          if (this.calendarpopup) {
            this.calendarpopup.toggle(false);
          }
        },
        err => this.handleError(err)
      );
  }
  private getPandSOfficers = (type: string) => {
    this.pandSOfficers = [];
    this._rootService.getPandSOfficers(type).subscribe((res: any) => {
      if (res) {
        this.pandSOfficers = res;

      }
    },
      err => { this.handleError(err); }
    );
  }
  private getApplicationStatus() {
    this._rootService.getApplicationStatus().subscribe(
      (response: any) => {
        if (response) {
          this.dropDowns.applicationStatus = response;
          this.dropDowns.applicationStatusData = this.dropDowns.applicationStatus;
        }
      },
      err => this.handleError(err)
    );
  }
  public dropdownValueChanged = (value, filter) => {
    if (filter == 'program') {
      this.program = value == 'Select Program' ? '' : value;
      this.getDepartmentReport();
    }
  }
  public cellCLickedFc(item, statusId) {
    let pso = this.pandSOfficers as any[];
    let o = pso.find(x => x.DesignationName == item.OfficerDesignation);
    if (o) {
      this.pandSOfficer = o;
      this.officerId = o.Id;
    }
    let sts = this.dropDowns.applicationStatus as any[];
    if (statusId == 33) {
      this.status = { Id: 33, Name: 'Disposed' };
      this.statusId = statusId;
    } else if (statusId == 0) {
      this.status = { Id: 0, Name: 'All PUCs' };
      this.statusId = statusId;
    } else {
      let st = sts.find(x => x.Id == statusId);
      if (st) {
        this.status = st;
        this.statusId = st.Id;
      }
    }
    this.tabs.push({ office: this.pandSOfficer, status: this.status });
  }
  private getApplicationData(type) {
    this._dashboardService.getApplicationData(this.application.Id, type).subscribe((data: any) => {
      if (data) {
        if (type == "logs") {
          this.getApplicationLog(true);
        }
        else if (type == "oldlogs") {
          this.applicationOlds = data.applicationForwardLogs;
        }
        else if (type == "file") {
          this.ddsFile = data.File;
        }
        else if (type == "parliamentarian") {
          this.personAppeared = data.applicationPersonAppeared;
        }
        else if (type == "filereqs") {
          this.fileRequisitions = data.applicationFileRecositions;
          /*   if (this.fileRequisitions.length > 0) {
              console.log(this.fileRequisitions);
            } */
        }
        else if (type == "applicationattachments") {
          this.applicationAttachments = data.applicationAttachments as ApplicationAttachment[];
        }

        this.loadingLogs = false;
      }
    }, err => { this.handleError(err); }
    );
  }
  public openInNewTab(link: string) {
    window.open('https://hrmis.pshealthpunjab.gov.pk/' + link, '_blank');
  }
  private getApplication(id, tracking) {
    this._dashboardService.getApplication(+id, +tracking).subscribe((data: any) => {
      if (data) {
        this.application = data.application;
        this._rootService.generateBars(this.application.TrackingNumber).subscribe((res: any) => { this.barcodeImgSrc = res.barCode; }, err => { this.handleError(err); });
        this.applicationAttachments = data.applicationAttachments as ApplicationAttachment[];
        this.applicationLog.Application_Id = this.application.Id;
        this.dropDowns.selectedFiltersModel.applicationStatus = { Id: this.application.Status_Id, Name: this.application.StatusName };
        this.dropDowns.selectedFiltersModel.officer = { Id: this.application.PandSOfficer_Id, DesignationName: this.application.PandSOfficerName };
        this.loadingApplication = false;
        this.getApplicationData("logs");
        this.getApplicationData("oldlogs");
        this.getApplicationData("file");
        this.getApplicationData("filereqs");
        this.getApplicationData("parliamentarian");
        /*  if (this.application.ApplicationType_Id == 2 && this.application.Status_Id == 2 && (this.user.UserName == 'og1' || this.user.UserName == 'ordercell')) {
           this.checkVacancy();
         } */
        this.applicatonDialogOpened = true;
      }
    }, err => { this.handleError(err); }
    );
  }
  public getApplicationLog(all: boolean) {

    this._dashboardService.getApplicationLogs(this.application.Id, 0, true).subscribe((data: any) => {
      if (data) {
        this.applicationLogs = data as ApplicationLogView[];
      }
    }, err => { this.handleError(err); }
    );
  }
  public onTabSelect(event, tab) {
    this.currentTab = event.heading;
    if (this.currentTab == 'Report') {
      this.reportView = true;
    } else {
      this.reportView = false;
    }
    if (tab) {
      this.officerId = tab.office.Id;
      this.statusId = tab.status.Id;
      this.kGridApplications.loading = true;
      this.getApplications();
    }
  }
  public onTabRemove(i: number) {
    this.tabs.splice(i, 1);
  }
  colCLicked(statusId) {
    //this.router.navigate(['/fts/my-applications/' + field]);
    this.officerId = 0;
    let sts = this.dropDowns.applicationStatus as any[];
    if (statusId == 33) {
      this.status = { Id: 33, Name: 'Disposed' };
      this.statusId = statusId;
    } else if (statusId == 0) {
      this.status = { Id: 0, Name: 'All PUCs' };
      this.statusId = statusId;
    } else {
      let st = sts.find(x => x.Id == statusId);
      if (st) {
        this.status = st;
        this.statusId = st.Id;
      }
    }
    this.tabs.push({ office: { Id: 0, DesignationName: 'All' }, status: this.status });
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
    this.kGridApplications.skip = event.skip;
    this.getApplications();
  }
  public changePagesize(value: any) {
    this.kGridApplications.pageSize = +value;
    this.kGridApplications.skip = 0;
    this.getApplications();
  }
  public closeApplicationWindow() {
    this.application = null;
    this.applicatonDialogOpened = false;
  }
  public openApplicationWindow(item) {
    //this.getApplication(item.Id, item.TrackingNumber);
    this.loadingApplication = true;
    this.application = item;
    this._rootService.generateBars(this.application.TrackingNumber).subscribe((res: any) => { this.barcodeImgSrc = res.barCode; }, err => { this.handleError(err); });
    //this.applicationAttachments = data.applicationAttachments as ApplicationAttachment[];
    this.applicationLog.Application_Id = this.application.Id;
    this.dropDowns.selectedFiltersModel.applicationStatus = { Id: this.application.Status_Id, Name: this.application.StatusName };
    this.dropDowns.selectedFiltersModel.officer = { Id: this.application.PandSOfficer_Id, DesignationName: this.application.PandSOfficerName };
    this.loadingApplication = false;
    this.getApplicationData("logs");
    this.getApplicationData("oldlogs");
    this.getApplicationData("file");
    this.getApplicationData("filereqs");
    this.getApplicationData("parliamentarian");
    this.getApplicationData("applicationattachments");
    this.applicatonDialogOpened = true;
  }
  public openTrackingDialog() {
    this.trackingDialogOpened = true;
  }
  public closeTrackingDialog() {
    this.trackingDialogOpened = false;
  }
  transform(value) {
    return this.sanitizer.bypassSecurityTrustHtml(value);
  }
  public barcodeSrc() {
    return this.sanitizer.bypassSecurityTrustUrl(this.barcodeImgSrc);
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
  public capitalize(val: string) {
    if (!val) return '';
    return val.toUpperCase();
  }
  private handleError(err: any) {
    this.kGrid.loading = false;
    if (err.status == 403) {
      this._authService.logout();
    }
  }
}
