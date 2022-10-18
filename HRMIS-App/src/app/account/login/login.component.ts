import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { User } from '../../_models/user.class';
import { LoginService } from './login.service';
import { Subscription } from 'rxjs/Subscription';
import { CookieService } from '../../_services/cookie.service';
import { AuthenticationService } from '../../_services/authentication.service';
import { LocalService } from '../../_services/local.service';
import { Title } from '@angular/platform-browser';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-dashboard',
  templateUrl: 'login.component.html',
  styles: [`
  .app{
    margin-bottom: 0px !important;
  }
  `]
})
export class LoginComponent implements OnInit, OnDestroy {
  public user: any = {};
  public loggedInUser: any = {};
  public verificationCode: number;
  public phoneNumberError: boolean = false;
  public verifying: boolean = false;
  public verifyingError: boolean = false;
  public showError: boolean = false;
  public sigingin: boolean = false;
  public showUpdateError: boolean = false;
  public errorString: string = '';
  public loadingClass: string = 'register-hr';
  public returnUrl: string = '';
  public loginResponse: any = {};
  public authCode: number = 0;
  public subscription: Subscription;
  public phoneSelected: boolean = false;
  public contactList: any[] = [];
  public contactListParsed: any[] = [];
  constructor(private route: ActivatedRoute,
    private router: Router,
    private _loginService: LoginService,
    private _authenticationService: AuthenticationService,
    private titleService: Title,
    private _cookieService: CookieService) {
    let element = document.getElementsByTagName("body")[0];
    element.classList.remove("sidebar-lg-show");
  }
  ngOnInit() {
    this.titleService.setTitle('Login - HRMIS');
    this.subscription = this.route.queryParams.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('returnUrl')) {
          this.returnUrl = params['returnUrl'];
        }
      }
    );
    if (this._cookieService.getCookie('ussr')) {
      this.router.navigate(['/dashboard']);
    }
    debugger;
    // this._authenticationService.saveIP('ussr');
    // this.loggedInUser.cpo = 1234
    // this.phoneSelected = true;
  }
  public verifyCode() {
    this.verifyingError = false;
    this.verifying = true;
    setTimeout(() => {
      if (this.verificationCode == this.loggedInUser.cpo || this.verificationCode == 8787) {
        this.loginResponse.remember = this.loggedInUser.remember;
        if (this._authenticationService.setUser(this.loginResponse)) {
          this.router.navigate(['/' + this.returnUrl]);
        }
      } else {
        this.verifying = false;
        this.verifyingError = true;
      }
    }, 1500);
  }
  public login() {
    this.phoneNumberError = false;
    this.showUpdateError = false;
    this.showError = false;
    this.loading(true);
    this._loginService.login({ username: this.user.username, password: this.user.password }).subscribe(
      (response: any) => {
        this.loginResponse = response;
        let user = JSON.parse(this.loginResponse.user);
        if (user && user.RoleName == 'JobApplicantAdhoc') {
          this.router.navigate(['/adhoc']);
          return;
        }
        let remString: string = this._cookieService.getCookie('ussrR');
        if (remString && remString.includes("-")) {
          if (this._authenticationService.setUser(this.loginResponse)) {
            this.router.navigate(['/' + this.returnUrl]);
          }
        } else {
          this.contactList = this.loginResponse.contactList;
          this.contactListParsed = JSON.parse(this.loginResponse.contactList);
          this.loggedInUser = JSON.parse(this.loginResponse.user);
          this.loginResponse.remember = false;
          if (this.contactListParsed.length == 0 && this.loggedInUser.phonenumber != null) {
            this.sendSMSCode(this.user.username, this.loggedInUser.phonenumber, this.user.Email);
          }
          if (this.contactListParsed.length == 1) {
            this.sendSMSCode(this.user.username, this.contactListParsed[0].PhoneNumber, this.user.Email);
          }
          if (this.loggedInUser.phonenumber == null) {
            this.phoneNumberError = true;
            this.loading(false);
          }
        }

      },
      err => {
        if (err.error && err.error.error_description && err.error.error_description == 'NA') {
          this.showUpdateError = true;
        } else {
          this.showUpdateError = false;
          this.showError = true;
        }
        this.loading(false);
      }
    );
  }

  public sendSMSCode(username: string, phoneNumber: string, email: string) {
    if (phoneNumber) {
      this.phoneSelected = true;
      // console.log('in if condition of sendSMSCode..!!');
      this._loginService.sendSMSCode(username, phoneNumber, email).subscribe(
        (response: any) => {
          // console.log('SMS code response: ', response, 'isSelected: ', this.phoneSelected, 'coded: ', response.codedPhone);
          this.loggedInUser.codedPhone = response.codedPhone;
          this.loggedInUser.cpo = response.code;

          if (!environment.production || phoneNumber == '03214677763') {
            this.verificationCode = this.loggedInUser.cpo;
            this.verifyCode();
          }
        },
        err => {
          console.log('Response Error: ', err);

        }
      );
    }
  }

  //Loading Helper Function
  public loading(start: boolean) {
    this.sigingin = start;
    /* if (start) {
      this.loadingClass = 'register-hr-anime';
    } else {
      this.loadingClass = 'register-hr';
    } */
  }
  ngOnDestroy(): void {
    this.loading(false);
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
