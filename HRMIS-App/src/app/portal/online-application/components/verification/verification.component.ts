import { Component, OnInit, Input, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import { ActivatedRoute, Router } from '@angular/router';
import { DropDownsHR } from '../../../../_helpers/dropdowns.class';
import { RootService } from '../../../../_services/root.service';
import { NotificationService } from '../../../../_services/notification.service';
import { OnlineApplicationService } from '../../online-application.service';
import { Subject } from 'rxjs/Subject';
import { debounceTime } from 'rxjs/operators/debounceTime';

@Component({
  selector: 'app-profile-verification',
  templateUrl: './verification.component.html',
  styles: []
})
export class VerificationComponent implements OnInit, OnDestroy {
  private subscription: Subscription;
  @Input() public profile: any = {};
  @ViewChild('photoRef', {static: false}) public photoRef: any;
  public hf: any;
  public orders: any[] = [];
  public application: any = {};
  public vpMaster: any;
  public dropDowns: DropDownsHR = new DropDownsHR();
  public transferReason: number = null;
  public disabilityReason: string = '';
  public showMessage: string = '';
  public cnic: string = "";
  public cnicMutual: string = "";
  public id: number = 0;
  public step: number = 1; public photoSrc = '';
  public photoSrces: any[] = [];
  public photoFile: any[] = [];
  public toOfficerId: number = 71;
  public cnicMask: string = "00000-0000000-0";
  public verifying: boolean = false;
  public noVacancyExist: boolean = false;
  public uploadingFile: boolean = false;
  public uploadingFileError: boolean = false;
  public savingApplication: boolean = false;
  public vacancyLoaded: boolean = false;
  public checkingVacancy: boolean = false;
  public otherProfileExist: boolean = false;
  public loadingMutualCNIC: boolean = false;
  public mutualProfile: any = {};
  public transferReasons: any[] = [
    { Name: 'General', Id: 1 },
    { Name: 'Disability', Id: 2 },
    /*   { Name: 'Spouse Death', Id: 3 }, */
    /*   { Name: 'Medical', Id: 4 }, */
    { Name: 'Mutual', Id: 7 },
    { Name: 'WedLock', Id: 5 }
    /*  { Name: 'Compassionate', Id: 6 },*/
  ];
  public inputChange: Subject<any>;
  private cnicSubscription: Subscription;

  constructor(private _rootService: RootService, private _onlineApplicationService: OnlineApplicationService,
    private _notificationService: NotificationService,
    private router: Router,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.fetchParams();
    /*  this._profileService.getProfileDetail(this.profile.CNIC, 3).subscribe((data: any) => {
       this.orders = [];
     },
       err => {
         console.log(err);
       });
  */
    this.subscribeCNIC();
    this.getDivisions('0');
    this.getDistricts('0');
    this.getTehsils('0');

  }
  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('cnic') && params.hasOwnProperty('id')) {
          this.cnic = params['cnic'];
          this.id = +params['id'];
          this.fetchData(this.cnic, this.id);
        }
      }
    );
  }
  public fetchMutualProfileInfo(cnic: string) {
    this._onlineApplicationService.getMutualProfile(this.id, cnic).subscribe((res: any) => {
      if (res) {
        this.mutualProfile = res;
        this.otherProfileExist = true;
      } else {
        this.mutualProfile = null;
        this.otherProfileExist = false;
      }
      this.loadingMutualCNIC = false;
    }, err => {
      console.log(err);
    });
  }
  public subscribeCNIC() {
    this.inputChange = new Subject();
    this.cnicSubscription = this.inputChange.pipe(debounceTime(300)).subscribe((query) => {
      this.loadingMutualCNIC = true;
      if (!query) { this.loadingMutualCNIC = false; return; }
      let cnic: string = query as string;
      cnic = cnic.replace(' ', '');
      if (cnic.length != 13) { this.loadingMutualCNIC = false; return; }
      if (cnic == this.cnic) { this.loadingMutualCNIC = false; return; }
      this.fetchMutualProfileInfo(cnic);
    });
  }
  private fetchData(cnic, id) {
    this._onlineApplicationService.getProfileApplicant(cnic, id).subscribe(
      (res: any) => {
        this.profile = res.profile;
        this.hf = res.hf;
        /*  if (this.profile && this.hf) {
           this.setRoutingOfficer(this.hf.CategoryCode, this.hf.Classification_Id, this.profile.CurrentGradeBPS);
         } */
        this.getHealthFacilities(this.profile.HfmisCode.substring(0, 3), this.profile.HfmisCode);
      }
    );
  }
  onSubmit() {
    this.savingApplication = true;
    /* let html = document.getElementById('applicationPrint').innerHTML;
    if (html) {
      this.application.RawText = html;
    } */
    this._onlineApplicationService.submitApplication(this.application).subscribe((response: any) => {
      if (response.application) {
        this.application.Id = response.application.Id;
        if (response) {
          this.application.TrackingNumber = response.application.TrackingNumber;

          let applicationLog: any = {};
          applicationLog.Application_Id = this.application.Id;


          applicationLog.ToOfficer_Id = this.toOfficerId;
          applicationLog.FromOfficer_Id = 164;
          // Received to First officer
          applicationLog.ToStatus_Id = 1;
          applicationLog.ToStatus = 'Under Process';
          applicationLog.IsReceived = false;
          this._onlineApplicationService.createApplicationLog(applicationLog).subscribe((response: any) => {
            if (response.Id) {
              this._notificationService.notify('success', 'Application Submitted Successfully, Tracking Number: ' + this.application.TrackingNumber);

              this.showMessage = 'Application Submitted Successfully. Tracking Number : ' + this.application.TrackingNumber;

              //this.router.navigate(['/online-application/profile/']);
            }
          },
            err => { this.handleError(err); }
          );
        }
      }
      this.savingApplication = false;
    },
      err => { this.handleError(err); }
    );
  }
  public readUrl(event: any, filter: string) {
    if (event.target.files && event.target.files[0]) {
      if (filter == 'pic') {
        this.photoFile = [];
        let inputValue = event.target;
        this.photoFile = inputValue.files;

        /*  var reader = new FileReader();
         reader.onload = ((event: any) => {
           this.photoSrc = event.target.result;
         }).bind(this);
         reader.readAsDataURL(event.target.files[0]); */
      }
    }
  }

  public uploadBtn(filter: string) {
    if (filter == 'pic') {
      this.photoRef.nativeElement.click();
      /*   this.uploadingAcceptanceLetter = true; */
    }
  }
  private setRoutingOfficer = (catCode: string, classification_Id: number, scale: number) => {
    if (classification_Id == 1) {
      this.toOfficerId = 12;
    } else {
      if (catCode == '001') {
        if (scale == 17) {
          this.toOfficerId = 8;
        } else if (scale > 17) {
          this.toOfficerId = 9;
        }
      } else if (catCode == '002') {
        if (scale == 17) {
          this.toOfficerId = 10;
        } else if (scale > 17) {
          this.toOfficerId = 11;
        }
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
  private getHealthFacilities = (hfmisCode: string, profileHfmisCode?: string) => {
    this._rootService.getHealthFacilities(hfmisCode).subscribe((res: any) => {
      this.dropDowns.healthFacilities = res;
      this.dropDowns.healthFacilitiesData = this.dropDowns.healthFacilities.slice();
      /*   let healthFacilities = this.dropDowns.healthFacilities as any;
        let healthFacility = healthFacilities.find(x => x.HfmisCode == profileHfmisCode);
        if (healthFacility) {
          this.dropDowns.selectedFiltersModel.healthFacility = { Id: healthFacility.Id, HfmisCode: healthFacility.HfmisCode, Name: healthFacility.Name };
          this.application.HfmisCode = healthFacility.HfmisCode;
          this.application.HealthFacility_Id = healthFacility.Id;
          this.application.FromHF_Id = healthFacility.Id;
          this.application.fromHealthFacility = healthFacility.Name;
        } */
    },
      err => { this.handleError(err); }
    );
  }
  public dropdownValueChanged = (value, filter) => {

    if (filter == 'division') {
      this.resetDrops(filter);
      this.getDistricts(value.Code);
      this.getTehsils(value.Code);
    }
    if (filter == 'district') {
      this.dropDowns.selectedFiltersModel.district = value;
      this.resetDrops(filter);
      this.getTehsils(value.Code);
    }
    if (filter == 'tehsil') {
      this.resetDrops(filter);
      this.getHealthFacilities(value.Code);
    }
    if (filter == 'healthFacility') {
      /*   this.application.HfmisCode = value.HfmisCode;
        this.application.HealthFacility_Id = value.Id; */

      this.application.ToHF_Id = value.Id;
      this.application.toHealthFacility = value.Name;
      this.application.ToHFCode = value.HfmisCode;
      this.mapProfileToApplicant(this.profile);
      this.setDesignationForTransfer(this.application.ToHF_Id, this.application.ToHFCode, this.application.Designation_Id);
    }
    if (filter == 'reason') {
      this._onlineApplicationService.verifyProfileForTransfer(this.profile.Id, this.transferReason).subscribe((res: any) => {
        if (res) {
          if (res.Status == 2) {
            this.showMessage = 'You are not eligible as per Policy.';
            this._notificationService.notify('warning', 'You are not eligible as per Policy.');
          } else {
            this.application.ApplicationSubType_Id = value;
          }
        }
      }, err => {
        this.handleError(err);
      });
    }
  }
  public next() {
    this.step = 2;
    if (this.transferReason == 7) {
      this.mapProfileToApplicant(this.profile);
      this.setDesignationForTransfer(this.application.ToHF_Id, this.application.ToHFCode, this.application.Designation_Id);
    }
  }
  public setDesignationForTransfer(hf_Id: number, hfmisCode: string, designation_Id: number) {
    this.checkingVacancy = true;
    this.noVacancyExist = false;
    this.vacancyLoaded = false;
    this.vpMaster = null;
    //this.checkVacancy(hf_Id, hfmisCode, designation_Id);
    this._onlineApplicationService.getOrderDesignations(hf_Id, hfmisCode, designation_Id).subscribe((res: any) => {
      if (res.vpMaster) {
        this.vpMaster = res.vpMaster;
      } else {
        this.noVacancyExist = true;
      }
      this.vacancyLoaded = true;
      this.checkingVacancy = false;
    }, err => {
      this.handleError(err);
    });
  }
  private resetDrops = (filter: string) => {
    this.dropDowns.selectedFiltersModel.healthFacility = { Name: 'Select Health Facility', Id: 0 };
    this.application.fromHealthFacility = '';
    if (filter == 'division') {
      this.dropDowns.selectedFiltersModel.district = { Name: 'Select District', Code: '000' };
      this.dropDowns.selectedFiltersModel.tehsil = { Name: 'Select Tehsil', Code: '000' };
    }
    if (filter == 'district') {
      this.application.fromHealthFacility = '';
      this.dropDowns.selectedFiltersModel.tehsil = { Name: 'Select Tehsil', Code: '000' };
    }
  }
  public mapProfileToApplicant = (profile: any) => {
    if (profile) {
      this.application.Profile_Id = profile.Id ? profile.Id : null;
      this.application.CNIC = profile.CNIC ? profile.CNIC : null;
      this.application.EmployeeName = profile.EmployeeName ? profile.EmployeeName : '';
      this.application.FatherName = profile.FatherName ? profile.FatherName : '';
      this.application.DateOfBirth = profile.DateOfBirth ? new Date(profile.DateOfBirth).toDateString() : new Date(2000, 1, 1).toDateString();
      this.application.Gender = profile.Gender ? profile.Gender : '';
      this.application.MobileNo = profile.MobileNo ? profile.MobileNo : '';
      this.application.EMaiL = profile.EMaiL ? profile.EMaiL : '';
      this.application.FileNumber = '';
      this.application.JoiningGradeBPS = profile.JoiningGradeBPS ? profile.JoiningGradeBPS : 0;
      this.application.CurrentGradeBPS = profile.CurrentGradeBPS ? profile.CurrentGradeBPS : 0;
      this.application.Designation_Id = profile.WDesignation_Id ? profile.WDesignation_Id : this.application.Designation_Id;
      this.application.FromDesignation_Id = profile.WDesignation_Id ? profile.WDesignation_Id : this.application.Designation_Id;
      this.application.HealthFacility_Id = profile.HealthFacility_Id;
      this.application.FromHF_Id = this.application.HealthFacility_Id;
      this.application.HfmisCode = profile.HfmisCode;
      this.application.Department_Id = 25;
      this.application.ApplicationSource_Id = 3;
      this.application.ForwardingOfficer_Id = 71;
      this.application.ApplicationType_Id = 2;
      this.application.ForwardingOfficerName = 'Chief Executive Officer';
      this.application.ForwardingOfficerName = 'Districts';
      this.application.DepartmentName = 'Primary & Secondary Healthcare Department';
      if (this.transferReason == 7 && this.mutualProfile) {
        this.application.ToHF_Id = this.mutualProfile.HealthFacility_Id;
        this.application.toHealthFacility = this.mutualProfile.HealthFacility + ', ' + this.mutualProfile.Tehsil + ', ' + this.mutualProfile.District;
        this.application.ToHFCode = this.mutualProfile.HfmisCode;
        this.application.MutualCNIC = this.mutualProfile.CNIC;
        this.application.Mutual_Id = this.mutualProfile.Id;
      }
      this.application.ToDesignation_Id = this.application.Designation_Id;
      let empModes = this.dropDowns.employementModesData as any[];
      if (empModes) {
        let empMode = empModes.find(x => x.Id == profile.EmpMode_Id);
        if (empMode) {
          this.application.EmpMode_Id = empMode.Id;
          this.application.EmpModeName = empMode.Name;
        }
      }
      let empStatuses = this.dropDowns.statusData as any[];
      if (empStatuses) {
        let empStatuse = empStatuses.find(x => x.Id == profile.EmpStatus_Id);
        if (empStatuse) {
          this.application.EmpStatus_Id = empStatuse.Id;
          this.application.EmpStatusName = empStatuse.Name;
        }
      }
      /*  let designations = this.dropDowns.designations as any[];
       if (designations) {
         let designation = designations.find(x => x.Id == profile.Designation_Id);
         if (designation) {
           this.application.Designation_Id = profile.Designation_Id ? profile.Designation_Id : this.application.Designation_Id;
           this.application.designationName = designation.Name;
           this.application.ToDesignation_Id = designation.Id;
           this.application.toDesignationName = designation.Name;
           this.application.FromDesignation_Id = designation.Id;
         }
       } */
      /*  if (profile.HealthFacility_Id && profile.HfmisCode.length == 19) {
         this.getDivisions('0');
         this.getDistricts('0');
         this.getTehsils('0');
         this.getHealthFacilities(profile.HfmisCode.substring(0, 3), profile.HfmisCode);
       } */
      if (this.application.ApplicationType_Id == 3) {
        this.application.CurrentScale = profile.CurrentGradeBPS ? profile.CurrentGradeBPS : 0;
        this.application.SeniorityNumber = profile.SeniorityNo ? profile.SeniorityNo : '';
      }
    }
  }

  public dashifyCNIC(cnic: string) {
    if (!cnic) return;
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
    this.vacancyLoaded = false;
    this.checkingVacancy = false;
    console.log(err);
  }
  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
    if (this.cnicSubscription) {
      this.cnicSubscription.unsubscribe();
    }
  }
}
