import { Component, OnInit } from '@angular/core';
import { RootService } from '../../../../../_services/root.service';
import { AuthenticationService } from '../../../../../_services/authentication.service';
import { EmployeeService } from '../../../employee.service';
import { NotificationService } from '../../../../../_services/notification.service';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';

@Component({
  selector: 'app-open-merit',
  templateUrl: './open-merit.component.html',
  styles: []
})
export class OpenMeritComponent implements OnInit {
  public currentUser: any;
  public subscription: Subscription;
  public stepString: 'Choose District';
  public showMessage: string = '';
  public showMessageAlertVacancy: string = '';
  public districtQuery: string = '';
  public facilityQuery: string = '';
  public reason: string = '';
  public applicationSubType_Id: number = 1;
  public compassionateType: number = 0;
  public typeOfTransfer: number = 0;
  public profile: any;
  public id: number = 0;
  public districts: any[] = [];
  public districtsOrigional: any[] = [];
  public healthFacilities: any[] = [];
  public healthFacilitiesOrigional: any[] = [];
  public preferences: any[] = [];
  public selectedDistrict: any;
  public showLahore: boolean = false;
  public loadingHealthFacilities: boolean = false;
  public savingApplication: boolean = false;
  public isMutual: boolean = false;
  public isMutualVerified: boolean = false;
  public eligibleCadidate: boolean = null;
  public application: any = {};
  public applicationMutualCode: any;
  public mutualProfile: any = { firstEmployee: {}, secondEmployee: {} };
  public toOfficerId: number = 22;
  public transferReason: number = 1;
  public cnicMask: string = "00000-0000000-0";
  public notEligibleMessage: string = "";
  constructor(private _rootService: RootService,
    private _authenticationService: AuthenticationService,
    private _notificationService: NotificationService,
    private _employeeService: EmployeeService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.currentUser = this._authenticationService.getUser();
    if (!this.currentUser) {
      this._authenticationService.logout();
    }
    this.fetchData(this.currentUser.cnic);
    this.fetchParams();
  }
  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('id')) {
          this.typeOfTransfer = +params['id'];
          if (this.typeOfTransfer == 2) {
            this.reason = 'Disability';
          }
          if (this.typeOfTransfer == 3) {
            this.reason = 'Wedlock';
          }
          if (this.typeOfTransfer == 4) {
            this.reason = 'Divorce';
          }
          if (this.typeOfTransfer == 5) {
            this.reason = 'Widow';
          }
          console.log(this.typeOfTransfer);
          console.log(this.reason);

          /* console.log(this.cnic);
          console.log(decodeURIComponent(this.cnic));
          this._encryptionService.decrypt(this.currentUser.Id, decodeURIComponent(this.cnic)).then((x2) => {
          console.log(x2);
        });
        this._encryptionService.demoEncrypt(this.currentUser.Id, this.cnic).then((x) => {
          console.log(x);
          console.log(encodeURIComponent(x));
          this._encryptionService.decrypt(this.currentUser.Id, x).then((x2) => {
            console.log(x2);
          });
        }); */
        }
      }
    );
  }
  public searchProfile = () => {
    if (!this.mutualProfile.secondEmployee) return;
    if (!this.mutualProfile.secondEmployee.CNIC) return;
    if (+this.mutualProfile.secondEmployee.CNIC[12]) {
      this._employeeService.searchProfile(this.mutualProfile.secondEmployee.CNIC).subscribe((data: any) => {
        this.notEligibleMessage = "";
        if (data && data.Designation_Id && data.EmpStatus_Id && data.EmpMode_Id) {
          if (data.Designation_Id == 2404) {
            if (this.profile.Designation_Id == 802 || this.profile.Designation_Id == 1320) {
              if (data.EmpStatus_Id == 2) {
                if (data.EmpMode_Id == 13) {
                  if (data.HealthFacility_Id != 11606) {
                    this.eligibleCadidate = true;
                  } else {
                    this.eligibleCadidate = false;
                    this.notEligibleMessage += ' Place of Posting cannot be ' + data.HealthFacility;
                  }
                } else {
                  this.eligibleCadidate = false;
                  this.notEligibleMessage += ' Only for Regular Candidate';
                }
              } else {
                this.eligibleCadidate = false;
                this.notEligibleMessage += ' Status ' + data.StatusName;
              }
            } else {
              this.eligibleCadidate = false;
              this.notEligibleMessage += ' Designation ' + data.Designation_Name;
            }
          } else if (data.Designation_Id == this.profile.Designation_Id) {
            if (data.EmpStatus_Id == 2) {
              if (data.EmpMode_Id == 13) {
                if (data.HealthFacility_Id != 11606) {
                  this.eligibleCadidate = true;
                } else {
                  this.eligibleCadidate = false;
                  this.notEligibleMessage += ' Place of Posting cannot be ' + data.HealthFacility;
                }
              } else {
                this.eligibleCadidate = false;
                this.notEligibleMessage += ' Only for Regular Candidate';
              }
            } else {
              this.eligibleCadidate = false;
              this.notEligibleMessage += ' Status ' + data.StatusName;
            }
          } else {
            this.eligibleCadidate = false;
            this.notEligibleMessage += ' Designation ' + data.Designation_Name;
          }
          if (this.eligibleCadidate) {
            this.mutualProfile.secondEmployee = data;
          }
        }
      });
    } else {
      this.eligibleCadidate = null;
      this.notEligibleMessage = "";
    }
  }
  public getApplicationMutualCode() {
    this._employeeService.getApplicationMutualCode(this.mutualProfile.firstEmployee.Id, this.mutualProfile.secondEmployee.Profile_Id).subscribe((res: any) => {
      if (res) {
        this.applicationMutualCode = res;
      } else {
        this.applicationMutualCode = null;
      }
      this.isMutualVerified = false;
    }, err => {
      console.log(err);
    });
  }
  public verifyMutualCode() {
    this.isMutualVerified = false;
    this._employeeService.verifyMutualCode(this.mutualProfile.firstEmployee.Id, this.mutualProfile.secondEmployee.Profile_Id, this.application.MutualCodeOne, this.application.MutualCodeTwo).subscribe((res: any) => {
      if (res) {

        this.isMutualVerified = true;
      } else {
        this.isMutualVerified = false;
      }
    }, err => {
      console.log(err);
    });
  }
  public onSelectDistrict(district: any) {
    this.facilityQuery = '';
    this.selectedDistrict = district;
    this.fetchFacilities(this.selectedDistrict.Code);
    this.id = 1;
  }
  onSubmit(hf?: any, index?: number) {
    /* let html = document.getElementById('applicationPrint').innerHTML;
    if (html) {
      this.application.RawText = html;
    } */
    this.savingApplication = true;
    let app: any = {};
    app.ApplicationSubType_Id = this.applicationSubType_Id;
    app = this.mapProfileToApplicant(app, hf, this.profile);
    hf.saving = true;
    this._employeeService.submitApplication(app).subscribe((response: any) => {
      if (response.application) {
        app.Id = response.application.Id;
        if (response) {
          hf.appId = app.Id;
          app.TrackingNumber = response.application.TrackingNumber;
          hf.TrackingNumber = app.TrackingNumber;
          let applicationLog: any = {};
          applicationLog.Application_Id = app.Id;
          applicationLog.ToOfficer_Id = this.toOfficerId;
          applicationLog.FromOfficer_Id = 164;
          // Received to First officer
          applicationLog.ToStatus_Id = 1;
          applicationLog.ToStatus = 'Under Process';
          applicationLog.IsReceived = false;
          this._employeeService.createApplicationLog(applicationLog).subscribe((response2: any) => {
            if (response2.Id) {
              this._employeeService.checkApplications(app.HealthFacility_Id, app.ToHF_Id, app.Designation_Id).subscribe((d: any) => {
                if (d && d.applicationFromCounts) {
                  app.FromAppCounts = d.applicationFromCounts.length;
                  hf.showMessageAlertVacancy = '. Total Applications From ' + app.HealthFacilityName + ' : ' + app.FromAppCounts;
                  this.showMessage = 'Submitted Successfully';
                  this._notificationService.notify('success', 'Submitted Successfully.');
                }
                this.savingApplication = false;
                hf.saving = false;
                hf.saved = true;
              }, err => {
                console.log(err);
              });
            }
          },
            err => {
              console.log(err);
            }
          );
        }
        hf.saving = false;
        this.savingApplication = false;
      }
    },
      err => { console.log(err); }
    );
  }
  public applicationSubtypeSelected(subtype_Id: number) {
    this.applicationSubType_Id = subtype_Id;
  }
  public applicationCompassionatetypeSelected(subtype_Id: number) {
    this.compassionateType = subtype_Id;
  }
  public mutualTransfer() {
    if (this.isMutual) {
      this.isMutual = false;
      this.transferReason = 1;
    } else {
      this.isMutual = true;
      this.transferReason = 7;
    }

  }
  private fetchData(cnic) {
    this.profile = null;
    this._employeeService.getProfile(cnic).subscribe(
      res => {
        if (res) {
          this.profile = res as any;
          this.mutualProfile.firstEmployee = this.profile;
          this.mutualProfile.secondEmployee = {};
          this._employeeService.verifyProfileForTransfer(this.profile.Id, 1).subscribe((res2: any) => {
            if (res2) {
              console.log(res2);

              if (res2.Status == 2) {
                this.showMessage = 'You are not eligible as per Policy.';
                this._notificationService.notify('warning', 'You are not eligible as per Policy.');
              } else {
                /*  if (res2.VacancyCheck) {
                   let vacancyCheck = res2.VacancyCheck; */
                /*   if (vacancyCheck.PercentageFrom < 50) {
                    this.showMessage = '50% vacancy check failed. Vacancy will be ' + vacancyCheck.PercentageFrom + '%';
                    this._notificationService.notify('warning', 'You are not eligible as per Policy.');
                  } else { */
                this.application.ApplicationSubType_Id = 0;
                this._rootService.getDistrictsVacancy(this.profile.Designation_Id).subscribe(
                  res3 => {
                    if (res3) {
                      this.districts = res3 as any[];
                      /*  if (res2.Status == 1) {
                         let tempDistricts = this.districts.filter(x => x.Name != 'Lahore');
                         this.districts = [];
                         this.districts = tempDistricts;
                       } */
                      if (res2.Status == 3) {
                        this.showLahore = true;
                      } else {
                        this.showLahore = false;
                      }
                      this.districtsOrigional = this.districts;
                    }
                  }, err => {
                    console.log(err);
                  }
                );
                /*    } */
                /*   } */
              }
            }
          }, err => {
            console.log(err);
          });
        }
      }, err => {
        console.log(err);
      }
    );


  }
  private fetchFacilities(districtCode) {
    this.healthFacilities = [];
    this.loadingHealthFacilities = true;
    this._rootService.getHFVacs(districtCode, this.profile.Designation_Id).subscribe(
      res => {

        this.healthFacilities = res as any;
        console.log(this.healthFacilities);
        this.healthFacilitiesOrigional = this.healthFacilities;
        this.loadingHealthFacilities = false;
      }, err => {
        console.log(err);
        this.loadingHealthFacilities = false;
      }
    );
  }
  public addToList(hf: any) {
    if (this.preferences.length < 1) {
      hf.added = true;
      this.preferences.push(hf);
    }
  }
  public backToDistricts() {
    this.healthFacilities = [];
    this.selectedDistrict = null;
  }
  public removeFromList(i: number) {
    var hf = this.preferences[i];
    hf.added = false;
    this.preferences.splice(i, 1);
  }
  /* public applyHere(hf: any) {
    this.application.ToHF_Id = hf.HF_Id;
    this.application.toHealthFacility = hf.HFName;
    this.application.ToHFCode = hf.HFMISCode;
    this.mapProfileToApplicant(this.profile);
    hf.saving = true;
    this.onSdubmit(hf);
  } */
  public applyAll() {
    for (let index = 0; index < this.preferences.length; index++) {
      const hf = this.preferences[index];
      this.onSubmit(hf, index);
    }
  }
  public subscribeAlert(item: any) {
    item.saving = true;
    this._employeeService.subscribeAlert(this.profile.Id, item.Id).subscribe((data: any) => {
      if (data) {
        this._notificationService.notify('success', 'You will be notified when the seat is vacant. ');
        item.saving = false;
      }
    }, err => {
      console.log(err);

    })
  }
  public searchDistrict() {
    this.districts = [];
    if (!this.districtQuery) {
      this.districts = this.districtsOrigional;
      return;
    }
    this.districts = this.districtsOrigional.filter(x => x.Name.toLowerCase().includes(this.districtQuery.toLowerCase()));
  }
  public searchFacility() {
    this.healthFacilities = [];
    if (!this.facilityQuery) {
      this.healthFacilities = this.healthFacilitiesOrigional;
      return;
    }
    this.healthFacilities = this.healthFacilitiesOrigional.filter(x => x.HFName.toLowerCase().includes(this.facilityQuery.toLowerCase()));
  }
  public mapProfileToApplicant = (app, hf, profile: any) => {
    if (profile) {
      app.Profile_Id = profile.Id ? profile.Id : null;
      app.CNIC = profile.CNIC ? profile.CNIC : null;
      app.EmployeeName = profile.EmployeeName ? profile.EmployeeName : '';
      app.FatherName = profile.FatherName ? profile.FatherName : '';
      app.DateOfBirth = profile.DateOfBirth ? new Date(profile.DateOfBirth).toDateString() : new Date(2000, 1, 1).toDateString();
      app.Gender = profile.Gender ? profile.Gender : '';
      app.MobileNo = profile.MobileNo ? profile.MobileNo : '';
      app.EMaiL = profile.EMaiL ? profile.EMaiL : '';
      app.FileNumber = '';
      app.JoiningGradeBPS = profile.JoiningGradeBPS ? profile.JoiningGradeBPS : 0;
      app.CurrentGradeBPS = profile.CurrentGradeBPS ? profile.CurrentGradeBPS : 0;
      if (profile.Designation_Id == 2404) {
        profile.Designation_Id = app.Gender == 'Male' ? 802 : 1320;
      }
      app.Designation_Id = profile.Designation_Id ? profile.Designation_Id : app.Designation_Id;
      app.FromDesignation_Id = profile.Designation_Id ? profile.Designation_Id : app.Designation_Id;
      app.HealthFacility_Id = profile.HealthFacility_Id;
      app.FromHF_Id = app.HealthFacility_Id;
      app.HfmisCode = profile.HfmisCode;
      app.Department_Id = 25;
      app.ApplicationSource_Id = 3;
      app.ForwardingOfficer_Id = 22;
      app.ApplicationType_Id = 2;
      app.ForwardingOfficerName = 'Section (WMO-II)';
      app.DepartmentName = 'Primary & Secondary Healthcare Department';
      if (this.applicationSubType_Id == 2 && this.mutualProfile && this.mutualProfile.firstEmployee && this.mutualProfile.secondEmployee) {
        app.ToHF_Id = this.mutualProfile.secondEmployee.HealthFacility_Id;
        app.toHealthFacility = this.mutualProfile.secondEmployee.HealthFacility;
        app.ToHFCode = this.mutualProfile.secondEmployee.HfmisCode;
        app.ToDesignation_Id = this.mutualProfile.secondEmployee.Designation_Id;
        app.MutualCNIC = this.mutualProfile.secondEmployee.CNIC;
        app.Mutual_Id = this.mutualProfile.secondEmployee.Profile_Id;
      } else {
        app.ToDesignation_Id = hf.Desg_Id;
        app.ToHF_Id = hf.HF_Id;
        app.toHealthFacility = hf.HFName;
        app.ToHFCode = hf.HFMISCode;
      }
      app.EmpMode_Id = this.profile.EmpMode_Id;
      app.EmpStatus_Id = this.profile.StatusId;
      app.Reason = this.reason;
      return app;
    }
  }
  ngOnDestroy() {
    this.subscription.unsubscribe();
  }
}
