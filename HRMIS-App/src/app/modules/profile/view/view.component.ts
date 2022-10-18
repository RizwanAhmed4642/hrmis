import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { AuthenticationService } from '../../../_services/authentication.service';
import { ProfileService } from '../profile.service';
import { RootService } from '../../../_services/root.service';
import { Subscription } from 'rxjs/Subscription';
import { ActivatedRoute } from '@angular/router';
import { Profile } from '../profile.class';
import { DialogService } from '../../../_services/dialog.service';
import { EncryptionLocalService } from '../../../_services/encryption.service';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';

@Component({
  selector: 'app-view',
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss']
})
export class ViewComponent implements OnInit, OnDestroy {
  @ViewChild('photoRef', { static: false }) public photoRef: any;
  private subscription: Subscription;
  public currentUser: any;
  public userRight: any;
  public cnic: string = '0';
  public profile: Profile;
  public amountSalary: number = 40000;
  public authorized: boolean = true;
  public dropDowns: DropDownsHR = new DropDownsHR();
  public chequeBookWindow: boolean = false;
  public submitting: boolean = false;
  public loadingReview: boolean = false;
  public profileReviews: any[] = [];
  public duplicateprofile: any[] = [];
  public profileReview: any = {};
  public hrReview: any = {};
  public orderRequest: any = {};
  public vaccinationCertificateNumber: string = '';
  public dateNow: string = '';
  public photoSrc = '';
  public photoFile: any[] = [];
  public today: Date = new Date();
  public uploadingFile: boolean = false;
  public uploadingFileError: boolean = false;
  public saveDialogOpened: boolean = false;
  public vaccCertificate: any = {};
  public vaccCertificates: any[] = [];
  constructor(private _rootService: RootService, private _profileService: ProfileService,
    private _dialogService: DialogService,
    private _authenticationService: AuthenticationService,
    private _encryptionService: EncryptionLocalService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.currentUser = this._authenticationService.getUser();
    this.getUserRightById();
    this.profileReview = {};
    this.fetchParams();
    this.getProfileAttachmentTypes();
    this.today = new Date();
    let dd: any = this.today.getDate(), mm: any = this.makeOrderMonth(this.today.getMonth() + 1), yyyy = this.today.getFullYear();

    this.dateNow = this.makeOrderDate(dd) + ', ' + mm + ' ' + yyyy;
  }
  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('cnic')) {
          this.cnic = params['cnic'];
          this.fetchData(this.cnic);
          this.fetchDuplication(this.cnic);
          /* console.log(this.cnic);
          console.log(decodeURIComponent(this.cnic));
          this._encryptionService.decrypt(this.currentUser.Id, decodeURIComponent(this.cnic)).then((x2) => {
          console.log(x2);
        });
        this._encryptionService.demoEncrypt(this.currentUser.Id, this.cnic).then((x) => {
          console.log(x);
          console.log(encodeURIComponent(x));
          this._encryptionService.decrypt(this.currentUser.Id, x).then((x2) => {
            console.log(x2);
          });
        }); */
        }
      }
    );
  }
  private fetchData(cnic) {
    this.profile = null;
    this._profileService.getProfile(cnic).subscribe(
      (res: any) => {
        if (res) {
          if (this.currentUser.UserName == 'dse') {
            if (res.Designation_Id == 302 || res.Designation_Id == 1320 || (res.Designation_Id == 2404 && res.Gender == 'female')) {
              this.authorized = true;
            } else {
              this.authorized = false;
            }
          }
          if (this.currentUser.UserName == 'ds.general') {
            if (res.Designation_Id == 802 || res.Designation_Id == 903 || res.Designation_Id == 362 || res.Designation_Id == 365 || res.Designation_Id == 368 ||
              res.Designation_Id == 369 || res.Designation_Id == 373 || res.Designation_Id == 374 ||
              res.Designation_Id == 375 || res.Designation_Id == 381 || res.Designation_Id == 382 ||
              res.Designation_Id == 383 || res.Designation_Id == 384 || res.Designation_Id == 385 ||
              res.Designation_Id == 387 || res.Designation_Id == 390 || res.Designation_Id == 1594 ||
              res.Designation_Id == 1598 || res.Designation_Id == 2136 || res.Designation_Id == 2313 ||
              res.Designation_Id == 21 || res.Designation_Id == 22 || res.Designation_Id == 802 ||
              res.Designation_Id == 1085 || res.Designation_Id == 932 || res.Designation_Id == 936 ||
              res.Designation_Id == 1157 || res.Designation_Id == 1320 || res.Designation_Id == 2255 || res.Designation_Id == 2404 ||
              res.Designation_Id == 325 || res.Designation_Id == 446 || res.Designation_Id == 486 || res.Designation_Id == 488 ||
              res.Designation_Id == 489 || res.Designation_Id == 573 || res.Designation_Id == 632 || res.Designation_Id == 812 ||
              res.Designation_Id == 863 || res.Designation_Id == 987 || res.Designation_Id == 988 || res.Designation_Id == 1209 ||
              res.Designation_Id == 1222 || res.Designation_Id == 1637 || res.Designation_Id == 2157 || res.Designation_Id == 2194 ||
              res.Designation_Id == 2257 || res.Designation_Id == 2273 || res.Designation_Id == 2288 || res.Designation_Id == 2289 ||
              res.Designation_Id == 2290 || res.Designation_Id == 2291 || res.Designation_Id == 2292 ||
              res.Designation_Id == 2293 || res.Designation_Id == 2294 || res.Designation_Id == 2304 ||
              res.Designation_Id == 2323 || res.Designation_Id == 2325 || res.Designation_Id == 2326 ||
              res.Designation_Id == 2345 || res.Designation_Id == 2372 || res.Designation_Id == 2382 || res.Designation_Id == 2392 ||
              res.Designation_Id == 2393 || res.Designation_Id == 2409 || res.Designation_Id == 2492 || res.Designation_Id == 2508 ||
              res.Designation_Id == 2516 || res.Designation_Id == 2517
            ) {
              this.authorized = true;
            } else {
              this.authorized = false;
            }
          }
          if (this.currentUser.UserName == 'sodhas') {
            if (res.Designation_Id == 362 || res.Designation_Id == 365 || res.Designation_Id == 368 ||
              res.Designation_Id == 369 || res.Designation_Id == 373 || res.Designation_Id == 374 ||
              res.Designation_Id == 375 || res.Designation_Id == 381 || res.Designation_Id == 382 ||
              res.Designation_Id == 383 || res.Designation_Id == 384 || res.Designation_Id == 385 ||
              res.Designation_Id == 387 || res.Designation_Id == 390 || res.Designation_Id == 1594 ||
              res.Designation_Id == 1598 || res.Designation_Id == 2136 || res.Designation_Id == 2313 ||
              res.Designation_Id == 21 || res.Designation_Id == 22 || res.Designation_Id == 802 ||
              res.Designation_Id == 1085 || res.Designation_Id == 932 || res.Designation_Id == 936 ||
              res.Designation_Id == 1157 || res.Designation_Id == 1320 || res.Designation_Id == 2255 || res.Designation_Id == 2404 ||
              res.Designation_Id == 325 ||
              res.Designation_Id == 446 ||
              res.Designation_Id == 486 ||
              res.Designation_Id == 488 ||
              res.Designation_Id == 489 ||
              res.Designation_Id == 573 ||
              res.Designation_Id == 632 ||
              res.Designation_Id == 812 ||
              res.Designation_Id == 863 ||
              res.Designation_Id == 987 ||
              res.Designation_Id == 988 ||
              res.Designation_Id == 1209 ||
              res.Designation_Id == 1222 ||
              res.Designation_Id == 1637 ||
              res.Designation_Id == 2157 ||
              res.Designation_Id == 2194 ||
              res.Designation_Id == 2257 ||
              res.Designation_Id == 2273 ||
              res.Designation_Id == 2288 ||
              res.Designation_Id == 2289 ||
              res.Designation_Id == 2290 ||
              res.Designation_Id == 2291 ||
              res.Designation_Id == 2292 ||
              res.Designation_Id == 2293 ||
              res.Designation_Id == 2294 ||
              res.Designation_Id == 2304 ||
              res.Designation_Id == 2323 ||
              res.Designation_Id == 2325 ||
              res.Designation_Id == 2326 ||
              res.Designation_Id == 2345 ||
              res.Designation_Id == 2372 ||
              res.Designation_Id == 2382 ||
              res.Designation_Id == 2392 ||
              res.Designation_Id == 2393 ||
              res.Designation_Id == 2409 ||
              res.Designation_Id == 2492 ||
              res.Designation_Id == 2508 ||
              res.Designation_Id == 2516 ||
              res.Designation_Id == 2517 ||
              res.Designation_Id == 2214
            ) {
              this.authorized = true;
            } else {
              this.authorized = false;
            }
          }
        }
        this.profile = res as Profile;
        this.getVaccinationCertificate();
        this.checkOrderRequest(this.profile.Id);

        if (this.profile.Id) {
          this.saveEntityLog();
        }
        this.getProfileReview();
      }
    );
  }

  public uploadBtn() {
    this.photoRef.nativeElement.click();
  }
  public readUrl(event: any) {
    if (event.target.files && event.target.files[0]) {
      this.photoFile = [];
      let inputValue = event.target;
      this.photoFile = inputValue.files;
      /* var reader = new FileReader();
      reader.onload = ((event: any) => {
        this.photoSrc = event.target.result;
      }).bind(this);
      reader.readAsDataURL(event.target.files[0]); */
    }
  }
  public uploadFile() {
    if (!this.vaccCertificate.type) {
      return;
    }
    this.uploadingFile = true;
    this._profileService.uploadVaccinationCertificate(this.photoFile, this.profile.Id, this.vaccCertificate.Number, this.vaccCertificate.type.Id).subscribe((x: any) => {
      if (!x.result) {
        this.uploadingFileError = true;
      }
      this.photoFile = [];
      this.vaccCertificate = {};
      this.uploadingFile = false;
      this.getVaccinationCertificate();
      //this.router.navigate(['/order/phfmc-order-list']);
    }, err => {
      this.uploadingFileError = true;
      this.uploadingFile = false;
    });
  }
  public getProfileAttachmentTypes() {
    this._rootService.getProfileAttachmentTypes().subscribe((x: any) => {
      if (x) {
        this.dropDowns.profileAttachments = x;
      }
    }, err => {
      console.log(err);
    });
  }
  private saveEntityLog() {
    let entity_Log: any = {};
    entity_Log.Entity_Id = 9;
    entity_Log.FK_Id = this.profile.Id;
    entity_Log.CNIC = this.profile.CNIC;
    entity_Log.HFMISCode = this.profile.HfmisCode;
    entity_Log.ProfileStatusId = this.profile.Status_Id;
    entity_Log.ProfileEmpModeId = this.profile.EmpMode_Id;
    entity_Log.ProfileDesignationId = this.profile.Designation_Id;
    entity_Log.ProfileHFId = this.profile.HealthFacility_Id;
    entity_Log.ProfileBPS = this.profile.CurrentGradeBPS;

    this._rootService.saveEntityLog(entity_Log).subscribe((res: any) => {
      if (res) {
        console.log('doneL');
      }
    }, err => { console.log(err); })
  }
  private fetchDuplication(cnic) {
    this.duplicateprofile = [];
    this._profileService.getDuplication(cnic).subscribe((res: any) => {
      if (res) {
        this.duplicateprofile = res as any[];
      }
    }
    )
  }
  public getProfileReview() {
    let calBack = (res: any) => {
      if (res && res.Id) {
        this.profileReview = res;
        this.getReviews();
      }
    };
    let err = err => { console.log(err); };
    this._profileService.getProfileReview(this.profile.Id).subscribe(calBack, err);
  }
  public submitForReview() {
    this.profileReview.ProfileId = this.profile.Id;
    this.submitting = true;
    let calBack = (res: any) => {
      if (res && res.Id) {
        this.getProfileReview();
        this.submitting = false;
      }
    };
    let err = err => { console.log(err); };
    this._profileService.submitForReview(this.profileReview).subscribe(calBack, err);
  }
  public submitReview() {
    this.hrReview.ReviewSubmissionId = this.profileReview.Id;
    this.hrReview.StatusId = 2;
    this.submitting = true;
    let calBack = (res: any) => {
      if (res && res.Id) {
        this.getProfileReview();
        this.submitting = false;
      }
    };
    let err = err => { console.log(err); };
    this._profileService.submitReview(this.hrReview).subscribe(calBack, err);
  }
  private getUserRightById() {
    let err = err => { console.log(err); };
    this._rootService.getUserRightById({ User_Id: this.currentUser.Id }).subscribe(
      res => {
        if (res) {
          this.userRight = res;
        } else {
          this.userRight = null;
        }
      },
      err
    );
  }
  private getVaccinationCertificate() {
    this._profileService.getVaccinationCertificate(this.profile.Id).subscribe(
      (res: any) => {
        if (res) {
          this.vaccCertificates = res;
        } else {
          this.vaccCertificate = {};
          this.vaccCertificates = [];
        }
      }, err => {
        console.log(err);
      }
    );
  }
  public checkOrderRequest(profileId: number) {
    this.orderRequest = {};
    let err = err => { console.log(err); };
    this._profileService.checkOrderRequest(profileId).subscribe(
      res => {
        if (res) {
          this.orderRequest = res;
        } else {
          this.orderRequest = {};
        }
      },
      err
    );
  }



  public getReviews() {
    this.loadingReview = true;
    let calBack = (res: any) => {
      if (res) {
        this.profileReviews = res;
        this.loadingReview = false;
      }
    };
    let err = err => { console.log(err); };
    this._profileService.getHrReview(this.profileReview.Id).subscribe(calBack, err);
  }
  public generateOrderRequest() {
    if (confirm('Are you sure you want to generate order request?')) {
      let calBack = (res: any) => {
        if (res) {
          alert(res);
        }
      };
      let err = err => { console.log(err); };
      this._rootService.generateOrderRequest({ ProfileId: this.profile.Id }).subscribe(calBack, err);
    }
  }
  public openOrderWindow() {
    this._dialogService.openDialog({ type: 'order', cnic: this.profile.CNIC, reqeustId: this.orderRequest.Id });
  }
  public openChequeBookWindow() {
    this.chequeBookWindow = true;
  }
  public closeChequeBookWindow() {
    this.chequeBookWindow = false;
  }
  public closeWindow() {
    this.saveDialogOpened = false;
  }
  public openInNewTab(link: string) {
    /* window.open('http://localhost:4200/' + link, '_blank'); */
    window.open(link, '_blank');
  }
  public openWindow() {
    this.saveDialogOpened = true;
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
    let html = document.getElementById('icpchart').innerHTML;
    var mywindow = window.open('', 'PRINT', 'height=600,width=900');
    if (mywindow) {
      mywindow.document.write(`<html><head>
      <style>
        table{
          border-collapse: collapse;
        }  
        table {
          background: white;
        }
        table tr>td {
          border: 1px solid black;
        }
        
        table p {
          margin: 0px !important;
          padding: 4px 6px !important;
          font-family: calibri !important;
        }
        
        table strong, .calibri {
          font-family: calibri !important;
        }
        .pull-right{
          float: right !important;
        }
        .m-0{
          margin: 0px  !important;
        }
        .p-heading-icp,
        .p-heading-icp-main {
          font-family: calibri !important;
          font-size: 18px;
        }
        
        .p-heading-icp-main,
        .p-heading-icp-sub {
          background: white;
          color: black;
        }
        
        .p-heading-icp-main {
          text-align: center;
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
      table { page-break-inside:auto }
      tr    { page-break-inside:avoid; page-break-after:auto }
    
      @media print {
        button.print {
          display: none;
        }
       *{
        -webkit-print-color-adjust: exact;
       }
      }
            </style>
            <title>Profile</title>`);
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

      mywindow.document.close(); // necessary for IE >= 10
      mywindow.focus(); // necessary for IE >= 10*/
      mywindow.print();
      mywindow.close();
    }
  }
  ngOnDestroy() {
    this.subscription.unsubscribe();
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
}
