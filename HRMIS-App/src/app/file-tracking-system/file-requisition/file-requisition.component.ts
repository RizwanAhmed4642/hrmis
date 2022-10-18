import { Component, OnInit } from '@angular/core';
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

@Component({
  selector: 'app-file-requisition',
  templateUrl: './file-requisition.component.html',
  styles: []
})
export class FileRequisitionComponent implements OnInit {
  public kGrid: KGridHelper = new KGridHelper();
  public dropDowns: DropDownsHR = new DropDownsHR();
  public inputChange: Subject<any>;
  public searchQuery: string = '';
  public statusId: number = 0;
  public tabActionName: string = 'Issue File';
  public selectedRequests: any[] = [];
  public selectedRequestsIndex: number[] = [];
  public officer_Id: number = 30;
  public fileIssueDialogOpened: boolean = false;
  public issuingFile: boolean = false;
  public bulkIssuingFile: boolean = false;
  public currentUser: any;
  constructor(public _notificationService: NotificationService, private _rootService: RootService, private _fileTrackingSystemService: FileTrackingSystemService, private _authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.currentUser = this._authenticationService.getUser();
    this.inputChange = new Subject();
    this.inputChange.pipe(debounceTime(200)).subscribe((query) => {
      this.searchQuery = query;
      if (this.searchQuery.length <= 2 && this.searchQuery.length != 0) {
        return;
      }
      this.getMyRequisitions();
    });
    this.initializeProps();
    this.loadDropDownValues();
    this.getMyRequisitions();
  }
  private initializeProps() {
    this.kGrid.loading = false;
    this.kGrid.pageSize = 100;
    this.kGrid.pageSizes = [50, 100, 200, 300, 500, 1000];
  }
  private loadDropDownValues() {
    this.getFileRequestStatus();
  }
  private getFileRequestStatus() {
    this._rootService.getFileRequestStatuses().subscribe(
      (response: any) => {
        this.dropDowns.fileRequestStatus = response;
      },
      err => this.handleError(err)
    );
  }
  public getMyRequisitions() {
    this.kGrid.loading = true;
    /*  if (this.statusId == 0) {
       return;
     } */
    this.selectedRequests = [];
    this.selectedRequestsIndex = [];
    this.kGrid.data = [];
    this._fileTrackingSystemService.getMyRequisitions(this.kGrid.skip, this.kGrid.pageSize, this.searchQuery, this.statusId).subscribe(
      (response: any) => {
        this.kGrid.data = response.List;
        this.kGrid.totalRecords = response.Count;
        this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords };
        this.kGrid.loading = false;
      },
      err => this.handleError(err)
    );
  }
  public removeFileRequest(Id: number) {
    if (confirm("Confirm Remove File Request?")) {
      this.kGrid.loading = true;
      this._fileTrackingSystemService.removeFileRequest(Id).subscribe(
        (response: any) => {
          if (response) {
            this.getMyRequisitions();
          }
        },
        err => this.handleError(err)
      );
    }
  }
  public bulkIssue() {
    this.bulkIssuingFile = true;
    this.selectedRequests = [];
    this.selectedRequestsIndex.forEach(index => {
      this.selectedRequests.push(this.kGrid.data[index]);
    });
    console.log(this.selectedRequests);

    setTimeout(() => {
      this.openWindow();
    }, 300);
  }
  public openWindow() {
    this.bulkIssuingFile = false;
    this.fileIssueDialogOpened = true;
  }

  public closeWindow() {
    this.fileIssueDialogOpened = false;
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
    this.getMyRequisitions();
  }
  public changePagesize(value: any) {
    this.kGrid.pageSize = +value;
    this.kGrid.skip = 0;
    this.getMyRequisitions();
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
