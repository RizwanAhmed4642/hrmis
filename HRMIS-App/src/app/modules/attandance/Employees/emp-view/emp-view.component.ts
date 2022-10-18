import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthenticationService } from '../../../../_services/authentication.service';
import { AttandanceService } from "../../attandance.service";
import { RootService } from '../../../../_services/root.service';
import { Subscription } from 'rxjs/Subscription';
import { ActivatedRoute } from '@angular/router';
import { Profile } from '../profile.class';
import { DialogService } from '../../../../_services/dialog.service';

@Component({
  selector: 'app-emp-view',
  templateUrl: './emp-view.component.html',
  styles: []
})
export class EmpViewComponent implements OnInit, OnDestroy {
  private subscription: Subscription;
  public cnic: string = '0';
  public profile: Profile;
  public amountSalary: number = 40000;
  public radnom: number = Math.random();
  
  public currentUser: any;
  constructor(private _rootService: RootService,  private _attandanceService: AttandanceService,
    private _dialogService: DialogService,
    private _authenticationService: AuthenticationService,
    private route: ActivatedRoute) { }

    ngOnInit() {
      this.currentUser = this._authenticationService.getUser();
      this.fetchParams();
    }
    private fetchParams() {
      this.subscription = this.route.params.subscribe(
        (params: any) => {
          if (params.hasOwnProperty('cnic')) {
            this.cnic = params['cnic'];
            this.fetchData(this.cnic);
          }
        }
      );
    }
    private fetchData(cnic) {
      this.profile = null;
      this._attandanceService.getProfile(cnic).subscribe(
        res => {
          this.profile = res as Profile;
         
        }
      );
    }
    public openOrderWindow() {
      this._dialogService.openDialog({ type: 'order', cnic: this.profile.CNIC });
    }
    
    public dashifyCNIC(cnic: string) {
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
    printProfile() {
      let html = document.getElementById('profileInfo').innerHTML;
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
              <title>Profile</title>`);
        mywindow.document.write('</head><body >');
        //mywindow.document.write('<button onclick="printFunc()" class="print">Print</button>');
        mywindow.document.write(html);
        mywindow.document.write(`
                <script>
          function printFunc() {
            window.print();
          }
          </script>
      `);
        mywindow.document.write('</body></html>');
  
        mywindow.document.close(); // necessary for IE >= 10
        mywindow.focus(); // necessary for IE >= 10*/
        mywindow.print();
        mywindow.close();
      }
    }
    ngOnDestroy() {
      this.subscription.unsubscribe();
    }
}
