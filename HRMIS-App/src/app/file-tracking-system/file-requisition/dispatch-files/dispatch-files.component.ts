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
import { ApplicationLog, FileMoveMaster, FileMoveDetail } from '../../../modules/application-fts/application-fts';
import { DomSanitizer } from '@angular/platform-browser';
import { PandSOfficerView } from '../../../modules/user/user-claims.class';

@Component({
  selector: 'app-dispatch-files',
  templateUrl: './dispatch-files.component.html',
  styles: []
})
export class DispatchFilesComponent implements OnInit, OnDestroy {

  public kGrid: KGridHelper = new KGridHelper();
  public dropDowns: DropDownsHR = new DropDownsHR();
  public selectedFile: any;
  public inputChange: Subject<any>;
  public searchQuery: string = '';
  public toOfficerName: string = '';
  public tabActionName: string = 'Issue File';
  public selectedRequests: any[] = [];
  public selectedRequestsIndex: number[] = [];
  public officer_Id: number = 30;
  public fileIssueDialogOpened: boolean = false;
  public fileRecieveDialogOpened: boolean = false;
  public requestDialogOpened: boolean = false;
  public requestingFile: boolean = false;
  public alreadyAdded: boolean = false;
  public issuingFile: boolean = false;
  public bulkIssuingFile: boolean = false;
  public fileNotAvailable: boolean = false;
  public fileIssued: boolean = false;
  public fileRequstedBy: string = '';
  public barcodeSearch: string = '';
  public filesList: any[] = [];
  public ddsFilesList: any[] = [];
  public searchingFiles: boolean = false;
  public fileNoteFound: boolean = false;
  public searchingDDSFiles: boolean = false;
  public searchEvent = new Subject<any>();
  public searchSubcription: Subscription = null;
  public officerSubcription: Subscription = null;
  public applicationLog: ApplicationLog;
  public fileMoveMaster: FileMoveMaster = new FileMoveMaster();
  public savingFileMovement: boolean = false;
  public sectionOfficers: any[] = [];
  public barcodeImgSrc: string = '';
  public dateNow: string = '';
  public fromOfficerName: string = '';
  public loadingMovements: boolean = false;
  public searchingMovement: boolean = false;
  public viewFMDetail: boolean = false;
  public fileMovements: any[] = [];
  public fMMRecieve: any;
  public fMDRecieve: any[] = [];
  public currentOfficer: PandSOfficerView;
  public range = { start: null, end: null };
  constructor(private sanitizer: DomSanitizer, public _notificationService: NotificationService, private _rootService: RootService, private _fileTrackingSystemService: FileTrackingSystemService, private _authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.officerSubcription = this._rootService.getCurrentOfficer().subscribe((res: PandSOfficerView) => {
      if (res) {
        this.currentOfficer = res;
      }
    });
    /*  this.inputChange = new Subject();
     this.inputChange.pipe(debounceTime(200)).subscribe((query) => {
       this.searchQuery = query;
       if (this.searchQuery.length <= 2 && this.searchQuery.length != 0) {
         return;
       }
     }); */
    this.initializeProps();
    this.loadDropDownValues();
    this.getFileMovements();
  }
  private initializeProps() {
    let today = new Date();
    let dd: any = today.getDate(), mm: any = today.getMonth() + 1, yyyy = today.getFullYear();
    if (dd < 10) dd = '0' + dd; if (mm < 10) mm = '0' + mm;
    this.dateNow = dd + '/' + mm + '/' + yyyy;
    this.kGrid.loading = false;
    this.kGrid.pageSize = 100;
    this.kGrid.pageSizes = [50, 100, 200, 300];
    this.handleSearchEvents();
  }
  private loadDropDownValues() {
    this.getPandSOfficers('fts');
  }
  private getPandSOfficers = (type: string) => {
    this.sectionOfficers = [];
    this._rootService.getPandSOfficers(type).subscribe((res: any) => {
      this.sectionOfficers = res;
    },
      err => { this.handleError(err); }
    );
  }
  public handleSearchEvents() {
    this.searchSubcription = this.searchEvent.pipe(
      debounceTime(400)).subscribe((x: any) => {
        this.search(x.event, x.filter);
      });
  }
  public getFileMovements() {
    let obj = { from: this.range.start, to: this.range.end };
    this.loadingMovements = true;
    this._fileTrackingSystemService.getFileMovements(obj).subscribe((res: any) => {
      if (res.List) {
        this.fileMovements = res.List;
        this.loadingMovements = false;
      }
    }, err => {
      this.handleError(err);
    });
  }
  public getDDsFiles(barcode: string) {
    this._rootService.getApplication(+barcode - 9001, +barcode).subscribe((response: any) => {
      if (response && response.application && response.application.Id) {
        let existing = this.ddsFilesList.find(x => x.Id == response.application.Id);
        if (existing) {
          this.alreadyAdded = true;
        } else {
          this.ddsFilesList.push(response.application);
          this.barcodeSearch = '';
        }
      } else if (response == 'Invalid') {
        this.fileNoteFound = true;
      }
      this.searchingDDSFiles = false;
    }, err => {
      this.searchingDDSFiles = false;
      this.handleError(err);
    });
  }
  public generateSlip = () => {
    this.ddsFilesList.forEach(file => {
      let fileMoveDetail = new FileMoveDetail();
      fileMoveDetail.Application_Id = file.Id as number;
      this.fileMoveMaster.fileMoveDetails.push(fileMoveDetail);
    });
    setTimeout(() => {
      this.openWindow();
    }, 600);
  }
  public getFileMoveMasterDetails(mid: number) {
    if (!mid || !+mid) return;
    this.searchingMovement = true;
    this._fileTrackingSystemService.getFileMoveMaster(mid).subscribe((res: any) => {
      if (res && res.fileMoveMaster && res.fileMoveDetails) {
        this.fMMRecieve = res.fileMoveMaster;
        this.fMDRecieve = res.fileMoveDetails;
        this.openRecieveWindow();
      }
      this.searchingMovement = false;
    }, err => {
      this.searchingMovement = false;
      this.handleError(err);
    });
  }
  public saveFileMovement() {
    this.savingFileMovement = true;
    this._fileTrackingSystemService.submitFileMovement(this.fileMoveMaster).subscribe((x: any) => {
      console.log(x);
      this.fileMoveMaster = x.fileMoveMaster;
      let fromOfficer = this.sectionOfficers.find(x => x.Id == this.fileMoveMaster.FromOfficer_Id);
      if (fromOfficer) {
        this.fromOfficerName = fromOfficer.DesignationName;
      }
      this.barcodeImgSrc = x.barCode;

      setTimeout(() => {
        this.savingFileMovement = false;
        this.printApplication();
      }, 300);
    },
      err => {
        this.handleError(err);
      });
  }
  public recieveFile() {
    this._fileTrackingSystemService.submitFileMovement(this.fMMRecieve).subscribe((x: any) => {
      console.log(x);
      /* this.fileMoveMaster = x.fileMoveMaster;
      let fromOfficer = this.sectionOfficers.find(x => x.Id == this.fileMoveMaster.FromOfficer_Id);
      if (fromOfficer) {
        this.fromOfficerName = fromOfficer.DesignationName;
      }
      this.barcodeImgSrc = x.barCode; */

      setTimeout(() => {
        this.savingFileMovement = false;
        this.printApplication();
      }, 300);
    },
      err => {
        this.handleError(err);
      });
  }
  public search(value: string, filter: string) {
    if (filter == 'ddsfiles') {
      console.log(value);
      if (!value || value.length <= 2) {
        this.searchingDDSFiles = false;
        this.fileNoteFound = false;
        this.alreadyAdded = false;
        return;
      }
      this.searchQuery = value;
      this.searchingDDSFiles = true;
      this.fileNoteFound = false;
      this.alreadyAdded = false;
      this.getDDsFiles(value);
    }
    /* if (filter == 'ddsfiles') {
      this.ddsFilesList = [];
      if (value.length > 2) {
        this.searchingDDSFiles = true;
        this._rootService.getDDSFilesByFileNumber(value).subscribe((response: any) => {
          this.kGrid.data = response.List;
          this.kGrid.totalRecords = response.Count;
          this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords };
          this.kGrid.loading = false;
          this.searchingDDSFiles = false;
        });
      }
    } */
  }

