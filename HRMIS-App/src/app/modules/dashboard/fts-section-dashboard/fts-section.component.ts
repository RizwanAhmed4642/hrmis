
import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { DashboardService } from '../dashboard.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { aggregateBy } from '@progress/kendo-data-query';

import { Router } from '@angular/router';
import { RootService } from '../../../_services/root.service';
import { PandSOfficerView } from '../../user/user-claims.class';
import { AgGrid } from '../../../_helpers/ag-grid.class';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';

@Component({
  selector: 'app-hrmis-ftssectiondashboard',
  templateUrl: './fts-section.component.html',
  styleUrls: ['./fts-section.component.scss']
})
export class FtsSectionDashboardComponent implements OnInit {
  @ViewChild('popup', { static: false }) calendarpopup;
  @Input() public currentUser: any;/* 
  @ViewChild('agGrid', {static: false}) agGrid: AgGridNg2; */
  public agGridHelper: AgGrid = new AgGrid();
  public dropDowns: DropDownsHR = new DropDownsHR();
  public currentOfficer: any;
  public loading: boolean = false;
  public fileMoveOpened: boolean = false;
  public showVacancy: boolean = false;
  public showVacancy2: boolean = false;
  public showFilters: boolean = false;
  public minDate = new Date(2017, 8, 9);
  public maxDate = new Date();
  public searchingProfile: boolean = false;
  public profileExist: boolean = false;
  public profileNotExist: boolean = false;
  public addingToPool: boolean = false;
  public removingFromPool: boolean = false;
  public cnicMask: string = "00000-0000000-0";
  public cnic: string = '';
  public profile: any;
  public records: any[] = [];
  public vacancyReport: any[] = [];
  public total: any;
  public totalSums: any;
  public totalSumsFDO: any;
  public totalSumsRI: any;
  public totalSumsN: any;
  public totalSumsMy: any;
  public totalSumsDept: any;
  public vacancyTotal: any;
  public filesCount: any;
  public filesList: any;
  public loadingPSO: boolean = false;
  public loadingList: boolean = false;
  public aggregates: any[] = [];
  public aggregatesFDO: any[] = [];
  public aggregatesRI: any[] = [];
  public aggregatesDept: any[] = [];
  public aggregatesN: any[] = [];
  public vacancyAggregates: any[] = [];
  public dashboardFcAppFwdCounts: any[] = [];
  public totals: any[] = [];
  public newSectionReport: any[] = [];
  public newSectionReportOld: any[] = [];
  public newMySectionReport: any[] = [];
  public newMySectionReportOld: any[] = [];
  public fdoReport: any[] = [];
  public riBranchReport: any[] = [];
  public pensionCases: any[] = [];
  public fileActionType: number = 0;
  public range = { start: null, end: null };
  public rangeSections = { start: null, end: null };
  public rangeDept = { start: null, end: null };
  public officer_Id: number = 0;
  public officerName: string = '';
  public program: string = '';
  public crrSectionReport: any[] = [];
  constructor(private _rootService: RootService, public _authService: AuthenticationService, private _dashboardService: DashboardService, private router: Router) { }

