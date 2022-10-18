import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import { ActivatedRoute, Router } from '@angular/router';
import { RootService } from '../../../_services/root.service';
import { ApplicationMaster, ApplicationDocument, ApplicationAttachment, ApplicationProfileViewModel, ApplicationLog } from '../application-fts';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { AuthenticationService } from '../../../_services/authentication.service';
import { User } from '../../../_models/user.class';
import { ApplicationFtsService } from '../application-fts.service';
import { DomSanitizer } from '@angular/platform-browser';
import { LivePreviewService } from '../../../_services/live-preview.service';
import { NgForm } from '@angular/forms';
import { Subject } from 'rxjs/Subject';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { FirebaseHisduService } from '../../../_services/firebase-hisdu.service';
import { DDS_Files } from '../../../file-tracking-system/file-search.class';
import { NotificationService } from '../../../_services/notification.service';

@Component({
  selector: 'app-add-edit',
  templateUrl: './add-edit.component.html',
  styles: []
})
export class AddEditComponent implements OnInit, OnDestroy {
  @ViewChild('f', { static: true }) ngForm: NgForm;
  private formChangesSubscription: Subscription;
  public loading: boolean = true;
  public savingApplication: boolean | number = false;
  public forwardingApplication: boolean | number = false;
  public ddsDialogOpened: boolean = false;
  public saveDialogOpened: boolean = false;
  public afterSubmitStep: number = 0;
  public savingApplicationText: string = '';
  public user: User;
  public application: ApplicationMaster;
  public applicationType: string = 'New Application';
  public filesList: any[] = [];
  public ddsFilesList: any[] = [];
  public searchingFiles: boolean = false;
  public searchingHfs: boolean = false;
  public searchingHfs2: boolean = false;
  public searchingDDSFiles: boolean = false;
  public isPendancy: boolean = false;
  public searchEvent = new Subject<any>();
  public searchSubcription: Subscription = null;
  public formEvent = new Subject<boolean>();
  public dds: any = {};
  public profile: any = {};
  public fromApplicationTypes: any[] = [
    { Id: 6, Name: 'General Application' },
    { Id: 7, Name: 'Chief Minister Office' },
    { Id: 8, Name: 'Prime Minister Office' },
    { Id: 9, Name: 'NAB' },
    { Id: 10, Name: 'High Court' },
    { Id: 11, Name: 'Supreme Court' }
  ];
  public appsRealtime: any[] = [];
  public divisions: any[] = [];
  public districts: any[] = [];
  public tehsils: any[] = [];
  public healthFacilities: any[] = [];
  public hfsList: any[] = [];
  public hfsList2: any[] = [];
  public loadingHealthFacilities = false;
  public addingFile = false;
  public loadingDocs = false;
  public sectionOfficers: any[] = [];
  public sectionOfficersData: any[] = [];
  public applicationTypes: any[] = [];
  public applicationAttachments: ApplicationAttachment[] = [];
  public applicationDocuments: any[] = [];
  public signedApplication: ApplicationAttachment;
  public canPrint = false;
  public maxDate = new Date(2000, 1, 1);
  public nowDate = new Date();
  public dateNow: string = '';
  public dropDowns: DropDownsHR = new DropDownsHR();
  public selectedFiltersModel: any = {};
  public cnicMask: string = "00000-0000000-0";
  public mobileMask: string = "0000-0000000";
  public previewApplication: number = 2;
  private subscription: Subscription;
  private formEventSubscription: Subscription;
  public toStatusId: number = 4;
  public toStatusName: string = 'Disposed';
  public isRecieved = false;
  constructor(private sanitizer: DomSanitizer,
    private _rootService: RootService,
    private _applicationFtsService: ApplicationFtsService,
    private route: ActivatedRoute,
    private router: Router,
    public _notificationService: NotificationService,
    private _livePreviewService: LivePreviewService,
    private _authenticationService: AuthenticationService,
    private _firebaseHisduService: FirebaseHisduService) { }

