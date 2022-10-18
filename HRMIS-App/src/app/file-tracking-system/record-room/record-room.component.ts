import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { KGridHelper } from '../../_helpers/k-grid.class';
import { DropDownsHR } from '../../_helpers/dropdowns.class';
import { Subject } from 'rxjs/Subject';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { SortDescriptor, orderBy } from '@progress/kendo-data-query';
import { PageChangeEvent } from '@progress/kendo-angular-grid';
import { RootService } from '../../_services/root.service';
import { FileTrackingSystemService } from '../file-tracking-system.service';
import { AuthenticationService } from '../../_services/authentication.service';
import { NotificationService } from '../../_services/notification.service';
import { FirebaseHisduService } from '../../_services/firebase-hisdu.service';

@Component({
  selector: 'app-record-room',
  templateUrl: './record-room.component.html',
  styles: [`
  .tab-content{
    padding : 5px 0px 14px 0px !important;
  }
  `]
})
export class RecordRoomComponent implements OnInit {
  public kGrid: KGridHelper = new KGridHelper();
  public dropDowns: DropDownsHR = new DropDownsHR();
  public inputChange: Subject<any>;
  public searchQuery: string = '';
  public statusId: number = 0;
  public tabActionName: string = 'Issue File';
  public selectedRequests: any[] = [];
  public selectedRequestsIndex: number[] = [];
  public officer_Id: number = 0;
  public fileIssueDialogOpened: boolean = false;
  public fileCommentsDialogOpened: boolean = false;
  public issuingFile: boolean = false;
  public bulkIssuingFile: boolean = false;
  public fpStringGuide: string = '';
  public matching: boolean = null;
  public fpMatched: boolean = null;
  public fpMatchedEnable: boolean = false;
  public startMatching: boolean = false;
  public afrLog: any = {};
  public currentUser: any;
  @ViewChild('fpImageRef', { static: false }) fpImage;
  @ViewChild('fpQualityRef', { static: false }) fpQuality: ElementRef;
  @ViewChild('fpRef', { static: false }) fptxt;
  constructor(private _firebaseHisduService: FirebaseHisduService, public _notificationService: NotificationService, private _rootService: RootService, private _fileTrackingSystemService: FileTrackingSystemService, private _authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.currentUser = this._authenticationService.getUser();

    this.inputChange = new Subject();
    this.inputChange.pipe(debounceTime(200)).subscribe((query) => {
      this.searchQuery = query;
      if (this.searchQuery.length <= 2 && this.searchQuery.length != 0) {
        return;
      }
      this.getRequisitions();
    });
    this.initializeProps();
    this.loadDropDownValues();
    this.getRequisitions();
  }
  public onTabSelect(e) {
    this.selectedRequests = [];
    this.selectedRequestsIndex = [];
    let selectedTab = e.heading;
    if (selectedTab == 'All') {
      this.statusId = 0;
    }
    if (selectedTab == 'Available Files') {
      this.statusId = 11;
      this.tabActionName = 'Issue File';
    }
    if (selectedTab == 'Pending Requests') {
      this.statusId = 1;
      this.tabActionName = 'File Available';
    }
    if (selectedTab == 'Issued Files') {
      this.statusId = 2;
      this.tabActionName = 'Return File';
    }
    if (selectedTab == 'Returned Files') {
      this.statusId = 3;
    }
    if (selectedTab == 'Expired Requests') {
      this.statusId = 10;
    }
    if (selectedTab == 'Duplicate Requests') {
      this.statusId = 99;
    }
    this.getRequisitions();
  }

  private initializeProps() {
    this.kGrid.loading = false;
    this.kGrid.pageSize = 50;
    this.kGrid.pageSizes = [50, 100, 200, 300, 500, 1000];
  }
  private loadDropDownValues() {
    this.getFileRequestStatus();
    this.getPandSOfficers('allCrr');

  }
  private getPandSOfficers = (type: string) => {
    this.dropDowns.officers = [];
    this.dropDowns.officersData = [];
    this._rootService.getPandSOfficers(type).subscribe((res: any) => {
      if (res) {
        this.dropDowns.officers = res;
        this.dropDowns.officers.forEach(officer => {
          officer.Name = officer.Name ? officer.Name : '';
          officer.DesignationName = officer.DesignationName ? officer.DesignationName : '';
        });
        this.dropDowns.officersData = this.dropDowns.officers;
      }
    },
      err => { this.handleError(err); }
    );
  }
  private getFileRequestStatus() {
    this._rootService.getApplicationTypes().subscribe(
      (response: any) => {
        this.dropDowns.applicationTypes = response;
        this.dropDowns.applicationTypesData = this.dropDowns.applicationTypes;
      },
      err => this.handleError(err)
    );
  }

