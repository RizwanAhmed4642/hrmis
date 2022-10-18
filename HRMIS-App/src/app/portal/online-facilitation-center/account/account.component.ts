import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthenticationService } from '../../../_services/authentication.service';
import { CookieService } from '../../../_services/cookie.service';
import { NotificationService } from '../../../_services/notification.service';
import { RootService } from '../../../_services/root.service';
import { OnlineFacilitationCenterService } from '../online-facilitation-center.service';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
})
export class AccountComponent implements OnInit {

  public forgotPassEmail;
  public errors: any[] = [];
  public user: any = {};
  public profile: any;
  public step: number = 2;
  public checking: boolean = false;
  public portalOpen: boolean = true;
  public sigingin: boolean = false;
  public profileExist: boolean = null;
  public generatingPassword: boolean = false;
  public passwordGenerated: boolean = false;
  public isError = false;
  public loginError: string = '';

  constructor(
    private router: Router, 
    private auth: AuthenticationService, 
    private _rootService: RootService, 
    private _onlineFacilitationService: OnlineFacilitationCenterService, 
    private _cookieService: CookieService, 
    private _notificationService: NotificationService) { }

  ngOnInit() {
    this._cookieService.deleteCookie('ussr');
    this._cookieService.deleteCookie('ussrpublic');
    this._cookieService.deleteCookie('cnicussrpublic');
  }

  public getPassword() {
   
    this.generatingPassword = true;
    this.errors = [];
    this.user.roles = [];
    this.user.roles.push('PublicUser');
    this.user.isUpdated = true;
    this.user.UserName = this.user.Cnic;
    this.user.UserDetail = 'Retirement Applicant.';

    this._onlineFacilitationService.getPassword(this.user).subscribe((response: any) => {
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

    this._onlineFacilitationService.getProfileExist(this.user.ProfileId, this.user.Cnic, this.user.PhoneNumber, this.user.Email).subscribe((res: any) => {
      console.log('In fetchProfileInfo..');
      if (res) {
        console.log('Res_Pro: ', res);
        this.profileExist = true;
        this.getPassword();
      } else {
        console.log('pro not found');
        this.profileExist = false;
      }
    }, err => {
      console.log(err);
    });
  }

  public login() {
    this.profileExist = null;
    this.sigingin = true;
    this._onlineFacilitationService.getProfile(this.user.Cnic).subscribe((res: any) => {
      console.log(res);
      
      if (res) {
        this._onlineFacilitationService.login({ username: this.user.Cnic, password: this.user.password }).subscribe((res2) => {
          console.log(res2);
          
          if (res2) {
            if (this.setUser(res2)) {
              this.sigingin = false;
              this.router.navigate(['/online-facilitation-center/profile']);
            }
            // else{
            //   this.isError = true;
            //   this.loginError = 'The user name of password is incorrect';
            // }
          }
        }, err => {
              this.sigingin = false;
              this.isError = true;
              console.log('login error: ', err);
        });
      } else {
        this.profileExist = false;
        this.sigingin = false;
      }
    }, err => {
      console.log('login error: ', err.Errors);
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