  public openWindow() {
    this.fileIssueDialogOpened = true;
  }

  public dialogAction(action: string) {
    if (action == 'yes') {
      this.saveFileMovement();
    } else {
      this.closeWindow();
    }
  }
  public closeWindow() {
    this.fileIssueDialogOpened = false;
  }
  public openRecieveWindow() {
    this.fileRecieveDialogOpened = true;
  }

  public dialogRecieveAction(action: string) {
    if (action == 'yes') {
      this.recieveFile();
    } else {
      this.closeRecieveWindow();
    }
  }
  public closeRecieveWindow() {
    this.fileRecieveDialogOpened = false;
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
    if (filter == 'officer') {
      this.fileMoveMaster.ToOfficer_Id = value.Id;
      this.toOfficerName = value.DesignationName;
    }
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
    this.officerSubcription.unsubscribe();
  }
  public barcodeSrc() {
    return this.sanitizer.bypassSecurityTrustUrl(this.barcodeImgSrc);
  }
  printApplication() {
    let html = document.getElementById('formPrint').innerHTML;
    var mywindow = window.open('', 'PRINT', 'height=600,width=900');
    if (mywindow) {
      mywindow.document.write(`<html><head>
     
    <style>
   
    body {
      padding: 30px 50px 0 50px;
      font-family: -apple-system,system-ui;
  }
  table.file-table, table.file-table th, table.file-table td {
    border: 1px solid black;
    border-collapse: collapse;
  }
  table.file-table td {
    padding: 0.5rem;
}
  p {
    margin-top: 0;
    margin-bottom: 1rem !important;
}.mt-2 {
  margin-top: 0.5rem !important;
}.mb-0 {
  margin-bottom: 0 !important;
}
.ml-1 {
  margin-left: 0.25rem !important;
}
.mb-2 {
  margin-bottom: 0.5rem !important;
}
button.print {
  padding: 10px 40px;
  font-size: 21px;
  position: absolute;
  margin-left: 40%;
  background: #46a23f;
  background-image: linear-gradient(rgba(63, 162, 79, 0), rgba(63, 162, 79, 0.2));
  cursor: pointer;
  border: none;
  color: #ffffff;
  z-index: 9999;
}
@media print {
  button.print {
    display: none;
  }
}
    </style>
    <title>Application</title>`);
      mywindow.document.write('</head><body >');
      mywindow.document.write('<button onclick="printFunc()" class="print">Print</button>');
      mywindow.document.write(html);
      mywindow.document.write(`
      <script>
function printFunc() {
  window.print();
}
</script>
      `);
      mywindow.document.write('</body></html>');
      this.closeWindow();
      /*     mywindow.document.close(); // necessary for IE >= 10
          mywindow.focus(); // necessary for IE >= 10
          mywindow.print();
          mywindow.close(); */
    }
  }
}
