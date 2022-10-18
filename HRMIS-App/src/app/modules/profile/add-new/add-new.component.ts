import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { AuthenticationService } from '../../../_services/authentication.service';
import { RootService } from '../../../_services/root.service';
import { Subscription } from 'rxjs/Subscription';
import { ActivatedRoute, Router } from '@angular/router';
import { ProfileService } from '../profile.service';
import { Profile } from '../profile.class';
import { Subject } from 'rxjs/Subject';
import { debounceTime } from 'rxjs/operators';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { NotificationService } from '../../../_services/notification.service';

@Component({
  selector: 'app-profile-add-new',
  templateUrl: './add-new.component.html',
  styles: []
})
export class AddNewComponent implements OnInit, OnDestroy {
  @ViewChild('photoRef', { static: false }) public photoRef: any;
  @ViewChild('cnicFrontRef', { static: false }) public cnicFrontRef: any;
  @ViewChild('cnicBackRef', { static: false }) public cnicBackRef: any;
  public loading = true;
  public userAccount = false;
  public userId = '';
  public oldPass = '';
  public newPass = '';
  public confirmPass = '';
  public savingProfile: boolean = false;
  public searchingHfs: boolean = false;
  public searchingHfsW: boolean = false;
  public loadingCNIC = true;
  public oldCNIC = '';
  public profile: any;
  public existingProfile: Profile;
  private subscription: Subscription;
  private cnicSubscription: Subscription;
  public searchEvent = new Subject<any>();
  public searchSubcription: Subscription = null;
  public birthDateMax: Date = new Date();

  //dropdowns variables
  public cnic: string = '0';
  public userHfmisCode: string = '000000000';
  public hfTypeCodes: any[] = [];
  public hfsList: any[] = [];
  public hfsWList: any[] = [];
  public editProfile: boolean = false;
  public changingPassword: boolean = false;
  public isUploading: boolean = false;
  public wDesignationWasNull: boolean = false;
  public photoFile: any[] = [];
  public cnicFrontFile: any[] = [];
  public cnicBackFile: any[] = [];
  public genderItems: any[] = [
    { text: 'Select Gender', value: null },
    { text: 'Male', value: 'Male' },
    { text: 'Female', value: 'Female' }
  ]
  public photoSrc = '';
  public cnicFrontSrc = '';
  public cnicBackSrc = '';
  public healthFacilityFullName = '';
  public workingHealthFacilityFullName = '';
  public dropDowns: DropDownsHR = new DropDownsHR();
  public inputChange: Subject<any>;
  public cnicMask: string = "00000-0000000-0";
  public landlineMask: string = "000-00000000";
  public mobileMask: string = "0000-0000000";
  public radnom: number = Math.random();


