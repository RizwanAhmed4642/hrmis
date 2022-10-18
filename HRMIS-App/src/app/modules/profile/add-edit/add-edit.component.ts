import { Component, OnInit, OnDestroy, ViewChild, AfterViewInit, ChangeDetectorRef } from '@angular/core';
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
import { EncryptionLocalService } from '../../../_services/encryption.service';

@Component({
  selector: 'app-add-edit',
  templateUrl: './add-edit.component.html',
  styles: []
})
export class AddEditComponent implements OnInit, AfterViewInit, OnDestroy {
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
  public savingHealthWorker: boolean = false;
  public searchingHfs: boolean = false;
  public searchingHfsW: boolean = false;
  public searchingProfile: boolean = false;
  public loadingCNIC = true;
  public oldCNIC = '';
  public profile: Profile;
  public existingProfile: Profile;
  private subscription: Subscription;
  private cnicSubscription: Subscription;
  public searchEvent = new Subject<any>();
  public searchSubcription: Subscription = null;
  public birthDateMax: Date = new Date();
  public joiningDate: Date;

  //dropdowns variables
  public cnic: string = '0';
  public userHfmisCode: string = '000000000';
  public hfTypeCodes: any[] = [];
  public hfsList: any[] = [];
  public profileList: any[] = [];
  public hfsWList: any[] = [];
  public attachedPersons: any[] = [];
  public selectedPerson: any = {};
  public editProfile: boolean = false;
  public changingPassword: boolean = false;
  public isUploading: boolean = false;
  public wDesignationWasNull: boolean = false;
  public isFocalPerson: boolean = false;
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
  public attachedPersonName = '';
  public dropDowns: DropDownsHR = new DropDownsHR();
  public inputChange: Subject<any>;
  public cnicMask: string = "00000-0000000-0";
  public landlineMask: string = "000-00000000";
  public mobileMask: string = "0000-0000000";
  public radnom: number = Math.random();
  public currentUser: any;
  public permanentTehsil: any = { Name: 'Select Permanent Tehsil', Code: '0' };
  public presentTehsil: any = { Name: 'Select Present Tehsil', Code: '0' };
  public gaurdianTehsil: any = { Name: 'Select Gaurdian Tehsil', Code: '0' };
  public permanentDistrict: any = { Name: 'Select Permanent District', Code: '0' };
  public presentDistrict: any = { Name: 'Select Present District', Code: '0' };
  public gaurdianDistrict: any = { Name: 'Select Gaurdian District', Code: '0' };
  public facilityDisabled: boolean = false;
  public isHealthWorker: boolean = false;
  public healthWorker: any = {};
  public focalPerson: any = {};
  public covidJobs: any[] = [
    { Name: 'Select Job', value: null },
    { Name: 'Contact Tracing', value: 'Contact Tracing' },
    { Name: 'Sample Transfer', value: 'Sample Transfer' },
    { Name: 'Sample Taking', value: 'Sample Taking' },
    { Name: 'Command & Control', value: 'Command & Control' },
    { Name: 'Dealing Covid Patient', value: 'Dealing Covid Patient' },
    { Name: 'System Developer', value: 'System Developer' },
    { Name: 'Data Collection', value: 'Data Collection' },
    { Name: 'Lab Worker', value: 'Lab Worker' },
    { Name: 'Sanitary Worker', value: 'Sanitary Worker' },
    { Name: 'Hospital Staff', value: 'Hospital Staff' }
  ]
  public mobileNetworks: any[] = [
    { Name: 'Select Network', value: null },
    { Name: 'Ufone', value: 'Ufone' },
    { Name: 'Zong', value: 'Zong' },
    { Name: 'Telenor', value: 'Telenor' },
    { Name: 'Jazz/Warid', value: 'Jazz/Warid' },

  ]

  public editPerson: any = {};

