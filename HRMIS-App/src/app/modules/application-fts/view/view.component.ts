import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { ApplicationFtsService } from '../application-fts.service';
import { RootService } from '../../../_services/root.service';
import { ApplicationView, ApplicationLog, ApplicationAttachment } from '../application-fts';
import { DomSanitizer } from '@angular/platform-browser';
import { AuthenticationService } from '../../../_services/authentication.service';
import { User } from '../../../_models/user.class';

@Component({
  selector: 'app-view',
  templateUrl: './view.component.html',
  styles: []
})
export class ViewComponent implements OnInit, OnDestroy {
  public loading: boolean = true;
  public requestingFile: boolean = false;
  public imageWindowOpened: boolean = false;
  public imagePath: string = '';
  private subscription: Subscription;
  public dropDowns: DropDownsHR = new DropDownsHR();
  public application: ApplicationView;
  public applicationLog: ApplicationLog = new ApplicationLog();
  public applicationLogs: ApplicationLog[] = [];
  public applicationAttachments: ApplicationAttachment[] = [];
  public barcodeImgSrc: string = '';
  public updatingRemarks: boolean = false;
  public forwardingApplication: boolean = false;
  public removingApplication: boolean = false;
  public successDialogOpened: boolean = false;
  public uploadingSignedCopy: boolean = false;
  public trackingDialogOpened: boolean = false;
  public activeIndex: number = 0;
  public sectionOfficers: any[] = [];
  public user: User;
  public signedApplication: ApplicationAttachment = new ApplicationAttachment();
  public personAppeared: any;

  constructor(private sanitizer: DomSanitizer, private _applicationFtsService: ApplicationFtsService,
    private route: ActivatedRoute, private _authenticationService: AuthenticationService,
    private router: Router, private _rootService: RootService) { }

