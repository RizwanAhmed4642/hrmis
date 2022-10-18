import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NotificationService } from '../../../_services/notification.service';
import { CookieService } from '../../../_services/cookie.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { OnlinePostApplyService } from '../online-post-apply.service';
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
  public checking: boolean = false;
  public portalOpen: boolean = true;
  public sigingin: boolean = false;
  public profileExist: boolean = null;
  public CnicMask: string = "00000-0000000-0";
  public mobileMask: string = "0000-0000000";
  public generatingPassword: boolean = false;
  public passwordGenerated: boolean = false;
  constructor(private router: Router, private auth: AuthenticationService, private _rootService: RootService
    , private _onlinePostApplyService: OnlinePostApplyService, private _cookieService: CookieService, private _notificationService: NotificationService) { }

  ngOnInit() {
    this._cookieService.deleteCookie('ussr');
    this._cookieService.deleteCookie('ussrpublic');
    this._cookieService.deleteCookie('cnicussrpublic');
    //this.checkPortalActive(36);
  }

  public checkPortalActive(id) {
    this.checking = true;
    this._onlinePostApplyService.getMeritActiveDesignation(id).subscribe((res) => {
      if (res) {
        this.portalOpen = true;
      } else {
        this.portalOpen = false;
      }
      this.checking = false;
    }, err => {
      console.log(err);
    });
  }

  public getPassword() {
    this.generatingPassword = true;
    this.errors = [];
    this.user.roles = [];
    this.user.roles.push('Employee Applicant');
    this.user.isUpdated = true;
    this.user.UserName = this.user.Cnic;
    this.user.UserDetail = 'Administrative post applications Portal User';

    this._onlinePostApplyService.getPassword(this.user).subscribe((response: any) => {
      this.generatingPassword = false;
      if (response.Errors && response.Errors.length > 0) {
        this.errors = response.Errors;
        this._notificationService.notify('danger', 'User Registration Failed.');
      } else {
        this._notificationService.notify('success', response);
        this.passwordGenerated = true;
      }
    }, err => {
      console.log(err);
      this.generatingPassword = false;
    });
  }
  public fetchMeritInfo() {
    this.generatingPassword = true;
    this._rootService.getProfileByCNIC(this.user.Cnic).subscribe((res: any) => {
      if (res) {
        let profile = res;
        this.profileExist = res.Id > 0 ? true : false;
        this.user.hfmiscode = profile.HfmisCode;
        if (this.profileExist) {
          this.getPassword();
        } else {
          this.generatingPassword = false;
        }
      }
      else {
        this.generatingPassword = false;
      }
    }, err => {
      console.log(err);
    });
  }
  public login() {
    this.profileExist = null;
    this.sigingin = true;
    this._rootService.getProfileByCNIC(this.user.Cnic).subscribe((res: any) => {
      if (res) {
        let profile = res;
        this._onlinePostApplyService.login({ username: this.user.Cnic, password: this.user.password }).subscribe((res2) => {
          if (res2) {
            if (this.setUser(res2) && this.auth.setUser(res2)) {
              let navigateTo = profile && profile.Id > 0 ? '/dashboard' : '/smo/profile';
              this.router.navigate([navigateTo]);
              this.sigingin = false;
            }
          }
        }, err => {
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
