import { Component, OnInit, OnDestroy, Input } from "@angular/core";
import { DomSanitizer } from "@angular/platform-browser";
import { FileTrackingSystemService } from "../../file-tracking-system.service";
import { ActivatedRoute, Router } from "@angular/router";
import { NotificationService } from "../../../_services/notification.service";
import { FirebaseHisduService } from "../../../_services/firebase-hisdu.service";
import { RootService } from "../../../_services/root.service";
import { AuthenticationService } from "../../../_services/authentication.service";
import { ApplicationView, ApplicationLogView, ApplicationAttachment, ApplicationLog } from "../../../modules/application-fts/application-fts";
import { Subscription } from "rxjs/Subscription";
import { DropDownsHR } from "../../../_helpers/dropdowns.class";

@Component({
  selector: 'app-application-details',
  templateUrl: './details.component.html',
  styles: []
})
export class DetailsComponent implements OnInit, OnDestroy {
  constructor(private sanitizer: DomSanitizer,
    private route: ActivatedRoute, public _notificationService: NotificationService,
    private router: Router, private _firebaseHisduService: FirebaseHisduService, private _rootService: RootService, private _authenticationService: AuthenticationService) {

  }
  @Input() public trackingNo: number = 0;
  public application: any;
  public applicationLogs: any[] = [];
  public applicationAttachments: any[] = [];
  public appsRealtimeSubscription: Subscription;
  public loading: boolean = false;
  public exist: boolean = true;
  public editApplication: boolean = true;
  public logRefreshing: boolean = false;
  public lastLogId: number = 0;
  public logAscOrder: boolean = true;
  public barcodeImgSrc: string = '';
  public applicationM: any;
  public appHistory: any[] = [];
  public personAppeared: any;

  public selectedDispatchOfficer: any;
  public selectedDispatch: any;
  public dispatchDialogOpened: boolean = false;
  public successDialogOpened: boolean = false;
  public forwardingApplication: boolean = false;
  public savingFileMovement: boolean = false;
  public isAdmin: boolean = false;
  public myPUC: boolean = false;
  public skipDisposalTypes: number[] = [1, 2, 4, 10, 15, 16, 20, 23, 24, 25, 30, 31, 32, 33, 34, 35, 44];
  public pucStep: number = 0;
  public pucLocation: string = '';
  public currentUser: any;
  public currentOfficer: any;
  public dropDowns: DropDownsHR = new DropDownsHR();
  public applicationLog: ApplicationLog = new ApplicationLog();


