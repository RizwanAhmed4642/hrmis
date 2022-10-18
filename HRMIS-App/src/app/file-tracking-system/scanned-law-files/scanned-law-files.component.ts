import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { KGridHelper } from '../../_helpers/k-grid.class';
import { DropDownsHR } from '../../_helpers/dropdowns.class';
import { Subject } from 'rxjs/Subject';
import { Subscription } from 'rxjs/Subscription';
import { NotificationService } from '../../_services/notification.service';
import { ApplicationLog } from '../../modules/application-fts/application-fts';
import { RootService } from '../../_services/root.service';
import { FileTrackingSystemService } from '../file-tracking-system.service';
import { AuthenticationService } from '../../_services/authentication.service';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { SortDescriptor, orderBy } from '@progress/kendo-data-query';
import { PageChangeEvent } from '@progress/kendo-angular-grid';
import { User } from '../../_models/user.class';

@Component({
  selector: 'app-scanned-law-files',
  templateUrl: './scanned-law-files.component.html',
  styleUrls: ['./scanned-law-files.component.scss']
})
export class ScannedLawFilesComponent implements OnInit, OnDestroy {
  @ViewChild('excelexport', { static: true }) public excelexport: any;
  public currentUser: User;
  public kGrid: KGridHelper = new KGridHelper();
  public dropDowns: DropDownsHR = new DropDownsHR();
  public selectedFile: any;
  public inputChange: Subject<any>;
  public searchQuery: string = '';
  public statusId: number = 11;
  public tabActionName: string = 'Issue File';
  public imagePath: string = '';
  public file: any = {};
  public selectedRequests: any[] = [];
  public allFiles: any[] = [];
  public selectedRequestsIndex: number[] = [];
  public officer_Id: number = 30;
  public fileIssueDialogOpened: boolean = false;
  public requestDialogOpened: boolean = false;
  public scannedFileDialogOpened: boolean = false;
  public requestingFile: boolean = false;
  public issuingFile: boolean = false;
  public bulkIssuingFile: boolean = false;
  public fileNotAvailable: boolean = false;
  public downloadingExcel: boolean = false;
  public fileIssued: boolean = false;
  public fileRequstedBy: string = '';
  public filesList: any[] = [];
  public ddsFilesList: any[] = [];
  public searchingFiles: boolean = false;
  public searchingDDSFiles: boolean = false;
  public searchEvent = new Subject<any>();
  public searchSubcription: Subscription = null;
  public applicationLog: ApplicationLog;
  public dateNow: string = '';
  public filterList: string[] = [
    'ALL',
    'PST',
    'Hight Court',
    'Hight Court Lahore',
    'Hight Court Multan',
    'Hight Court Bahawalpur',
    'Hight Court Rawalpindi',
    'Supreme Court',
    'Civil Court',
    'Intra Court Appeal'
  ];
  constructor(public _notificationService: NotificationService, private _rootService: RootService, private _fileTrackingSystemService: FileTrackingSystemService, private _authenticationService: AuthenticationService) { }

