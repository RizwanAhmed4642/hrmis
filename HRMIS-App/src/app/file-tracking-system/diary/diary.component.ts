import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { KGridHelper } from '../../_helpers/k-grid.class';
import { DropDownsHR } from '../../_helpers/dropdowns.class';
import { Subject } from 'rxjs/Subject';
import { RootService } from '../../_services/root.service';
import { ApplicationFtsService } from '../../modules/application-fts/application-fts.service';
import { AuthenticationService } from '../../_services/authentication.service';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { PageChangeEvent } from '@progress/kendo-angular-grid';
import { FileTrackingSystemService } from '../file-tracking-system.service';
import { FileMoveMaster, ApplicationLog } from '../../modules/application-fts/application-fts';
import { DomSanitizer } from '@angular/platform-browser';
import { FirebaseHisduService } from '../../_services/firebase-hisdu.service';
import { PandSOfficerView } from '../../modules/user/user-claims.class';
import { Subscription } from 'rxjs/Subscription';
import { orderBy, SortDescriptor, aggregateBy } from '@progress/kendo-data-query';
@Component({
  selector: 'app-diary',
  templateUrl: './diary.component.html',
  styleUrls: ['./diary.component.scss']
})
export class DiaryComponent implements OnInit, OnDestroy {
  @ViewChild('popup', {static: true}) calendarpopup;
  public kGrid: KGridHelper = new KGridHelper();
  public officerSubcription: Subscription = null;
  public loadingMovements: boolean = false;
  public loadingDiary: boolean = false;
  public searchingMovement: boolean = false;
  public barcodeImgSrc: string = '';
  public dateNow: string = '';
  public fileMovements: any[] = [];
  public fileMoveMasters: any[] = [];
  public fileMoveDetails: any[] = [];
  public diaries: any[] = [];
  public viewType: string = 'main';
  public viewVal: string = 'all';
  public midNumber: number;
  public fMMRecieve: any;
  public fMDRecieve: any[] = [];
  public currentOfficer: PandSOfficerView;
  public applicationLog: ApplicationLog;
  public fileMoveMaster: any;
  public savingFileMovement: boolean = false;
  public fileRecieveDialogOpened: boolean = false;
  public aggregates: any[] = [];
  public totalSums: any;
  public sectionOfficers: any[] = [];
  public officesConcerned: any[] = [];
  public selectedOffice: any;
  public range = { start: null, end: null };
  constructor(private sanitizer: DomSanitizer, private _rootService: RootService, private _firebaseHisduService: FirebaseHisduService, private _fileTrackingSystemService: FileTrackingSystemService, private _applicationFtsService: ApplicationFtsService, private _authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.subscribeOfficer();
    //this.getFileMovements();
    this.getDiary();
    this.getOfficers();
  }
  public subscribeOfficer() {
    this.officerSubcription = this._rootService.getCurrentOfficer().subscribe((res: PandSOfficerView) => {
      if (res) {
        this.currentOfficer = res;
      }
    });
  }
  public getOfficers() {
    this._rootService.getPandSOfficers('concerned').subscribe((res: any) => {
      this.officesConcerned = res;
    }, err => {
      this.handleError(err);
    });
  }
  public getDiary() {
    this.loadingDiary = true;
    this._fileTrackingSystemService.getDiary(this.range.start, this.range.end).subscribe((res: any) => {
      if (res) {
        this.diaries = res;
        this.kGrid.data = [];
        this.kGrid.data = res;
        this.kGrid.totalRecords = this.kGrid.data.length;
        this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords };
        this.setTotalAggregates();
        this.loadingDiary = false;
        if (this.calendarpopup) this.calendarpopup.toggle(false);
        this.viewType = 'main';
      }
    }, err => {
      this.handleError(err);
    });
  }
  public back() {
    if (this.viewType == 'master') {
      this.viewType = 'main';
    }
  }
  public getFileMovements(office: any, type: string) {
    this.selectedOffice = office;
    this.loadingMovements = true;
    let obj = { from: this.range.start ? this.range.start.toDateString() : null, to: this.range.end ? this.range.end.toDateString() : null, MIDNumber: this.midNumber, FromOfficer_Id: 0, ToOfficer_Id: 0 };
    if (type == 'from') {
      obj.FromOfficer_Id = office.Id;
      this.viewVal = 'Recieved';
    } else if (type == 'to') {
      obj.ToOfficer_Id = office.Id;
      this.viewVal = 'Sent';
    } else if (type == 'all') {
      this.viewVal = 'Sent/Recieved';
    }
    this._fileTrackingSystemService.getFileMovements(obj).subscribe((res: any) => {
      if (res.List) {
        this.fileMoveMasters = res.List;
        this.fileMoveDetails = res.listDetails;
        this.loadingMovements = false;
        this.viewType = 'master';
        if (this.calendarpopup) this.calendarpopup.toggle(false);
      }
    }, err => {
      this.handleError(err);
    });
  }
  public getDetailsByMasterId(mid: number) {
    return this.fileMoveDetails.filter(x => x.Master_Id == mid);
  }
  public getFileMoveMasterDetails(mid: number) {
    if (!mid || !+mid) return;
    this.searchingMovement = true;
    this._fileTrackingSystemService.getFileMoveMaster(mid).subscribe((res: any) => {
      if (res && res.fileMoveMaster && res.fileMoveDetails) {
        this.fMMRecieve = res.fileMoveMaster;
        this.fMDRecieve = res.fileMoveDetails;
        this._rootService.generateBars(this.fMMRecieve.MID_Number).subscribe((res: any) => {
          if (res) {
            this.barcodeImgSrc = res.barCode;
          }
        });
        this.openRecieveWindow();
      }
      this.searchingMovement = false;
    }, err => {
      this.searchingMovement = false;
      this.handleError(err);
    });
  }
  public setTotalAggregates() {
    this.aggregates = [
      { field: 'ToOfficerCount', aggregate: 'sum' },
      { field: 'FromOfficerCount', aggregate: 'sum' },
    ];
    this.totalSums = aggregateBy(this.kGrid.gridView.data, this.aggregates);
    if (this.calendarpopup) {
      this.calendarpopup.toggle(false);
    }
  }
  public saveFileMovement() {
    this.savingFileMovement = true;
    this._fileTrackingSystemService.submitFileMovement(this.fileMoveMaster).subscribe((x: any) => {
      this.fileMoveMaster = x.fileMoveMaster;
      let fromOfficer = this.sectionOfficers.find(x => x.Id == this.fileMoveMaster.FromOfficer_Id);
      if (fromOfficer) {
        this.currentOfficer.DesignationName = fromOfficer.DesignationName;
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
      this.fileMoveMaster = x.fileMoveMaster;
      this.barcodeImgSrc = x.barCode;
      setTimeout(() => {
        this.savingFileMovement = false;
        this.closeRecieveWindow();
        this.printApplication();
      }, 300);
    },
      err => {
        this.handleError(err);
      });
  }
  public searchOffice(query: string) {
    console.log(query);

    this.viewType = 'main';
    if (!query) {
      this.kGrid.gridView.data = this.kGrid.data;
      return;
    }
    this.kGrid.gridView.data = [];
    this.kGrid.gridView.data = this.kGrid.data.filter(x => x.DesignationName.toLowerCase().indexOf(query.toLowerCase()) >= 0);
  }
  public searchMIDNumber() {
    if (!this.midNumber || this.midNumber.toString().length < 3) {
      this.viewType = 'main';
      return;
    }
    this._fileTrackingSystemService.getFileMovements({ MIDNumber: this.midNumber }).subscribe((res: any) => {
      if (res.List) {
        this.fileMoveMasters = res.List;
        this.fileMoveDetails = res.listDetails;
        this.loadingMovements = false;
        this.viewType = 'master';
        if (this.calendarpopup) this.calendarpopup.toggle(false);
      }
    }, err => {
      this.handleError(err);
    });
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
    this.getDiary();
  }
  public changePagesize(value: any) {
    this.kGrid.pageSize = +value;
    this.kGrid.skip = 0;
    this.getDiary();
  }
  public openRecieveWindow() {
    this.fileRecieveDialogOpened = true;
  }

  public dialogRecieveAction(action: string) {
    if (action == 'yes') {
      this.recieveFile();
    } else if (action == 'print') {
      this.printApplication();
    } else {
      this.closeRecieveWindow();
    }
  }
  public closeRecieveWindow() {
    this.fileRecieveDialogOpened = false;
  }
  private handleError(err: any) {
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
      /* this.dropDowns.selectedFiltersModel.inboxOfficers = { Name: 'Select Office', Id: 0 };
      this.dropdownValueChanged(this.dropDowns.selectedFiltersModel.inboxOfficers, 'office'); */
      //this.getFileMovements();
      /*     mywindow.document.close(); // necessary for IE >= 10
          mywindow.focus(); // necessary for IE >= 10
          mywindow.print();
          mywindow.close(); */
    }
  }
}