  constructor(private router: Router, private _rootService: RootService, private _profileService: ProfileService,
    private _authenticationService: AuthenticationService, public _notificationService: NotificationService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.userHfmisCode = this._authenticationService.getUser().HfmisCode;
    this.birthDateMax.setFullYear(this.birthDateMax.getFullYear() - 18);
    this.subscribeCNIC();
    this.fetchParams();
    this.loadDropdownValues();
    this.handleSearchEvents();
  }
  public onSubmit() {
    this.savingProfile = true;
    if (this.profile.DateOfBirth) {
      this.profile.DateOfBirth = this.profile.DateOfBirth.toDateString();
    }
    this._profileService.saveShortProfile(this.profile).subscribe((response: any) => {
      if (response && response.Id != null) {
        if (this.photoFile.length > 0) {
          this._profileService.uploadPhoto(this.photoFile, response.Id).subscribe((res: any) => {
            if (res) {
              this.savingProfile = false;
              this._notificationService.notify('success', 'Employee Profile Saved!');
              this.router.navigate(['/profile/' + response.CNIC]);
            }
          }, err => {
            this.handleError(err);
          });
        } else {
          this.savingProfile = false;
          this._notificationService.notify('success', 'Employee Profile Saved!');
          this.router.navigate(['/profile/' + response.CNIC]);
        }
      }
    }, err => {
      this.savingProfile = false;
      this.handleError(err);
    });
  }
  public changePassword() {
    this.changingPassword = true;
    this._profileService.changePassword(this.oldPass, this.newPass, this.confirmPass).subscribe((res: any) => {
      console.log(res);
      if (res) {
        this._notificationService.notify('success', 'Password Changed!');
      }
      this.changingPassword = false;
    }, err => {
      console.log(err);
      this.handleError(err);
    })
  }
  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('cnic')) {
          this.cnic = params['cnic'];
          console.log(this.cnic);
          if (this.cnic && this.cnic.length == 13) {
            this.fetchData(this.cnic);
          } else {
            this.router.navigate(['/profile/new']);
          }
        } else {
          this.profile = new Profile();
          this.profile.Department_Id = 25;
          this.editProfile = false;
          this.loading = false;
        }
        if (params.hasOwnProperty('userId')) {
          this.userId = params['userId'];
          if (this.userId == this._authenticationService.getUser().Id) {
            this.userAccount = true;
          }
        }
      }
    );
  }
  private fetchData(cnic) {
    this._profileService.getProfile(cnic).subscribe(
      (res: any) => {
        if (res) {
          this.profile = res as Profile;
          this.bindProfileValues();
        } else {
          this.editProfile = false;
        }
      },
      err => {
        this.handleError(err);
      }
    );
  }
  public hasUpperCase(str) {
    return (/[A-A]/.test(str));
  }
  public subscribeCNIC() {
    this.inputChange = new Subject();
    this.cnicSubscription = this.inputChange.pipe(debounceTime(400)).subscribe((query) => {
      this.loadingCNIC = true;
      this.existingProfile = null;
      if (!query) { this.loadingCNIC = false; return; }
      let cnic: string = query as string;
      cnic = cnic.replace(' ', '');
      if (cnic.length != 13) { this.loadingCNIC = false; return; }
      if (cnic == this.oldCNIC) { this.loadingCNIC = false; return; }
      this._profileService.getProfile(cnic).subscribe(
        res => {
          console.log(res);
          if (res) {
            this.existingProfile = res as Profile;
            console.log(this.existingProfile.CNIC);
          }
          this.loadingCNIC = false;
        },
        err => {
          this.handleError(err);
        }
      );
      if (this.profile.CNIC.length == 13) {
        let i = +this.profile.CNIC[this.profile.CNIC.length - 1];
        this.profile.Gender = (i % 2 == 0) ? 'Female' : 'Male';
      }
    });
  }
  public handleSearchEvents() {
    this.searchSubcription = this.searchEvent.pipe(
      debounceTime(400)).subscribe((x: any) => {
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
    if (filter == 'whfs') {
      this.hfsWList = [];
      if (value.length > 2) {
        this.searchingHfsW = true;
        this._rootService.searchHealthFacilitiesAll(value).subscribe((data) => {
          this.hfsWList = data as any[];
          this.searchingHfsW = false;
        });
      }
    }

  }
  public searchClicked(FullName, filter) {

    if (filter == 'hfs') {
      let item = this.hfsList.find(x => x.FullName === FullName);
      if (item) {
        this.profile.HealthFacility_Id = item.Id;
        this.profile.HfmisCode = item.HFMISCode;
      }
    }
    if (filter == 'whfs') {
      if (!FullName) {
        this.profile.WorkingHealthFacility_Id = null;
        this.profile.WorkingHFMISCode = null;
      }
      let item = this.hfsWList.find(x => x.FullName === FullName);
      if (item) {
        this.profile.WorkingHealthFacility_Id = item.Id;
        this.profile.WorkingHFMISCode = item.HFMISCode;
      }
    }
  }
  private loadDropdownValues = () => {
    this.getDomiciles();
    this.getDepartments();
    this.getDesignations();
    this.getQualifications();
    this.getEmploymentModes();
    this.getSpecializations();
    this.getPostTypes();
    this.getProfileStatuses();
  }
  private getDomiciles = () => {
    this.dropDowns.domicile = [];
    this.dropDowns.domicileData = [];
    this._rootService.getDomiciles().subscribe((res: any) => {
      this.dropDowns.domicile = res;
      this.dropDowns.domicileData = this.dropDowns.domicile.slice();
    },
      err => { this.handleError(err); }
    );
  }
  private getDepartments = () => {
    this.dropDowns.departments = [];
    this.dropDowns.departmentsData = [];
    this._rootService.getDepartments().subscribe((res: any) => {
      this.dropDowns.departments = res;
      this.dropDowns.departmentsData = this.dropDowns.departments.slice();
    },
      err => { this.handleError(err); }
    );
  }
  private getDesignations = () => {
    this.dropDowns.designations = [];
    this.dropDowns.designationsData = [];
    this._rootService.getDesignations().subscribe((res: any) => {
      this.dropDowns.designations = res;
      this.dropDowns.designationsData = this.dropDowns.designations.slice();
    },
      err => { this.handleError(err); }
    );
  }
  private getQualifications = () => {
    this.dropDowns.qualifications = [];
    this.dropDowns.qualificationsData = [];
    this._rootService.getQualifications().subscribe((res: any) => {
      this.dropDowns.qualifications = res;
      this.dropDowns.qualificationsData = this.dropDowns.qualifications.slice();
    },
      err => { this.handleError(err); }
    );
  }
  private getEmploymentModes = () => {
    this.dropDowns.employementModes = [];
    this.dropDowns.employementModesData = [];
    this._rootService.getEmploymentModes().subscribe((res: any) => {
      this.dropDowns.employementModes = res;
      this.dropDowns.employementModesData = this.dropDowns.employementModes;
    },
      err => { this.handleError(err); }
    );
  }
  private getSpecializations = () => {
    this.dropDowns.specialization = [];
    this.dropDowns.specializationData = [];
    this._rootService.getSpecializations().subscribe((res: any) => {
      this.dropDowns.specialization = res;
      this.dropDowns.specializationData = this.dropDowns.specialization.slice();
    },
      err => { this.handleError(err); }
    );
  }
  private getPostTypes = () => {
    this.dropDowns.postTypes = [];
    this.dropDowns.postTypesData = [];
    this._rootService.getPostTypes().subscribe((res: any) => {
      this.dropDowns.postTypes = res;
      this.dropDowns.postTypesData = this.dropDowns.postTypes.slice();
    },
      err => { this.handleError(err); }
    );
  }
  private getProfileStatuses = () => {
    this.dropDowns.statusData = [];
    this._rootService.getProfileStatuses().subscribe((res: any) => {
      this.dropDowns.statusData = res;
    },
      err => { this.handleError(err); }
    );
  }
  public dropdownValueChanged = (value, filter) => {
    if (!value) {
      return;
    }
    if (filter == 'domicile') {
      this.profile.Domicile_Id = value.Id;
    }
    if (filter == 'religion') {
      this.profile.Religion_Id = value.Id;

    }
    if (filter == 'language') {
      this.profile.Language_Id = value.Id;

    }
    if (filter == 'department') {
      this.profile.Department_Id = value.Id;

    }
    if (filter == 'actualDesignation') {
      this.profile.Designation_Id = value.Id;
      if (!this.profile.WDesignation_Id || this.wDesignationWasNull) {
        this.profile.WDesignation_Id = value.Id;
        this.dropDowns.selectedFiltersModel.workingDesignation = { Id: value.Id, Name: value.Name };
        this.wDesignationWasNull = true;
      }
      this.profile.Postaanctionedwithscale = value.Scale ? value.Scale : this.profile.Postaanctionedwithscale;

    }
    if (filter == 'workingDesignation') {
      this.profile.WDesignation_Id = value.Id;
    }
    if (filter == 'qualification') {
      this.profile.Qualification_Id = value.Id;

    }
    if (filter == 'specialization') {
      this.profile.Specialization_Id = value.Id;

    }
    if (filter == 'employementMode') {
      this.profile.EmpMode_Id = value.Id;

    }
    if (filter == 'postType') {
      this.profile.Posttype_Id = value.Id;

    }
    if (filter == 'hfac') {
      this.profile.Hfac = value.Id;

    }
    if (filter == 'profileStatus') {
      this.profile.Status_Id = value.Id;
    }
  }


  private bindProfileValues() {
    if (this.profile.DateOfBirth) {
      this.profile.DateOfBirth = this.compareDateTo1900(this.profile.DateOfBirth) ? null : new Date(this.profile.DateOfBirth);
    }
    if (this.profile.DateOfFirstAppointment) {
      this.profile.DateOfFirstAppointment = this.compareDateTo1900(this.profile.DateOfFirstAppointment) ? null : new Date(this.profile.DateOfFirstAppointment);
    }
    if (this.profile.DateOfRegularization) {
      this.profile.DateOfRegularization = this.compareDateTo1900(this.profile.DateOfRegularization) ? null : new Date(this.profile.DateOfRegularization);
    }
    if (this.profile.PresentPostingDate) {
      this.profile.PresentPostingDate = this.compareDateTo1900(this.profile.PresentPostingDate) ? null : new Date(this.profile.PresentPostingDate);
    }
    if (this.profile.ContractStartDate) {
      this.profile.ContractStartDate = this.compareDateTo1900(this.profile.ContractStartDate) ? null : new Date(this.profile.ContractStartDate);
    }
    if (this.profile.ContractEndDate) {
      this.profile.ContractEndDate = this.compareDateTo1900(this.profile.ContractEndDate) ? null : new Date(this.profile.ContractEndDate);
    }
    if (this.profile.LastPromotionDate) {
      this.profile.LastPromotionDate = this.compareDateTo1900(this.profile.LastPromotionDate) ? null : new Date(this.profile.LastPromotionDate);
    }
    this.dropDowns.selectedFiltersModel.domicile = this.profile.Domicile_Id != 0 ? { Name: this.profile.Domicile_Name, Id: this.profile.Domicile_Id } : this.dropDowns.selectedFiltersModel.domicile;
    this.dropDowns.selectedFiltersModel.religion = this.profile.Religion_Id != 0 ? { Name: this.profile.Religion_Name, Id: this.profile.Religion_Id } : this.dropDowns.selectedFiltersModel.religion;
    this.dropDowns.selectedFiltersModel.language = this.profile.Religion_Id != 0 ? { Name: this.profile.Language_Name, Id: this.profile.Language_Id } : this.dropDowns.selectedFiltersModel.language;
    this.dropDowns.selectedFiltersModel.departmentDefault = this.profile.Department_Id != 0 ? { Name: this.profile.Department_Name, Id: this.profile.Department_Id } : this.dropDowns.selectedFiltersModel.department;

    if (this.profile.HealthFacility_Id != 0 && this.profile.HealthFacility) {
      this._rootService.searchHealthFacilities(this.profile.HealthFacility).subscribe((data) => {
        this.dropDowns.selectedFiltersModel.actualHealthFacility = data[0] && data[0].FullName != 0 ? data[0].FullName : this.dropDowns.selectedFiltersModel.actualHealthFacility;
      });
    }
    if (this.profile.WorkingHealthFacility_Id != 0 && this.profile.WorkingHealthFacility) {
      this._rootService.searchHealthFacilities(this.profile.WorkingHealthFacility).subscribe((data) => {
        this.dropDowns.selectedFiltersModel.workingHealthFacility = data[0] && data[0].FullName != 0 ? data[0].FullName : this.dropDowns.selectedFiltersModel.workingHealthFacility;
      });
    }
    this.healthFacilityFullName = this.profile.HealthFacility ? this.profile.HealthFacility + ', ' + this.profile.Tehsil + ', ' + this.profile.District : '';
    this.workingHealthFacilityFullName = this.profile.WorkingHealthFacility ? this.profile.WorkingHealthFacility + ', ' + this.profile.WorkingTehsil + ', ' + this.profile.WorkingDistrict : '';
    this.dropDowns.selectedFiltersModel.actualDesignation = this.profile.Designation_Id != 0 ? { Name: this.profile.Designation_Name, Id: this.profile.Designation_Id } : this.dropDowns.selectedFiltersModel.actualDesignation;
    this.dropDowns.selectedFiltersModel.workingDesignation = this.profile.WDesignation_Id != 0 ? { Name: this.profile.WDesignation_Name, Id: this.profile.WDesignation_Id } : this.dropDowns.selectedFiltersModel.workingDesignation;
    this.dropDowns.selectedFiltersModel.qualification = this.profile.Qualification_Id != 0 ? { qualificationname: this.profile.QualificationName, Id: this.profile.Qualification_Id } : this.dropDowns.selectedFiltersModel.qualification;
    this.dropDowns.selectedFiltersModel.specialization = this.profile.Specialization_Id != 0 ? { Name: this.profile.SpecializationName, Id: this.profile.Specialization_Id } : this.dropDowns.selectedFiltersModel.specialization;
    this.dropDowns.selectedFiltersModel.employementMode = this.profile.EmpMode_Id != 0 ? { Name: this.profile.EmpMode_Name, Id: this.profile.EmpMode_Id } : this.dropDowns.selectedFiltersModel.employementMode;
    this.dropDowns.selectedFiltersModel.postType = this.profile.Posttype_Id != 0 ? { Name: this.profile.PostType_Name, Id: this.profile.Posttype_Id } : this.dropDowns.selectedFiltersModel.postType;
    this.dropDowns.selectedFiltersModel.profileStatus = this.profile.Status_Id != 0 ? { Name: this.profile.StatusName, Id: this.profile.Status_Id } : this.dropDowns.selectedFiltersModel.profileStatus;
    /* this.dropDowns.selectedFiltersModel.headOfDepartment =  */

    if (this.profile.Hfac) {
      let hfacs = this.dropDowns.hfacData as any[];
      let hfac = hfacs.find(x => x.Name == this.profile.Hfac);
      if (hfac) {
        this.dropDowns.selectedFiltersModel.hfac = { Name: this.profile.Hfac, Id: hfac.Id };
      }
    }
    this.oldCNIC = this.profile.CNIC;
    this.photoSrc = 'https://hrmis.pshealthpunjab.gov.pk/Uploads/ProfilePhotos/' + this.profile.CNIC + '_23.jpg?v=' + this.radnom;
    /*    this.photoSrc = 'https://hrmis.pshealthpunjab.gov.pk/Uploads/ProfilePhotos/' + this.profile.CNIC + '_23.jpg?v=' + this.radnom; */
    this.editProfile = true;
    this.loading = false;
  }
  private compareDateTo1900(actualDate) {
    let defaultDBDate = new Date(1900, 0, 1);
    let dateComparable = new Date(actualDate);
    return dateComparable.getTime() == defaultDBDate.getTime();
  }
  public readUrl(event: any, filter: string) {
    if (event.target.files && event.target.files[0]) {
      if (filter == 'pic') {
        this.photoFile = [];
        let inputValue = event.target;
        this.photoFile = inputValue.files;
        var reader = new FileReader();
        reader.onload = ((event: any) => {
          this.photoSrc = event.target.result;
        }).bind(this);
        reader.readAsDataURL(event.target.files[0]);
      }
      if (filter == 'cnicFront') {
        this.cnicFrontFile = [];
        let inputValue = event.target;
        this.cnicFrontFile = inputValue.files;
        var reader = new FileReader();
        reader.onload = ((event: any) => {
          this.cnicFrontSrc = event.target.result;
        }).bind(this);
        reader.readAsDataURL(event.target.files[0]);
      }
      if (filter == 'cnicBack') {
        this.cnicBackFile = [];
        let inputValue = event.target;
        this.cnicBackFile = inputValue.files;
        var reader = new FileReader();
        reader.onload = ((event: any) => {
          this.cnicBackSrc = event.target.result;
        }).bind(this);
        reader.readAsDataURL(event.target.files[0]);
      }

    }
  }
  private handleError(err: any) {
    this.loading = false;
    this.loadingCNIC = false;
    this.changingPassword = false;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }
  public uploadBtn(filter: string) {
    if (filter == 'pic') {
      this.photoRef.nativeElement.click();
    } else
      if (filter == 'cnicFront') {
        this.cnicFrontRef.nativeElement.click();
      } else
        if (filter == 'cnicBack') {
          this.cnicBackRef.nativeElement.click();
        }
  }
  public dashifyCNIC(cnic: string) {
    if (!cnic) return '';
    return cnic[0] +
      cnic[1] +
      cnic[2] +
      cnic[3] +
      cnic[4] +
      '-' +
      cnic[5] +
      cnic[6] +
      cnic[7] +
      cnic[8] +
      cnic[9] +
      cnic[10] +
      cnic[11] +
      '-' +
      cnic[12];
  }
  ngOnDestroy() {
    this.subscription.unsubscribe();
    this.cnicSubscription.unsubscribe();
    this.searchSubcription.unsubscribe();
  }
}