  ngOnInit() {
    this.range.start = new Date(2017, 8, 9);
    this.range.end = new Date();
    this.maxDate.setDate(this.maxDate.getDate() + 1);
    this._rootService.getCurrentOfficer().subscribe((res: PandSOfficerView) => {
      if (res) {
        this.currentOfficer = res;
      }
    });
    this.currentUser = this._authService.getUser();
    if (this.currentUser.RoleName == 'Deputy Secretary') {
      this.getPensionCasesReport();
    }
    this.getNewSectionReport();
    this.getDashboardFcAppFwdCounts();
    this.getPandSOfficers('ds.as');
    this.getCRRReport();
    this.getOfficerFilesCount();
    this.getFDOReportTypeWiseDate();
    this.getRiBranchReportTypeWiseDate();
  }
  public getFDOReportTypeWiseDate() {
    this.loading = true;
    this._dashboardService.getFDOReportTypeWiseDate(null, null, null).subscribe((data: any) => {
      this.fdoReport = data.d;
      this.setTotalAggregatesFDO();

    }, err => {
      this.handleError(err);
    });
  } public getRiBranchReportTypeWiseDate() {
    this.loading = true;
    this._dashboardService.getRiBranchReportTypeWiseDate(null, null, null).subscribe((data: any) => {
      this.riBranchReport = data.d;
      this.setTotalAggregatesRI();

    }, err => {
      this.handleError(err);
    });
  }
  public setTotalAggregatesRI() {
    this.aggregatesRI = [
      { field: 'Total', aggregate: 'sum' },
      { field: 'Today', aggregate: 'sum' },
      { field: 'InProcess', aggregate: 'sum' },
      { field: 'Disposed', aggregate: 'sum' },
    ];
    this.totalSumsRI = aggregateBy(this.riBranchReport, this.aggregatesRI);
    this.loading = false;
  }
  public setTotalAggregatesFDO() {
    this.aggregatesFDO = [
      { field: 'Total', aggregate: 'sum' },
      { field: 'Today', aggregate: 'sum' },
      { field: 'InProcess', aggregate: 'sum' },
      { field: 'Disposed', aggregate: 'sum' },
    ];
    this.totalSumsFDO = aggregateBy(this.fdoReport, this.aggregatesFDO);
    this.loading = false;
  }
  public getCRRReport() {
    this.loading = true;
    this._dashboardService.getCRRReport().subscribe((data: any) => {
      if (data && data.crr) {
        this.crrSectionReport = data.crr;
        if (this.crrSectionReport.length > 0) {
          // this.setTotalAggregates();
        }
      }
      this.loading = false;
    }, err => {
      console.log(err);
    });
  }
  public getOfficerFilesCount() {
    this._dashboardService.getOfficerFilesCount().subscribe((data: any) => {
      if (data) {
        this.filesCount = data;
      }
    }, err => {
      console.log(err);
    });
  }
  public getOfficerFilesFiles(type: number) {
    this.filesList = null;
    this.loadingList = true;
    this._dashboardService.getOfficerFilesFiles(type).subscribe((data: any) => {
      if (data) {
        this.filesList = data.List;
      }
      this.loadingList = false;
    }, err => {
      console.log(err);
    });
  }
  private getPandSOfficers = (type: string) => {
    this.dropDowns.officers = [];
    this._rootService.getPandSOfficers(type).subscribe((res: any) => {
      if (res) {
        this.dropDowns.officers = res;
      }
    },
      err => { this.handleError(err); }
    );
  }
  public cellCLickedFc(a, b) {

  }
  public getDashboardFcAppFwdCounts() {
    this._dashboardService.getDashboardFcAppFwdCounts(this.rangeDept.start, this.rangeDept.end, this.program).subscribe((data: any) => {
      this.dashboardFcAppFwdCounts = data.d;
      this.setTotalAggregatesDept();
    }, err => {
      this.handleError(err);
    });
  }
  public setTotalAggregatesDept() {
    this.aggregatesDept = [
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
    this.totalSumsDept = aggregateBy(this.dashboardFcAppFwdCounts, this.aggregatesDept);
    if (this.calendarpopup) {
      this.calendarpopup.toggle(false);
    }
    this.loadingPSO = false;
  }
  public openInNewTab(link: string) {
    window.open('/' + link, '_blank');
  }
  public getNewSectionReport() {
    this.loading = true;
    this._dashboardService.getSectionReportNew(this.range.start, this.range.end).subscribe((data: any) => {
      if (data && data.d) {

        this.newSectionReport = data.d;
        if (data && data.n) {
          this.newSectionReportOld = data.n;
          this.setTotalAggregatesN();
        }
        this.agGridHelper.rowData = data.d;
        if (this.newSectionReport.length > 0) {
          this.initializeGridColumns();
          this.setTotalAggregates();
        }
      }

      this.loading = false;
    }, err => {
      this.handleError(err);
    });
  }
  public getSectionReportNew22(sourceId: number) {
    this.loading = true;
    this._dashboardService.getSectionReportNew22(this.range.start, this.range.end, sourceId).subscribe((data: any) => {
      if (data && data.d) {

        this.newSectionReport = data.d;
        if (data && data.n) {
          this.newSectionReportOld = data.n;
          this.setTotalAggregatesN();
        }
        this.agGridHelper.rowData = data.d;
        if (this.newSectionReport.length > 0) {
          this.initializeGridColumns();
          this.setTotalAggregates();
        }
      }

      this.loading = false;
    }, err => {
      this.handleError(err);
    });
  }
  public getMySectionReportNew() {
    this.loading = true;
    this._dashboardService.getMySectionReportNew(this.rangeSections.start, this.rangeSections.end, this.officer_Id).subscribe((data: any) => {
      if (data && data.d) {
        this.newMySectionReport = data.d;
        if (this.newMySectionReport.length > 0) {
          this.setTotalAggregatesMyN();
        }
        /*   if (data && data.n) {
         this.newMySectionReportOld = data.n;
         this.setTotalAggregatesMyN();
       } */
      }

      this.loading = false;
    }, err => {
      this.handleError(err);
    });
  }
  public getPensionCasesReport() {
    this.loading = true;
    this._dashboardService.getPensionCasesReport(this.rangeSections.start, this.rangeSections.end, "").subscribe((data: any) => {
      if (data) {
        this.pensionCases = data;
        console.log(this.pensionCases);

        /*  if (this.pensionCases.length > 0) {
           this.setTotalAggregatesMyN();
         } */
        /*   if (data && data.n) {
         this.newMySectionReportOld = data.n;
         this.setTotalAggregatesMyN();
       } */
      }

      this.loading = false;
    }, err => {
      this.handleError(err);
    });
  }
  public dropdownValueChanged = (value, filter) => {
    if (!value) return;
    if (filter == 'officer') {
      this.officer_Id = value.Id;
      this.officerName = value.DesignationName;
      this.getMySectionReportNew();
    }
    if (filter == 'program') {
      this.program = value == 'Select Program' ? '' : value;
      this.getDashboardFcAppFwdCounts();
    }
    if (filter == 'source') {
      if (value.Id == 0) {
        this.getNewSectionReport();
      } else {
        this.getSectionReportNew22(value.Id);
      }
    }
  }
  private initializeGridColumns() {
    this.agGridHelper.columnDefs = [
      { headerName: 'Application Type', field: 'ApplicationType', sortable: true, filter: true },
      { headerName: 'Total', field: 'Total', sortable: true, filter: true },
      { headerName: 'No Process Initiated', field: 'No_Process_Initiated', sortable: true, filter: true },
      { headerName: 'Under Process', field: 'UnderProcess', sortable: true, filter: true },
      { headerName: 'File Request', field: 'Pending_CRR', sortable: true, filter: true },
      { headerName: 'Disposed Cases', field: 'Approved', sortable: true, filter: true },
      { headerName: 'Section Pendency', field: 'Section_Pendency', sortable: true, filter: true },
    ];
  }
  cellCLicked(item, statusId) {
    let typeName = encodeURIComponent(item.ApplicationType);
    this.openInNewTab('fts/my-applications/' + typeName + '/' + statusId + '/' + this.toTimestamp(this.range.start) + '/' + this.toTimestamp(this.range.end));
  }
  colCLicked(statusId) {
    this.openInNewTab('fts/my-applications/' + null + '/' + statusId);
  }
  toTimestamp(strDate) {
    

    return new Date(strDate).toDateString();
  }
  public setTotalAggregates() {
    this.aggregates = [{ field: 'Total', aggregate: 'sum' },
    { field: 'Total_Applications', aggregate: 'sum' },
    { field: 'No_Process_Initiated', aggregate: 'sum' },
    { field: 'Under_Process', aggregate: 'sum' },
    { field: 'Waiting_Documents', aggregate: 'sum' },
    { field: 'Pending_CRR', aggregate: 'sum' },
    { field: 'Approved', aggregate: 'sum' },
    { field: 'Rejected', aggregate: 'sum' },
    { field: 'Section_Pendency', aggregate: 'sum' },
    { field: 'Marked', aggregate: 'sum' },
    { field: 'Disposed', aggregate: 'sum' },
    { field: 'Initiated', aggregate: 'sum' },
    { field: 'Reject_and_Dispose', aggregate: 'sum' },
    { field: 'Approve_and_Return_File', aggregate: 'sum' },
    { field: 'No_Further_Action', aggregate: 'sum' },
    ];
    this.totalSums = aggregateBy(this.newSectionReport, this.aggregates);
    if (this.calendarpopup) {
      this.calendarpopup.toggle(false);
    }
    this.loading = false;
  }
  public setTotalAggregatesN() {
    this.aggregatesN = [
      { field: 'Total', aggregate: 'sum' },
      { field: 'Total_Applications', aggregate: 'sum' },
      { field: 'No_Process_Initiated', aggregate: 'sum' },
      { field: 'Under_Process', aggregate: 'sum' },
      { field: 'Waiting_Documents', aggregate: 'sum' },
      { field: 'Pending_CRR', aggregate: 'sum' },
      { field: 'Approved', aggregate: 'sum' },
      { field: 'Rejected', aggregate: 'sum' },
      { field: 'Section_Pendency', aggregate: 'sum' },
      { field: 'Marked', aggregate: 'sum' },
      { field: 'Disposed', aggregate: 'sum' },
      { field: 'Initiated', aggregate: 'sum' },
      { field: 'Reject_and_Dispose', aggregate: 'sum' },
      { field: 'Approve_and_Return_File', aggregate: 'sum' },
      { field: 'No_Further_Action', aggregate: 'sum' },
    ];
    this.totalSumsN = aggregateBy(this.newSectionReportOld, this.aggregatesN);
    if (this.calendarpopup) {
      this.calendarpopup.toggle(false);
    }
    this.loading = false;
  }
  public setTotalAggregatesMyN() {
    this.aggregates = [
      { field: 'Total', aggregate: 'sum' },
      { field: 'No_Process_Initiated', aggregate: 'sum' },
      { field: 'Under_Process', aggregate: 'sum' },
      { field: 'Waiting_Documents', aggregate: 'sum' },
      { field: 'Pending_CRR', aggregate: 'sum' },
      { field: 'Approved', aggregate: 'sum' },
      { field: 'Rejected', aggregate: 'sum' },
      { field: 'No_Further_Action', aggregate: 'sum' },
      { field: 'Section_Pendency', aggregate: 'sum' },
    ];
    this.totalSumsMy = aggregateBy(this.newMySectionReport, this.aggregates);
    if (this.calendarpopup) {
      this.calendarpopup.toggle(false);
    }
    this.loading = false;
  }
  fetchDetailsCount(officerName: string, statusId: number) {
    /*  let from: any = (this.dateRange && this.dateRange[0]) ? this.dateRange[0].toDateString() : null;
     let to: any = (this.dateRange && this.dateRange[1]) ? this.dateRange[1].toDateString() : null;
     this.selectedDesignation = officerName;
     this.selectedStatusId = statusId;
     this.displayedApplications = []; */

    //var url = `/#/home/dashboard-summary-detail?officerName=${officerName}&statusId=${statusId}&from=${from}&to=${to}`;
    // window.open(url);

  }
  public numberWithCommas(x) {
    if (x) {
      return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }
  }
  public getSum(arr: number[]) {
    let sum: number = 0;
    arr.forEach(element => {
      sum += element;
    });
    return sum;
  }
  public cnicValueChange() {
    this.profileExist = false;
    this.profileNotExist = false;
    this.profile = null;
  }

  public closeWindow() {
    this.fileMoveOpened = false;
    this.profileExist = false;
    this.profileNotExist = false;
    this.cnic = '';
    this.profile = null;
    this.fileActionType = 0;
  }
  public nothing() {
    this.fileMoveOpened = true;
  }
  public openWindow(id: number) {
    this.fileActionType = id;
    this.fileMoveOpened = true;
  }
  private handleError(err: any) {
    if (err.status == 403) {
      this._authService.logout();
    }
  }
}
