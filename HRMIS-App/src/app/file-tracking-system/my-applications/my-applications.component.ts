import { Component, OnInit, OnDestroy } from '@angular/core';
import { KGridHelper } from '../../_helpers/k-grid.class';
import { DropDownsHR } from '../../_helpers/dropdowns.class';
import { Subject } from 'rxjs/Subject';
import { RootService } from '../../_services/root.service';
import { ApplicationFtsService } from '../../modules/application-fts/application-fts.service';
import { AuthenticationService } from '../../_services/authentication.service';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { PageChangeEvent } from '@progress/kendo-angular-grid';
import { FileTrackingSystemService } from '../file-tracking-system.service';
import { User } from '../../_models/user.class';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { ApplicationLog, FileMoveMaster } from '../../modules/application-fts/application-fts';
import { FirebaseHisduService } from '../../_services/firebase-hisdu.service';
import { NotificationService } from '../../_services/notification.service';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-my-applications',
  templateUrl: './my-applications.component.html',
  styles: []
})
export class MyApplicationsComponent implements OnInit, OnDestroy {
  public kGrid: KGridHelper = new KGridHelper();
  public user: User;
  public dropDowns: DropDownsHR = new DropDownsHR();
  public inputChange: Subject<any>;
  public searchQuery: string = '';
  public bulkRemarks: string = '';
  public type: string = '';
  public office_Id: number = 0;
  public typeId: number = 0;
  public statusId: number = 0;
  public minDate = new Date(2017, 8, 9);
  public maxDate = new Date();
  public showBulkActionToolBar: boolean = false;
  public showFilters: boolean = false;
  public range = { start: null, end: null };
  public generatingSlip: boolean = false;
  public generatingStatusSlip: boolean = false;
  public forwardingFiles: boolean = false;
  public statusUpdating: boolean = false;
  public forwardingWindow: boolean = false;
  public statusUpdatingWindow: boolean = false;
  public isCEO: boolean = false;
  public recieveFiles: boolean = false;
  public fromDate: Date = new Date();
  public concernedOfficers: number[] = [];
  public selectedApplications: any[] = [];
  public selectedApplicationsIndex: number[] = [];
  private subscription: Subscription;
  private inputChangeSubscription: Subscription;
  public fileMoveMaster: FileMoveMaster;
  public barcodeImgSrc: string = '';
  public dateNow: string = '';
  constructor(private sanitizer: DomSanitizer, private _rootService: RootService,
    private _fileTrackingSystemService: FileTrackingSystemService, private _notificationService: NotificationService,
    private _applicationFtsService: ApplicationFtsService, private route: ActivatedRoute, private router: Router,
    private _authenticationService: AuthenticationService, private _firebaseHisduService: FirebaseHisduService) { }

