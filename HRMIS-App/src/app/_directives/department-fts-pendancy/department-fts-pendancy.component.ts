import { Component, OnInit, ViewChild, Input, OnDestroy } from '@angular/core';
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
import { Subscription } from 'rxjs/Subscription';
import { debounceTime } from 'rxjs/operators/debounceTime';

@Component({
  selector: 'app-department-fts-pendancy',
  templateUrl: 'department-fts-pendancy.component.html',
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
  .box-cstm {
    background: #f7f7f7;
    box-shadow: 0px 0px 6px #00000057;
    border-radius: 6px !important;
  }
  .box-cstm:hover {
    box-shadow: inset 0px 0px 6px #00000057;
  }
  .font-4xl{
    font-size: 1.1vw !important;
  }
  .font-5xl{
    font-size: 1.17vw !important;
  }
  `]
})
export class DepartmentFTSPendancyComponent implements OnInit, OnDestroy {
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
  public kGridSearchedApplications: KGridHelper = new KGridHelper();
  public dropDowns: DropDownsHR = new DropDownsHR();
  public currentOfficer: any;
  public rangeDept = { start: null, end: null };
  public data: any[] = [];
  public aggregatesDept: any[] = [];
  public tabs: any[] = [];
  public totalSumsDept: any;
  public program: string = '';
  public currentTab: string = 'Report';
  public inputChange: Subject<any>;
  public searchQuery: string = '';
  public type: string = '';
  public typeId: number = 0;
  public fontSize: number = 12;
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
  public secretary: any = {};
  public selectedOfficer: any = {};
  public user: any = {};
  public psofficer: any = {};

  public showTrackingDetail: boolean = false;
  public searchingTrack: boolean = false;
  public trackingNumber: number = 0;
  public innerWidth: any;
  public innerHeight: any;
  private inputChangeSubscription: Subscription;

  constructor(private sanitizer: DomSanitizer, private _rootService: RootService,
    public _authService: AuthenticationService,
    private _dashboardService: DashboardService,
    private router: Router) { }

  ngOnInit() {
    this.user = this._authService.getUser();
    this.psofficer = this._authService.getCurrentOfficer();
    this.innerWidth = window.innerWidth;
    this.innerHeight = window.innerHeight;
    this.fontSize = this.innerWidth
    this.kGrid.multiple = true;
    this.kGridApplications.loading = false;
    this.kGridApplications.pageSize = 150;
    this.kGridApplications.pageSizes = [50, 100, 200, 300, 500, 1000];
    this.kGridSearchedApplications.pageSize = 300;
    this.kGridSearchedApplications.pageSizes = [50, 100, 200, 300, 500, 1000];
    this.getPandSOfficers('all');
    this.getApplicationStatus();
    this.getDepartmentReport();
    this.subscribeInputChange();
    setTimeout(() => {
      this.getDepartmentReport();
    }, 300000);
  }

  private subscribeInputChange() {
    this.inputChange = new Subject();
    this.inputChangeSubscription = this.inputChange.pipe(debounceTime(300)).subscribe((query) => {
      this.searchQuery = query;
      this.searchApplications();
    });
  }
  public getDepartmentReport() {
    this.kGrid.loading = true;
    this._dashboardService.dashboardPendencyCount(this.rangeDept.start, this.rangeDept.end, this.program).subscribe((data: any) => {
      if (data) {
        this.kGrid.data = [];
        this.kGrid.data = data.dept;
        console.log('Pendency: ' , data.pendancy);
        /*  this.kGrid.totalRecords = this.kGrid.data.length;
         this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords };
         this.setTotalAggregatesDept(); */
        this.getDashboardPendency3();
        this.setOffices();
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
  public setOffices() {
    console.log('secretary: ', this.secretary);
    
    this.kGrid.data.forEach(d => {
      let name = d.OfficerDesignation;
      if (name) {
        name = d.OfficerDesignation.toLowerCase();
        if (name.includes('secretary') && name.includes('primary')) {
          this.secretary.pendancy = d;
        }
        if (name.includes('additional') && name.includes('admn')) { this.secretary.asadmn = d; }
        if (name.includes('additional') && name.includes('drug control')) { this.secretary.asdc = d; }
        if (name.includes('additional') && name.includes('technical')) { this.secretary.astech = d; }
        if (name.includes('additional') && name.includes('development')) { this.secretary.asdev = d; }
        if (name.includes('additional') && name.includes('vertical')) { this.secretary.asvp = d; }
        if (name.includes('deputy') && name.includes('admn')) { this.secretary.dsadmin = d; }
        if (name.includes('deputy') && name.includes('general')) { this.secretary.dsg = d; }
        if (name.includes('deputy') && name.includes('establishment')) { this.secretary.dsestab = d; }
        if (name.includes('deputy') && name.includes('drug control')) { this.secretary.dsdc = d; }
        if (name.includes('deputy') && name.includes('promotion')) { this.secretary.dspromotion = d; }
        if (name.includes('deputy') && name.includes('liason')) { this.secretary.dsstaff = d; }
        if (name.includes('deputy') && name.includes('(technical-i)')) { this.secretary.dstechi = d; }
        if (name.includes('deputy') && name.includes('(technical-ii)')) { this.secretary.dstechii = d; }
        if (name.includes('deputy') && name.includes('(technical-iii)')) { this.secretary.dstechiii = d; }
        if (name.includes('deputy') && name.includes('vertical')) { this.secretary.dsvp = d; }
        if (name.includes('deputy') && name.includes('development')) { this.secretary.dsdev = d; }
        if (name.includes('section') && name.includes('budget')) { this.secretary.sobugdet = d; }
        if (name.includes('section') && name.includes('c&c')) { this.secretary.socnc = d; }
        if (name.includes('section') && name.includes('central')) { this.secretary.socentral = d; }
        if (name == 'section (confidential)') { this.secretary.soconf = d; }
        if (name.includes('section') && name.includes('(confidential') && name.includes('ii')) { this.secretary.soconf2 = d; }
        if (name.includes('section') && name.includes('(pension)')) { this.secretary.sopension = d; }
        if (name.includes('section') && name.includes('(dc-i)')) { this.secretary.sodci = d; }
        if (name.includes('section') && name.includes('(dc-ii)')) { this.secretary.sodcii = d; }
        // if (name.includes('section') && name.includes('hp')) { this.secretary.sohp = d; }
        if (name == 'section (hp)') { this.secretary.sohp = d; }
        if (name.includes('section') && name.includes('dental')) { this.secretary.sodental = d; }
        if (name.includes('section') && name.includes('dhas')) { this.secretary.sodhas = d; }
        if (name.includes('section') && name.includes('(general)')) { this.secretary.sog = d; }
        if (name.includes('section') && name.includes('(gc-i)')) { this.secretary.sogci = d; }
        if (name.includes('section') && name.includes('(gc-ii)')) { this.secretary.sogcii = d; }
        if (name.includes('section') && name.includes('inq')) { this.secretary.soinq = d; }
        if (name.includes('section') && name.includes('liason')) { this.secretary.soliason = d; }
        if (name.includes('section') && name.includes('(nd)')) { this.secretary.sond = d; }
        if (name.includes('section') && name.includes('north')) { this.secretary.sonorth = d; }
        if (name.includes('section') && name.includes('nursing')) { this.secretary.sonursing = d; }
        if (name.includes('section') && name.includes('pharmacy')) { this.secretary.sopharmacy = d; }
        if (name.includes('section') && name.includes('php')) { this.secretary.sophp = d; }
        if (name.includes('section') && name.includes('promotion')) { this.secretary.sopromotion = d; }
        if (name.includes('section') && name.includes('(specialist cadre)')) { this.secretary.sosc = d; }
        if (name.includes('section') && name.includes('south')) { this.secretary.sosouth = d; }
        if (name.includes('section') && name.includes('vertical')) { this.secretary.sovp = d; }
        if (name.includes('section') && name.includes('(wmo-i)')) { this.secretary.sowmoi = d; }
        if (name.includes('section') && name.includes('(wmo-ii)')) { this.secretary.sowmoii = d; }
        if (name.includes('section') && name.includes('(ahp-i)')) { this.secretary.soahpi = d; }
        if (name.includes('section') && name.includes('(ahp-ii)')) { this.secretary.soahpii = d; }
        if (name.includes('section') && name.includes('h&d')) { this.secretary.sohd = d; }
        
      }
    });
    this.kGrid.loading = false;
    if (this.calendarpopup) {
      this.calendarpopup.toggle(false);
    }
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
  }
  public searchApplications() {
    if (!this.searchQuery) {
      this.kGridSearchedApplications.gridView = { data: [], total: 0 };
      return;
    }
    this.kGrid.loading = true;
    this._dashboardService.getApplications(this.kGridSearchedApplications.skip, this.kGridSearchedApplications.pageSize,
      this.searchQuery, 0, 0, 0, null, null).subscribe(
        (response: any) => {
          this.kGridSearchedApplications.data = [];
          this.kGridSearchedApplications.data = response.List;
          let temp: any[] = [];
          this.kGridSearchedApplications.data.forEach(element => {
            temp.push(element.Id);
          });
          this.kGridSearchedApplications.totalRecords = response.Count;
          this.kGridSearchedApplications.gridView = { data: this.kGridSearchedApplications.data, total: this.kGridSearchedApplications.totalRecords };
          this.kGrid.loading = false;
          if (this.calendarpopup) {
            this.calendarpopup.toggle(false);
          }
        },
        err => this.handleError(err)
      );
  }
  public getApplications() {
    this._dashboardService.getApplications(this.kGridApplications.skip, this.kGridApplications.pageSize,
      this.searchQuery, this.typeId, this.statusId, this.officerId,
      this.rangeDept.start, this.rangeDept.end).subscribe(
        (response: any) => {
          this.kGridApplications.data = [];
          this.kGridApplications.data = response.List;
          let temp: any[] = [];
          this.kGridApplications.data.forEach(element => {
            temp.push(element.Id);
          });
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
  public cellCLickedFc(item, statusId) {
    this.kGridApplications.loading = true;
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
    this.getApplications();
    /* this.tabs.push({ office: this.pandSOfficer, status: this.status }); */
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
  private sortSearchData() {
    this.kGridSearchedApplications.gridView = {
      data: orderBy(this.kGridSearchedApplications.data, this.kGridSearchedApplications.sort),
      total: this.kGridSearchedApplications.data.length
    };
  }
  public sortSearchChange(sort: SortDescriptor[]): void {
    if (sort[0].field == 'asd') { return; }
    this.kGridSearchedApplications.sort = sort;
    this.sortSearchData();
  }
  public pageSearchChange(event: PageChangeEvent): void {
    this.kGridSearchedApplications.skip = event.skip;
    this.searchApplications();
  }
  public changeSearchPagesize(value: any) {
    this.kGridSearchedApplications.pageSize = +value;
    this.kGridSearchedApplications.skip = 0;
    this.searchApplications();
  }
  public openApplicationDialog(item: any) {
    this.selectedOfficer = item;
    this.applicatonDialogOpened = true;
  }
  public closeApplicationDialog() {
    this.selectedOfficer = {};
    this.kGridApplications.gridView = { data: [], total: 0 };
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
    this.openTrackingDialog();
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
  ngOnDestroy() {
    this.inputChangeSubscription.unsubscribe();
  }
}