  ngOnInit() {
    //this.subscribeLiveApp(); 
    this.currentUser = this._authenticationService.getUser();
    this.currentOfficer = this._authenticationService.getCurrentOfficer();
    this.isAdmin = this.currentUser.UserName == 'chairman' || this.currentUser.UserName == 'secretary' ? true : false;

    this.getApplication();
    //this.searchFtsApplicaiton();
    this.getPandSOfficers('fts');
  }
  private subscribeLiveApp() {
    /*  this.appsRealtimeSubscription = this._firebaseHisduService.getAppsChanged().subscribe((apps: any) => {
         if (apps && this.application) {
             let app = apps.find(x => x.trackingNo == this.application.TrackingNumber);
             if (app) {
                 this.getApplicationLog(false);
             }
         }
     }); */
  }
  public getApplication() {
    if (this.trackingNo == 0) {
      this._notificationService.notify('warning', 'Tracking Id cannot be zero');
      this.exist = true;
      return;
    }
    if (!+this.trackingNo) {
      this._notificationService.notify('warning', 'Invalid Tracking Id');
      this.exist = true;
      return;
    }
    this.loading = true;
    this.exist = true;
    this._rootService.getApplication((+this.trackingNo - 9001), this.trackingNo).subscribe((data: any) => {
      if (data) {
        this.application = data.application;
        this.getApplicationStatus();

        if (this.currentOfficer) {
          if (this.application.PandSOfficer_Id == this.currentOfficer.Id && !this.application.IsPending) {
            this.myPUC = true;
            this.pucStep = 2;
            this.pucLocation = 'My Application';
            /*   this.dropDowns.selectedFiltersModel.officer = { Id: this.application.PandSOfficer_Id, DesignationName: this.application.PandSOfficerName }; */
          }
          if (this.application.ToOfficer_Id == this.currentOfficer.Id && this.application.IsPending == true) {
            this.myPUC = true;
            this.pucStep = 1;
            this.pucLocation = 'Inbox';
          }
          if (this.application.PandSOfficer_Id == this.currentOfficer.Id && this.application.IsPending == true) {
            this.myPUC = true;
            this.pucStep = 3;
            this.pucLocation = 'Sent Application';
            this.dropDowns.selectedFiltersModel.officer = { Id: this.application.ToOfficer_Id, DesignationName: this.application.ToOfficerName };
          }
        }
        this._rootService.generateBars(this.application.TrackingNumber).subscribe((res: any) => { this.barcodeImgSrc = res.barCode; }, err => { this.handleError(err); });
        this.applicationLog.Application_Id = this.application.Id;
        this.dropDowns.selectedFiltersModel.applicationStatus = { Id: this.application.Status_Id, Name: this.application.StatusName };
        this.applicationAttachments = data.applicationAttachments as ApplicationAttachment[];
        this.getApplicationData('logs');
        this.getApplicationData("parliamentarian");
      } else {
        this.loading = false;
        this.exist = false;
        this._notificationService.notify('warning', 'No Application Found.');
      }
    }, err => {
      this.loading = false;
      this.exist = true;
      console.log(err);
    })
  }
  private getApplicationData(type) {
    this._rootService.getApplicationData(this.application.Id, type).subscribe((data: any) => {
      if (data) {
        if (type == "logs") {
          this.getApplicationLog(true);
        } else if (type == "parliamentarian") {
          this.personAppeared = data.applicationPersonAppeared;
        }
        this.loading = false;
      }
    }, err => { this.handleError(err); }
    );
  }
  public getApplicationLog(all: boolean) {
    this.logRefreshing = true;
    let lastLog = null;
    if (this.applicationLogs.length > 0) {
      lastLog = this.applicationLogs[this.applicationLogs.length - 1];
      this.lastLogId = lastLog ? lastLog.Id : 0;
    }
    if (!all) {
      this.lastLogId = 0;
    }
    this._rootService.getApplicationLogs(this.application.Id, 0, this.logAscOrder).subscribe((data: any) => {
      if (data) {
        /* if (lastLog) {
            data.forEach(element => {
                if (this.logAscOrder) {
                    this.applicationLogs.push(element);
                } else {
                    this.applicationLogs.unshift(element);
                }
            });
        } else { */
        this.applicationLogs = [];

        this.applicationLogs = data;
        /*  } */
        this.logRefreshing = false;
      }
    }, err => { this.handleError(err); }
    );
  }


  private getApplicationStatus() {
    this._rootService.getApplicationStatus().subscribe(
      (response: any) => {
        if (response) {
          let statuses = [];
          response.forEach(element => {
            if (this.currentUser.RoleName == 'Section Officer') {
              if (element.Id == 1 || element.Id == 3 || element.Id == 8 || element.Id == 10) {
                statuses.push(element);
              }
            } else {
              if (element.Id == 1 || element.Id == 2 || element.Id == 3 || element.Id == 8 || element.Id == 10) {
                statuses.push(element);
              }
            }
          });
          if (this.application.ApplicationSource_Id == 10) {
            statuses = [];
            response.forEach(element => {
              if (element.Id == 1 || element.Id == 4) {
                statuses.push(element);
              }
            });
          } else {

            let exist = this.skipDisposalTypes.find(x => x == this.application.ApplicationType_Id);
            if (!exist) {
              response.forEach(element => {
                if (element.Id == 4) {
                  statuses.push(element);
                }
              });
            }
          }
          statuses = statuses.sort((a, b) => (a.Id > b.Id ? -1 : 1));
          this.dropDowns.applicationStatus = statuses;
          this.dropDowns.applicationStatusData = this.dropDowns.applicationStatus;
        }
      },
      err => this.handleError(err)
    );
  }
  public saveFileMovement() {
    this.savingFileMovement = true;
    this._rootService.submitFileMovement([this.application.Id]).subscribe((x: any) => {
      if (x && x.fileMoveMaster && x.fileMoveMaster.Id) {
        this._notificationService.notify('success', 'File Recieved');
        /*    if (!this.shouldPrint) {
             this.closeWindow();
           } */
        this.savingFileMovement = false;
        this.getApplication();
        /*  setTimeout(() => { */
        /* if (this.shouldPrint) {
          this.openSuccessDialog(); */
        //this.printApplication();
        /*    } else { */
        /*  this.dropDowns.selectedFiltersModel.inboxOfficers = { Name: 'Select Office', Id: 0 };
         this.dropdownValueChanged(this.dropDowns.selectedFiltersModel.inboxOfficers, 'office');
       */
        /*    } */
        /*   }, 300); */
        //this._firebaseHisduService.updateApplicationFirebase(this.selectedApplications[0].TrackingNumber);

      }
    },
      err => {
        this.handleError(err);
      });
  }