  ngOnInit() {
    this.user = this._authenticationService.getUser();
    this.isCEO = this.user.UserName.toLowerCase().startsWith('ceo.') ? true : false;
    //this.getApplications();
    this.fetchParams();
    this.subscribeInputChange();
    this.range.start = new Date(2017, 8, 9);
    this.range.end = new Date();
    this.maxDate.setDate(this.maxDate.getDate() + 1);
    this.kGrid.pageSizes = [50, 100, 200, 300, 500, 1000];
  }
  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('type')) {
          this.type = decodeURIComponent(params['type']);
        } else {
          this.type = null;
        }
        if (params.hasOwnProperty('statusId') && +params['statusId']) {
          this.statusId = +params['statusId'];
        } else {
          this.statusId = 0;
        }
        if (params.hasOwnProperty('from')) {

          this.range.start = new Date(params['from']);
        } else {
          this.range.start = null;
        }
        if (params.hasOwnProperty('to')) {
          this.range.end = new Date(params['to']);
        } else {
          this.range.end = null;
        }
        this.initializeProps();
      }
    );
  }
  private initializeProps() {
    let today = new Date();
    let dd: any = today.getDate(), mm: any = today.getMonth() + 1, yyyy = today.getFullYear();
    if (dd < 10) dd = '0' + dd; if (mm < 10) mm = '0' + mm;
    this.dateNow = dd + '/' + mm + '/' + yyyy;
    this.loadDropDownValues();
    this.kGrid.firstLoad = true;
    this.kGrid.loading = false;
    this.kGrid.pageSize = 50;
    this.kGrid.pageSizes = [50, 100];
  }
  private loadDropDownValues() {
    this.getApplicationTypes();
    this.getApplicationStatus();
    this.getInboxOffices();
    if (this.isCEO) {
      this.getPandSOfficers('ceo');
    } else {
      this.getPandSOfficers('concerned');
    }
  }
  private subscribeInputChange() {
    this.inputChange = new Subject();
    this.inputChangeSubscription = this.inputChange.pipe(debounceTime(300)).subscribe((query) => {
      this.searchQuery = query;
      if (this.searchQuery.length <= 2 && this.searchQuery.length != 0) {
        return;
      }
      this.getApplications();
    });
  }
  private getApplicationTypes() {
    this._rootService.getApplicationTypes().subscribe(
      (response: any) => {
        if (response) {
          this.dropDowns.applicationTypes = response;
          this.dropDowns.applicationTypesData = this.dropDowns.applicationTypes;
          if (this.type) {
            let ts = this.dropDowns.applicationTypes as any[];
            let t = ts.find(x => x.Name == this.type);
            if (t) {
              this.dropDowns.selectedFiltersModel.applicationType = t;
              this.typeId = t.Id;
            }
          } else {
            this.typeId = 0;
          }
          this.getApplications();
        }
      },
      err => this.handleError(err)
    );
  }
  private getApplicationStatus() {
    this._rootService.getApplicationStatus().subscribe(
      (response: any) => {
        if (response) {
          this.dropDowns.applicationStatus = response;
          this.dropDowns.applicationStatusData = this.dropDowns.applicationStatus;
          let sts = this.dropDowns.applicationStatus as any[];
          let st = sts.find(x => x.Id == this.statusId);
          if (st) {
            this.dropDowns.selectedFiltersModel.applicationStatus = st;
          }
        }
      },
      err => this.handleError(err)
    );
  }
  public dropdownValueChanged = (value, filter) => {
    if (filter == 'types') {
      this.typeId = value.Id;
    }
    if (filter == 'status') {
      this.statusId = value.Id;
    }
    if (filter == 'office') {
      this.office_Id = value.Id;
    }
    if (filter == 'officer') {
      this.dropDowns.selectedFiltersModel.officer = value;
    }
  }
  private getInboxOffices() {
    this.kGrid.loading = true;
    /* this._applicationFtsService.getInboxApplications(this.kGrid.skip, this.kGrid.pageSize, this.searchQuery, this.office_Id).subscribe(
      (response: any) => {
        this.kGrid.data = [];
        this.kGrid.data = response.List;
        this.kGrid.totalRecords = response.Count;
        this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords };
        this.kGrid.loading = false;
        console.log(response.List);
      },
      err => this.handleError(err)
    ); */
  }
  private getPandSOfficers = (type: string) => {
    this.concernedOfficers = [];
    this._rootService.getPandSOfficers(type).subscribe((res: any) => {
      if (res) {
        this.concernedOfficers = res;
      }
    },
      err => { this.handleError(err); }
    );
  }
  public getApplications() {
    this.kGrid.loading = true;
    this._fileTrackingSystemService.getApplications(this.kGrid.skip,
      this.kGrid.pageSize,
      this.searchQuery,
      this.office_Id,
      false,
      this.typeId,
      this.statusId,
      null,
      null,
      this.range.start, this.range.end).subscribe(
        (response: any) => {
          if (response) {
            this.selectedApplications = [];
            this.selectedApplicationsIndex = [];
            this.kGrid.data = [];
            this.kGrid.data = response.List;
            this.kGrid.totalRecords = response.Count;
            this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords };
          }
          this.kGrid.loading = false;
          this.kGrid.firstLoad = false;
          this.showFilters = false;
        },
        err => this.handleError(err)
      );
  }
  private bulkUpdateLogs() {
    this.forwardingFiles = true;
    for (let i = 0; i < this.selectedApplications.length; i++) {
      const application = this.selectedApplications[i];
      let applicationLog = new ApplicationLog();
      applicationLog.Application_Id = application.Id;
      applicationLog.ToOfficer_Id = this.dropDowns.selectedFiltersModel.officer.Id;
      applicationLog.ToOfficerName = this.dropDowns.selectedFiltersModel.officer.DesignationName;
      applicationLog.Remarks = this.bulkRemarks;
      application.updateingLog = true;
      this._fileTrackingSystemService.createApplicationLog(applicationLog).subscribe((response: ApplicationLog) => {
        if (response.Id) {
          //this._firebaseHisduService.updateApplicationFirebase(application.TrackingNumber);
          application.updateingLog = false;
          if (i == (this.selectedApplications.length - 1)) {
            this._notificationService.notify('success', 'Forwarded (' + this.selectedApplications.length + ') file' +
              (this.selectedApplications.length == 1 ? '' : 's' + ' to ' + applicationLog.ToOfficerName));
            this.selectedApplications[0].PandSOfficer_Id = applicationLog.ToOfficer_Id;
            this.saveFileMovement();
          }
        }
      },
        err => { this.forwardingFiles = false; this.handleError(err); }
      );
    }
  }
  private bulkUpdateStatusLogs() {
    this.statusUpdating = true;
    for (let i = 0; i < this.selectedApplications.length; i++) {
      const application = this.selectedApplications[i];
      let applicationLog = new ApplicationLog();
      applicationLog.Application_Id = application.Id;
      applicationLog.ToStatus_Id = 4;
      applicationLog.ToStatus = 'Disposed';
      if ((applicationLog.ToStatus_Id == 2 || applicationLog.ToStatus_Id == 3) && application.MobileNo) {
        applicationLog.SMS_SentToApplicant = true;
      }
      applicationLog.Remarks = this.bulkRemarks;
      application.updateingLog = true;
      this._fileTrackingSystemService.createApplicationLog(applicationLog).subscribe((response: ApplicationLog) => {
        if (response.Id) {
          //this._firebaseHisduService.updateApplicationFirebase(application.TrackingNumber);
          this.statusUpdating = false;
          this._notificationService.notify('success', 'Disposed (' + this.selectedApplications.length + ') file' +
            (this.selectedApplications.length == 1 ? '' : 's'));
          this.closeStatusUpdatingWindow();
        }
      },
        err => { this.statusUpdating = false; this.handleError(err); }
      );
    }
  }
  public saveFileMovement() {
    this._applicationFtsService.submitFileMovement2(this.selectedApplications).subscribe((x: any) => {
      this.fileMoveMaster = x.fileMoveMaster;
      if (this.fileMoveMaster.Id) {
        /* this._firebaseHisduService.updateApplicationFirebase(this.selectedApplications[0].TrackingNumber); */
        this.forwardingFiles = false;
        this.barcodeImgSrc = x.barCode;
        this.selectedApplications = [];
        this.selectedApplicationsIndex = [];
        this.getApplications();
        this.closeWindow();
        setTimeout(() => {
          //this.printApplication();
          this.closeWindow();
          //this.getInboxOffices();
        }, 300);
      }
    },
      err => {
        this.handleError(err);
      });
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
  public generateStatusSlip = () => {
    this.generatingStatusSlip = true;
    this.selectedApplications = [];
    this.selectedApplicationsIndex.forEach(index => {
      this.selectedApplications.push(this.kGrid.data[index]);
    });
    setTimeout(() => {
      this.openStatusUpdatingWindow();
    }, 600);
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
    this.dropDowns.selectedFiltersModel.officer = this.dropDowns.defultFiltersModel.officer;
    this.generatingSlip = false;
    this.forwardingWindow = false;
  }
  public closeStatusUpdatingWindow() {
    this.getApplications();
    this.generatingStatusSlip = false;
    this.statusUpdatingWindow = false;
  }
  public dialogAction(action: string) {
    if (action == 'yes') {
      this.bulkUpdateLogs();
    } else {
      this.closeWindow();
    }
  }
  public dialogStatusAction(action: string) {
    if (action == 'yes') {
      this.bulkUpdateStatusLogs();
    } else {
      this.closeStatusUpdatingWindow();
    }
  }
  public openWindow() {
    this.forwardingWindow = true;
  }
  public openStatusUpdatingWindow() {
    this.statusUpdatingWindow = true;
  }
  private handleError(err: any) {
    this.kGrid.loading = false;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }
  public numberWithCommas(x) {
    if (x) {
      return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
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
    this.subscription.unsubscribe();
    this.inputChangeSubscription.unsubscribe();
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
}
