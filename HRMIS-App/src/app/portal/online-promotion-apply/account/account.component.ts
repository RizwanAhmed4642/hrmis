import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { NotificationService } from '../../../_services/notification.service';
import { CookieService } from '../../../_services/cookie.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { Subscription } from 'rxjs/Subscription';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { RootService } from '../../../_services/root.service';
import { LocalService } from '../../../_services/local.service';
import { DatePipe } from '@angular/common';
import { OnlinePromotionApplyService } from '../online-promotion-apply.service';

@Component({
  selector: 'app-account-preferences-portal',
  templateUrl: './account.component.html',
  styles: []
})
export class AccountComponent implements OnInit, OnDestroy {
  public dropDowns: DropDownsHR = new DropDownsHR();
  public errors: any[] = [];
  public jobs: any[] = [];
  public user: any = {};
  public searchedUser: any = {};
  //آپ کا شناختی کارڈ نمبر پہلے سے درج ہے لہذا اپنے پاس ورڈ کا اندراج کیجئے پاس ورڈ معلوم نہ ہونے کی صورت میں

  public urdu: any = {
    first: 'اپنے شناختی کارڈ نمبر کا اندراج کیجئے',
    second1: `
    آپ کا شناختی کارڈ نمبر پہلے سے درج ہے لہذا اپنے پاس ورڈ کا اندراج کیجئے
    `,
    second2: `Forgot Password?`,
    second3: `
     کا انتخاب کریں۔
    `,
    secondeng: 'Your CNIC is already registered, please enter your password. You can reset password by clicking on Forgot Password!',
    third: `
    آپ کے شناختی کارڈ نمبر کا پہلے سےاندراج موجود نہیں ہے۔ لہذا اپنے موبائل نمبر اور ای میل ایڈریس کا اندراج کر کے خود کو رجسٹر کروائیں ۔ موبائل نمبر اور ای میل ایڈریس پر موصول ہونے والے پاس ورڈ سے لاگ اِن کریں۔
    `,
    thirdeng: 'Your CNIC is not registered. Please enter your mobile number and email address. You will recieve password on your mobile number and email address',
    fourth: `
کسی بھی قسم کی دشواری کی صورت میں ہیلپ لائن نمبر 1033سے رہنمائی حاصل کی جاسکتی ہے۔
`,
    fifth: `اپنا موبائل نمبر یا ای میل ایڈریس تبدیل کرنے کے لیے براہ کرم 04299206173 پر رابطہ کریں کیونکہ ویب سائٹ میں لاگ ان کرنے کا پن کوڈ آپ کے رجسٹرڈ موبائل نمبر اور ای میل پر بھیجا جائے گا۔`,
    fiftheng: 'To change your mobile number or email address please contact at 042-99206173 because the pin code for login the portal will be sent on your registered mobile number and email.',
    difurdu: `ویب سائٹ کو لاگ ان کرنے میں کسی مشکل کی صورت میں آپ سے گزارش ہے کہ   04299206173  پر صبح 10 بجے سے شام 4 بجے (پیر سے جمعہ) رابطہ کریں۔`,
    desc1: `سپیشلسٹ ڈاکٹرز کی عارضی سنیارٹی لسٹ کا جائزہ لینے کے لیے اپنے شناختی کارڈ نمبر کا اندراج کرتے ہوئے لاگ اِن کریں۔`,
    desc2: ` :لاگ ان کرنے کے بعد آپ کو درج ذیل مراحل پر عمل کرنا ہوگا`,
    desc3: `۔(۱) اگر آپ کا ریکارڈ عارضی سنیارٹی لسٹ میں درست ہے تو آپ سے درخواست کی جاتی ہے کہ مطلوبہ دستاویزات اپ لوڈ کریں۔`,
    desc4: `۔(۲) اگر آپ کا ریکارڈ عارضی سنیارٹی لسٹ میں غلط ہے تو آپ سے درخواست ہے کہ اپنا ریکارڈ اپ ڈیٹ کریں اور مطلوبہ دستاویزات اپ لوڈ کریں۔`,
    desc5: `۔(۳) اگر آپ کا نام عارضی سنیارٹی لسٹ میں موجودنا ہے تو آپ سے درخواست ہے کہ اپنا پروفائل بنائیں اور مطلوبہ دستاویزات اپ لوڈ کریں۔`,
  };
  public profile: any;
  public jobId: number = 0;
  public desigId: number = 802;
  public forgetPassword: boolean = false;
  public hideInfo: boolean = false;
  public register: boolean = true;
  public userExists: boolean = null;
  public checking: boolean = false;
  public checkingUser: boolean = false;
  public portalOpen: boolean = false;
  public sigingin: boolean = false;
  public registering: boolean = false;
  public meritExist: boolean = null;
  public routeTo: string = "profile";
  public designationName: string = "Medical Officer";
  public CnicMask: string = "00000-0000000-0";
  public mobileMask: string = "0000-0000000";
  public loginError: string = "";
  public generatingPassword: boolean = false;
  public passwordGenerated: boolean = false;
  private subscription: Subscription;
  public portalClosed: boolean = false;
  public closureDate: any = new Date('12-11-2021');
  constructor(private router: Router,
    private route: ActivatedRoute,
    private auth: AuthenticationService,
    private _onlineAdhocService: OnlinePromotionApplyService,
    private _cookieService: CookieService,
    private _rootService: RootService,
    private _localService: LocalService,
    private _notificationService: NotificationService,
    private datePipe: DatePipe) { }

