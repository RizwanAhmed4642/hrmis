import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { UserService } from '../user.service';
import { Subscription } from 'rxjs/Subscription';
import { ActivatedRoute, Router } from '@angular/router';
import { cstmdummyHFAC, cstmdummyUserLevels } from '../../../_models/cstmdummydata';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { RootService } from '../../../_services/root.service';
import { UserClaims, User } from '../user-claims.class';
import { switchMap } from 'rxjs/operators/switchMap';
import { from } from 'rxjs/observable/from';
import { delay } from 'rxjs/operators/delay';
import { map } from 'rxjs/operators/map';
import { AuthenticationService } from '../../../_services/authentication.service';
import { Subject } from 'rxjs/Subject';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { NotificationService } from '../../../_services/notification.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styles: []
})
export class RegisterComponent implements OnInit, OnDestroy {


  @ViewChild("division", { static: false }) division;
  @ViewChild("district", { static: false }) district;
  @ViewChild("tehsil", { static: false }) tehsil;
  @ViewChild("category", { static: false }) category;
  @ViewChild("hfTypeList", { static: false }) hfType;

  private subscription: Subscription;
  public divisions: any[] = [];
  public districts: any[] = [];
  public tehsils: any[] = [];
  public categories: any[] = [];
  public hfTypes: any[] = [];
  public loading: boolean = false;
  public saving: boolean = false;
  public loadingCNIC: boolean = false;
  public editUserMode: boolean = false;
  public searchingHfs: boolean = false;
  public userId: string = '';
  public currenUser: User;
  public diaryOfficer: any = {};
  public user: User = new User();
  public roles: any[] = [];
  public rolesData: any[] = [];
  public userClaims: UserClaims[] = [];
  public userClaim: UserClaims = new UserClaims();
  public accessLevelsData: Array<{ Name: string, Id: number }> = cstmdummyUserLevels.slice();
  public selectedAccessLevels: any[] = [];
  public cnicMask: string = "00000-0000000-0";
  public mobileMask: string = "0000-0000000";
  public dropDowns: DropDownsHR = new DropDownsHR();
  public selectedFiltersModel: any;
  public dropdata: Array<{ Name: string, Id: number }> = cstmdummyHFAC.slice();
  public hfsList: any[] = [];
  public inputChange: Subject<any>;
  public searchEvent: Subject<any>;
  public profile: any;
  public errors: string[] = [];
  private cnicSubscription: Subscription;
  private searchSubcription: Subscription;