  public handleFilter = (value, filter) => {
    if (filter == 'officer') {
      this.dropDowns.officersData = this.dropDowns.officers.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1 || s.DesignationName.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }

  }
  public getRequisitions() {
    this.kGrid.loading = true;
    this.selectedRequests = [];
    this.selectedRequestsIndex = [];
    this.kGrid.data = [];
    this._fileTrackingSystemService.getRequisitions(this.kGrid.skip, this.kGrid.pageSize, this.searchQuery, this.statusId).subscribe(
      (response: any) => {
        if (response) {
          this.kGrid.data = response.List;
          this.kGrid.totalRecords = response.Count;
          this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords };
        }
        this.kGrid.loading = false;
      },
      err => this.handleError(err)
    );
  }
  public bulkIssue() {
    this.bulkIssuingFile = true;
    this.selectedRequests = [];
    this.selectedRequestsIndex.forEach(index => {
      this.selectedRequests.push(this.kGrid.data[index]);
    });
    setTimeout(() => {
      this.openWindow();
    }, 300);
  }
  public fileAlreadyIssued(Id: number) {
    if (confirm("Confirm Remove File Request?")) {
      this.kGrid.loading = true;
      this._fileTrackingSystemService.fileAlreadyIssued(Id).subscribe(
        (response: any) => {
          if (response) {
            this.getRequisitions();
          }
        },
        err => this.handleError(err)
      );
    }
  }
  public verifyFp(fpValue) {
    this.matching = true;
    this._fileTrackingSystemService.fingerPrintCompare({ metaData: fpValue }).subscribe((x: any) => {
      if (x && x.officer) {
        this.fpMatched = true;
        this.officer_Id = x.officer.Id;
      } else {
        this.officer_Id = 0;
        this.fpMatched = false;
      }
      this.matching = false;
    }, err => {

    });
  }
  public openWindow() {
    this.fileIssueDialogOpened = true;
  }
  public openCommentsWindow(item: any) {
    let officer = this._authenticationService.getCurrentOfficer();
    this.afrLog.Officer_Id = officer ? officer.Id : 30;
    this.afrLog.AFR_Id = item.Id;
    this.fileCommentsDialogOpened = true;

    this._fileTrackingSystemService.getApplicationFileReqLogs(this.afrLog.AFR_Id).subscribe((x: any) => {
      if (x) {
        this.afrLog.logs = x;

      } else {
        this._notificationService.notify('danger', 'Comments Error!');
      }
    }, err => { this.handleError(err); });
  }
  public issueReturnFile() {
    this.issuingFile = true;
    let reqIds: number[] = [];
    this.selectedRequests.forEach((req) => { reqIds.push(req.Id); });

    let toStatus_Id = this.statusId == 11 ? 2 : this.statusId == 1 ? 11 : this.statusId == 2 ? 3 : 0;
    if (toStatus_Id != 0) {
      this._fileTrackingSystemService.issueReturnFile(reqIds, this.officer_Id, toStatus_Id).subscribe((x: any) => {
        if (x.res) {
          let trackings: number[] = x.trackings as number[];
          if (trackings && trackings.length > 0) {
            /*  x.trackings.forEach(tracking => {
               setTimeout(() => {
                 this._firebaseHisduService.updateApplicationFirebase(tracking);
               }, 900);
             }); */
          }
          this.issuingFile = false;
          this.officer_Id = 0;
          this._notificationService.notify('success', toStatus_Id == 11 ? 'File Marked Available!' : toStatus_Id == 2 ? 'File Issued!' : toStatus_Id == 3 ? 'File Returned!' : '');
          this.getRequisitions();
          this.closeWindow();
        }
      }, err => { this.handleError(err); });
    }

  }
  public openInNewTab(link: string) {
    window.open(link, '_blank');
  }
  public saveApplicationFileReqLog() {
    this._fileTrackingSystemService.saveApplicationFileReqLog(this.afrLog).subscribe((x: any) => {
      if (x) {

        this.issuingFile = false;
        this.officer_Id = 0;
        this._notificationService.notify('success', 'Comments Saved');
        this.getRequisitions();
        this.closeCommentsWindow();
      } else {
        this._notificationService.notify('danger', 'Comments Error!');
      }
    }, err => { this.handleError(err); });

  }

  public dialogAction(action: string) {
    if (action == 'yes') {
      this.issueReturnFile();
    } else {
      this.closeWindow();
    }
  }
  public closeWindow() {
    this.issuingFile = false;
    this.officer_Id = 0;
    this.fpMatched = null;
    this.matching = false;
    this.bulkIssuingFile = false;
    this.fileIssueDialogOpened = false;
  }
  public closeCommentsWindow() {
    this.afrLog = {};
    this.fileCommentsDialogOpened = false;
  }

  public dropdownValueChanged = (value, filter) => {
    if (filter == 'status') {
      this.statusId = value.Id;
    }
    if (filter == 'officer') {
      this.officer_Id = value.Id;
    }
  }
  public sortChange(sort: SortDescriptor[]): void {
    if (sort[0].field == 'asd') { return; }
    this.kGrid.sort = sort;
    this.sortData();
  }
  private sortData() {
    this.kGrid.gridView = {
      data: orderBy(this.kGrid.data, this.kGrid.sort),
      total: this.kGrid.totalRecords
    };
  }
  public pageChange(event: PageChangeEvent): void {
    this.kGrid.skip = event.skip;
    this.getRequisitions();
  }
  public changePagesize(value: any) {
    this.kGrid.pageSize = +value;
    this.kGrid.skip = 0;
    this.getRequisitions();
  }
  private handleError(err: any) {
    this.kGrid.loading = false;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
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