  ngOnInit() {
    /*  this.inputChange = new Subject();
     this.inputChange.pipe(debounceTime(200)).subscribe((query) => {
       this.searchQuery = query;
       if (this.searchQuery.length <= 2 && this.searchQuery.length != 0) {
         return;
       }
     }); */
    this.currentUser = this._authenticationService.getUser();
    this.initializeProps();
    this.loadDropDownValues();
    this.dateNow = this.formatDate(new Date());
  }
  private initializeProps() {
    this.kGrid.loading = false;
    this.kGrid.pageSize = 100;
    this.kGrid.pageSizes = [50, 100, 200, 300];
    this.handleSearchEvents();
    this.getFiles();
  }
  private loadDropDownValues() {
  }
  public handleSearchEvents() {
    this.searchSubcription = this.searchEvent.pipe(
      debounceTime(400)).subscribe((x: any) => {
        this.search(x.event, x.filter);
      });
  }
  public getFiles() {
    this.searchingFiles = true;
    this.kGrid.loading = true;
    this._fileTrackingSystemService.getLawFiles(
      {
        file: this.file,
        filter: {
          Query: this.searchQuery,
          Skip: this.kGrid.skip,
          PageSize: this.kGrid.pageSize,
        }
      }
    ).subscribe((response: any) => {
      this.kGrid.data = response.List;
      this.kGrid.totalRecords = response.Count;
      this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords };
      this.kGrid.loading = false;
      this.searchingFiles = false;
    }, err => {
      this.handleError(err);
    });
  }
  public getAllFiles() {
    this.downloadingExcel = true;
    this._fileTrackingSystemService.getLawFiles(
      {
        file: this.file,
        filter: {
          Query: this.searchQuery,
          Skip: 0,
          PageSize: this.kGrid.totalRecords,
        }
      }
    ).subscribe((response: any) => {
      this.allFiles = response.List;
      setTimeout(() => {
        this.downloadingExcel = false;
        this.excelexport.save();
      }, 1000);
    }, err => {
      this.handleError(err);
    });
  }
  public sendFileRequest(reason: string, fileId: number) {
    this.requestingFile = true;
    this.applicationLog = new ApplicationLog();
    // this.applicationLog.Application_Id = trackingId ? (trackingId - 9001) : null;
    this.applicationLog.Remarks = reason;
    this._fileTrackingSystemService.generateFileRequest(this.applicationLog, fileId).subscribe((data: any) => {
      if (data) {
        if (data.result) {
          this.applicationLog = new ApplicationLog();
          this.requestingFile = false;
          this.fileNotAvailable = false;
          this.fileIssued = false;
          this.fileRequstedBy = null;

          this._notificationService.notify('success', 'Requested File ' + this.selectedFile.DiaryNo ? this.selectedFile.DiaryNo : this.selectedFile.FileNumber ?
            this.selectedFile.FileNumber : this.selectedFile.UpdatedFileNumber);
          this.getFiles();
          this.closeRequestDialog();
        } else if (!data.result) {
          this.applicationLog = new ApplicationLog();
          this.requestingFile = false;
          if (this.selectedFile.InOutStatus == 'Out') {
            this.fileIssued = true;
            this.fileNotAvailable = false;
          } else {
            this.fileNotAvailable = true;
          }
          this.fileRequstedBy = data.madeBy;
        }
      }
    }, err => {
      this.handleError(err);
    });
  }
  public search(value: string, filter: string) {
    if (filter == 'files') {
      this.filesList = [];
      this.searchQuery = value == 'ALL' ? '' : value;
      this.searchingFiles = true;
      this.getFiles();
    }
    if (filter == 'ddsfiles') {
      this.ddsFilesList = [];
      if (value.length > 2) {
        this.searchingDDSFiles = true;
        this._rootService.getDDSFilesByCNIC(value).subscribe((response: any) => {
          this.kGrid.data = response.List;
          this.kGrid.totalRecords = response.Count;
          this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords };
          this.kGrid.loading = false;
          this.searchingFiles = false;
        });
      }
    }
  }
  public searchClicked(FileNumber, filter) {
    if (filter == 'files') {
      let item = this.filesList.find(x => x.FileNumber === FileNumber);
      console.log(item);
      if (item) {
        //for Id //this.application.FileNumber = item.Id;
      }
    }
    if (filter == 'ddsfiles') {
      let item = this.ddsFilesList.find(x => x.FileNumber === FileNumber);
      console.log(item);
      if (item) {
        //for Id //this.application.FileNumber = item.Id;
        //for dds //this.application.FileNumber = item.Id;
      }
    }
  }
  private formatDate(date) {
    var d = new Date(date),
      month = '' + (d.getMonth() + 1),
      day = '' + d.getDate(),
      year = d.getFullYear();

    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;

    return [day, month, year].join('-');
  }
  public openWindow() {
    this.bulkIssuingFile = false;
    this.fileIssueDialogOpened = true;
  }

  public dialogAction(action: string) {
    if (action == 'yes') {
    } else {
      this.closeWindow();
    }
  }
  public closeWindow() {
    this.fileIssueDialogOpened = false;
  }
  public openRequestDialog(item: any) {
    this.selectedFile = item;
    this.requestDialogOpened = true;
  }
  public closeRequestDialog() {
    this.requestingFile = false;
    this.fileNotAvailable = false;
    this.fileIssued = false;
    this.requestDialogOpened = false;
  }
  public openScannedFileDialog(item: any) {
    if (item.F_FileType_Id == 1) {
      this.imagePath = 'https://hrmis.pshealthpunjab.gov.pk/Uploads/Files/ACRFiles/' + item.Id + '.jpeg'
    } else {
      this.imagePath = 'https://hrmis.pshealthpunjab.gov.pk/Uploads/CRRFiles/' + item.RequestId + '-23.jpg'
    }
    this.scannedFileDialogOpened = true;
  }
  public closeScannedFileDialog() {
    this.scannedFileDialogOpened = false;
  }
  public dropdownValueChanged = (value, filter) => {
    if (filter == 'status') {
      this.statusId = value.Id;
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
    this.getFiles();
  }
  public changePagesize(value: any) {
    this.kGrid.pageSize = +value;
    this.kGrid.skip = 0;
    this.getFiles();
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
  ngOnDestroy() {
    this.searchSubcription.unsubscribe();
  }
}
