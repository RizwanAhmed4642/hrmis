import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import { ActivatedRoute, Router } from '@angular/router';
import { cstmdummyHFAC, cstmdummyUserLevels } from '../../../_models/cstmdummydata';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { RootService } from '../../../_services/root.service';
import { switchMap } from 'rxjs/operators/switchMap';
import { from } from 'rxjs/observable/from';
import { delay } from 'rxjs/operators/delay';
import { map } from 'rxjs/operators/map';
import { AuthenticationService } from '../../../_services/authentication.service';
import { Subject } from 'rxjs/Subject';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { NotificationService } from '../../../_services/notification.service';
import { UserService } from '../../user/user.service';
import { CookieService } from '../../../_services/cookie.service';
import { HelplineService } from '../helpline.service';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styles: []
})


export class UserComponent implements OnInit, OnDestroy {

  public CnicMask: string = "00000-0000000-0";
  public phonenumberMask: string = "0000-0000000";
  public user: any = {};
  public registering: boolean = false;
  public saving: boolean = false;
  public userExists: boolean = null;
  public searchedUser: any = {};
  public applicant: any = {};
  public errors: any[] = [];
  constructor(private router: Router,
    private route: ActivatedRoute,
    private auth: AuthenticationService,
    private _helplineService: HelplineService,
    private _cookieService: CookieService,
    private _rootService: RootService,
    private _notificationService: NotificationService) { }

  ngOnDestroy(): void {
  }
  ngOnInit(): void {
  }

  public checkUSer() {
    if (this.user.Cnic && this.user.Cnic[12] != ' ') {
      this._helplineService.getAdhocuser(this.user.Cnic).subscribe((res: any) => {
        if (res) {
          if (res.user && res.user.PhoneNumber) {
            if (!res.user.PhoneNumber.startsWith('0')) {
              res.user.PhoneNumber = '0' + res.user.PhoneNumber;
            }
          }
          this.searchedUser = res.user;
          this.applicant = res.applicant;
          this.user.PhoneNumber = this.searchedUser.PhoneNumber;
          this.user.Email = this.searchedUser.Email;
          this.userExists = true;
        }
      },
        err => { console.log(err); }
      );
      console.log(this.user.Cnic);
    }
  }

  public updateUser() {
    this.saving = true;
    if (this.user.RoleName && this.user.RoleName.length > 0) {
      this.user.roles = [];
      this.user.roles.push(this.user.RoleName);
      this.user.isUpdated = true;
    }
    this._helplineService.editUserPhoneEmail(
      {
        Id: this.searchedUser.Id,
        CNIC: this.searchedUser.Cnic,
        PhoneNumber: this.searchedUser.PhoneNumber,
        Email: this.searchedUser.Email
      }
    ).subscribe((response: any) => {
      if (response) {

        if (!this.applicant) {
          this._notificationService.notify('success', 'User Saved');
          this.searchedUser = {};
          this.applicant = {};
          this.user = {};
          this.userExists = false;
          this.saving = false;
        } else {
          this._helplineService.editAdhocApplicant(
            {
              CNIC: this.applicant.CNIC,
              MobileNumber: this.searchedUser.PhoneNumber,
              Email: this.searchedUser.Email
            }
          ).subscribe((res: any) => {
            this.saving = false;
            if (response.Errors && response.Errors.length > 0) {
              this.errors = response.Errors;
              this._notificationService.notify('danger', 'User Update Failed.');
            } else if (res) {
              this._notificationService.notify('success', 'User Saved');
              this.searchedUser = {};
              this.applicant = {};
              this.user = {};
              this.userExists = false;
            }
          }, err => {
            this._notificationService.notify('danger', err.Message);
            console.log(err);

          });
        }



      }
    }, err => {
      this._notificationService.notify('danger', err.Message);
      console.log(err);

    });
  }


}
