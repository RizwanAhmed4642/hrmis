import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NotificationService } from '../../../_services/notification.service';
import { CookieService } from '../../../_services/cookie.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { RetirementApplicationService } from '../retirement-application.service';
import { RootService } from '../../../_services/root.service';

@Component({
  selector: 'app-account-preferences-portal',
  templateUrl: './account.component.html',
  styles: []
})
export class AccountComponent implements OnInit {
  public errors: any[] = [];
  public user: any = {};
  public profile: any;
  public step: number = 2;
  public profileId: number = 0;
  public checking: boolean = false;
  public portalOpen: boolean = true;
  public sigingin: boolean = false;
  public profileExist: boolean = null;
  public CnicMask: string = "00000-0000000-0";
  public mobileMask: string = "0000-0000000";
  public generatingPassword: boolean = false;
  public passwordGenerated: boolean = false;
  constructor(private router: Router, private auth: AuthenticationService, private _rootService: RootService, private _retirementApplicationService: RetirementApplicationService, private _cookieService: CookieService, private _notificationService: NotificationService) { }

  ngOnInit() {
    this._cookieService.deleteCookie('ussr');
    this._cookieService.deleteCookie('ussrpublic');
    this._cookieService.deleteCookie('cnicussrpublic');
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

    this.errors = [];
    this.user.roles = [];
    this.user.roles.push('PublicUser');
    this.user.isUpdated = true;
    this.user.UserName = this.user.Cnic;
    this.user.UserDetail = 'Retirement Applicant.';

    this._retirementApplicationService.getPassword(this.user).subscribe((response: any) => {
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

  public fetchProfileInfo() {
    this.generatingPassword = true;
    this._retirementApplicationService.getProfileExist(this.user.ProfileId, this.user.Cnic, this.user.PhoneNumber, this.user.Email).subscribe((res: any) => {
      if (res) {
        this.profileExist = true;
        this.getPassword();
      } else {
        this.profileExist = false;
        this.generatingPassword = false;
      }
    }, err => {
      console.log(err);
    });
  }
  public login() {
    this.profileExist = null;
    this.sigingin = true;
    this._retirementApplicationService.getProfile(this.user.Cnic).subscribe((res: any) => {
      if (res) {
        this._retirementApplicationService.login({ username: this.user.Cnic, password: this.user.password }).subscribe((res2) => {
          if (res2) {
            if (this.setUser(res2)) {
              this.sigingin = false;
              this.router.navigate(['/retirement-application/profile']);
            }
          }
          this.sigingin = false;
        }, err => {
          this.sigingin = false;
          console.log(err);
        });
      } else {
        this.profileExist = false;
        this.sigingin = false;
      }
    }, err => {
      console.log(err);
    });
  }
  public setUser(_: any): boolean {
    let token = _.access_token;
    let userName = _.userName;
    if (token !== '') {
      this._cookieService.deleteAndSetCookie('cnicussrpublic', userName);
      this._cookieService.deleteAndSetCookie('ussrpublic', token);
      this._cookieService.deleteAndSetCookie('ussr', token);
      return true;
    }
    else {
      return false;
    }
  }
}
