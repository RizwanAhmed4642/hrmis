import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import { Notification } from '../_models/notification.class';
import { NotificationService } from '../_services/notification.service';
import { RootService } from '../_services/root.service';
import { Subject } from 'rxjs/Subject';
import { DropDownsHR } from '../_helpers/dropdowns.class';
import { AuthenticationService } from '../_services/authentication.service';
import { DialogService } from '../_services/dialog.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-print-it',
  templateUrl: './print-it.component.html',
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
   @page {size: landscape}
    button.print, hr {
      display: none;
    }
    img {
      opacity: 0;
    }
   
  }
  
  `]
})
export class PrintItComponent implements OnInit, OnDestroy {
  public currentUser: any;
  public cheque: any;
  constructor(private router: Router, private _rootService: RootService, private _dialogService: DialogService, private _authenticationService: AuthenticationService) { }
  ngOnInit() {
    this.currentUser = this._authenticationService.getUser();
    this.cheque = this._rootService.getCheque();
    if (!this.cheque) this.router.navigate(['/dashboard']);
  }

  public th = ['', 'Thousand', 'Million', 'Billion', 'Trillion'];
  public dg = ['Zero', 'One', 'Two', 'Three', 'Four', 'Five', 'Six', 'Seven', 'Eight', 'Nine'];
  public tn = ['Ten', 'Eleven', 'Twelve', 'Thirteen', 'Fourteen', 'Fifteen', 'Sixteen', 'Seventeen', 'Eighteen', 'Nineteen'];
  public tw = ['Twenty', 'Thirty', 'Forty', 'Fifty', 'Sixty', 'Seventy', 'Eighty', 'Ninety'];


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
  public printFunc() {
    window.print();
  }
  ngOnDestroy() {
  }

}