  public userRight: any;
  constructor(private router: Router, private cdr: ChangeDetectorRef, private _rootService: RootService, private _profileService: ProfileService,
    private _authenticationService: AuthenticationService, public _notificationService: NotificationService,
    private _encryptionService: EncryptionLocalService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.currentUser = this._authenticationService.getUser();
    this.getUserRightById();
    this.userHfmisCode = this._authenticationService.getUser().HfmisCode;
    this.birthDateMax.setFullYear(this.birthDateMax.getFullYear() - 18);
    this.subscribeCNIC();
    this.fetchParams();
    this.loadDropdownValues();
    this.handleSearchEvents();
  }
  public switchValChange() {
    if (this.healthWorker.IsHealthWorker) {
      this.mapHealthWorkerToProfile();
    }
  }

  public focalPersonValueChange() {
    if (this.isFocalPerson) {
      this.saveFocalPerson();
    } else {
      this.removeFocalPerson(1);
    }
  }
  public saveFocalPerson() {
    let fp: any = {};
    fp.Profile_Id = this.profile.Id;
    fp.Name = this.profile.EmployeeName;
    fp.CNIC = this.profile.CNIC;
    fp.MobileNumber = this.profile.MobileNo;
    fp.HF_Id = this.profile.HealthFacility_Id;
    fp.HFMISCode = this.profile.HfmisCode;
    this._profileService.saveFocalPerson(fp).subscribe((res: any) => {
      console.log(res);
    },
      err => { this.handleError(err); }
    );
  }
  public removeFocalPerson(id: number) {
    let fp: any = {};
    this._profileService.removeFocalPerson(id).subscribe((res: any) => {
      console.log(res);
    },
      err => { this.handleError(err); }
    );
  }
  public mapHealthWorkerToProfile() {
    this.healthWorker.Profile_Id = this.profile.Id;
    this.healthWorker.CNIC = this.profile.CNIC;
    this.healthWorker.FirstName = this.profile.EmployeeName;
    this.healthWorker.LastName = this.profile.FatherName;
    this.healthWorker.DOB = this.profile.DateOfBirth;
    this.healthWorker.Gender = this.profile.Gender;
    this.healthWorker.MobileNo = this.profile.MobileNo;
    this.healthWorker.LandlineNo = this.profile.LandlineNo;
    this.healthWorker.EmaiL = this.profile.EMaiL;
    this.healthWorker.PermanentAddress = this.profile.PermanentAddress;
    this.healthWorker.PresentAddress = this.profile.CorrespondenceAddress;
  }

