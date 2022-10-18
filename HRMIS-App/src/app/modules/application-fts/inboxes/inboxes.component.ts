import { Component, OnInit, OnDestroy } from '@angular/core';
import { KGridHelper } from '../../../_helpers/k-grid.class';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { Subject } from 'rxjs/Subject';
import { FileMoveMaster } from '../application-fts';
import { DomSanitizer } from '@angular/platform-browser';
import { FirebaseHisduService } from '../../../_services/firebase-hisdu.service';
import { FileTrackingSystemService } from '../../../file-tracking-system/file-tracking-system.service';
import { ApplicationFtsService } from '../application-fts.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { PageChangeEvent } from '@progress/kendo-angular-grid';
import { RootService } from '../../../_services/root.service';
import { Subscription } from 'rxjs/Subscription';

@Component({
  selector: 'app-inboxes',
  templateUrl: './inboxes.component.html',
  styles: []
})
export class InboxesComponent implements OnInit, OnDestroy {
  public kGrid: KGridHelper = new KGridHelper();
  public dropDowns: DropDownsHR = new DropDownsHR();
  public inputChange: Subject<any>;
  public dateNow: string = '';
  public searchQuery: string = '';
  public office_Id: number = 0;
  public officeName: number = 0;
  public recieveFiles: boolean = false;
  public generatingSlip: boolean = false;
  public savingFileMovement: boolean = false;
  public acknowledgeDialogOpened: boolean = false;
  public fileMoveMaster: FileMoveMaster;
  public selectedApplications: any[] = [];
  public selectedApplicationsIndex: number[] = [];
  public barcodeImgSrc: string = '';
  public officer_Id: number = 0;
  public officerName: string = '';
  public searchEvent = new Subject<any>();
  public searchSubcription: Subscription = null;
  constructor(private sanitizer: DomSanitizer,
    private _firebaseHisduService: FirebaseHisduService,
    private _applicationFtsService: ApplicationFtsService,
    private _rootService: RootService,
    private _authenticationService: AuthenticationService) { }

  ngOnInit() {
    window.scroll(0, 0);
    this.kGrid.firstLoad = true;
    this.inputChange = new Subject();
    this.inputChange.pipe(debounceTime(300)).subscribe((query) => {
      this.searchQuery = query;
      if (this.searchQuery.length <= 2 && this.searchQuery.length != 0) {
        return;
      }
      this.getApplications();
    });
    this.initializeProps();
    this.loadDropDownValues();
    this.handleSearchEvents();
  }
  private initializeProps() {
    let today = new Date();
    let dd: any = today.getDate(), mm: any = today.getMonth() + 1, yyyy = today.getFullYear();
    if (dd < 10) dd = '0' + dd; if (mm < 10) mm = '0' + mm;
    this.dateNow = dd + '/' + mm + '/' + yyyy;

    this.kGrid.loading = false;
    this.kGrid.pageSize = 50;
    this.kGrid.pageSizes = [50, 100];
  }
  private loadDropDownValues() {
    this.getPandSOfficers('hisduInbox');
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
  public handleSearchEvents() {
    this.searchSubcription = this.searchEvent.pipe(
      debounceTime(400)).subscribe((query: any) => {
        this.searchQuery = query;
        if (this.searchQuery.length <= 2 && this.searchQuery.length != 0) {
          return;
        }
        this.getApplications();
      });
  }
  private getInboxOffices() {
    if (this.officer_Id == 0) return;
    this.selectedApplicationsIndex = [];
    this._applicationFtsService.getOfficesHisdu(this.officer_Id).subscribe((response: any) => {
      this.dropDowns.inboxOfficers = response;
    },
      err => {
        this.handleError(err);
      });
  }
  private getApplications() {
    if (this.officer_Id == 0) return;
    this.recieveFiles = false;
    this.kGrid.loading = true;
    this.selectedApplicationsIndex = [];
    this._applicationFtsService.getApplicationsHisdu(this.kGrid.skip, this.kGrid.pageSize, this.searchQuery, this.office_Id, this.officer_Id).subscribe(
      (response: any) => {
        if (response) {
          this.kGrid.data = [];
          this.kGrid.data = response.List;
          this.kGrid.dataOrigional = response.List;
          this.kGrid.totalRecords = response.Count;
          this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords };
        }
        this.kGrid.loading = false;
        this.kGrid.firstLoad = false;
        this.recieveFiles = true;
      },
      err => this.handleError(err)
    );
  }
  public generateSlip = () => {
    this.generatingSlip = true;
    this.selectedApplications = [];
    this.selectedApplicationsIndex.forEach(index => {
      this.selectedApplications.push(this.kGrid.data[index]);
    });
    setTimeout(() => {
      this.openWindow();
    }, 600);
  }
  public saveFileMovement() {
    this.savingFileMovement = true;
    this._applicationFtsService.submitFileMovementFDO(this.selectedApplications, this.officer_Id).subscribe((x: any) => {
      this.fileMoveMaster = x.fileMoveMaster;
      if (this.fileMoveMaster.Id) {
       /*  this._firebaseHisduService.updateApplicationFirebase(this.selectedApplications[0].TrackingNumber); */
        this.barcodeImgSrc = x.barCode;
        setTimeout(() => {
          this.savingFileMovement = false;
          this.printApplication();
          this.closeWindow();
          this.getInboxOffices();
        }, 300);
      }
    },
      err => {
        this.handleError(err);
      });
  }
  public dropdownValueChanged = (value, filter) => {
    this.searchQuery = '';
    if (filter == 'officer') {
      this.officer_Id = value.Id;
      this.officerName = value.DesignationName;
      this.getInboxOffices();
      this.getApplications();
    }
    if (filter == 'office') {
      this.office_Id = value.Id;
      this.officeName = value.Name;
      this.selectedApplicationsIndex = [];
      this.getApplications();
    }
  }
  public pageChange(event: PageChangeEvent): void {
    this.kGrid.skip = event.skip;
    this.getApplications();
  }
  public changePagesize(value: any) {
    this.kGrid.pageSize = +value;
    this.kGrid.skip = 0;
    this.getApplications();
  }
  public closeWindow() {
    this.generatingSlip = false;
    this.acknowledgeDialogOpened = false;
  }
  public dialogAction(action: string) {
    if (action == 'yes') {
      this.saveFileMovement();
    } else {
      this.closeWindow();
    }
  }
  public openWindow() {
    this.acknowledgeDialogOpened = true;
  }
  private handleError(err: any) {
    this.savingFileMovement = false;
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
  public numberWithCommas(x) {
    if (x) {
      return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }
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
      font-family: -apple-system,system-ui;
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
      this.dropDowns.selectedFiltersModel.inboxOfficers = { Name: 'Select Office', Id: 0 };
      this.dropdownValueChanged(this.dropDowns.selectedFiltersModel.inboxOfficers, 'office');
      this.getApplications();

      /*     mywindow.document.close(); // necessary for IE >= 10
          mywindow.focus(); // necessary for IE >= 10
          mywindow.print();
          mywindow.close(); */
    }
  }
  ngOnDestroy() {
    this.searchSubcription.unsubscribe();
  }
}
