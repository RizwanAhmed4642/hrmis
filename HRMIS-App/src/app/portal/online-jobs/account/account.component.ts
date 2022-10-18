import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { NotificationService } from '../../../_services/notification.service';
import { CookieService } from '../../../_services/cookie.service';
import { OnlineJobsService } from '../online-jobs.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { Subscription } from 'rxjs/Subscription';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { RootService } from '../../../_services/root.service';
import { LocalService } from '../../../_services/local.service';

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
  public profile: any;
  public jobId: number = 0;
  public desigId: number = 0;
  public register: boolean = false;
  public checking: boolean = false;
  public portalOpen: boolean = false;
  public sigingin: boolean = false;
  public meritExist: boolean = null;
  public routeTo: string = "profile";
  public designationName: string = "";
  public CnicMask: string = "00000-0000000-0";
  public mobileMask: string = "0000-0000000";
  public generatingPassword: boolean = false;
  public passwordGenerated: boolean = false;
  private subscription: Subscription;
  constructor(private router: Router,
    private route: ActivatedRoute,
    private auth: AuthenticationService,
    private _onlineJobsService: OnlineJobsService,
    private _cookieService: CookieService,
    private _rootService: RootService,
    private _localService: LocalService,
    private _notificationService: NotificationService) { }

  ngOnInit() {
    window.location.href = "https://onlinejobs.pshealthpunjab.gov.pk/main/apply"
    // this._cookieService.deleteCookie('ussr');
    // this._cookieService.deleteCookie('ussrpublic');
    // this._cookieService.deleteCookie('cnicussrpublic');
    // this.getJobs();
    // this.fetchParams();
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
  private getJobs = () => {
    this.jobs = [];
    this._onlineJobsService.getJobs().subscribe((res: any) => {
      if (res) {
        this.jobs = res;
      }
    },
      err => { console.log(err); }
    );
  }
  public checkPortalActive(id) {
    this.checking = true;
    this._onlineJobsService.getMeritActiveDesignation(id).subscribe((res: any) => {
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
    this.user.roles.push('JobApplicantPHFMC');
    this.user.isUpdated = true;
    this.user.UserName = this.user.Cnic;
    this.user.UserDetail = 'Job Applicant PHFMC Portal User.';

    this._onlineJobsService.getPassword(this.user).subscribe((response: any) => {
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
    this._onlineJobsService.getMeritProfile(this.user.Cnic).subscribe((res: any) => {
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
    this._onlineJobsService.login({ username: this.user.Cnic, password: this.user.password }).subscribe((res2) => {
      if (res2) {
        let setuser = this.setUserPHFMC(res2);
        this._onlineJobsService.getApplicant(this.user.Cnic).subscribe((res3: any) => {
          if (res3) {
            if (res3.Status_Id == 2) {
              this._onlineJobsService.getApplicantDocuments(res3.Id).subscribe((res: any) => {
                if (res) {
                  this.routeTo = 'experience';
                } else {
                  this.routeTo = 'document';
                }
              }, err => {
                console.log(err);
                this.routeTo = 'profile';
              });
            } else if (res3.Status_Id == 3) {
              this.routeTo = 'apply-now/' + this.desigId;
            }
          }
          if (setuser) {
            this.router.navigate(['/job/' + this.routeTo]);
          }
          this.sigingin = false;
        }, err2 => {
          console.log(err2);
          this.sigingin = false;
        });
      }
    }, err => {
      console.log(err);
      this.sigingin = false;
    });
  }
  public setUserPHFMC(_: any): boolean {
    let token = _.access_token;
    let userName = _.userName;
    if (token !== '') {
      this._cookieService.deleteAndSetCookie('cnicussrphfmc', userName);
      this._cookieService.deleteAndSetCookie('ussrphfmc', token);
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