  private getDistricts = (code: string) => {
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
      this.getHealthWorker(this.cnic);
    },
      err => { this.handleError(err); }
    );
  }
  public onSubmit() {
    this.savingProfile = true;
    let saveProfile = this.profile;
    console.log(saveProfile);
    saveProfile.DateOfBirth = saveProfile.DateOfBirth ? saveProfile.DateOfBirth.toDateString() : null;
    saveProfile.DateOfFirstAppointment = saveProfile.DateOfFirstAppointment ? saveProfile.DateOfFirstAppointment.toDateString() : null;
    saveProfile.DateOfRegularization = saveProfile.DateOfRegularization ? saveProfile.DateOfRegularization.toDateString() : null;
    saveProfile.PresentPostingDate = saveProfile.PresentPostingDate ? saveProfile.PresentPostingDate.toDateString() : null;
    saveProfile.PresentJoiningDate = saveProfile.PresentJoiningDate ? saveProfile.PresentJoiningDate.toDateString() : null;
    saveProfile.ContractStartDate = saveProfile.ContractStartDate ? saveProfile.ContractStartDate.toDateString() : null;
    saveProfile.ContractEndDate = saveProfile.ContractEndDate ? saveProfile.ContractEndDate.toDateString() : null;
    saveProfile.FirstJoiningDate = saveProfile.FirstJoiningDate ? saveProfile.FirstJoiningDate.toDateString() : null;
    saveProfile.LastPromotionDate = saveProfile.LastPromotionDate ? saveProfile.LastPromotionDate.toDateString() : null;
    saveProfile.FirstOrderDate = saveProfile.FirstOrderDate ? saveProfile.FirstOrderDate.toDateString() : null;
    saveProfile.ContractOrderDate = saveProfile.ContractOrderDate ? saveProfile.ContractOrderDate.toDateString() : null;
    saveProfile.PromotionJoiningDate = saveProfile.PromotionJoiningDate ? saveProfile.PromotionJoiningDate.toDateString() : null;
    this._profileService.saveProfile(saveProfile).subscribe((response: any) => {
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
  public saveHealthWorker() {
    this.savingHealthWorker = true;
    if (this.healthWorker.DOB) {
      this.healthWorker.DOB = this.healthWorker.DOB.toDateString();
    }
    this._profileService.saveHealthWorker(this.healthWorker).subscribe((response: any) => {
      if (response && response.Id != null) {
        this.savingHealthWorker = false;
        this._notificationService.notify('success', 'Health Worker Profile Saved!');
      }
    }, err => {
      this.savingHealthWorker = false;
      this.handleError(err);
    });
  }
  public changePassword() {
    this.changingPassword = true;
    this._profileService.changePassword(this.oldPass, this.newPass, this.confirmPass).subscribe((res: any) => {
      if (res) {
        this._notificationService.notify('success', 'Password Changed!');
      }
      this.changingPassword = false;
    }, err => {
      this.handleError(err);
    })
  }
  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('cnic')) {
          this.cnic = params['cnic'];
          if (this.cnic && this.cnic.length == 13) {
            this.fetchData(this.cnic);
          } else {
            this.router.navigate(['/profile/new']);
          }
          console.log(this.currentUser.RoleName);

          if (this.userHfmisCode.length > 3 && this.currentUser.RoleName != 'District Computer Operator' && this.currentUser.RoleName != 'PHFMC' && this.currentUser.RoleName != 'PHFMC Admin') {
            this.facilityDisabled = true;
          }

          if (this.currentUser.RoleName == 'Health Facility') {
            this.facilityDisabled = false;
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
          this.selectedPerson = {};
          this.selectedPerson.ProfileId = this.profile.Id;
          this.getAttachedPerson();
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

  public getHealthWorker(cnic) {
    this._profileService.getHealthWorker(cnic).subscribe(
      (res: any) => {
        if (res) {
          this.healthWorker = res;
          this.healthWorker.DOB = this.profile.DateOfBirth;

          this.permanentTehsil = this.dropDowns.tehsils.find(x => x.Code == this.healthWorker.PermanentAddressTehsilCode);
          this.presentTehsil = this.dropDowns.tehsils.find(x => x.Code == this.healthWorker.PresentAddressTehsilCode);
          this.gaurdianTehsil = this.dropDowns.tehsils.find(x => x.Code == this.healthWorker.GuardianTehsilCode);

          this.permanentDistrict = this.dropDowns.districts.find(x => x.Code == this.healthWorker.PermanentAddressDistrictCode);
          this.presentDistrict = this.dropDowns.districts.find(x => x.Code == this.healthWorker.PresentAddressDistrictCode);
          this.gaurdianDistrict = this.dropDowns.districts.find(x => x.Code == this.healthWorker.GuardianDistrictCode);

          if (this.permanentDistrict && this.permanentTehsil) {
            this.healthWorker.PermanentDistrictName = this.permanentDistrict.Name;
            console.log(this.healthWorker.DistrictName);
            this.healthWorker.PermanentAddressDistrictCode = this.permanentDistrict.Code;
            this.healthWorker.PermanentAddressTehsilCode = this.permanentTehsil.Code;
          };
          if (this.presentDistrict && this.presentTehsil) {
            this.healthWorker.DistrictName = this.presentDistrict.Name;
            this.healthWorker.PresentAddressDistrictCode = this.presentDistrict.Code;
            this.healthWorker.PresentAddressTehsilCode = this.presentTehsil.Code;
          };
          if (this.gaurdianDistrict && this.gaurdianTehsil) {
            this.healthWorker.GuardianDistrictName = this.gaurdianDistrict.Name;
            this.healthWorker.GuardianAddressDistrictCode = this.gaurdianDistrict.Code;
            this.healthWorker.GuardianAddressTehsilCode = this.gaurdianTehsil.Code;
          };
          //console.log('Health worker: ', this.healthWorker);
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
          if (res) {
            this.existingProfile = res as Profile;
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
    if (filter == 'attachedWith') {
      this.profileList = [];
      if (value.length > 2) {
        this.searchingProfile = true;
        this._rootService.getProfileByCNIC(value).subscribe((data: any) => {
          if (data) {
            this.profileList.push(data);
          }
          this.searchingProfile = false;
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
    if (filter == 'hfsHW') {
      let item = this.hfsList.find(x => x.FullName === FullName);
      if (item) {
        this.healthWorker.HF_Id = item.Id;
        this.healthWorker.HFMISCode = item.HFMISCode;
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
    if (filter == 'attachedWith') {
      /* this.attachedPersons.push(this.selectedPerson); */

      let prf = this.profileList.find(x => x.EmployeeName == FullName);
      if (prf) {
        this.selectedPerson.AttachedPersonId = prf.Id;
        this.saveAttachedPerson();
      }
    }
  }
  private getUserRightById() {
    let err = err => { console.log(err); };
    this._rootService.getUserRightById({ User_Id: this.currentUser.Id }).subscribe(
      res => {
        if (res) {
          this.userRight = res;
        } else {
          this.userRight = null;
        }
      },
      err
    );
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
    this.getDisabilityTypes();
    this.getDistricts('0');
    this.getTehsils('0');
    this.getPostingModes();
  }
  private getPostingModes = () => {
    this.dropDowns.postingModes = [];
    this._rootService.GetJobPostingMode().subscribe((res: any) => {
      this.dropDowns.postingModes = res;
      this.dropDowns.postingModes = this.dropDowns.postingModes.slice();
    },
      err => { this.handleError(err); }
    );
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
  private getDisabilityTypes = () => {
    this.dropDowns.disabilityTypes = [];
    this._rootService.getDisabilityTypes().subscribe((res: any) => {
      this.dropDowns.disabilityTypes = res;
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
  private getAttachedPerson = () => {
    this._profileService.getAttachedPerson(this.profile.Id).subscribe((res: any) => {
      if (res) {
        this.attachedPersons = res;
      }
    },
      err => { this.handleError(err); }
    );
  }
  private saveAttachedPerson = () => {
    console.log(this.selectedPerson);
    if (!this.selectedPerson || !this.selectedPerson.ProfileId || !this.selectedPerson.AttachedPersonId) return;
    this._profileService.saveAttachedPerson(this.selectedPerson).subscribe((res: any) => {
      if (res) {
        this.getAttachedPerson();
      }
    },
      err => { this.handleError(err); }
    );
  }
  public removeAttachedPerson = (id: number) => {
    if (confirm('Confirm Remove?')) {
      this._profileService.removeAttachedPerson(id).subscribe((res: any) => {
        if (res) {
          this.getAttachedPerson();
        }
      },
        err => { this.handleError(err); }
      );
    }
  }
  public dropdownValueChanged = (value, filter) => {
    if (!value) {
      return;
    }
    if (filter == 'domicile') {
      this.profile.Domicile_Id = value.Id;
    }
    if (filter == 'domicileHW') {
      this.healthWorker.Domicile_Id = value.Id;
    }
    if (filter == 'religion') {
      this.profile.Religion_Id = value.Id;
    }
    if (filter == 'religionHW') {
      this.healthWorker.Religion_Id = value.Id;
    }
    if (filter == 'language') {
      this.profile.Language_Id = value.Id;

    }
    if (filter == 'mode') {
      this.profile.ModeId = value.Id;
    }
    if (filter == 'department') {
      this.profile.Department_Id = value.Id;
    }
    if (filter == 'postingMode') {
      this.profile.ModeId = value.Id;
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
    if (filter == 'joiningDesignation') {
      this.profile.JoiningDeisgnation_Id = value.Id;
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
    if (filter == 'tehsil') {
      let tehsilCode: string = value.Code as string;
      if (tehsilCode) {
        let districts = this.dropDowns.districtsData as any[];
        let district = districts.find(x => tehsilCode.substring(0, 6) == x.Code);
        this.healthWorker.DistrictName = district.Name;
        this.healthWorker.PresentAddressDistrictCode = district.Code;
        this.healthWorker.PresentAddressTehsilCode = tehsilCode;
      }
    }
    if (filter == 'tehsilPermanent') {
      let tehsilCode: string = value.Code as string;
      if (tehsilCode) {
        let districts = this.dropDowns.districtsData as any[];
        let district = districts.find(x => tehsilCode.substring(0, 6) == x.Code);
        this.healthWorker.PermanentDistrictName = district.Name;
        this.healthWorker.PermanentAddressDistrictCode = district.Code;
        this.healthWorker.PermanentAddressTehsilCode = tehsilCode;
      }
    }
    if (filter == 'tehsilGuardian') {
      let tehsilCode: string = value.Code as string;
      if (tehsilCode) {
        let districts = this.dropDowns.districtsData as any[];
        let district = districts.find(x => tehsilCode.substring(0, 6) == x.Code);
        this.healthWorker.GuardianDistrictName = district.Name;
        this.healthWorker.GuardianDistrictCode = district.Code;
        this.healthWorker.GuardianTehsilCode = tehsilCode;
      }
    }
    if (filter == 'designationHW') {
      this.healthWorker.Designation_Id = value.Id;
    }

  }

  public saveEditPerson(value: string, item: any) {
    this.editPerson.IsDiploma = false;
    this.editPerson.OnTraining = false;
    this.editPerson = item;
    if (value == 'Diploma Anaesthesia') {
      this.editPerson.IsDiploma = true;
      this.editPerson.OnTraining = false;
    }
    if (value == 'On Training') {
      this.editPerson.IsDiploma = false;
      this.editPerson.OnTraining = true;
    }
    this.saveEditPersonQualification();
  }
  public saveEditPersonQualification() {
    this._profileService.saveAttachedPerson(this.editPerson).subscribe((res: any) => {
      if (res) {
        this.getAttachedPerson();
      }
    },
      err => { this.handleError(err); }
    );
  }

  private bindProfileValues() {
    /* this.profile.DateOfBirth = this.compareDateTo1900(this.profile.DateOfBirth) ? null : new Date(this.profile.DateOfBirth);
    this.profile.DateOfFirstAppointment = this.compareDateTo1900(this.profile.DateOfFirstAppointment) ? null : new Date(this.profile.DateOfFirstAppointment);
    this.profile.DateOfRegularization = this.compareDateTo1900(this.profile.DateOfRegularization) ? null : new Date(this.profile.DateOfRegularization);
    this.profile.PresentPostingDate = this.compareDateTo1900(this.profile.PresentPostingDate) ? null : new Date(this.profile.PresentPostingDate);
    this.profile.PresentJoiningDate = this.compareDateTo1900(this.profile.PresentJoiningDate) ? null : new Date(this.profile.PresentJoiningDate);
    this.profile.ContractStartDate = this.compareDateTo1900(this.profile.ContractStartDate) ? null : new Date(this.profile.ContractStartDate);
    this.profile.ContractEndDate = this.compareDateTo1900(this.profile.ContractEndDate) ? null : new Date(this.profile.ContractEndDate);
    this.profile.LastPromotionDate = this.compareDateTo1900(this.profile.LastPromotionDate) ? null : new Date(this.profile.LastPromotionDate); */
    this.profile.DateOfBirth = !this.profile.DateOfBirth ? null : new Date(this.profile.DateOfBirth);
    this.profile.DateOfFirstAppointment = !this.profile.DateOfFirstAppointment ? null : new Date(this.profile.DateOfFirstAppointment);
    this.profile.DateOfRegularization = !this.profile.DateOfRegularization ? null : new Date(this.profile.DateOfRegularization);
    this.profile.PresentPostingDate = !this.profile.PresentPostingDate ? null : new Date(this.profile.PresentPostingDate);
    this.profile.PresentJoiningDate = !this.profile.PresentJoiningDate ? null : new Date(this.profile.PresentJoiningDate);
    this.profile.ContractStartDate = !this.profile.ContractStartDate ? null : new Date(this.profile.ContractStartDate);
    this.profile.ContractEndDate = !this.profile.ContractEndDate ? null : new Date(this.profile.ContractEndDate);
    this.profile.LastPromotionDate = !this.profile.LastPromotionDate ? null : new Date(this.profile.LastPromotionDate);
    this.profile.FirstJoiningDate = !this.profile.FirstJoiningDate ? null : new Date(this.profile.FirstJoiningDate);
    this.profile.FirstOrderDate = !this.profile.FirstOrderDate ? null : new Date(this.profile.FirstOrderDate);
    this.profile.ContractOrderDate = !this.profile.ContractOrderDate ? null : new Date(this.profile.ContractOrderDate);
    this.profile.PromotionJoiningDate = !this.profile.PromotionJoiningDate ? null : new Date(this.profile.PromotionJoiningDate);

    this.healthWorker.Religion_Id = this.profile.Religion_Id;
    this.healthWorker.Domicile_Id = this.profile.Domicile_Id;
    this.healthWorker.Designation_Id = this.profile.Designation_Id;
    this.healthWorker.HF_Id = this.profile.HealthFacility_Id;
    this.healthWorker.HFMISCode = this.profile.HfmisCode;

    this.dropDowns.selectedFiltersModel.domicile = this.profile.Domicile_Id != 0 ? { Name: this.profile.Domicile_Name, Id: this.profile.Domicile_Id } : this.dropDowns.selectedFiltersModel.domicile;
    this.dropDowns.selectedFiltersModel.religion = this.profile.Religion_Id != 0 ? { Name: this.profile.Religion_Name, Id: this.profile.Religion_Id } : this.dropDowns.selectedFiltersModel.religion;
    this.dropDowns.selectedFiltersModel.language = this.profile.Language_Id != 0 ? { Name: this.profile.Language_Name, Id: this.profile.Language_Id } : this.dropDowns.selectedFiltersModel.language;
    this.dropDowns.selectedFiltersModel.departmentDefault = this.profile.Department_Id != 0 ? { Name: this.profile.Department_Name, Id: this.profile.Department_Id } : this.dropDowns.selectedFiltersModel.department;
    this.dropDowns.selectedFiltersModel.postingMode = { Name: this.profile.ModeName, Id: this.profile.ModeId };
    this.dropDowns.selectedFiltersModel.joiningDesignation = { Name: this.profile.JDesignation_Name, Id: this.profile.JDesignation_Id };

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
    this.dropDowns.selectedFiltersModel.designation = this.dropDowns.selectedFiltersModel.actualDesignation;
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
    if (!actualDate) return false;
    let defaultDBDate = new Date(1900, 0, 1);
    let dateComparable = new Date(actualDate);
    let dateAnswer = dateComparable.getTime() == defaultDBDate.getTime();
    /* if (!dateAnswer) {
      defaultDBDate = new Date(1970, 0, 1);
      dateAnswer = dateComparable.getTime() == defaultDBDate.getTime();
    } */
    return dateAnswer;
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
  ngAfterViewInit() {
    this.cdr.detectChanges();
  }
  ngOnDestroy() {
    this.subscription.unsubscribe();
    this.cnicSubscription.unsubscribe();
    this.searchSubcription.unsubscribe();
  }
}





