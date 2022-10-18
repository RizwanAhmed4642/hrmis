import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ReportingRoutingModule } from '../../../modules/reporting/reporting-routing.module';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { Subject } from 'rxjs/Subject';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { RootService } from '../../../_services/root.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { CookieService } from '../../../_services/cookie.service';
import * as moment from 'moment';
import { LocalService } from '../../../_services/local.service';
import { OnlinePromotionApplyService } from '../online-promotion-apply.service';
import { ifError } from 'assert';
import { toJSON } from '@progress/kendo-angular-grid/dist/es2015/filtering/operators/filter-operator.base';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styles: [`
  .zoom {
    transition: transform .2s;
    border: 1px solid black;
  }
  
  .zoom:hover {
    transform: scale(1.08);
    box-shadow: 0px 0px 5px black !important;
  }`]
})
export class ProfileComponent implements OnInit, OnDestroy {
  public dropDowns: DropDownsHR = new DropDownsHR();
  public cnicMask: string = "00000-0000000-0";
  public applicant: any = {};
  public user: any = {};
  public validations: any = {
    cnicCopy: false,
    orderCopy: false,
    joiningReport: false,
    meritList: false,
    contractOrderCopy: false,
    contractJoiningReport: false,
    masterFlag: false
  };
  public urdu: any = {
    info: `	اپنے پہلے سے مہیا کردہ ریکارڈ کی تصحیح کرتے ہوئے مطلوبہ دستاویزات اپلوڈ کرنے کے لیے (PROCEED) کا بٹن دبائیں`,
    info1: `اپنے سروس ریکارڈ کا اندراج کیجئے اور مطلوبہ دستاویزات اپلوڈ کرنے کے بعد SAVE))کا بٹن دبائیں`,
    info2: `اپنے مہیا کردہ ریکارڈ اور دستاویزات کا جائزہ لیں اور & LOCK) (SUBMIT کا بٹن دبائیں اور 
      (SUBMIT & LOCK) کا بٹن دبانے کے بعد آپ ریکارڈ اور دستاویزات میں کوئی تبدیلی نہیں کر پائیں گے۔
      `,
    info3: `اپنے کوائف کا درست اندراج کیجئے اور مطلوبہ کاغذات اپلوڈ کرنے کے بعد سیو پروسیڈ کا بٹن دبائیں۔`,
    infoeng: 'Enter your data correctly and upload the required documents then press the Save button',

    infoeng0: 'Fill all the required information below and click on proceed button',
    infoeng1: 'Please provide your service record and upload all the supporting documents. After your application is saved, you need to lock your application as well.',
    infoeng2: 'Please review uploaded documents and Click on (SUBMIT & LOCK) button, You will not be able to change the record and documents once application is locked!',

  };
  public ageError: boolean = false;
  public proceeding: boolean = false;
  public savingProfile: boolean = false;
  public lockingProfile: boolean = false;

  public id: number = 0;
  public step: number = 1;
  public birthDateMax: Date = new Date();
  public birthDateMin: Date = new Date();
  public mobileMask: string = "0000-0000000";
  public selectedFiles: any[] = [];
  public subscription: any;
  constructor(private route: ActivatedRoute, private _rootService: RootService,
    private _cookieService: CookieService,
    private router: Router,
    private _localService: LocalService,
    private _authenticationService: AuthenticationService,
    private _onlinePromotionApplyService: OnlinePromotionApplyService
  ) { }


