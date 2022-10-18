import { Component, OnInit } from '@angular/core';
import { PageChangeEvent } from '@progress/kendo-angular-grid';
import { orderBy, SortDescriptor } from '@progress/kendo-data-query';
import { debounceTime } from 'rxjs/operators';
import { Subject } from 'rxjs/Subject';
import { Subscription } from 'rxjs/Subscription';
import { ApplicationLog } from '../../modules/application-fts/application-fts';
import { DropDownsHR } from '../../_helpers/dropdowns.class';
import { KGridHelper } from '../../_helpers/k-grid.class';
import { User } from '../../_models/user.class';
import { FileTrackingSystemService } from '../file-tracking-system.service';

@Component({
  selector: 'app-inquiry-files',
  templateUrl: './inquiry-files.component.html',
  styleUrls: ['./inquiry-files.component.scss']
})
export class InquiryFilesComponent implements OnInit {

  public fileMaster: [] = [];

  public currentUser: User;
  public kGrid: KGridHelper = new KGridHelper();
  public dropDowns: DropDownsHR = new DropDownsHR();
  public selectedFile: any;
  public inputChange: Subject<any>;
  public searchQuery: string = '';
  public statusId: number = 11;
  public tabActionName: string = 'Issue File';
  public imagePath: string = '';
  public selectedRequests: any[] = [];
  public selectedRequestsIndex: number[] = [];
  public officer_Id: number = 30;
  public fileIssueDialogOpened: boolean = false;
  public requestDialogOpened: boolean = false;
  public scannedFileDialogOpened: boolean = false;
  public requestingFile: boolean = false;
  public issuingFile: boolean = false;
  public bulkIssuingFile: boolean = false;
  public fileNotAvailable: boolean = false;
  public fileIssued: boolean = false;
  public fileRequstedBy: string = '';
  public filesList: any[] = [];
  public ddsFilesList: any[] = [];
  public searchingFiles: boolean = false;
  public searchingDDSFiles: boolean = false;
  public searchEvent = new Subject<any>();
  public searchSubcription: Subscription = null;
  public applicationLog: ApplicationLog;
  

  constructor(
    private _fileTrackingSystemService: FileTrackingSystemService,
  ) { }
  
  ngOnInit() {
    this.initializeProps();
    
  }

  private initializeProps() {
    this.kGrid.loading = false;
    this.kGrid.pageSize = 100;
    this.kGrid.pageSizes = [50, 100, 200, 300];
    // this.handleSearchEvents();
    // this.getFiles();
    this.getFileMaster();
  }

  // public handleSearchEvents() {
  //   this.searchSubcription = this.searchEvent.pipe(
  //     debounceTime(400)).subscribe((x: any) => {
  //       this.search(x.event, x.filter);
  //     });
  // }

  public changePagesize(value: any) {
    this.kGrid.pageSize = +value;
    this.kGrid.skip = 0;
    this.getFileMaster();
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
    this.getFileMaster();
  }

  public getFileMaster() {
    this._fileTrackingSystemService.getFileMaster(this.searchQuery, this.kGrid.skip, this.kGrid.pageSize).subscribe((data: any) => {
      if (data) {
      this.kGrid.data = data.List;
      this.kGrid.totalRecords = data.Count;
      this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords };
      this.kGrid.loading = false;
        console.log('File Master: ', this.kGrid.gridView);
      }
    },
      err => {
        console.log(err);
      });
  }
  

}
