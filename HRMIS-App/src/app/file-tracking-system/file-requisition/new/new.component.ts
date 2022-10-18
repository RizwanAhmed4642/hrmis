import { Component, OnInit, OnDestroy } from '@angular/core';
import { KGridHelper } from '../../../_helpers/k-grid.class';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { Subject } from 'rxjs/Subject';
import { NotificationService } from '../../../_services/notification.service';
import { RootService } from '../../../_services/root.service';
import { FileTrackingSystemService } from '../../file-tracking-system.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { SortDescriptor, orderBy } from '@progress/kendo-data-query';
import { PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subscription } from 'rxjs/Subscription';
import { ApplicationLog } from '../../../modules/application-fts/application-fts';

@Component({
  selector: 'app-new',
  templateUrl: './new.component.html',
  styles: []
})
export class NewComponent implements OnInit, OnDestroy {

  public kGrid: KGridHelper = new KGridHelper();
  public dropDowns: DropDownsHR = new DropDownsHR();
  public selectedFile: any;
  public inputChange: Subject<any>;
  public searchQuery: string = '';
  public statusId: number = 11;
  public tabActionName: string = 'Issue File';
  public selectedRequests: any[] = [];
  public selectedRequestsIndex: number[] = [];
  public officer_Id: number = 30;
  public fileIssueDialogOpened: boolean = false;
  public requestDialogOpened: boolean = false;
  public requestingFile: boolean = false;
  public issuingFile: boolean = false;
  public bulkIssuingFile: boolean = false;
  public fileNotAvailable: boolean = false;
  public fileIssued: boolean = false;
  public fileshifted: boolean = false;
  public fileshiftedTo: string = '';
  public fileRequstedBy: string = '';
  public filesList: any[] = [];
  public ddsFilesList: any[] = [];
  public searchingFiles: boolean = false;
  public searchingDDSFiles: boolean = false;
  public searchEvent = new Subject<any>();
  public searchSubcription: Subscription = null;
  public applicationLog: ApplicationLog;
  constructor(public _notificationService: NotificationService, private _rootService: RootService, private _fileTrackingSystemService: FileTrackingSystemService, private _authenticationService: AuthenticationService) { }

  ngOnInit() {
    /*  this.inputChange = new Subject();
     this.inputChange.pipe(debounceTime(200)).subscribe((query) => {
       this.searchQuery = query;
       if (this.searchQuery.length <= 2 && this.searchQuery.length != 0) {
         return;
       }
     }); */
    this.initializeProps();
    this.loadDropDownValues();
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
        console.log(x);
        this.search(x.event, x.filter);
      });
  }
  public getFiles() {
    this._rootService.getDDSFiles(this.searchQuery, this.kGrid.skip, this.kGrid.pageSize).subscribe((response: any) => {
      this.kGrid.data = response.List;
      this.kGrid.totalRecords = response.Count;
      this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords };
      this.kGrid.loading = false;
      this.searchingFiles = false;
    }, err => {
      this.handleError(err);
    });
  }
  public sendFileRequest(reason: string, fileId: number) {
    this.requestingFile = true;
    this.fileshifted = false;
    this.applicationLog = new ApplicationLog();
    this.applicationLog.Remarks = reason;
    this._fileTrackingSystemService.generateFileRequest(this.applicationLog, fileId).subscribe((data: any) => {
      console.log(data);
      debugger;
      if (data) {
        if (data.result) {
          this.applicationLog = new ApplicationLog();
          this.requestingFile = false;
          this.fileNotAvailable = false;
          this.fileIssued = false;
          this.fileRequstedBy = null;
          this._notificationService.notify('success', 'Requested File ' + this.selectedFile.FileNumber);
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

          if (data.status == 'File has been shifted to South Punjab, Secretariat.') {
            this.fileshifted = true;
            this.fileshiftedTo = data.status;
          }
        }
      }
    }, err => {
      this.handleError(err);
    });
  }
  public search(value: string, filter: string) {
    if (filter == 'files') {
      this.filesList = [];
      this.searchQuery = value;
      this.searchingFiles = true;
      this.getFiles();
    }
    if (filter == 'ddsfiles') {
      this.ddsFilesList = [];
      if (value.length > 2) {
        this.searchingDDSFiles = true;
        this._rootService.getDDSFilesByFileNumber(value).subscribe((response: any) => {
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
      if (item) {
        //for Id //this.application.FileNumber = item.Id;
      }
    }
    if (filter == 'ddsfiles') {
      let item = this.ddsFilesList.find(x => x.FileNumber === FileNumber);
      if (item) {
        //for Id //this.application.FileNumber = item.Id;
        //for dds //this.application.FileNumber = item.Id;
      }
    }
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
    this.requestDialogOpened = false;
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
