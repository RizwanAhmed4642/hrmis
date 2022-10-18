import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { getStyle, hexToRgba } from '@coreui/coreui/dist/js/coreui-utilities';
import { CustomTooltips } from '@coreui/coreui-plugin-chartjs-custom-tooltips';
import { DashboardService } from './dashboard.service';
import { NotificationService } from '../../_services/notification.service';
import { LocalService } from '../../_services/local.service';
import { User } from '../../_models/erp.class';
import { Subject } from 'rxjs/Subject';
import { Subscription } from 'rxjs/Subscription';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { RootService } from '../../_services/root.service';
import { AuthenticationService } from '../../_services/authentication.service';

@Component({
  templateUrl: 'dashboard.component.html'
})
export class DashboardComponent implements OnInit {
  public range = { start: null, end: null };
  public searchEvent = new Subject<any>();
  public searchSubcription: Subscription = null;
  public listItems: any[] = [];
  public applications: any[] = [];
  public leaveRecord: any[] = [];
  public serviceHistory: any[] = [];
  public qualifications: any[] = [];
  public profile: any;
  public mutualCode: any = {};
  public local: boolean = false;
  public mphAttached: boolean = false;
  public advertisement: boolean = true;
  public searching: boolean = false;
  public infoCorrect: boolean = false;
  public addNew: boolean = false;
  public savingApplication: boolean = false;
  public currentUser: any;
  public designationId: number = 0;
  public days: number = 0;
  public adminDays: number = 0;
  constructor(private router: Router, public _authService: AuthenticationService,
    public _notificationService: NotificationService, private _rootService: RootService,
    private _dashboardService: DashboardService, private _localService: LocalService) { }

  ngOnInit(): void {
    /*  let testObj = {
       PointTypeId: 1,
       SourceId: 2,
       DestinationId: 9,
       Latitude: 31.5203696,
       Longitude: 74.35874729
     };
     this._rootService.travelGuideTest(testObj).subscribe((res: any) => {
       console.log(res);
     }, err => {
       console.log(err);
     }); */

    this.handleSearchEvents();
    this.local = window.location.origin == 'http://localhost:4200';
    this.currentUser = this._authService.getUser();
    if (this.currentUser.RoleName == 'Employee' || this.currentUser.RoleName == 'Employee Applicant') {
      this.checkApplicationMutualCode();
      this._rootService.getProfileDetail(this.currentUser.UserName, 2).subscribe((data: any) => {
        this.applications = data;
      },
        err => {
          console.log(err);
        }
      );
      this._rootService.getProfileDetail(this.currentUser.UserName, 0).subscribe((data: any) => {
        this.profile = data;
      },
        err => {
          console.log(err);
        });
      this._rootService.getProfileDetail(this.currentUser.UserName, 6).subscribe((data: any) => {
        this.leaveRecord = data;
      },
        err => {
          console.log(err);
        });
      this._rootService.getProfileDetail(this.currentUser.UserName, 7).subscribe((data: any) => {
        this.serviceHistory = data;

        this.serviceHistory.forEach(s => {

        });
      },
        err => {
          console.log(err);
        });
      this._rootService.getProfileDetail(this.currentUser.UserName, 11).subscribe((data: any) => {
        this.qualifications = data;
        this.mphAttached = false;
        this.qualifications.forEach(q => {
          if (q.Required_Degree_Id == 76 || q.Required_Degree_Id == 115) {
            this.mphAttached = true;
          }
        });
      },
        err => {
          console.log(err);
        });
    }
    /* let user: User = this._localService.getUser();
    console.log(user.erpModules[0].erpComponents.find(x => x.Name == 'CREATE')); */

  }