  public dropdownValueChanged = (value, filter) => {
    if (filter == 'officer') {
      this.applicationLog.ToOfficer_Id = value.Id;
      this.applicationLog.ToOfficerName = value.DesignationName;
    }
    if (filter == 'status') {
      this.applicationLog.ToStatus_Id = value.Id;
      this.applicationLog.ToStatus = value.Name;
      if ((this.applicationLog.ToStatus_Id == 2 || this.applicationLog.ToStatus_Id == 3 || this.applicationLog.ToStatus_Id == 8) && this.application.MobileNo) {
        this.applicationLog.SMS_SentToApplicant = true;
      }
    }
  }
  public handleFilter = (value, filter) => {
    if (filter == 'officer') {
      this.dropDowns.officersData = this.dropDowns.officers.filter((s: any) => (s.DesignationName ? s.DesignationName.toLowerCase().indexOf(value.toLowerCase()) : 0) !== -1);
    }

  }
  private getPandSOfficers = (type: string) => {
    this.dropDowns.officers = [];
    this.dropDowns.officersData = [];
    this._rootService.getPandSOfficers(type).subscribe((res: any) => {
      if (res) {
        this.dropDowns.officers = res;
        this.dropDowns.officers = this.dropDowns.officers.filter(x => x.Id != this.currentOfficer.Id);
        this.dropDowns.officersData = this.dropDowns.officers.filter(x => x.Id != this.currentOfficer.Id);
      }
    },
      err => { this.handleError(err); }
    );
  }
  updateLog() {
    this.forwardingApplication = true;
    if (this.currentUser.UserName.toLowerCase().startsWith('ceo.')) {
      this.applicationLog.FromOfficer_Id = 71;
    }
    this._rootService.createApplicationLog(this.applicationLog).subscribe((response: ApplicationLog) => {
      if (response.Id) {
        this.forwardingApplication = false;
        this.applicationLog = new ApplicationLog();
        this.getApplication();
        this.openSuccessDialog();
      }
    },
      err => { this.handleError(err); }
    );
  }
  public openSuccessDialog() {
    this.successDialogOpened = true;
  }
  public closeSuccessDialog() {
    this.successDialogOpened = false;
    //this.router.navigate(['/fts/my-applications']);
  }
  public searchFtsApplicaiton() {
    if (!this.trackingNo || !+this.trackingNo) {
      return;
    }
    this._rootService.getFTsToken({ userName: 'hr.viewonly', password: 'asdfg23' }).subscribe((res: any) => {
      console.log(res);
      if (res && res.access_token) {
        console.log(res);
        this._rootService.getApplicationsFTs(this.trackingNo, res.access_token).subscribe((x: any) => {
          if (x && x.result == true) {
            this.applicationM = x.applicationM;
            this.appHistory = x.appHistory;
            console.log(x);
          }
        }, err => {
          console.log(err);
          this.handleError(err);
        });
      }
    },
      err => {
        console.log(err);
        this.handleError(err);
      }
    );
  }
  public getAwesomeDate(date: any) {
    let newDate = new Date(date);
    let dd: any = newDate.getDate(), mm: any = this.makeOrderMonth(newDate.getMonth() + 1), yyyy = newDate.getFullYear();

    return this.makeOrderDate(dd) + ', ' + mm + ' ' + yyyy;
  }
  transform(value) {
    return this.sanitizer.bypassSecurityTrustHtml(value);
  }
  public capitalize(val: string) {
    if (!val) return '';
    return val.toUpperCase();
  }
  makeOrderDate(day: number) {
    return day == 1 || day == 21 || day == 31 ? day + '<sup>st</sup>'
      : day == 2 || day == 22 ? day + '<sup>nd</sup>'
        : day == 3 || day == 23 ? day + '<sup>rd</sup>'
          : (day => 4 && day <= 20) || (day => 24 && day <= 30) ? day + '<sup>th</sup>' : '';
  }
  makeOrderMonth(month: number) {
    return month == 1 ? 'January'
      : month == 2 ? 'February'
        : month == 3 ? 'March'
          : month == 4 ? 'April'
            : month == 5 ? 'May'
              : month == 6 ? 'June'
                : month == 7 ? 'July'
                  : month == 8 ? 'August'
                    : month == 9 ? 'September'
                      : month == 10 ? 'October'
                        : month == 11 ? 'November'
                          : month == 12 ? 'December' : '';
  }
  public barcodeSrc() {
    return this.sanitizer.bypassSecurityTrustUrl(this.barcodeImgSrc);
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
  ngOnDestroy(): void {

  }
  public handleError(err) {
    console.log(err);
  }

  printBarcode() {
    let html = document.getElementById('barcodeFileBars').innerHTML;
    var mywindow = window.open('', 'PRINT', 'height=600,width=900');
    if (mywindow) {
      mywindow.document.write(`<html><head>
      <style>
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
      <title>File</title>`);
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
      //show upload signed copy input chooser

      /*  mywindow.document.close(); // necessary for IE >= 10
      mywindow.focus(); // necessary for IE >= 10
        mywindow.print();
        mywindow.close(); */
    }
  }
  printApplication() {
    if (this.application.ApplicationSource_Id == 5 || this.application.ApplicationSource_Id == 10) {
      this.printBarcode();
      return;
    }
    let html = document.getElementById('applicationPrint').innerHTML;
    var mywindow = window.open('', 'PRINT', 'height=600,width=900');
    if (mywindow) {
      mywindow.document.write(`<html><head>
            <style>
                body {
                  font-family: -apple-system, BlinkMacSystemFont, 
                'Segoe UI', 'Roboto' , 'Oxygen' , 'Ubuntu' , 'Cantarell' , 'Fira Sans' , 'Droid Sans' , 'Helvetica Neue' ,
                  sans-serif !important;
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
            .application-page {
        
              padding: 50px;
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
            .watermark-hisdu {
              text-align: center;
              position: absolute;
              left: 0;
              width: 100%;
              opacity: 0.25;
            }
    
            .watermark-hisdu img {
              display: inline-block;
            }
    
            table.header-pshealth,
            .applicant-information,
            .application-type-detail-preview,
            .attached-document,
            .remarks-preview,
            .info-application-preview,
            table.pshealth {
              border-color: transparent !important;
              width: 100%;
            }
    
            table.header-pshealth td {
              border-color: transparent !important;
            }
    
            table.header-pshealth td.gop-logo-a4-header {
              text-align: left;
            }
    
            table.header-pshealth td.gop-logo-a4-header img {
              display: inline-block;
              width: 134px;
            }
    
            table.header-pshealth td.pshealth-right-a4-td-header {
              text-align: right;
            }
    
            table.header-pshealth td.pshealth-right-a4-td-header .pshealth-right-a4-text-header {
              display: inline-block;
              text-align: center;
            }
    
            table.header-pshealth td.app-type-preview {
              text-align: left;
              width: 100%;
            }
    
            /* Applicant Information */
    
            table.applicant-information {
              border-color: transparent !important;
              width: 100%;
            }
    
            table.applicant-information td.applicant-info-heading,
            table.application-type-detail-preview td.application-type-detail-preview-heading,
            table.remarks-preview td.remarks-heading,
            table.info-application-preview td.info-application-preview-heading,
            table.attached-document td.attached-document-heading {
              text-align: center;
              border: 1px solid black;
            }
    
            table.applicant-information td.applicant-info-detail-1 {
              width: 20% !important;
            }
    
            table.applicant-information td.applicant-info-detail-2 {
              width: 40% !important;
            }
    
            table.applicant-information td.applicant-info-detail-3 {
              width: 10% !important;
            }
    
            table.applicant-information td.applicant-info-detail-4 {
              width: 30% !important;
            }
            table.applicant-information td.applicant-info-detail-5 {
              width: 15% !important;
            }
            
            table.applicant-information td.applicant-info-detail-6 {
              width: 30% !important;
            }
            
            table.applicant-information td.applicant-info-detail-7 {
              width: 20% !important;
            }
            
            table.applicant-information td.applicant-info-detail-8 {
              width: 35% !important;
            }
            table.info-application-preview td.info-application-preview-left {
              border-left: 1px solid black;
            }
    
            table.info-application-preview td.info-application-preview-right {
              text-align: center;
              margin: 5px 5px !important;
              border-right: 1px solid black;
              border-left: 1px solid black;
            }
    
            table.application-route-detail {
              border-color: transparent !important;
              width: 100% !important;
              text-align: center;
            }
    
            table.application-route-detail td.application-route-detail-header {
              width: 50% !important;
              border: 1px solid black;
            }
            @page 
            {
                size:  auto;   
                margin: 0mm;
            }
            .w-20 { width: 20% !important; }
            .w-30 { width: 30% !important; }
            .w-50 { width: 50% !important; }
            .w-70 { width: 70% !important; }
            .w-80 { width: 80% !important; }
    
            .mt-10 { margin-top: 10px !important; }
            .mt-30 { margin-top: 30px  !important; }
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
      //show upload signed copy input chooser

      /*     mywindow.document.close(); // necessary for IE >= 10
          mywindow.focus(); // necessary for IE >= 10
          mywindow.print();
          mywindow.close(); */
    }
  }

}