  constructor(private router: Router, private _notificationService: NotificationService, private _authenticationService: AuthenticationService, private _rootService: RootService, private _userService: UserService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.currenUser = this._authenticationService.getUser();
    this.selectedFiltersModel = this.dropDowns.selectedFiltersModel;

    this.fetchParams();
    this.getRoles();
    this.loadDropdownValues();
    this.subscribeCNIC();
    this.handleSearchEvents();
  }
  public getRoles = () => {
    this._userService.getRoles().subscribe(
      (res: any) => {
        this.roles = res as any;
        this.rolesData = this.roles;
      }
    );
  }
  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('id')) {
          this.userId = params['id'];
          if (this.userId) {
            this.editUserMode = true;
            this.getUser();
          } else {
            this.editUserMode = false;
          }
        }
      }
    );
  }
  private getUser() {
    this.loading = true;
    this._userService.getUserById(this.userId).subscribe(
      response => this.handleResponse(response),
      err => this.handleError(err)
    );
  }
  private handleResponse(response: any) {
    this.user = response;
    if (this.user.Cnic && this.user.Cnic.length == 13) {
      this.searchProfile(this.user.Cnic);
    }
    this.loading = false;
  }
  public onSubmit() {
    this.saving = true;
    this.errors = [];
    if (this.user.RoleName && this.user.RoleName.length > 0) {
      this.user.roles = [];
      this.user.roles.push(this.user.RoleName);
      this.user.isUpdated = true;
    }
    if (this.editUserMode == true) {
      this._userService.editUser(this.user).subscribe((response: any) => {
        this.saving = false;
        if (response.Errors && response.Errors.length > 0) {
          this.errors = response.Errors;
          this._notificationService.notify('danger', 'User Update Failed.');
        } else if (response) {
          this._notificationService.notify('success', response + '.');
          if (this.user.RoleName != 'Office Diary') {
            this.router.navigate(['/user']);
          }
        }
      }, err => {
        this._notificationService.notify('danger', err.Message);
        this.handleError(err);
      });
    } else {
      this._userService.saveUser(this.user).subscribe((response: any) => {
        this.saving = false;
        if (response.Errors && response.Errors.length > 0) {
          this.errors = response.Errors;
          this._notificationService.notify('danger', 'User Registration Failed.');
        } else {
          this._notificationService.notify('success', response);
          if (this.user.RoleName != 'Office Diary') {
            this.router.navigate(['/user']);
          }
        }
      }, err => {
        this._notificationService.notify('danger', err.Message);
        this.handleError(err);
      });
    }
  }
  public saveOffice() {
    this._userService.editUser(this.user).subscribe((response: any) => {
      this.saving = false;
      if (response.Errors && response.Errors.length > 0) {
        this.errors = response.Errors;
        this._notificationService.notify('danger', 'User Update Failed.');
      } else if (response) {
        this._notificationService.notify('success', response + '.');
        if (this.user.RoleName != 'Office Diary') {
          this.router.navigate(['/user']);
        }
      }
    }, err => {
      this._notificationService.notify('danger', err.Message);
      this.handleError(err);
    });
  }
  private loadDropdownValues = () => {
    this.getDivisions(this._authenticationService.getUserHfmisCode());
    this.getDistricts(this._authenticationService.getUserHfmisCode());
    this.getTehsils(this._authenticationService.getUserHfmisCode());
    this.getHFTypes();
    this.getHealthFacilities(this._authenticationService.getUserHfmisCode());
    this.getPandSOfficers('fts');
  }
  public subscribeCNIC() {
    this.inputChange = new Subject();
    this.cnicSubscription = this.inputChange.pipe(debounceTime(300)).subscribe((query) => {
      this.loadingCNIC = true;
      if (!query) { this.loadingCNIC = false; return; }
      let cnic: string = query as string;
      cnic = cnic.replace(' ', '');
      if (cnic.length != 13) { this.loadingCNIC = false; return; }
      this.searchProfile(cnic);
    });
  }
  private getPandSOfficers = (type: string) => {
    this.dropDowns.officers = [];
    this.dropDowns.officersData = [];
    this._rootService.getPandSOfficers(type).subscribe((res: any) => {
      if (res) {
        this.dropDowns.officers = res;
        this.dropDowns.officersData = this.dropDowns.officers;
      }
    },
      err => { this.handleError(err); }
    );
  }
  public searchProfile(cnic) {
    this._rootService.getProfileByCNIC(cnic).subscribe(
      (res: any) => {
        if (res) {
          this.profile = res;
          this.user.ProfileId = this.profile.Id;
          this.user.PhoneNumber = this.profile.MobileNo;
          this.user.Email = this.profile.EMaiL;
        }
        this.loadingCNIC = false;
      },
      err => {
        this.handleError(err);
      }
    );
  }
  public handleSearchEvents() {
    this.searchEvent = new Subject();
    this.searchSubcription = this.searchEvent.pipe(
      debounceTime(400)).subscribe((x: any) => {
        console.log(x);
        this.search(x.event, x.filter);
      });
  }
  public search(value: string, filter: string) {
    if (filter == 'hfs') {
      this.hfsList = [];
      if (value.length > 2) {
        this.searchingHfs = true;
        this._rootService.searchHealthFacilities(value).subscribe((data) => {
          this.hfsList = data as any[];
          this.searchingHfs = false;
        });
      }
    }
  }
  public searchClicked(FullName, filter) {
    if (filter == 'hfs') {
      let item = this.hfsList.find(x => x.FullName === FullName);
      if (item) {
        this.user.hfmiscode = item.HFMISCode;
        this.user.HfmisCodeNew = item.HFMISCode;
      }
    }
  }
  private getDivisions = (code: string) => {
    this.dropDowns.divisions = [];
    this.dropDowns.divisionsData = [];
    this._rootService.getDivisions(code).subscribe((res: any) => {
      this.dropDowns.divisions = res;
      this.dropDowns.divisionsData = this.dropDowns.divisions.slice();
    },
      err => { this.handleError(err); }
    );
  }
  private getDistricts = (code: string) => {
    this.dropDowns.districts = [];
    this.dropDowns.districtsData = [];
    this._rootService.getDistricts(code).subscribe((res: any) => {
      this.dropDowns.districts = res;
      this.dropDowns.districtsData = this.dropDowns.districts.slice();
    },
      err => { this.handleError(err); }
    );
  }
  private getTehsils = (code: string) => {
    this.dropDowns.tehsils = [];
    this.dropDowns.tehsilsData = [];
    this._rootService.getTehsils(code).subscribe((res: any) => {
      this.dropDowns.tehsils = res;
      this.dropDowns.tehsilsData = this.dropDowns.tehsils.slice();
    },
      err => { this.handleError(err); }
    );
  }
  private getHFTypes = () => {
    this.dropDowns.hfTypes = [];
    this.dropDowns.hfTypesData = [];
    this._rootService.getHFTypes().subscribe((res: any) => {
      this.dropDowns.hfTypes = res;
      this.dropDowns.hfTypesData = this.dropDowns.hfTypes.slice();
    },
      err => { this.handleError(err); }
    );
  }
  private getHealthFacilities = (code: string) => {
    this._rootService.getHealthFacilities(code).subscribe((res: any) => {
      console.log(res);
      this.dropDowns.healthFacilitiesData = res;
    },
      err => { this.handleError(err); }
    );
  }
  public addLevel() {
    this.userClaims.push(this.userClaim);
    this.userClaim = new UserClaims();
    console.log(this.userClaims);

  }
  public dropdownValueChanged = (value, filter) => {
    if (!value) { return; }
    if (filter == 'district') {
      this.user.TehsilID = '000';
      this.user.hfmiscode = '000';
      this.user.HfmisCodeNew = '000';
      this.getHealthFacilities(value);
    }
    if (filter == 'tehsil') {
      this.user.hfmiscode = '000';
      this.user.HfmisCodeNew = '000';
      this.getHealthFacilities(value);
    }

    if (filter == 'officer') {

      this.diaryOfficer.User_Id = this.user.Id;
      this.diaryOfficer.Office_Id = value.Id;
      this.diaryOfficer.Profile_Id = this.user.ProfileId;
    }
    /*  if (filter == 'accessLevel') {
       this.userClaim.ClaimType = value.Name;
       console.log(value);
 
     }
     if (filter == 'division') {
       this.divisions = this.addCodes(value);
       this.userClaim.ClaimValue = JSON.stringify(this.divisions);
     }
     if (filter == 'district') {
       this.districts = this.addCodes(value);
       this.userClaim.ClaimValue = JSON.stringify(this.districts);
     }
     if (filter == 'tehsil') {
       this.tehsils = this.addCodes(value);
       this.userClaim.ClaimValue = JSON.stringify(this.tehsils);
     }
     if (filter == 'hfType') {
       this.hfTypes = this.addCodes(value);
       this.userClaim.ClaimValue = JSON.stringify(this.hfTypes);
     }
     if (filter == 'category') {
       this.categories = this.addCodes(value);
       this.userClaim.ClaimValue = JSON.stringify(this.categories);
     } */
  }
  public dropdownValueRemoved = (value, filter) => {
    if (!value) { return; }
    if (filter == 'accessLevel') {
      this.userClaim.ClaimType = value;
    }
    if (filter == 'division') {
      console.log(value);
    }
    if (filter == 'district') {
      this.districts = this.addCodes(value);
    }
    if (filter == 'tehsil') {
      this.tehsils = this.addCodes(value);
    }
    if (filter == 'hfType') {
      this.hfTypes = this.addCodes(value);
    }
    if (filter == 'category') {
      this.categories = this.addCodes(value);
    }
  }
  public handleFilter = (value, filter) => {
    if (filter == 'division') {
      this.dropDowns.divisionsData = this.dropDowns.divisions.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
    if (filter == 'district') {
      this.dropDowns.districtsData = this.dropDowns.districts.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
    if (filter == 'tehsil') {
      this.dropDowns.tehsilsData = this.dropDowns.tehsils.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
    if (filter == 'category') {
      this.dropDowns.hfCategoryData = this.dropDowns.hfCategory.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
    if (filter == 'hfType') {
      this.dropDowns.hfTypesData = this.dropDowns.hfTypes.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
    if (filter == 'officer') {
      this.dropDowns.officersData = this.dropDowns.officers.filter((s: any) => (s.DesignationName ? s.DesignationName.toLowerCase().indexOf(value.toLowerCase()) : 0) !== -1);
    }
  }

  public addCodes(value: any[]) {
    let arr: any[] = [];
    value.forEach(val => arr.push(val.Code));
    return arr;
  }
  private handleError(err: any) {
    this.saving = false;
    this.loading = false;
    this.loadingCNIC = false;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }

  ngOnDestroy() {
    this.cnicSubscription.unsubscribe();
    this.searchSubcription.unsubscribe();
  }
}
