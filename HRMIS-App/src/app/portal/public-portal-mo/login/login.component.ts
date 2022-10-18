import { Component, OnInit } from '@angular/core';
import { PublicPortalMOService } from '../public-portal-mo.service';
import { Router } from '@angular/router';
import { NotificationService } from '../../../_services/notification.service';
import { CookieService } from '../../../_services/cookie.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styles: []
})
export class LoginComponent implements OnInit {
  public errors: any[] = [];
  public user: any = {};
  public profile: any;
  public notEligible: boolean = false;
  public sigingin: boolean = false;
  public CnicMask: string = "00000-0000000-0";
  public mobileMask: string = "0000-0000000";
  public generatingPassword: boolean = false;
  public passwordGenerated: boolean = false;
  constructor(private router: Router, private _publicPortalService: PublicPortalMOService, private _cookieService: CookieService, private _notificationService: NotificationService) { }

  ngOnInit() {
    this._cookieService.deleteCookie('ussrpublic');
  }
  public getProfileAndVerify() {
    this._publicPortalService.getPromotedCandidate(this.user.Cnic).subscribe((res: any) => {
      if (res) {
        /*if (res.Designation_Id && res.EmpMode_Id && res.PresentPostingDate) {
             if (res.Designation_Id != 802) {
              this.notEligible = true;
            }
            if (res.EmpMode_Id != 13) {
              this.notEligible = true;
            }
            let serviceDate: Date = res.PresentPostingDate as Date;
            if (this.datediff(serviceDate, new Date()) >= 1095) {
              this.notEligible = true;
            } 
            if (!this.notEligible) {
              this.getPassword();
              return;
            }
        }*/
        console.log(res);
        this.getPassword();
        return;
        //this.notEligible = true;
      }
      //this.notEligible = true;
    }, err => {
      console.log(err);
    });
  }
  public datediff(first, second) {
    // Take the difference between the dates and divide by milliseconds per day.
    // Round to nearest whole number to deal with DST.
    return Math.round((second - first) / (1000 * 60 * 60 * 24));
  }
  public getPassword() {
    /*   
    this.generatingPassword = true;
      this._publicPortalService.getPassword({ Cnic: this.user.Cnic, MobileNo: this.user.MobileNo }).subscribe((res: any) => {
        if (res) {
          this.passwordGenerated = true;
          this.generatingPassword = false;
        }
      }, err => {
        console.log(err);
        this.generatingPassword = false;
      }); */

    this.generatingPassword = true;

    this.errors = [];
    this.user.roles = [];
    this.user.roles.push('PublicUser');
    this.user.isUpdated = true;
    this.user.UserName = this.user.Cnic;
    this.user.UserDetail = 'Medical Officer - Employee.';

    this._publicPortalService.getPassword(this.user).subscribe((response: any) => {
      this.generatingPassword = false;
      if (response.Errors && response.Errors.length > 0) {
        this.errors = response.Errors;
        this._notificationService.notify('danger', 'User Registration Failed.');
      } else {
        this._notificationService.notify('success', response);
        this.passwordGenerated = true;
        //this.router.navigate(['/user']);
      }
    }, err => {
      console.log(err);
      this.generatingPassword = false;
    });
  }

  public getProfileAndVerifyAndLogin() {
    this._publicPortalService.getProfile(this.user.Cnic).subscribe((res: any) => {
      if (res) {
        if (res.Designation_Id && res.EmpMode_Id && res.PresentPostingDate) {
          if (res.Designation_Id != 802) {
            this.notEligible = true;
          }
          if (res.EmpMode_Id != 2) {
            this.notEligible = true;
          }
          let serviceDate: Date = res.PresentPostingDate as Date;
          if (this.datediff(serviceDate, new Date()) >= 1095) {
            this.notEligible = true;
          }
          if (!this.notEligible) {
            this.login();
            return;
          }
        }
        this.notEligible = true;
      }
      this.notEligible = true;
    }, err => {
      console.log(err);
    });
  }
  public login() {
    this.sigingin = true;
    this._publicPortalService.login({ username: this.user.Cnic, password: this.user.password }).subscribe((res) => {
      if (res) {
        if (this.setUser(res)) {
          this.sigingin = false;
          this.router.navigate(['/preference/apply/' + this.user.Cnic]);
        }
      }
    }, err => {
      console.log(err);
    });
  }
  public setUser(_: any): boolean {
    let token = _.access_token;
    if (token !== '') {
      this._cookieService.deleteAndSetCookie('ussrpublic', token);
      return true;
    }
    else {
      return false;
    }
  }
}
