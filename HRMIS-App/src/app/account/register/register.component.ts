import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { RegisterService } from './register.service';
import { User } from '../../_models/user.class';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { Subject } from 'rxjs/Subject';
import { Subscription } from 'rxjs/Subscription';
import { RootService } from '../../_services/root.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: 'register.component.html',
  styles: []
})
export class RegisterComponent implements OnInit {
  public user: any = {};
  public userAuth: any = {};
  public profile: any = {};

  public cnicMask: string = "00000-0000000-0";
  public mobileMask: string = "0000-0000000";
  public codeMask: string = "0000";
  public EMAIL: number = 1;
  public MOBILE: number = 2;
  public PINCODE: number = 3;
  public CONFIRMPASSWORD: number = 4;

  public pinCodeFromServer: number = 0;
  public showPin: boolean = false;

  public mobileExist: boolean = false;
  public emailExist: boolean = false;
  public codeVerificationFailed: boolean = false;
  public passwordNotMatched: boolean = false;
  public profileExist: boolean = null;
  public codeGenerated: boolean = null;
  public codeMatched: boolean = null;
  public generatingPassword: boolean = false;
  public passwordGenerated: boolean = false;
  public completeUser: boolean = false;
  public searching: boolean = false;
  public matchingCode: boolean = false;
  public generatingCode: boolean = false;
  public searchEvent = new Subject<any>();
  public searchSubcription: Subscription = null;
  public step: number = 1;
  public nextText: string = 'Verify';
  public loadingClass: string = 'register-hr';
  public mask: string = "0-0-0-0";
  public pin: string = '';
  public errors: any[] = [];
  constructor(private router: Router, private _registerService: RegisterService, private _rootService: RootService) {
    let element = document.getElementsByTagName("body")[0];
    element.classList.remove("sidebar-lg-show");
  }
  ngOnInit() {
    this.handleSearchEvents();
  }
  public getPmisUsers() {

  }
  public register() {
    this.getPassword();
  }
  public getPassword() {
    this.generatingPassword = true;
    this.errors = [];
    this.user.roles = [];
    this.user.roles.push('Employee');
    this.user.isUpdated = true;
    this.user.UserName = this.user.Cnic;
    this.user.UserDetail = 'Self Registered from Portal';
    this.user.phoneNumber = '03214677763';
    this.user.email = 'belalmughal@gmail.com';
    this._registerService.addEUser(this.user).subscribe((response: any) => {
      this.generatingPassword = false;
      if (response.Errors && response.Errors.length > 0) {
        this.errors = response.Errors;
        this.loading(false);
      } else {
        this.passwordGenerated = true;
        this.loading(false);
        //this.router.navigate(['/account/login']);
      }
    }, err => {
      this.loading(false);
      this.generatingPassword = false;
    });
  }
  public inputChanged(inputRef: number, value?: any) {
    switch (inputRef) {
      case this.MOBILE:
        this.mobileExist = false;
        break;
      case this.EMAIL:
        this.emailExist = false;
      case this.PINCODE:
        this.codeVerificationFailed = false;
        break;
      case this.CONFIRMPASSWORD:

        if (this.user.password !== this.user.confirmPassword) {
          this.passwordNotMatched = true;
        }
        else if (this.user.password === this.user.confirmPassword) {
          this.passwordNotMatched = false;
        }
        break;
      default:
        break;
    }
  }
  public handleSearchEvents() {
    this.searchSubcription = this.searchEvent.pipe(
      debounceTime(400)).subscribe((x) => {
        if (!x || !+x[12]) {
          this.profileExist == null;
          this.searching = false;
          this.loading(false);
          return;
        }
        this.search(x);
      });
  }
  public getVerificationCode() {
    this.generatingCode = true;
    this.userAuth.phoneNumber = '03214677763';
    this.userAuth.email = 'belalmughal@gmail.com';
    this._registerService.getVerificationCode(this.userAuth).subscribe((res: any) => {
      if (res) {
        this.codeGenerated = true;
      } else {

      }
      this.generatingCode = false;
    }, err => {
      console.log(err);
    });
  }
  public verifyCode() {
    this.matchingCode = true;
    this.loading(true);
    this.userAuth.phoneNumber = '03214677763';
    this.userAuth.email = 'belalmughal@gmail.com';
    this._registerService.verifySmsCode(this.userAuth).subscribe((res: any) => {
      if (res) {
        this.codeMatched = true;
        this.register();
      } else {
        this.codeMatched = false;
      }
      this.matchingCode = false;
    }, err => {
      console.log(err);
    });
  }
  public search(value: string) {
    this.searching = true;
    this.loading(true);
    this._registerService.getRegularProfileByCNIC(this.user.username).subscribe((res: any) => {
      if (res) {
        this.profileExist = true;
        this.profile = res;
        this.setProfileValues();

        //this.getPassword();
      } else {
        this.profileExist = false;
        this.loading(false);
      }
    }, err => {
      console.log(err);
    });
  }
  public setProfileValues() {
    this.userAuth.CNIC = this.profile.CNIC;
    if (this.profile.MobileNo) {
      this.user.phoneNumber = this.profile.MobileNo;
      this.userAuth.MobileNo = this.profile.MobileNo;
    }
    if (this.profile.EMaiL) {
      this.user.email = this.profile.EMaiL;
      this.userAuth.Email = this.profile.EMaiL;
    }
    if (this.profile.MobileNo && this.profile.EMaiL) {
      this.completeUser = true;
    }
    this.searching = false;
    this.loading(false);
  }
  //Loading Helper Function
  public loading(start: boolean) {
    if (start) {
      this.mobileExist = false;
      this.emailExist = false;
      this.codeVerificationFailed = false;
      this.profileExist = null;
      this.loadingClass = 'register-hr-anime';
    } else {
      this.loadingClass = 'register-hr';
    }
  }
  public censorNumber(str) {
    return '**' + str[2] + str[3] + '-' + "*".repeat(str.length - 6) + str.slice(-2);
  }
  public censorWord(str) {
    return str[0] + str[1] + "*".repeat(str.length - 3) + str.slice(-1);
  }
  public censorEmail(email) {
    var arr = email.split("@");
    return this.censorWord(arr[0]) + "@" + arr[1];
  }
}
