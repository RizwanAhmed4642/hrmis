import { Component, OnInit, OnDestroy } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { FileTrackingSystemService } from '../file-tracking-system.service';
import { ActivatedRoute, Router } from '@angular/router';
import { NotificationService } from '../../_services/notification.service';
import { RootService } from '../../_services/root.service';
import { AuthenticationService } from '../../_services/authentication.service';
import { Subscription } from 'rxjs/Subscription';
import { ApplicationView, ApplicationLog, ApplicationAttachment } from '../../modules/application-fts/application-fts';

@Component({
  selector: 'app-show-cause-letter',
  templateUrl: './show-cause-letter.component.html',
  styleUrls: ['./show-cause-letter.component.scss']
})
export class ShowCauseLetterComponent implements OnInit, OnDestroy {
  public loading: boolean = false;
  public successDialogOpened: boolean = false;
  private subscription: Subscription;
  public sectionOfficers: any[] = [];
  public letterType: number = 0;
  public letterTypeName: string = 'Waiting Documents';
  public applicationId: number = 0;
  public applicationtracking: number = 0;
  public application: ApplicationView;
  public profile: any;
  public applicationLog: ApplicationLog = new ApplicationLog();
  public applicationLogs: ApplicationLog[] = [];
  public applicationOlds: any[] = [];
  public applicationAttachments: ApplicationAttachment[] = [];
  public barcodeImgSrc: string = '';
  public imageWindowOpened: boolean = false;
  public imagePath: string = '';
  public activeIndex: number = 0;
  public CC2 = '';
  public CC3 = '';
  public CC4 = '';
  public cnic: string = '';
  public signedBy: string = '';
  public dateNow: string = '';
  public Message: string = '';
  constructor(private sanitizer: DomSanitizer, private _fileTrackingSystemService: FileTrackingSystemService,
    private route: ActivatedRoute, public _notificationService: NotificationService,
    private router: Router, private _rootService: RootService, private _authenticationService: AuthenticationService) { }

  ngOnInit() {
    let today = new Date();
    let dd: any = today.getDate(), mm: any = this.makeOrderMonth(today.getMonth() + 1), yyyy = today.getFullYear();
    this.dateNow = this.makeOrderDate(dd) + ', ' + mm + ' ' + yyyy;
    this.fetchParams();
    this.loadDropdownValues();
  }
  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('id') && +params['id']) {
          this.cnic = params['id'];
          this.getProfile();
        } else {
        }
      }
    );
  }
  private loadDropdownValues = () => {
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
  private getProfile() {
    this._rootService.getProfileByCNIC(this.cnic).subscribe((data: any) => {
      if (data) {
        this.profile = data;
      }
    }, err => { this.handleError(err); }
    );
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
    this.router.navigate(['/fts/my-applications']);
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
  public capitalize(val: string) {
    if (!val) return '';
    return val.toUpperCase();
  }
  private handleError(err: any) {
    this.loading = false;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }
  ngOnDestroy() {
    this.subscription.unsubscribe();
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
  makeOrderDate(day: number) {
    return day == 1 || day == 21 || day == 31 ? day + '<sup>st</sup>'
      : day == 2 || day == 22 ? day + '<sup>nd</sup>'
        : day == 3 || day == 23 ? day + '<sup>rd</sup>'
          : (day => 4 && day <= 20) || (day => 24 && day <= 30) ? day + '<sup>th</sup>' : '';
  }
  printLetter() {
    let html = document.getElementById('applicationPrint').innerHTML;
    var mywindow = window.open('', 'PRINT', 'height=600,width=900');
    if (mywindow) {
      mywindow.document.write(`<html><head>
      <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.2.1/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-GJzZqFGwb1QTTN6wy59ffF1BuGJpLSa9DkKMp0DgiMDm4iYMj70gZWKYbI706tWS" crossorigin="anonymous">
            <style>
              body {
                font-family: -apple-system, system-ui, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue' , Arial, sans-serif !important;
                  }
                  p {
                    margin-top: 0;
                    margin-bottom: 1px !important;
                }.mt-2 {
                  margin-top: 0.5rem !important;
                }.mb-0 {
                  margin-bottom: 0 !important;
                }
                .ml-1 {
                  margin-left: 0.25rem !important;
                }
                .ml-2 {
                  margin-left: 0.5rem !important;
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