  ngOnInit() {
    this.checkPortalClosure();
    this._cookieService.deleteCookie('ussr');
    this._cookieService.deleteCookie('ussrpublic');
    this._cookieService.deleteCookie('cnicussrpublic');
    this.proceed();
    this.getJobs();
    this.fetchParams();
  }

  public checkPortalClosure() {
    this.portalClosed = false;
    /*  let currentDate: any = new Date();
     currentDate = this.datePipe.transform(currentDate, 'dd-MM-yyyy');
     this.closureDate = this.datePipe.transform(this.closureDate, 'dd-MM-yyyy');
     if (this.closureDate === currentDate) {
       this.portalClosed = false;
     } */
  }
  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('jobId')) {
          let param = params['jobId'];
          if (+param) {
            this.jobId = +params['jobId'] as number;
          }
        }
      }
    );
  }
  public checkUSer() {
    if (this.user.Cnic && this.user.Cnic[12] != ' ') {
      this.checkingUser = true;
      this.hideInfo = true;
      this._onlineAdhocService.getUserPromotion(this.user.Cnic).subscribe((res: any) => {
        if (res == false) {
          this._onlineAdhocService.getUserFull(this.user.Cnic).subscribe((res: any) => {
            if (res) {
              this.searchedUser = res;
              this.user.PhoneNumber = this.searchedUser.PhoneNumber;
              this.user.Email = this.searchedUser.Email;
              if (this.searchedUser.UserDetail == 'Promotion User') {
                this.userExists = true;
                this.registering = false;
              } else {
                this.userExists = false;
                this.registering = true;
              }
              this.checkingUser = false;
            }
          },
            err => { console.log(err); }
          );
        } else if (res == true) {
          this.userExists = false;
          this.registering = true;
          this.checkingUser = false;
        }
      },
        err => { console.log(err); }
      );
    } else {
      this.hideInfo = false;
    }
  }
  public showregisterFrom(n: number) {
    if (n == 1) {
      this.registering = true;
    }
    if (n == 2) {
      this.registering = false;
    }
  }
  public goBack() {
    this.userExists = null;
    if (this.registering) {
      this.registering = false;
    }
    if (this.forgetPassword) {
      this.forgetPassword = false;
    }
  }
  public test(phone) {
    phone = phone.replace(/(\d{3})(\d{2})/, '$1****$2');
    return phone;
  }
  private getJobs = () => {
    this.jobs = [];
    this._onlineAdhocService.getJobs().subscribe((res: any) => {
      if (res) {
        this.jobs = res;
      }
    },
      err => { console.log(err); }
    );
  }
  public checkPortalActive(id) {
    this.checking = true;
    this._onlineAdhocService.getMeritActiveDesignation(id).subscribe((res: any) => {
      /*  if (res && res.IsActive && res.IsActive == 'Y') {
         this.portalOpen = true;
       } else {
         this.portalOpen = false;
       } */
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

  public dropdownValueChanged = (value, filter) => {
    if (!value) {
      return;
    }
    if (filter == 'designation') {
      this.desigId = value.Designation_Id;
      this.designationName = value.DesignationName;
    }
  }
  public proceed() {
    this._localService.set('desigaplid', this.desigId);
    this._localService.set('desigaplname', this.designationName);
    this.register = true;
  }
  public getPassword() {
    this.passwordGenerated = false;
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
    this.user.roles.push('Promotion Applicant');
    this.user.isUpdated = true;
    this.user.UserName = this.user.Cnic;
    this.user.PhoneNumber = this.user.PhoneNumber;
    this.user.UserDetail = 'Promotion User';
    this.user.ConfirmPassword = this.user.Password;
    this._onlineAdhocService.getPassword(this.user).subscribe((response: any) => {
      this.generatingPassword = false;
      if (response.Errors && response.Errors.length > 0) {
        this.errors = response.Errors;
        this._notificationService.notify('danger', 'User Registration Failed.');
      } else {
        this._notificationService.notify('success', response);
        this.passwordGenerated = true;
        this.registering = false;
        this.forgetPassword = false;
        this.checkUSer();
        //this.router.navigate(['/user']);
      }
    }, err => {
      console.log(err);
      this.generatingPassword = false;
    });
  }
  public fetchMeritInfo() {
    this._onlineAdhocService.getMeritProfile(this.user.Cnic).subscribe((res: any) => {
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
    this.sigingin = true;
    this.loginError = '';
    this._onlineAdhocService.login({ username: this.user.Cnic, password: this.user.password }).subscribe((res) => {
      if (res) {
        let setuser = this.setUserPromotion(res);
        console.log(setuser);

        this._onlineAdhocService.getApplicant(this.user.Cnic).subscribe((res2: any) => {
          if (res2) {
            console.log(res2);

            /*  if (res2.Status_Id == 2) {
              this._onlineAdhocService.getApplicantDocuments(res2.Id).subscribe((res3: any) => {
                if (res3) {
                  this.routeTo = 'experience';
                } else {
                  this.routeTo = 'document';
                }
              }, err => {
                console.log(err);
                this.routeTo = 'profile';
              });
            } else if (res2.Status_Id == 3) {
              this.routeTo = 'apply-now/' + this.desigId;
            }
            else if (res2.Status_Id == 4) {
              this.router.navigate(['/adhoc/home']);
            }
            else if (res2.Status_Id == 5) {
              this.router.navigate(['/adhoc/apply']);
            } */
          }
          /*  if (setuser) {
             // this.router.navigate(['/adhoc/' + this.routeTo]);
             this.router.navigate(['/adhoc/apply']);
           } */
          this.router.navigate(['/promotion-apply/seniority-list']);
          /*            if (res2.Status_Id == 4 || this.portalClosed) {
                      console.log('Portal Closed!');
                      this.router.navigate(['/adhoc/home']);
                    } else {
                      this.router.navigate(['/adhoc/apply']);
                    } */
          this.sigingin = false;
        }, err2 => {
          console.log(err2);
          this.sigingin = false;
        });
      }
    }, err => {
      this.loginError = err.error.error_description;
      console.log(err);
      this.sigingin = false;
    });
  }
  public setUserPHFMC(_: any): boolean {
    console.log('in set user..!!');
    let token = _.access_token;
    let userName = _.userName;
    if (token !== '') {
      this._cookieService.deleteAndSetCookie('cnicussradhoc', userName);
      this._cookieService.deleteAndSetCookie('ussradhoc', token);
      this._cookieService.deleteAndSetCookie('ussr', token);
      return true;
    }
    else {
      return false;
    }
  }
  public setUserPromotion(_: any): boolean {
    console.log('in set user..!!');
    let token = _.access_token;
    let userName = _.userName;
    if (token !== '') {
      this._cookieService.deleteAndSetCookie('cnicussrpromotion', userName);
      this._cookieService.deleteAndSetCookie('ussrpromotion', token);
      this._cookieService.deleteAndSetCookie('ussr', token);
      return true;
    }
    else {
      return false;
    }
  }
  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
