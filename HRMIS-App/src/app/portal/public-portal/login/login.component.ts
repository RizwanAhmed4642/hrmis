import { Component, OnInit } from '@angular/core';
import { PublicPortalComponent } from '../public-portal.component';
import { PublicPortalService } from '../public-portal.service';
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
  public eligible: boolean = null;
  public sigingin: boolean = false;
  public CnicMask: string = "00000-0000000-0";
  public notEligibleMessage: string = "";
  public mobileMask: string = "0000-0000000";
  public generatingPassword: boolean = false;
  public passwordGenerated: boolean = false;
  constructor(private router: Router, private _publicPortalService: PublicPortalService, private _cookieService: CookieService, private _notificationService: NotificationService) { }

  ngOnInit() {
    this._cookieService.deleteCookie('ussrpublic');
  }
  public getProfileAndVerify() {
    this.generatingPassword = true;
    this._publicPortalService.getProfile(this.user.Cnic).subscribe((res: any) => {
      if (res) {
        console.log(res);
        console.log(res.PresentPostingDate);
        if (res.Designation_Id && res.Status_Id && res.EmpMode_Id) {
          this.notEligibleMessage = "";
          debugger;
          let desigIds: number[] = [362, 365, 368, 369, 373, 374, 375, 381, 382, 383, 384, 385, 387, 390, 1594, 1598, 2136, 2313];
          /* let desigIds: number[] = [502, 903]; */
          let desig = desigIds.find(x => x == res.Designation_Id);
          if (desig) {
            if (res.Status_Id == 2) {
              if (res.EmpMode_Id == 13) {
                this.eligible = true;
                let serviceDate: Date = new Date(res.LastPromotionDate);
                if (this.datediff(serviceDate, new Date()) >= 1797) {
                  this.eligible = true;
                } else {
                  this.eligible = false;
                  this.notEligibleMessage += ' Only for Regular Consultants (BS-18) with 5 years service minimum';
                }
              } else {
                this.eligible = false;
                this.notEligibleMessage += ' Only for Regular Consultants (BS-18)';
              }
            } else {
              this.eligible = false;
              this.notEligibleMessage += ' Only for Active Status';
            }
          } else {
            this.eligible = false;
            this.notEligibleMessage += ' Only for Consultants (BS-18)';
          }
          if (this.eligible) {
            this.getPassword();
            return;
          }

          /*   if ([362, 365, 368, 369, 373, 374, 375, 381, 382, 383, 384, 385, 387, 390, 1594, 1598, 2136, 2313]
              .find(x => x == res.Designation_Id)
              && [2, 3, 9, 15, 17, 25, 30, 31, 34, 38]
                .find(x => x == res.Status_Id)
              && res.EmpMode_Id == 2
              && this.datediff(res.PresentPostingDate as Date, new Date()) >= 730) {
              this.eligible = true;
              this.getPassword();
              return;
            }
            else {
              this.eligible = false;
            } */
        } else {
          this.eligible = false;
        }
      } else {
        this.eligible = null;
        this.notEligibleMessage = "";
      }
      this.generatingPassword = false;
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
    this.user.UserDetail = 'Consultant - Employee.';

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
        if (res.Designation_Id && res.Status_Id && res.EmpMode_Id && res.PresentPostingDate) {
          this.notEligibleMessage = "";
          let desigIds: number[] = [362, 365, 368, 369, 373, 374, 375, 381, 382, 383, 384, 385, 387, 390, 1594, 1598, 2136, 2313];
          let desig = desigIds.find(x => x == res.Designation_Id);
          if (desig) {
            if (res.Status_Id == 2) {
              if (res.EmpMode_Id == 13) {
                let serviceDate: Date = new Date(res.PresentPostingDate);
                if (this.datediff(serviceDate, new Date()) >= 730) {
                  this.eligible = true;
                } else {
                  this.eligible = false;
                  this.notEligibleMessage += ' Only for Regular Consultants';
                }
              } else {
                this.eligible = false;
                this.notEligibleMessage += ' Only for Regular Consultants';
              }
            } else {
              this.eligible = false;
              this.notEligibleMessage += ' Only for Active Status';
            }
          } else {
            this.eligible = false;
            this.notEligibleMessage += ' Only for Consultants (BS-18)';
          }
          if (this.eligible) {
            this.login();
            return;
          }
        }
        this.eligible = false;
      }
      this.eligible = null;
      this.notEligibleMessage = "";
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
          this.router.navigate(['/main/consultant/' + this.user.Cnic]);
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
