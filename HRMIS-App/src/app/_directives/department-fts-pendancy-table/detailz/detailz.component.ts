import { Component, OnInit, OnDestroy, Input } from "@angular/core";
import { DomSanitizer } from "@angular/platform-browser";
import { ActivatedRoute, Router } from "@angular/router";
import { NotificationService } from "../../../_services/notification.service";
import { FirebaseHisduService } from "../../../_services/firebase-hisdu.service";
import { RootService } from "../../../_services/root.service";
import { AuthenticationService } from "../../../_services/authentication.service";
import { ApplicationView, ApplicationLogView, ApplicationAttachment } from "../../../modules/application-fts/application-fts";
import { Subscription } from "rxjs/Subscription";

@Component({
    selector: 'app-application-details-zoom-table',
    templateUrl: './detailz.component.html',
    styles: []
})
export class DetailzComponent implements OnInit, OnDestroy {
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
    ngOnInit() {
        //this.subscribeLiveApp();
        this.getApplication();
        //this.searchFtsApplicaiton();
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
                this.application = data.application as ApplicationView;
                this._rootService.generateBars(this.application.TrackingNumber).subscribe((res: any) => { this.barcodeImgSrc = res.barCode; }, err => { this.handleError(err); });
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
            console.log(data);
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


    printApplication() {
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