  public openEditDialog(fieldName, fieldLabel) {

  }
  public handleSearchEvents() {
    this.searchSubcription = this.searchEvent.pipe(
      debounceTime(400)).subscribe((x) => {
        this.search(x);
      });
  }
  public search(value: string) {
    this.listItems = [];
    if (value.length > 2) {
      this.searching = true;
      this._rootService.search(value).subscribe((data) => {
        this.listItems = data as any[];
      });
    }
  }
  public checkApplicationMutualCode() {
    this._dashboardService.checkApplicationMutualCode(this.currentUser.UserName).subscribe((data: any) => {
      if (data && data.Id > 0 && data.SecondCode > 0) {
        data.SecondCode = 0;
      }
      this.mutualCode = data;
    });
  }
  public verifySecondMutualCode() {
    this.mutualCode.saving = true;
    this._dashboardService.verifySecondMutualCode(this.mutualCode.Id, this.mutualCode.SecondCode).subscribe((res: any) => {
      this.mutualCode = res;
      if (this.mutualCode && this.mutualCode.SecondVerified == true) {
        this.submitMutualApplication();
      } else {
        this.mutualCode.saving = false;
      }
    }, err => {
      console.log(err);
    });
  }
  public submitAdministrativeApplication() {
    let obj: any = {};
    obj.ProfileId = this.profile.Id;
    obj.DesignationId = this.designationId;
    obj.HF_Id = this.profile.HealthFacility_Id;
    obj.StatusId = 1;
    this._dashboardService.submitHrApplication(obj).subscribe((res) => {
      if (res) {
        this._notificationService.notify('success', 'Application submitted.')
        this.infoCorrect = true;
        setTimeout(() => {
          window.location.reload();
        }, 3000);
      }
    }, err => {
      console.log(err);
    });
  }
  submitMutualApplication() {
    this.savingApplication = true;
    this.mutualCode.saving = true;
    this._rootService.getProfileById(this.mutualCode.FirstProfile_Id).subscribe((secondProfile: any) => {
      if (secondProfile && secondProfile.Id == this.mutualCode.FirstProfile_Id) {
        let app = this.mapProfileToApplicant(secondProfile, this.profile);
        this._dashboardService.submitApplication(app).subscribe((response: any) => {
          if (response.application) {
            app.Id = response.application.Id;
            if (response) {
              app.TrackingNumber = response.application.TrackingNumber;
              let applicationLog: any = {};
              applicationLog.Application_Id = app.Id;
              applicationLog.ToOfficer_Id = 71;
              applicationLog.FromOfficer_Id = 164;
              // Received to First officer
              applicationLog.ToStatus_Id = 1;
              applicationLog.ToStatus = 'Under Process';
              applicationLog.IsReceived = false;
              this._dashboardService.createApplicationLog(applicationLog).subscribe((response2: any) => {
                if (response2.Id) {
                  this._dashboardService.checkApplications(app.HealthFacility_Id, app.ToHF_Id, app.Designation_Id).subscribe((d: any) => {
                    if (d && d.applicationFromCounts) {
                      app.FromAppCounts = d.applicationFromCounts.length;
                      this._notificationService.notify('success', 'Submitted Successfully.');
                    }
                    this.savingApplication = false;
                    this.mutualCode.saving = false;
                  }, err => {
                    console.log(err);
                  });
                } else {
                  this.mutualCode.saving = false;
                }
              },
                err => {
                  console.log(err);
                }
              );
            }
            this.savingApplication = false;
          } else {
            this.mutualCode.saving = false;
          }
        },
          err => { console.log(err); }
        );
      }
    });

  }
  public mapProfileToApplicant = (profile: any, secondProfile: any) => {
    let app: any = {};
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
      app.ForwardingOfficer_Id = 71;
      app.ApplicationType_Id = 2;
      app.ForwardingOfficerName = 'Chief Executive Officer';
      app.ForwardingOfficerName = 'Districts';
      app.DepartmentName = 'Primary & Secondary Healthcare Department';
      app.ToHF_Id = secondProfile.HealthFacility_Id;
      app.toHealthFacility = secondProfile.HealthFacility;
      app.ToHFCode = secondProfile.HfmisCode;
      app.ToDesignation_Id = secondProfile.Designation_Id;
      app.MutualCNIC = secondProfile.CNIC;
      app.Mutual_Id = secondProfile.Id;
      app.EmpMode_Id = this.profile.EmpMode_Id;
      app.EmpStatus_Id = this.profile.StatusId;
      return app;
    }
  }
  public setDesignationId(id) {
    this.designationId = id;
    this.advertisement = false;
  }
  public dashifyCNIC(cnic: string) {
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
  public searchClicked(itemName) {
    let item = this.listItems.find(x => x.Name === itemName);
    let navigateTo: string[] = [];
    if (item.Type == 1) {
      navigateTo.push('/health-facility/' + item.HFMISCode);
    } else if (item.Type == 2) {
      navigateTo.push('/profile/' + item.CNIC);
    } else if (item.Type == 3) {
      navigateTo.push('/order/' + item.Id);
    }
    this.router.navigate(navigateTo);
  }

}
