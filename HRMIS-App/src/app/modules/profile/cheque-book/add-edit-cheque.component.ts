import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { AuthenticationService } from '../../../_services/authentication.service';
import { RootService } from '../../../_services/root.service';
import { Subscription } from 'rxjs/Subscription';
import { ActivatedRoute, Router } from '@angular/router';
import { ProfileService } from '../profile.service';
import { Profile } from '../profile.class';
import { Subject } from 'rxjs/Subject';
import { debounceTime } from 'rxjs/operators';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { NotificationService } from '../../../_services/notification.service';
import { LocalService } from '../../../_services/local.service';

@Component({
  selector: 'app-add-edit-cheque',
  templateUrl: './add-edit-cheque.component.html',
  styles: [`
  
  .cheque {
    position: absolute;
    display: inline-block;
    color: black;
  }
  .amount {
    margin: 281px -511px;
  }
  .amount-words {
    margin: 200px -481px;
  }
  .pay-to {
    margin: 180px -481px;
  }
  .account-title {
    margin: 223px -481px 0px -316px;
  }
  .govtpnj {
    margin: 113px -555px;
  }
  .pd-hisdu {
    margin: 69px -563px;
  }
  .hisdu {
    margin: 89px -539px;
  }
  .dated-cheque {
    margin: 121px -117px;
  }
  .project-code {
    margin: 35px -107px;
    font-size: 9px;
  }
  .ddofund {
    margin: 47px -94px;
    font-size: 9px;
  }
  .costcenter {
    margin: 58px -111px;
    font-size: 9px;
  }
  .payeesac {
    border: 1px solid black;
    margin: 35px -515px;
    padding: 0px 10px;
    font-size: 12px;
  }

  `]
})
export class AddEditChequeComponent implements OnInit, OnDestroy {
  public cheque = {
    designation: { name: 'Project Director', x: 69, y: -563, fontSize: 12 },
    program: { name: 'HISDU', x: 89, y: -539, fontSize: 12 },
    government: { name: 'Government of the Punjab', x: 113, y: -555, fontSize: 12 },
    projectCode: { name: 'LZ 4349', x: 35, y: -107, fontSize: 9 },
    ddoCode: { code: '12345', x: 47, y: -94, fontSize: 9 },
    costcenter: { name: 'asd1234', x: 58, y: -111, fontSize: 9 },
    dated: { date: new Date(), x: 121, y: -117, fontSize: 12 },
    accountTitle: { title: 'ADP Scheme 2018-2019', x: 223, y: -313, fontSize: 12 },
    payTo: { to: 'Zubair mughal', x: 180, y: -481, fontSize: 12 },
    amount: { amount: 63568, x: 281, y: -511, fontSize: 12 },
    amountWords: { x: 200, y: -481, fontSize: 12 },
    payeesac: { show: true, x: 35, y: -515, px: 0, py: 10, fontSize: 12 }
  }
  public Ocheque = {
    designation: { name: 'Project Director', x: 69, y: -563, fontSize: 12 },
    program: { name: 'HISDU', x: 89, y: -539, fontSize: 12 },
    government: { name: 'Government of the Punjab', x: 113, y: -555, fontSize: 12 },
    projectCode: { name: 'LZ 4349', x: 35, y: -107, fontSize: 9 },
    ddoCode: { code: '12345', x: 47, y: -94, fontSize: 9 },
    costcenter: { name: 'asd1234', x: 58, y: -111, fontSize: 9 },
    dated: { date: new Date(), x: 121, y: -117, fontSize: 12 },
    accountTitle: { title: 'ADP Scheme 2018-2019', x: 223, y: -313, fontSize: 12 },
    payTo: { to: 'Zubair mughal', x: 180, y: -481, fontSize: 12 },
    amount: { amount: 63568, x: 281, y: -511, fontSize: 12 },
    amountWords: { x: 200, y: -481, fontSize: 12 },
    payeesac: { show: true, x: 35, y: -515, px: 0, py: 10, fontSize: 12 }
  }
  @ViewChild('photoRef', {static: false}) public photoRef: any;
  @ViewChild('cnicFrontRef', {static: false}) public cnicFrontRef: any;
  @ViewChild('cnicBackRef', {static: false}) public cnicBackRef: any;
  public loading = true;
  public userAccount = false;
  public userId = '';
  public oldPass = '';
  public newPass = '';
  public confirmPass = '';
  public savingProfile: boolean = false;
  public searchingHfs: boolean = false;
  public searchingHfsW: boolean = false;
  public loadingCNIC = true;
  public oldCNIC = '';
  public profile: Profile;
  public existingProfile: Profile;
  private subscription: Subscription;
  private cnicSubscription: Subscription;
  public searchEvent = new Subject<any>();
  public searchSubcription: Subscription = null;
  public birthDateMax: Date = new Date();
  //dropdowns variables
  public cnic: string = '0';
  public userHfmisCode: string = '000000000';
  public hfTypeCodes: any[] = [];
  public hfsList: any[] = [];
  public hfsWList: any[] = [];
  public editProfile: boolean = false;
  public changingPassword: boolean = false;
  public isUploading: boolean = false;
  public photoFile: any[] = [];
  public cnicFrontFile: any[] = [];
  public cnicBackFile: any[] = [];
  public genderItems: any[] = [
    { text: 'Select Gender', value: null },
    { text: 'Male', value: 'Male' },
    { text: 'Female', value: 'Female' },
  ]
  public photoSrc = '';
  public cnicFrontSrc = '';
  public cnicBackSrc = '';
  public dropDowns: DropDownsHR = new DropDownsHR();
  public inputChange: Subject<any>;
  public cnicMask: string = "00000-0000000-0";
  public mobileMask: string = "0000-0000000";
  public radnom: number = Math.random();