  ngOnInit() {
    this.user = this._authenticationService.getUser();
    this.fetchParams();
  }
  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('id') && +params['id'] && params.hasOwnProperty('tracking') && +params['tracking']) {
          let id = +params['id'];
          let tracking = +params['tracking'];
          this.getApplication(id, tracking);
          this.getPandSOfficers('section');
        } else {
        }
      }
    );
  }
  public removeApplication() {
    if (confirm('Remove Tracking # ' + this.application.TrackingNumber + '?')) {
      this.removingApplication = true;
      this._applicationFtsService.removeApplication(this.application.Id, this.application.TrackingNumber).subscribe((res: any) => {
        if (res) {
          this.removingApplication = false;
          this.router.navigate(['/application']);
        }
      }, err => {
        this.handleError(err);
      });
    }
  }
  private getPandSOfficers = (type: string) => {
    this.sectionOfficers = [];
    this._rootService.getPandSOfficers(type).subscribe((res: any) => {
      this.sectionOfficers = res;
    },
      err => { this.handleError(err); }
    );
  }
  private getApplication(id, tracking) {
    this._applicationFtsService.getApplication(+id, +tracking).subscribe((data: any) => {
      if (data) {
        this.application = data.application as ApplicationView;
        this._rootService.generateBars(this.application.TrackingNumber).subscribe((res: any) => {
          this.barcodeImgSrc = res.barCode;
        }, err => {
          this.handleError(err);
        });
        this.applicationAttachments = data.applicationAttachments as ApplicationAttachment[];
        this.applicationLog.Application_Id = this.application.Id;
        this.loading = false;
        this.getApplicationData("logs");
        this.getApplicationData("parliamentarian");
      }
    }, err => {
      this.handleError(err);
    });
  }
  private getApplicationData(type) {
    this._applicationFtsService.getApplicationData(this.application.Id, type).subscribe((data: any) => {
      if (data) {
        if (type == "logs") {
          this.applicationLogs = data.applicationLogs as ApplicationLog[];
        } else if (type == "parliamentarian") {
          this.personAppeared = data.applicationPersonAppeared;
        }
        this.loading = false;
      }
    }, err => { this.handleError(err); }
    );
  }
  public dropdownValueChanged = (value, filter) => {

    if (filter == 'officer') {
      this.applicationLog.ToOfficer_Id = value.Id;
      this.applicationLog.ToOfficerName = value.DesignationName;
    }
  }
  updateLog() {
    this.forwardingApplication = true;
    this.applicationLog.ToOfficer_Id = this.application.ForwardingOfficer_Id;
    this.applicationLog.ToOfficerName = this.application.ForwardingOfficerName;
    this.applicationLog.ToStatus_Id = 11;
    this.applicationLog.ToStatus = 'Marked';
    this._applicationFtsService.createApplicationLog(this.applicationLog).subscribe((response: any) => {
      if (response.Id) {
        this.forwardingApplication = false;
        this.openSuccessDialog();
      }
    },
      err => { this.handleError(err); }
    );
  }
  updateRemarks() {
    this.updatingRemarks = true;
    this._applicationFtsService.createApplicationLog(this.applicationLog).subscribe((response: any) => {
      if (response.Id) {
        this.updatingRemarks = false;
        this.openSuccessDialog();
      }
    },
      err => { this.handleError(err); }
    );
  }
  uploadSignedCopy(event) {
    let inputValue = event.target;
    this.signedApplication.files = inputValue.files;
    this.signedApplication.Document_Id = 1;
    this.signedApplication.attached = true;
    //show send button
  }
  uploadSignedApplication() {
    // upload signed copy
    this.uploadingSignedCopy = true;
    this._applicationFtsService.uploadSignedApplication(this.signedApplication, this.application.Id).subscribe((response) => {
      if (response) {
        this.uploadingSignedCopy = false;
        this.openSuccessDialog();
      }
    }, err => {
      this.handleError(err);
    });
  }
  
  updateMarkLog() {
    this.forwardingApplication = true;
    this.applicationLog.ToOfficer_Id = this.application.ForwardingOfficer_Id;
    this.applicationLog.ToOfficerName = this.application.ForwardingOfficerName;
    this.applicationLog.ToStatus_Id = 11;
    this.applicationLog.ToStatus = 'Marked';
    this._applicationFtsService.createApplicationLog(this.applicationLog).subscribe((response: any) => {
      if (response.Id) {
        this.forwardingApplication = false;
        this.openSuccessDialog();
      }
    },
      err => { this.handleError(err); }
    );
  }
  public openTrackingDialog() {
    this.trackingDialogOpened = true;
  }
  public closeTrackingDialog() {
    this.trackingDialogOpened = false;
  }
  public openWindow(imagePath: string, index: number) {
    this.activeIndex = index;
    this.imagePath = imagePath;
    this.imageWindowOpened = true;
  }
  public closeWindow() {
    this.imageWindowOpened = false;
  }
  public openSuccessDialog() {
    this.successDialogOpened = true;
  }
  public closeSuccessDialog() {
    this.successDialogOpened = false;
    this.router.navigate(['/application']);
  }
  public capitalize(val: string) {
    if (!val) return '';
    return val.toUpperCase();
  }
  public barcodeSrc() {
    return this.sanitizer.bypassSecurityTrustUrl(this.barcodeImgSrc);
  }
  private handleError(err: any) {
    this.loading = false;
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
    this.subscription.unsubscribe();
  }
  printApplication() {
    let html = document.getElementById('applicationPrint').innerHTML;
    var mywindow = window.open('', 'PRINT', 'height=600,width=900');
    if (mywindow) {
      mywindow.document.write(`<html><head>
        <style>
            body {
              font-family: -apple-system, system-ui, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue' , Arial, sans-serif !important;
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

        table.info-application-preview td.info-application-preview-left {
          border-left: 1px solid black;
        }

        table.info-application-preview td.info-application-preview-right {
          text-align: center;
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