  ngOnInit() {
    this.user = this._authenticationService.getUser();
    this.selectedFiltersModel = this.dropDowns.selectedFiltersModel;
    this.initializeProps();
    this.fetchParams();
  }
  private initializeProps() {
    let today = new Date();
    let dd: any = today.getDate(), mm: any = today.getMonth() + 1, yyyy = today.getFullYear();
    if (dd < 10) dd = '0' + dd; if (mm < 10) mm = '0' + mm;
    this.dateNow = dd + '/' + mm + '/' + yyyy;
    /* setInterval(() => {
      this.updateLivePreview(true);
    }, 1000); */
    this.handleSearchEvents();
  }

  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('id')) {
          let typeId = +params['id'];
          this.application = new ApplicationMaster(typeId);
          this.getAppTypePendancy(typeId);
          this.subscribeToForm();
          this.loading = false;
          this.fetchData();
        } else {
          this.loading = false;
        }
      }
    );
  }
  private fetchData() {
    this._rootService.getApplicationTypes().subscribe((data: any) => {
      this.applicationTypes = data;
      this.applicationType = this.applicationTypes.find(x => x.Id == this.application.ApplicationType_Id).Name;
      this.loadDropdownValues();
    });
  }
  private subscribeToForm() {
    this.formChangesSubscription = this.ngForm.form.valueChanges.pipe(
      debounceTime(800)).subscribe(x => {
        this.updateLivePreview(true);

      });
    this.formEventSubscription = this.formEvent.pipe(
      debounceTime(800)).subscribe((event: boolean) => {
        this.updateLivePreview(event as boolean);
      });
  }
  public async updateLivePreview(go: boolean) {
    let metaInfo: any = {
      uid: this.user.Id,
      go: go,
      TokenNumber: this.application.TokenNumber ? this.application.TokenNumber : '',
      applicationType: this.applicationType,
      dateNow: this.dateNow,
      applicationAttachments: this.applicationAttachments ? this.applicationAttachments : [],
    };
    /*  try {
       let x = await this._firebaseHisduService.updateApplicationPreview(JSON.parse(JSON.stringify(this.application)), metaInfo);
 
     } catch (error) {
       console.log(error);
 
     } */
    /* if (go) {
   
      let elem = document.getElementById('applicationPrint');
       let html = elem ? elem.innerHTML : '';
       this._livePreviewService.update(html ? html : '');
       this._livePreviewService.updateTokenNumber(this.application.TokenNumber); 
        }
        else {
                this._livePreviewService.update('');
                this._livePreviewService.updateTokenNumber('');
        } */
  }
  public handleSearchEvents() {
    this.searchSubcription = this.searchEvent.pipe(
      debounceTime(400)).subscribe((x: any) => {
        console.log(x);
        this.search(x.event, x.filter);
      });
  }

  private loadDropdownValues = () => {
    this.getDivisions('0');
    this.getDistricts('0');
    this.getTehsils('0');
    this.getLeaveTypes();
    this.getDesignations();
    this.getAll('0');
    this.getDepartments();
    if (this.user.UserName.startsWith('pl')) {
      this.getPandSOfficers('fts');
    } if (this.user.RoleName == 'Facilitation Centre South') {
      this.getPandSOfficers('south');
    } else {
      this.getPandSOfficers('section');
    }
    this.getApplicationDocuments(this.application.ApplicationType_Id);
    this.getEmploymentModes();
    this.getProfileStatuses();
  }
  private getAll = (code: string) => {
    if (code.length <= 1) {
      this._rootService.getDivisions(code).subscribe((res: any) => {
        this.divisions = res;
      },
        err => { this.handleError(err); }
      );
    }
    if (code.length <= 3) {
      this.resetDropsBelow('division');
      this._rootService.getDistricts(code).subscribe((res: any) => {
        this.districts = res;
      },
        err => { this.handleError(err); }
      );
    }
    if (code.length <= 6) {
      this.resetDropsBelow('district');
      this._rootService.getTehsils(code).subscribe((res: any) => {
        this.tehsils = res;
      },
        err => { this.handleError(err); }
      );
    }
  }
  private getAppTypePendancy = (typeId: number) => {
    this.dropDowns.appTypePendancy = [];
    this.toStatusId = 11;
    this.toStatusName = 'Marked';
    this.isRecieved = false;
    /* this._rootService.getAppTypePendancy(typeId).subscribe((res: any) => {
      if (res) {
        this.isPendancy = res.IsPendancy;
      } else {
        this.isPendancy = false;
      }
      if (this.isPendancy) {
        this.toStatusId = 10;
        this.toStatusName = 'No Process Initiated';
        this.isRecieved = true;
      } else {
        this.toStatusId = 5;
        this.toStatusName = 'NFA/FIC';
        this.isRecieved = true;
      }
    },
      err => { this.handleError(err); }
    ); */
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
  private getHealthFacilities = (hfmisCode: string, profileHfmisCode?: string) => {
    this._rootService.getHealthFacilities(hfmisCode).subscribe((res: any) => {
      this.dropDowns.healthFacilities = res;
      this.dropDowns.healthFacilitiesData = this.dropDowns.healthFacilities.slice();
      if (profileHfmisCode) { this.setProfileDefaultValues(profileHfmisCode); }
    },
      err => { this.handleError(err); }
    );
  }
  private getHealthFacilitiesForTransfer = (hfmisCode) => {
    this.loadingHealthFacilities = true;
    this._rootService.getHealthFacilities(hfmisCode, this.application.ToDept_Id).subscribe((res: any) => {
      this.healthFacilities = res;
      this.loadingHealthFacilities = false;
    },
      err => { this.handleError(err); }
    );
  }
  private getPandSOfficers = (type: string) => {
    this.sectionOfficers = [];
    this._rootService.getPandSOfficers(type).subscribe((res: any) => {
      this.sectionOfficers = res;
      this.sectionOfficersData = this.sectionOfficers;
    },
      err => { this.handleError(err); }
    );
  }
  public getApplicationDocuments = (applicationTypeId: number) => {
    this.loadingDocs = true;
    this.applicationDocuments = [];
    this._rootService.getApplicationDocuments(applicationTypeId).subscribe((res: any) => {
      this.applicationDocuments = res;
      this.loadingDocs = false;
    },
      err => { this.handleError(err); }
    );
  }
  private getDesignations = () => {
    this._rootService.getDesignations().subscribe((res: any) => {
      this.dropDowns.designations = res;
      this.dropDowns.designationsData = this.dropDowns.designations.slice();
    },
      err => { this.handleError(err); }
    );
  }
  private getDepartments = () => {
    this._rootService.getDepartmentsHealth().subscribe((res: any) => {
      this.dropDowns.departments = res;
      this.dropDowns.departmentsData = this.dropDowns.departments.slice();
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
  private getProfileStatuses = () => {
    this.dropDowns.statusData = [];
    this._rootService.getProfileStatuses().subscribe((res: any) => {
      this.dropDowns.statusData = res;
    },
      err => { this.handleError(err); }
    );
  }
  private getLeaveTypes = () => {
    this._rootService.getLeaveTypes().subscribe((res: any) => {
      this.dropDowns.leaveTypes = res;
      this.dropDowns.leaveTypesData = this.dropDowns.leaveTypes.slice();
    },
      err => { this.handleError(err); }
    );
  }
  public dropdownValueChanged = (value, filter) => {
    this.formEvent.next(true);
    if (filter == 'division') {
      this.application.HfmisCode = value.Code;
      this.resetDrops(filter);
      this.getDistricts(value.Code);
      this.getTehsils(value.Code);
    }
    if (filter == 'district') {
      this.application.HfmisCode = value.Code;
      this.resetDrops(filter);
      this.getTehsils(value.Code);
    }
    if (filter == 'tehsil') {
      this.resetDrops(filter);
      this.application.HfmisCode = value.Code;
      this.getHealthFacilities(this.application.HfmisCode);
    }
    if (filter == 'healthFacility') {
      let item = this.hfsList.find(x => x.FullName === value);
      if (item) {
        this.application.HfmisCode = item.HFMISCode;
        this.application.HealthFacility_Id = item.Id;
        this.application.FromHF_Id = item.Id;
        this.application.fromHealthFacility = item.FullName;
        let divisions = this.dropDowns.divisions as any;
        let division = divisions.find(x => x.Code == this.application.HfmisCode.substring(0, 3));
        if (division) {
          this.dropDowns.selectedFiltersModel.division = { Code: division.Code, Name: division.Name };
        }

        let districts = this.dropDowns.districts as any;
        let district = districts.find(x => x.Code == this.application.HfmisCode.substring(0, 6));
        if (district) {
          this.dropDowns.selectedFiltersModel.district = { Code: district.Code, Name: district.Name };
        }

        let tehsils = this.dropDowns.tehsils as any;
        let tehsil = tehsils.find(x => x.Code == this.application.HfmisCode.substring(0, 9));
        if (tehsil) {
          this.dropDowns.selectedFiltersModel.tehsil = { Code: tehsil.Code, Name: tehsil.Name };
        }
      }
    }
    if (filter == 'employementMode') {
      this.application.EmpMode_Id = value.Id;
      this.application.EmpModeName = value.Name;
    }
    if (filter == 'fromType') {
      this.application.Purpose_Id = value.Id;
    }
    if (filter == 'pldistrict') {
      debugger;
      if (this.dropDowns.districts.length > 0) {
        let dists = this.dropDowns.districts as any[];
        let dist = dists.find(x => x.Code == value) as any;
        if (dist) {
          this.application.ApplicationPersonAppeared.DistrictName = dist.Name;
        }
      }
    }
    if (filter == 'profileStatus') {
      this.application.EmpStatus_Id = value.Id;
      this.application.EmpStatusName = value.Name;
    }
    if (filter == 'dd') {
      this.application.ToHFCode = value.Code;
      this.application.ToHF_Id = null;
      this.getAll(this.application.ToHFCode);
    }
    if (filter == 't') {
      this.application.ToHFCode = value.Code;
      this.application.ToHF_Id = null;
      this.resetDropsBelow('tehsil');
      this.getHealthFacilitiesForTransfer(value.Code);
    }

    if (filter == 'healthFacility2') {
      let item = this.hfsList2.find(x => x.FullName === value);
      if (item) {
        this.application.ToHFCode = item.HFMISCode;
        this.application.ToHF_Id = item.Id;
        this.application.toHealthFacility = item.FullName;

        let divisions = this.dropDowns.divisions as any;
        let division = divisions.find(x => x.Code == this.application.ToHFCode.substring(0, 3));
        if (division) {
          this.dropDowns.selectedFiltersModel.divisionForTransfer = { Code: division.Code, Name: division.Name };
        }

        let districts = this.dropDowns.districts as any;
        let district = districts.find(x => x.Code == this.application.ToHFCode.substring(0, 6));
        if (district) {
          this.dropDowns.selectedFiltersModel.districtForTransfer = { Code: district.Code, Name: district.Name };
        }

        let tehsils = this.dropDowns.tehsils as any;
        let tehsil = tehsils.find(x => x.Code == this.application.ToHFCode.substring(0, 9));
        if (tehsil) {
          this.dropDowns.selectedFiltersModel.tehsilForTransfer = { Code: tehsil.Code, Name: tehsil.Name };
        }
      }

    }
    if (filter == 'sectionOfficer') {
      this.application.ForwardingOfficer_Id = value.Id;
      this.application.ForwardingOfficerName = value.DesignationName;
    }

    if (filter == 'leaveType') {
      this.application.LeaveType_Id = value.Id;
      this.application.leaveType = value.LeaveType1;
    }
    if (filter == 'retirementType') {
      this.application.RetirementType_Id = value.Id;
      this.application.retirementTypeName = value.Name;
    }
    if (filter == 'designation') {
      this.application.Designation_Id = value.Id;
      this.application.designationName = value.Name;
    }
    if (filter == 'department') {
      this.application.Department_Id = value.Id;
      this.application.DepartmentName = value.Name;
    }
    if (filter == 'toDepartment') {
      this.application.ToDept_Id = value.Id;
      this.application.toDepartmentName = value.Name;
    }
    if (filter == 'toDesignation') {
      this.application.ToDesignation_Id = value.Id;
      this.application.toDesignationName = value.Name;
    }
  }
  private resetDrops = (filter: string) => {
    this.dropDowns.selectedFiltersModel.healthFacility = { Name: 'Select Health Facility', Id: 0 };
    this.application.fromHealthFacility = '';
    if (filter == 'division') {
      this.dropDowns.selectedFiltersModel.district = { Name: 'Select District', Code: '0' };
      this.dropDowns.selectedFiltersModel.tehsil = { Name: 'Select Tehsil', Code: '0' };
    }
    if (filter == 'district') {
      this.application.fromHealthFacility = '';
      this.dropDowns.selectedFiltersModel.tehsil = { Name: 'Select Tehsil', Code: '0' };
    }
  }
  private resetDropsBelow = (filter: string) => {
    this.application.toHealthFacility = '';
    this.dropDowns.selectedFiltersModel.healthFacilityForTransfer = { Name: 'Select Health Facility', Id: 0 };
    if (filter == 'division') {
      this.dropDowns.selectedFiltersModel.districtForTransfer = { Name: 'Select District', Code: '0' };
      this.dropDowns.selectedFiltersModel.tehsilForTransfer = { Name: 'Select Tehsil', Code: '0' };
    }
    if (filter == 'district') {
      this.dropDowns.selectedFiltersModel.tehsilForTransfer = { Name: 'Select Tehsil', Code: '0' };
    }
  }
  public search(value: string, filter: string) {

    if (filter == 'ddsfiles') {
      this.ddsFilesList = [];
      if (value.length > 2) {
        this.searchingDDSFiles = true;
        this._rootService.getDDSFilesByFileNumber(value).subscribe((data) => {
          this.ddsFilesList = data as any[];
          this.searchingDDSFiles = false;
        });
      }
    }
    if (filter == 'hfs') {
      this.hfsList = [];
      if (value.length > 2) {
        this.searchingHfs = true;
        this._applicationFtsService.searchHealthFacilities(value, this.application.HfmisCode).subscribe((data) => {
          this.hfsList = data as any[];
          this.searchingHfs = false;
        });
      }
    }
    if (filter == 'hfs2') {
      this.hfsList2 = [];
      if (value.length > 2) {
        this.searchingHfs2 = true;
        this._applicationFtsService.searchHealthFacilities(value, this.application.ToHFCode).subscribe((data) => {
          this.hfsList2 = data as any[];
          this.searchingHfs2 = false;
        });
      }
    }
  }
  public searchClicked(FileNumber, filter) {
    if (filter == 'files') {
      let item = this.filesList.find(x => x.FileNumber === FileNumber);
      if (item) {
        //for Id //this.application.DDS_Id = item.Id;
        this.application.FileNumber = item.FileNumber;
      }
    }
    if (filter == 'ddsfiles') {
      let item = this.ddsFilesList.find(x => x.DiaryNo === FileNumber);
      if (item) {
        this.application.DDS_Id = item.Id;
        this.application.FileNumber = item.DiaryNo;
      }
    }
  }
  public searchFileByCNIC() {
    this._rootService.getDDSFilesByCNIC(this.application.CNIC).subscribe((data: any[]) => {
      let dssFiles = data;
      let dssFileFirst = data[0];
      console.log(dssFiles);
      console.log(dssFileFirst);

      if (dssFileFirst) {
        if (this.application.ApplicationType_Id == 13) {
          let acrFile = dssFiles.find(dds => dds.FileType_Id == 1);
          if (acrFile) {
            this.dds.Id = dssFileFirst.Id;
            this.application.DDS_Id = dssFileFirst.Id;
            this.application.FileNumber = data[0].DiaryNo;
            this.formEvent.next(true);
          } else {
            this.mapProfileToDDS(this.profile);
          }
        } else {
          this.application.DDS_Id = dssFileFirst.Id;
          this.application.FileNumber = dssFileFirst.DiaryNo;
          this.formEvent.next(true);
        }
      }
    });
  }
  public searchProfile = () => {
    if (!this.application.CNIC) {
      this.mapProfileToApplicant(null);
      return;
    }
    this._applicationFtsService.searchProfile(this.application.CNIC).subscribe((data: any) => {
      if (data == 'Invalid') {
        this.mapProfileToApplicant(null);
      }
      if (data) {
        this.mapProfileToApplicant(data);
      }
    });
  }

  public mapProfileToApplicant = (profile: ApplicationProfileViewModel) => {
    if (profile) {
      this.application.Profile_Id = profile.Profile_Id ? profile.Profile_Id : null;
      this.application.EmployeeName = profile.EmployeeName ? profile.EmployeeName : '';
      this.application.FatherName = profile.FatherName ? profile.FatherName : '';
      this.application.DateOfBirth = profile.DateOfBirth ? new Date(profile.DateOfBirth) : new Date(2000, 1, 1);
      this.application.Gender = profile.Gender ? profile.Gender : 'Select Gender';
      this.application.MobileNo = profile.MobileNo ? profile.MobileNo : '';
      this.application.EMaiL = profile.EMaiL ? profile.EMaiL : '';
      this.application.FileNumber = '';
      this.application.DispatchFrom = profile.Address ? profile.Address : '';
      this.application.JoiningGradeBPS = profile.JoiningGradeBPS ? profile.JoiningGradeBPS : 0;
      this.application.CurrentGradeBPS = profile.CurrentGradeBPS ? profile.CurrentGradeBPS : 0;


      this.application.Department_Id = profile.Department_Id ? profile.Department_Id : this.application.Department_Id;
      this.application.DepartmentName = profile.Department_Id == 28 ? 'Specialized Healthcare & Medical Education' : 'Primary & Secondary Healthcare Department';
      this.dropDowns.selectedFiltersModel.departmentDefault = profile.Department_Id == 28 ? { Name: 'Specialized Healthcare & Medical Education', Id: 28 } : profile.Department_Id == 25 ? { Name: 'Primary & Secondary Healthcare Department', Id: 25 } : this.dropDowns.selectedFiltersModel.department;



      let empModes = this.dropDowns.employementModesData as any[];
      if (empModes) {
        let empMode = empModes.find(x => x.Id == profile.EmpMode_Id);
        if (empMode) {
          this.application.EmpMode_Id = empMode.Id;
          this.application.EmpModeName = empMode.Name;
          this.dropDowns.selectedFiltersModel.employementMode = { Name: empMode.Name, Id: empMode.Id };
        }
      }
      let empStatuses = this.dropDowns.statusData as any[];
      if (empStatuses) {
        let empStatuse = empStatuses.find(x => x.Id == profile.EmpStatus_Id);
        if (empStatuse) {
          this.application.EmpStatus_Id = empStatuse.Id;
          this.application.EmpStatusName = empStatuse.Name;
          this.dropDowns.selectedFiltersModel.profileStatus = { Name: empStatuse.Name, Id: empStatuse.Id };
        }
      }

      let designations = this.dropDowns.designations as any[];
      if (designations) {
        let designation = designations.find(x => x.Id == profile.Designation_Id);
        if (designation) {
          this.application.Designation_Id = profile.Designation_Id ? profile.Designation_Id : this.application.Designation_Id;
          this.application.designationName = designation.Name;
          this.dropDowns.selectedFiltersModel.designation = { Name: designation.Name, Id: designation.Id };
          if (this.application.ApplicationType_Id == 2) {
            this.application.ToDesignation_Id = designation.Id;
            this.application.toDesignationName = designation.Name;
            this.dropDowns.selectedFiltersModel.designationForTransfer = this.dropDowns.selectedFiltersModel.designation;
          }
        }
      }

      if (profile.HealthFacility_Id && profile.HfmisCode.length == 19) {
        this.getDivisions('0');
        this.getDistricts('0');
        this.getTehsils('0');
        this.getHealthFacilities(profile.HfmisCode.substring(0, 9), profile.HfmisCode);
      }
      if (this.application.ApplicationType_Id == 3) {
        this.application.CurrentScale = profile.CurrentGradeBPS ? profile.CurrentGradeBPS : 0;
        this.application.SeniorityNumber = profile.SeniorityNo ? profile.SeniorityNo : '';
      }
      this.profile = profile;
    } else {
      this.application.EmployeeName = '';
      this.application.FatherName = '';
      this.application.DateOfBirth = new Date(2000, 1, 1);
      this.application.Gender = 'Select Gender';
      this.application.MobileNo = '';
      this.application.EMaiL = '';
      this.application.FileNumber = '';
      this.application.DispatchFrom = '';
      this.application.Department_Id = 25;
      this.application.DepartmentName = 'Primary & Secondary Healthcare Department';
      this.application.Designation_Id = 0;
      this.application.designationName = '';
      this.dropDowns.selectedFiltersModel.designation = { Name: 'Select Designation', Id: 0 };
      this.dropDowns.selectedFiltersModel.healthFacility = { Name: 'Select Health Facility', Id: 0 };
      this.application.fromHealthFacility = '';
      this.application.HfmisCode = '';
      this.application.HealthFacility_Id = 0;
      this.application.fromHealthFacility = '';
    }
    this.searchFileByCNIC();
    this.formEvent.next(true);
  }
  public mapProfileToDDS = (profile: ApplicationProfileViewModel) => {
    if (profile) {
      this.dds.F_Name = profile.EmployeeName ? profile.EmployeeName : '';
      this.dds.Subject = profile.EmployeeName ? profile.EmployeeName : '';
      this.dds.F_FileNumber = '';
      this.dds.DiaryNo = '';
      this.dds.F_DateOfBirth = profile.DateOfBirth ? new Date(profile.DateOfBirth) : new Date(2000, 1, 1);
      this.dds.F_Designation_Id = profile.Designation_Id;
      this.dds.F_CNIC = profile.CNIC ? profile.CNIC : '';
      this.dds.FileType = profile.CNIC ? profile.CNIC : '';
      this.dds.F_BPS = profile.CurrentGradeBPS ? profile.CurrentGradeBPS : 0;
      this.dds.F_DateOfJoining = profile.JoiningDate ? profile.JoiningDate : 0;
      this.dds.FileType_Id = 1;
      this.dds.F_FileType_Id = 1;
      console.log(this.dds);

    }
  }
  public submitFile() {
    this.mapProfileToDDS(this.profile);
    this.addingFile = true;
    /*  if (this.dds.F_DateOfBirth) {
       this.dds.F_DateOfBirth = this.dds.F_DateOfBirth.toDateString();
     }
     if (this.dds.F_DateOfJoining) {
       this.dds.F_DateOfJoining = this.dds.F_DateOfJoining.toDateString();
     } */
    this._applicationFtsService.addFileACR(this.dds).subscribe((res: any) => {
      if (res && res.barCode && res.ddsFile.Id) {
        this.dds = res.ddsFile as DDS_Files;
        this._notificationService.notify('success', 'File Added Successfully');
      }
      this.addingFile = false;
    }, err => {
      this.handleError(err);
    })
  }
  public setProfileDefaultValues = (profileHfmisCode) => {
    if (profileHfmisCode) {
      let divisions = this.dropDowns.divisions as any;
      let division = divisions.find(x => x.Code == profileHfmisCode.substring(0, 3));
      if (division) {
        this.dropDowns.selectedFiltersModel.division = { Code: division.Code, Name: division.Name };
      }

      let districts = this.dropDowns.districts as any;
      let district = districts.find(x => x.Code == profileHfmisCode.substring(0, 6));
      if (district) {
        this.dropDowns.selectedFiltersModel.district = { Code: district.Code, Name: district.Name };
      }

      let tehsils = this.dropDowns.tehsils as any;
      let tehsil = tehsils.find(x => x.Code == profileHfmisCode.substring(0, 9));
      if (tehsil) {
        this.dropDowns.selectedFiltersModel.tehsil = { Code: tehsil.Code, Name: tehsil.Name };
      }
      let healthFacilities = this.dropDowns.healthFacilities as any;
      let healthFacility = healthFacilities.find(x => x.HfmisCode == profileHfmisCode);
      if (healthFacility) {
        this.dropDowns.selectedFiltersModel.healthFacility = { Id: healthFacility.Id, HfmisCode: healthFacility.HfmisCode, Name: healthFacility.Name };
        this.application.HfmisCode = healthFacility.HfmisCode;
        this.application.HealthFacility_Id = healthFacility.Id;
        this.application.FromHF_Id = healthFacility.Id;
        this.application.fromHealthFacility = healthFacility.Name;
      }
    }
    this.formEvent.next(true);
  }
  public selectFile(event, document: ApplicationDocument): void {
    let inputValue = event.target;
    let applicationAttachment: any = {};
    applicationAttachment.Document_Id = document.Id;
    applicationAttachment.documentName = document.Name;
    applicationAttachment.files = inputValue.files;
    this.applicationAttachments.push(applicationAttachment);
    let doc = this.applicationDocuments.find(x => x.Id == document.Id);
    if (doc) {
      doc.attached = true;
      doc.error = false;
    }

    this.formEvent.next(true);
  }
  // public unSelectFile(event, document: ApplicationDocument): void {
  //   let inputValue = event.target;
  //   let applicationAttachment: ApplicationAttachment = new ApplicationAttachment();
  //   applicationAttachment.Document_Id = document.Id;
  //   applicationAttachment.documentName = document.Name;
  //   applicationAttachment.files = inputValue.files;
  //   this.applicationAttachments.push(applicationAttachment);
  //   this.applicationAttachments.splice(applicationAttachment);
  //   this.applicationDocuments.find(x => x.Id == document.Id).attached = true;
  //   this.formEvent.next(true);
  // }
  public checkDocumentsAttached() {
    if (this.applicationDocuments.length == 0) {
      return true;
    } else if (this.applicationDocuments.length > 0) {
      let allDocsAttached: boolean = false;
      this.applicationDocuments.forEach((d: any) => {
        if (d && d.attached == true) {
          allDocsAttached = true;
        } else {
          allDocsAttached = false;
          d.attached = false;
          d.error = true;
        }
      });
    }
  }
  onSubmit() {
    let allDocsAttached: boolean = true;
    let docsCount: number = 0;
    if (this.applicationDocuments.length == 0) {
      allDocsAttached = true;
    } else if (this.applicationDocuments.length > 0) {
      let reqiredDocLength: number = 0;
      let requiredDocs = this.applicationDocuments.filter(x => x.IsRequired == true);
      if (requiredDocs && requiredDocs.length > 0) {
        reqiredDocLength = requiredDocs.length;
      }
      requiredDocs.forEach((d: any) => {
        if (d && d.attached == true) {
          docsCount = (docsCount + 1);
        } else {
          allDocsAttached = false;
          d.attached = false;
          d.error = true;
        }
      });
      console.log(docsCount);
      console.log(reqiredDocLength);
      if (docsCount == reqiredDocLength) {
        allDocsAttached = true;
      }
    }
    if (allDocsAttached == true) {
      if (this.user.UserName.startsWith('pl') || this.user.UserName.startsWith('fdo')) {
        this.savingApplication = true;
        let html = document.getElementById('applicationPrint').innerHTML;
        if (html) {
          this.application.RawText = html;
        }
        this._applicationFtsService.submitApplication(this.application).subscribe((response: any) => {
          if (response.application) {
            this.application.Id = response.application.Id;
            if (response.barCode) {
              this.application.TrackingNumber = response.application.TrackingNumber;
              let barcode = response.barCode;
              this.application.barcode = barcode;
              if (this.applicationAttachments.length > 0) {
                this._applicationFtsService.uploadApplicationAttachments(this.applicationAttachments, this.application.Id).subscribe((res) => {
                  if (res) {
                    //Proocess Further
                    this.signedApplication = new ApplicationAttachment();
                    this.signedApplication.Document_Id = 1;
                    window.scroll(0, 0);
                    this.canPrint = true;
                    //show print button
                    this.afterSubmitStep = 1;
                  }
                }, err => {
                  this.handleError(err);
                });
              } else {
                //Proocess Further
                this.signedApplication = new ApplicationAttachment();
                this.signedApplication.Document_Id = 1;
                window.scroll(0, 0);
                this.canPrint = true;
                //show print button
                this.afterSubmitStep = 1;
              }
            }
          }
          this.savingApplication = false;
        },
          err => { this.handleError(err); }
        );
      }
    }
    else {
      this.savingApplication = false;
      alert('Documents Required Error!');
    }
  }
  public handleFilter = (value, filter) => {
    if (filter == 'officer') {
      this.sectionOfficersData = this.sectionOfficers.filter((s: any) => s.DesignationName.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }

  }
  public uploadAttachments() {
    if (this.applicationAttachments.length > 0) {
      this._applicationFtsService.uploadApplicationAttachments(this.applicationAttachments, this.application.Id).subscribe((response) => {
        console.log(response);
      }, err => {
        this.handleError(err);
      });
    }
  }
  uploadSignedCopy(event) {
    let inputValue = event.target;
    this.signedApplication.files = inputValue.files;
    this.signedApplication.attached = true;
    //show send button
    this.afterSubmitStep = 3;
  }
  forwardApplication() {
    // upload signed copy
    this.forwardingApplication = true;
    if (this.application.ApplicationType_Id == 14) {
      let applicationLog: ApplicationLog = new ApplicationLog();
      applicationLog.Application_Id = this.application.Id;
      applicationLog.ToOfficer_Id = this.application.ForwardingOfficer_Id;
      // Marked to First officer
      applicationLog.ToStatus_Id = this.toStatusId;
      applicationLog.ToStatus = this.toStatusName;
      applicationLog.IsReceived = false;
      this._applicationFtsService.createApplicationLog(applicationLog).subscribe((response: any) => {
        if (response.Id) {
          this.forwardingApplication = false;
          this.openWindow();
        }
      },
        err => { this.handleError(err); }
      );
    } else {
      this._applicationFtsService.uploadSignedApplication(this.signedApplication, this.application.Id).subscribe((response) => {
        if (response) {
          let applicationLog: ApplicationLog = new ApplicationLog();
          applicationLog.Application_Id = this.application.Id;
          applicationLog.ToOfficer_Id = this.application.ForwardingOfficer_Id;
          // Marked to First officer
          applicationLog.ToStatus_Id = this.toStatusId;
          applicationLog.ToStatus = this.toStatusName;
          applicationLog.IsReceived = false;
          this._applicationFtsService.createApplicationLog(applicationLog).subscribe((response: any) => {
            if (response.Id) {
              this.forwardingApplication = false;
              this.openWindow();
            }
          },
            err => { this.handleError(err); }
          );
        }
      }, err => {
        this.handleError(err);
      });
    }

  }
  public leaveInputsChanged(type: number) {
    if (type == 1) {
      if (this.application.FromDate && this.application.ToDate) {
        this._rootService.calcDate(this.application.FromDate.toDateString(), this.application.ToDate.toDateString(), 0).subscribe((x: number) => {
          this.application.TotalDays = x;
        });
      }
    } else if (type == 2) {
      this._rootService.calcDate(this.application.FromDate.toDateString(), 'noDate', this.application.TotalDays).subscribe((x: any) => {
        this.application.ToDate = new Date(x);
      });
    }
  }
  private handleError(err: any) {
    this.savingApplication = false;
    this.canPrint = false;
    this.forwardingApplication = false;
    this.loading = false;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }
  public barcodeSrc() {
    return this.sanitizer.bypassSecurityTrustUrl(this.application.barcode);
  }
  public setPreview() {
    console.log(this.previewApplication);
    this.previewApplication = this.previewApplication + 1;
    if (this.previewApplication == 3) this.previewApplication = 0;
    console.log(this.previewApplication);

  }
  public closeWindow() {
    this.saveDialogOpened = false;
    this.router.navigate(['/application']);
  }

  public openWindow() {
    this.saveDialogOpened = true;
  }
  public closeDDsWindow() {
    this.ddsDialogOpened = false;
  }

  public openDDsWindow() {
    this.ddsDialogOpened = true;
  }
  public capitalize(val: string) {
    if (!val) return '';
    if (!val.toLowerCase().endsWith('application')) val += ' Application';
    return val.toUpperCase();
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
    this.updateLivePreview(false);
    this.subscription.unsubscribe();
    this.formChangesSubscription.unsubscribe();
    this.formEventSubscription.unsubscribe();
    this.searchSubcription.unsubscribe();
  }
  printApplication() {
    let html = document.getElementById('applicationPrint').innerHTML;
    var mywindow = window.open('', 'PRINT', 'height=600,width=900');
    if (mywindow) {
      mywindow.document.write(`<html><head>
        <style>
            body {
              font-family: -apple-system, BlinkMacSystemFont, 
            'Segoe UI', 'Roboto' , 'Oxygen' , 'Ubuntu' , 'Cantarell' , 'Fira Sans' , 'Droid Sans' , 'Helvetica Neue' ,
              sans-serif !important;
            }
          p {
            margin-top: 0;
            margin-bottom: 1rem !important;
        }.mt-2 {
          margin-top: 0.5rem !important;
        }.mb-0 {
          margin-bottom: 0 !important;
        }
        .ml-1 {
          margin-left: 0.25rem !important;
        }
        .mb-2 {
          margin-bottom: 0.5rem !important;
        }
        .application-page {
    
          padding: 50px;
        }
        button.print {
          padding: 10px 40px;
          font-size: 21px;
          position: absolute;
          margin-left: 40%;
          background: #46a23f;
          background-image: linear-gradient(rgba(63, 162, 79, 0), rgba(63, 162, 79, 0.2));
          cursor: pointer;
          border: none;
          color: #ffffff;
          z-index: 9999;
        }
        .watermark-hisdu {
          text-align: center;
          position: absolute;
          left: 0;
          width: 100%;
          opacity: 0.25;
        }

        .watermark-hisdu img {
          display: inline-block;
        }

        table.header-pshealth,
        .applicant-information,
        .application-type-detail-preview,
        .attached-document,
        .remarks-preview,
        .info-application-preview,
        table.pshealth {
          border-color: transparent !important;
          width: 100%;
        }

        table.header-pshealth td {
          border-color: transparent !important;
        }

        table.header-pshealth td.gop-logo-a4-header {
          text-align: left;
        }

        table.header-pshealth td.gop-logo-a4-header img {
          display: inline-block;
          width: 134px;
        }

        table.header-pshealth td.pshealth-right-a4-td-header {
          text-align: right;
        }

        table.header-pshealth td.pshealth-right-a4-td-header .pshealth-right-a4-text-header {
          display: inline-block;
          text-align: center;
        }

        table.header-pshealth td.app-type-preview {
          text-align: left;
          width: 100%;
        }

        /* Applicant Information */

        table.applicant-information {
          border-color: transparent !important;
          width: 100%;
        }

        table.applicant-information td.applicant-info-heading,
        table.application-type-detail-preview td.application-type-detail-preview-heading,
        table.remarks-preview td.remarks-heading,
        table.info-application-preview td.info-application-preview-heading,
        table.attached-document td.attached-document-heading {
          text-align: center;
          border: 1px solid black;
        }

        table.applicant-information td.applicant-info-detail-1 {
          width: 20% !important;
        }

        table.applicant-information td.applicant-info-detail-2 {
          width: 40% !important;
        }

        table.applicant-information td.applicant-info-detail-3 {
          width: 10% !important;
        }

        table.applicant-information td.applicant-info-detail-4 {
          width: 30% !important;
        }
        table.applicant-information td.applicant-info-detail-5 {
          width: 15% !important;
        }
        
        table.applicant-information td.applicant-info-detail-6 {
          width: 30% !important;
        }
        
        table.applicant-information td.applicant-info-detail-7 {
          width: 20% !important;
        }
        
        table.applicant-information td.applicant-info-detail-8 {
          width: 35% !important;
        }
        table.info-application-preview td.info-application-preview-left {
          border-left: 1px solid black;
        }

        table.info-application-preview td.info-application-preview-right {
          text-align: center;
          margin: 5px 5px !important;
          border-right: 1px solid black;
          border-left: 1px solid black;
        }

        table.application-route-detail {
          border-color: transparent !important;
          width: 100% !important;
          text-align: center;
        }

        table.application-route-detail td.application-route-detail-header {
          width: 50% !important;
          border: 1px solid black;
        }

        .w-20 { width: 20% !important; }
        .w-30 { width: 30% !important; }
        .w-50 { width: 50% !important; }
        .w-70 { width: 70% !important; }
        .w-80 { width: 80% !important; }

        .mt-10 { margin-top: 10px !important; }
        .mt-30 { margin-top: 30px  !important; }
        @media print {
          button.print {
            display: none;
          }
        }
              </style>
              <title>Application</title>`);
      mywindow.document.write('</head><body >');
      mywindow.document.write('<button onclick="printFunc()" class="print">Print</button>');
      mywindow.document.write(html);
      mywindow.document.write(`
                <script>
          function printFunc() {
            window.print();
          }
          </script>
      `);
      mywindow.document.write('</body></html>');
      //show upload signed copy input chooser
      if (this.application.ApplicationType_Id == 14 || this.application.ApplicationType_Id == 14) {
        this.afterSubmitStep = 3;

      } else {

        this.afterSubmitStep = 2;
      }

      /*     mywindow.document.close(); // necessary for IE >= 10
          mywindow.focus(); // necessary for IE >= 10
          mywindow.print();
          mywindow.close(); */
    }
  }
}