  constructor(private router: Router, private _rootService: RootService, private _profileService: ProfileService,
    private _authenticationService: AuthenticationService, public _notificationService: NotificationService,
    private route: ActivatedRoute, private _localService: LocalService) { }

  ngOnInit() {
    this.userHfmisCode = this._authenticationService.getUser().HfmisCode;
    let locCheque = this._localService.get('checkckck');
    if (locCheque) {
      locCheque.dated.date = new Date(locCheque.dated.date);
      this.cheque = locCheque;
    } else {
      this.cheque = this.Ocheque;
    }
  }

  public th = ['', 'Thousand', 'Million', 'Billion', 'Trillion'];
  public dg = ['Zero', 'One', 'Two', 'Three', 'Four', 'Five', 'Six', 'Seven', 'Eight', 'Nine'];
  public tn = ['Ten', 'Eleven', 'Twelve', 'Thirteen', 'Fourteen', 'Fifteen', 'Sixteen', 'Seventeen', 'Eighteen', 'Nineteen'];
  public tw = ['Twenty', 'Thirty', 'Forty', 'Fifty', 'Sixty', 'Seventy', 'Eighty', 'Ninety'];

  public onChange() {
    this._localService.set('checkckck', this.cheque);
  }
  public onReset() {
    this.cheque = this.Ocheque;
    this._localService.set('checkckck', this.cheque);
  }
  public toWords(s) {
    s = s.toString();
    s = s.replace(/[\, ]/g, '');
    if (s != parseFloat(s)) return 'not a number';
    var x = s.indexOf('.');
    if (x == -1) x = s.length;
    if (x > 15) return 'too big';
    var n = s.split('');
    var str = '';
    var sk = 0;
    for (var i = 0; i < x; i++) {
      if ((x - i) % 3 == 2) {
        if (n[i] == '1') {
          str += this.tn[Number(n[i + 1])] + ' ';
          i++;
          sk = 1;
        }
        else if (n[i] != 0) {
          str += this.tw[n[i] - 2] + ' ';
          sk = 1;
        }
      }
      else if (n[i] != 0) {
        str += this.dg[n[i]] + ' ';
        if ((x - i) % 3 == 0) str += 'Hundred ';
        sk = 1;
      }


      if ((x - i) % 3 == 1) {
        if (sk) str += this.th[(x - i - 1) / 3] + ' ';
        sk = 0;
      }
    }
    if (x != s.length) {
      var y = s.length;
      str += 'point ';
      for (let i = x + 1; i < y; i++) str += this.dg[n[i]] + ' ';
    }
    return str.replace(/\s+/g, ' ');
  }
  public saveAndPrint() {
    this._rootService.setCheque(this.cheque);
    this.router.navigate(['/print-it']);
  }
  private handleError(err: any) {
    this.loading = false;
    this.loadingCNIC = false;
    this.changingPassword = false;
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
            .cheque {
              position: absolute;
              display: inline-block;
              color: black;
            }
            .amount {
              margin: 281px -511px;
            }
            .amount-words {
              margin: 200px -481px;
            }
            .pay-to {
              margin: 180px -481px;
            }
            .account-title {
              margin: 223px -481px 0px -316px;
            }
            .govtpnj {
              margin: 113px -555px;
            }
            .pd-hisdu {
              margin: 69px -563px;
            }
            .hisdu {
              margin: 89px -539px;
            }
            .dated-cheque {
              margin: 121px -117px;
            }
            .project-code {
              margin: 35px -107px;
              font-size: 9px;
            }
            .ddofund {
              margin: 47px -94px;
              font-size: 9px;
            }
            .costcenter {
              margin: 58px -111px;
              font-size: 9px;
            }
            .payeesac {
              border: 1px solid black;
              margin: 35px -515px;
              padding: 0px 10px;
              font-size: 12px;
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
      mywindow.document.write(`
      
      `);
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

  ngOnDestroy() {
    /* this.subscription.unsubscribe(); */
    /* this.cnicSubscription.unsubscribe(); */
    /* this.searchSubcription.unsubscribe(); */
  }
}
