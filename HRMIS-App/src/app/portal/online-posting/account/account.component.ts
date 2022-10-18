import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NotificationService } from '../../../_services/notification.service';
import { CookieService } from '../../../_services/cookie.service';
import { OnlinePostingService } from '../online-porting.service';
import { AuthenticationService } from '../../../_services/authentication.service';

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
  public portalOpen: boolean = false;
  public sigingin: boolean = false;
  public meritExist: boolean = null;
  public routeTo: string = "";
  public CnicMask: string = "00000-0000000-0";
  public mobileMask: string = "0000-0000000";
  public generatingPassword: boolean = false;
  public passwordGenerated: boolean = false;
  constructor(private router: Router, private auth: AuthenticationService, private _onlinePostingService: OnlinePostingService, private _cookieService: CookieService, private _notificationService: NotificationService) { }

  ngOnInit() {
    this._cookieService.deleteCookie('ussr');
    this._cookieService.deleteCookie('ussrpublic');
    this._cookieService.deleteCookie('cnicussrpublic');
    // this.checkPortalActive(69);
    this.portalOpen = true;
  }
  public checkPortalActive(id) {
    this.checking = true;
    this._onlinePostingService.getMeritActiveDesignation(id).subscribe((res: any) => {
      if (res && res.IsActive && res.IsActive == 'Y') {
        this.portalOpen = true;
      } else {
        this.portalOpen = false;
      }
      if (res && res.Id >= 58) {
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
    this.user.UserDetail = 'Preferences Portal User.';

    this._onlinePostingService.getPassword(this.user).subscribe((response: any) => {
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
  public fetchMeritInfo() {
    this._onlinePostingService.getMeritProfile(this.user.Cnic).subscribe((res: any) => {
      if (res && res.Id) {
        this.meritExist = true;
        this.getPassword();
      } else {
        this.meritExist = false;
      }
    }, err => {
      console.log(err);
    });
  }
  public login() {
    this.meritExist = null;
    this.sigingin = true;
    this._onlinePostingService.getMeritProfile(this.user.Cnic).subscribe((res: any) => {
      if (res && res.Id) {
        this._onlinePostingService.getMeritActiveDesignation(res.MeritsActiveDesignationId).subscribe((data: any) => {
          this._onlinePostingService.getMeritPosting(res.Id).subscribe((resPosting: any) => {
            if (resPosting) {
              this.routeTo = 'preferences';
            } else {
              if (data.IsActive == 'Y') {
                this.routeTo = 'preferences';
              } else {
                this.routeTo = 'preferences';
              }
            }

            this._onlinePostingService.login({ username: this.user.Cnic, password: this.user.password }).subscribe((res2) => {
              if (res2) {
                if (this.setUser(res2)) {
                  this.sigingin = false;
                  this.router.navigate(['/ppsc/' + this.routeTo]);
                }
              }
            }, err => {
              console.log(err);
            });
          }, err => {
            console.log(err);
          });


        }, err => {
          console.log(err);
          this.sigingin = false;
        });
      } else {
        this.meritExist = false;
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