  ngOnInit() {
    this.selectedFiles = [];
    this.applicant.PromotionSection = false;
    this.applicant.CNIC = this._cookieService.getCookie('cnicussrpromotion');
    this.birthDateMax.setFullYear(this.birthDateMax.getFullYear() - 18);
    this.birthDateMin.setFullYear(this.birthDateMin.getFullYear() - 60);
    this.loadDropdowns();
    this.fetchParams();
  }
  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('id')) {
          this.id = +params['id'];
          this.getApplicant();
        }
      }
    );
  }
  private getApplicant() {
    this.step = 0;
    this._onlinePromotionApplyService.getApplicant(this.applicant.CNIC).subscribe((res: any) => {
      if (res) {
        res.DateOfBirth = res.DateOfBirth ? new Date(res.DateOfBirth) : null;
        res.ContractStartDate = res.ContractStartDate ? new Date(res.ContractStartDate) : null;
        res.ContractEndDate = res.ContractEndDate ? new Date(res.ContractEndDate) : null;
        res.LastPromotionDate = res.LastPromotionDate ? new Date(res.LastPromotionDate) : null;
        res.DateOfRegularization = res.DateOfRegularization ? new Date(res.DateOfRegularization) : null;
        res.FirstJoiningDate = res.FirstJoiningDate ? new Date(res.FirstJoiningDate) : null;
        res.ContractOrderDate = res.ContractOrderDate ? new Date(res.ContractOrderDate) : null;
        res.PromotionJoiningDate = res.PromotionJoiningDate ? new Date(res.PromotionJoiningDate) : null;

        res.PMDCRegDate = new Date(res.PMDCRegDate);
        res.PMDCValidUpto = new Date(res.PMDCValidUpto);
        if (res.ModeId == 2 || res.ModeId == 3) {
          res.entry = 101;
        }
        if (res.Id && res.Id > 0 && this.proceeding) {
          this.step = 3;
        } else {
          this.step = 1;
        }
        if (res.IsLocked) {
          this.step = 3;
        }
        this.applicant = res;

        if (this.applicant.ModeId != 0) {
          this.dropDowns.selectedFiltersModel.postingMode = { Name: this.applicant.ModeName, Id: this.applicant.ModeId };
        }

        if (this.applicant.DistrictC != 0 && this.applicant.DistrictC != null) {
          this.dropDowns.selectedFiltersModel.district = { Name: this.applicant.DistrictName, Code: this.applicant.DistrictC }
        }

        if (this.applicant.HealthFacility_Id && this.applicant.HealthFacility_Id > 0 && this.applicant.HealthFacility) {

          this.dropDowns.selectedFiltersModel.healthFacility = { Name: this.applicant.HealthFacility, Id: this.applicant.HealthFacility_Id }
          if (this.applicant.HfmisCode) {
            this.getHealthFacilities(this.applicant.HfmisCode.substring(0, 6));
          }
        }
        if (this.applicant.Designation_Id != 0 && this.applicant.Designation_Name != null) {
          this.dropDowns.selectedFiltersModel.designation = { Name: this.applicant.Designation_Name, Id: this.applicant.Designation_Id }
        }
        if (this.applicant.Status_Id != 0 && this.applicant.StatusName != null) {
          this.dropDowns.selectedFiltersModel.currentStatus = { Name: this.applicant.StatusName, Id: this.applicant.Status_Id }
        }
        if (this.applicant.Department_Id != 0 && this.applicant.Department_Name != null) {
          this.dropDowns.selectedFiltersModel.department = { Name: this.applicant.Department_Name, Id: this.applicant.Department_Id }
        }
        this.dropDowns.selectedFiltersModel.domicile = { DistrictName: this.applicant.Domicile_Name, Id: this.applicant.Domicile_Id };
      }
      this.step = this.step == 0 ? 1 : this.step;
      this.proceeding = false;
      this.lockingProfile = false;
      this.checkValidations();

    }, err => {
      this.proceeding = false;
      this.lockingProfile = false;
      console.log(err);
    });
  }
  public loadDropdowns() {
    this.getDomiciles();
    this.getPostingModes();
    this.getDistricts();
    this.getDesignations();
  }
  public appendFilesToList(file: File, id) {
    if (this.selectedFiles.length > 0) {
      var index = this.selectedFiles.findIndex(x => x.Id === id);
      if (index == 0 || index > 0) {
        this.selectedFiles[index] = file;
        this.selectedFiles[index].Id = id;
      }
      else {
        file[0].Id = id;
        this.selectedFiles.push(file[0]);
      }
    }
    else {
      file[0].Id = id;
      this.selectedFiles.push(file[0]);
    }
    this.selectedFiles.forEach(fileItem => {
      if (fileItem) {
        if (fileItem.Id == 1) {
          this.validations.cnicCopy = true;
        }
        if (fileItem.Id == 2 || fileItem.Id == 5) {
          this.validations.orderCopy = true;
        }
        if (fileItem.Id == 3) {
          this.validations.joiningReport = true;
        }
        if (fileItem.Id == 4) {
          this.validations.meritList = true;
        }
        if (fileItem.Id == 6) {
          this.validations.contractOrderCopy = true;
        }
        if (fileItem.Id == 7) {
          this.validations.contractJoiningReport = true;
        }
      }
    });
    this.checkValidations();
  }

  public checkValidations() {
    this.validations.masterFlag = false;

    if (this.applicant.CNIC_FilePath) {
      this.validations.cnicCopy = true;
    }
    if (this.applicant.PromotionCopy_FilePath || this.applicant.OrderCopy_FilePath) {
      this.validations.orderCopy = true;
    }
    if (this.applicant.AssumptionReport_FilePath) {
      this.validations.joiningReport = true;
    }
    if (this.applicant.MeritList_FilePath) {
      this.validations.meritList = true;
    }
    if (this.applicant.ContractOrderCopy_FilePath) {
      this.validations.contractOrderCopy = true;
    }
    if (this.applicant.ContractJoining_FilePath) {
      this.validations.contractJoiningReport = true;
    }

    if (this.applicant.ModeId == 1) {
      if (this.validations.cnicCopy && this.validations.orderCopy && this.validations.joiningReport
        && this.applicant.LastPromotionDate && this.applicant.PromotionOrderNumber && this.applicant.PromotionJoiningDate) {
        this.validations.masterFlag = true;
      } else {
        this.validations.masterFlag = false;
      }
    } else if (this.applicant.ModeId == 2) {
      if (this.validations.cnicCopy && this.validations.orderCopy && this.validations.joiningReport && this.validations.meritList
        && this.applicant.RegularOrderNumber && this.applicant.DateOfRegularization && this.applicant.FirstJoiningDate && this.applicant.PPSCMeritNumber) {
        this.validations.masterFlag = true;
      } else {
        this.validations.masterFlag = false;
      }
    } else if (this.applicant.ModeId == 3) {
      if (this.validations.cnicCopy && this.validations.orderCopy && this.validations.joiningReport && this.validations.contractOrderCopy && this.validations.contractJoiningReport
        && this.applicant.ContractOrderNumber && this.applicant.ContractStartDate && this.applicant.ContractOrderDate
        && this.applicant.RegularOrderNumber && this.applicant.DateOfRegularization && this.applicant.FirstJoiningDate) {
        this.validations.masterFlag = true;
      } else {
        this.validations.masterFlag = false;
      }
    } else if (this.applicant.ModeId == 5) {
      if (this.validations.cnicCopy && this.validations.orderCopy && this.validations.joiningReport
        && this.applicant.RegularOrderNumber && this.applicant.DateOfRegularization && this.applicant.FirstJoiningDate) {
        this.validations.masterFlag = true;
      } else {
        this.validations.masterFlag = false;
      }
    }
  }


  public proceed(locked?: boolean) {
    this.proceeding = true;
    if (locked) {
      this.lockingProfile = true;
      this.applicant.IsLocked = true;
    }
    if (this.applicant.DateOfBirth) { this.applicant.DateOfBirth = this.applicant.DateOfBirth.toDateString() };
    if (this.applicant.LastPromotionDate) { this.applicant.LastPromotionDate = this.applicant.LastPromotionDate.toDateString() };
    if (this.applicant.PromotionJoiningDate) { this.applicant.PromotionJoiningDate = this.applicant.PromotionJoiningDate.toDateString() };
    if (this.applicant.ContractStartDate) { this.applicant.ContractStartDate = this.applicant.ContractStartDate.toDateString() };
    if (this.applicant.ContractOrderDate) { this.applicant.ContractOrderDate = this.applicant.ContractOrderDate.toDateString() };
    if (this.applicant.FirstJoiningDate) { this.applicant.FirstJoiningDate = this.applicant.FirstJoiningDate.toDateString() };
    if (this.applicant.DateOfRegularization) { this.applicant.DateOfRegularization = this.applicant.DateOfRegularization.toDateString() };

    this._onlinePromotionApplyService.saveSeniorityApplication(this.applicant, this.selectedFiles).subscribe((res3: any) => {
      if (res3) {
        this.selectedFiles = [];
        this.getApplicant();
      }
    }, err2 => {
      this.savingProfile = false;
      this.proceeding = false;
      this.lockingProfile = false;
      console.log(err2);
    });
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

  private getPostingModes = () => {
    this.dropDowns.postingModes = [];
    this._rootService.GetJobPostingMode().subscribe((res: any) => {
      this.dropDowns.postingModes = res;
      this.dropDowns.postingModes = this.dropDowns.postingModes.slice();
    },
      err => { this.handleError(err); }
    );
  }
  private getDistricts() {
    this.dropDowns.districts = [];
    this._rootService.getDistricts('0').subscribe((res: any) => {
      this.dropDowns.districts = res;
      this.dropDowns.districts = this.dropDowns.districts.slice();
    },
      err => { this.handleError(err); });
  }
  private getHealthFacilities(code: string) {
    this.dropDowns.healthFacilities = [];
    this._rootService.getHealthFacilitiesByType(code, this.applicant.Department_Id).subscribe((res: any) => {
      this.dropDowns.healthFacilities = res;
      this.dropDowns.healthFacilities = this.dropDowns.healthFacilities.slice();
    },
      err => { this.handleError(err); }
    );
  }
  private getDesignations() {
    this.dropDowns.designations = [];
    this._rootService.getConsultantDesignations().subscribe((res: any) => {
      this.dropDowns.designations = res.List;
      this.dropDowns.designations = this.dropDowns.designations.slice();
    },
      err => { this.handleError(err); }
    );
  }
  public dropdownValueChanged = (value, filter) => {
    if (!value) {
      debugger;
      return;
    }
    if (filter == 'domicile') {
      debugger;
      this.applicant.Domicile_Id = value.Id;
    }
    if (filter == 'religion') {
      this.applicant.Religion_Id = value.Id;
    }
    if (filter == 'status') {
      this.applicant.Status_Id = value.Id;
    }
    if (filter == 'department') {
      this.applicant.Department_Id = value.Id;
      this.dropDowns.selectedFiltersModel.district = this.dropDowns.defultFiltersModel.district;
      this.dropDowns.selectedFiltersModel.healthFacility = this.dropDowns.defultFiltersModel.healthFacility;
    }
    if (filter == 'mode') {
      this.applicant.ModeId = value.Id;
      if (this.applicant.ModeId == 1) {

      }
      if (this.applicant.ModeId == 2) {

      }
      if (this.applicant.ModeId == 3) {

      }
    }
    if (filter == 'district') {
      this.applicant.DistrictC;
      this.dropDowns.selectedFiltersModel.healthFacility = this.dropDowns.defultFiltersModel.healthFacility;
      this.getHealthFacilities(value.Code);
    }
    if (filter == 'healthfacility') {
      this.applicant.HealthFacility_Id = value.Id;
      this.applicant.HfmisCode = value.HfmisCode;
    }
    if (filter == 'designation') {
      debugger;
      this.applicant.Designation_Id = value.Id;
    }
  }
  public changeImage(Id) {
    if (Id == 1) {
      this.applicant.CNIC_FileName = null;
    }
    if (Id == 2) {
      this.applicant.OrderCopy_FileName = null;
    }
    if (Id == 3) {
      this.applicant.AssumptionReport_FileName = null;
    }
    if (Id == 4) {
      this.applicant.MeritList_FileName = null;
    }
    if (Id == 5) {
      this.applicant.PromotionCopy_FileName = null;
    }
    if (Id == 6) {
      this.applicant.ContractOrderCopy_FileName = null;
    }
    if (Id == 7) {
      this.applicant.ContractJoining_FileName = null;
    }
    if (Id == 8) {
      this.applicant.Status_FileName = null;
    }
  }
  public openInNewTab(link: string) {
    window.open('https://hrmis.pshealthpunjab.gov.pk/' + link, '_blank');
  }
  public dashifyCNIC(cnic: string) {
    if (!cnic) {
      return;
    }
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
  private handleError(err: any) {
    if (err.status == 403) {
      this._authenticationService.logout();
    }
    this.savingProfile = false;
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }





}
