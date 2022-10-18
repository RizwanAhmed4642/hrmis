import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { SortDescriptor, orderBy, aggregateBy } from '@progress/kendo-data-query';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { HealthFacilityService } from '../../../health-facility.service';
import { VpMProfileView, VPDetail, VpDProfileView } from '../../../../vacancy-position/vacancy-position.class';
import { DropDownsHR } from '../../../../../_helpers/dropdowns.class';
import { Subject } from 'rxjs/Subject';
import { Subscription } from 'rxjs/Subscription';
import { RootService } from '../../../../../_services/root.service';
import { AuthenticationService } from '../../../../../_services/authentication.service';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { NotificationService } from '../../../../../_services/notification.service';

@Component({
  selector: 'app-vacancy',
  templateUrl: './vacancy.component.html',
  styles: [
    `
   .details-head-vacancy {
     font-size: 14px;
     font-family: "Roboto-Regular";
   }
   .details-content-vacancy {
    font-size: 14px;
    font-family: "Roboto-Light";
  }
    `
  ]
})
export class VacancyComponent implements OnInit, OnDestroy {
  @Input() public hfmisCode: string = '';
  @Input() public healthFacility: any;
  @Input() public userRight: any;
  @Input() public vacancy: any[] = [];
  public vacancyOrigional: any[] = [];
  public vpMasterLogs: any[] = [];
  public vpDetailLogs: any[] = [];
  public entityModifiedLogs: any[] = [];
  @Input() public employementModes: any[] = [];
  public currentUser: any;
  public vpMaster: any;
  public loading: boolean = true;
  public profileLoading: boolean = true;
  public systemLog: boolean = false;
  public showVacancy: boolean = false;
  public showVacancyControls: boolean = true;
  public loadingVpMaster: boolean = true;
  public saving: boolean = false;
  public removing: boolean = false;
  public vacancyChanged: boolean = false;
  public vacancyBefore: number = 0;
  public vacancySanctionedBefore: number = 0;
  public sumOrders: number = 1;
  public filledExceed: boolean = false;
  public dropDowns: DropDownsHR = new DropDownsHR();
  public searchEvent = new Subject<any>();
  public searchSubcription: Subscription = null;
  public multiple = false;
  public allowUnsort = true;
  public sort: SortDescriptor[] = [];
  public gridView: GridDataResult;
  public showProfile = false;
  public showLog = false;
  public showProfileEmpMode_Id: number = 0;
  public suggestedProfiles: any[] = [];
  public profiles: any[] = [];
  public profileWindowView = false;
  public profilesWindow: any = {
    dialogOpened: false,
    data: null,
    profiles: [],
  }
  public vacancyTotal: any;
  public vacancyAggregates: any[] = [];
  constructor(private _notificationService: NotificationService, private _authenticationService: AuthenticationService, private _rootService: RootService, private _healthFacilityService: HealthFacilityService) { }

  ngOnInit() {
    this.currentUser = this._authenticationService.getUser();
    this.getVacancy(this.hfmisCode);
    this.handleSearchEvents();
  }
  public saveVacancy() {
    this.checkFilledExceed();
    if (this.filledExceed) return;
    this.saving = true;
    this._healthFacilityService.saveVacancy(this.vpMaster).subscribe((res: any) => {
      if (res && res.Id) {
        this._notificationService.notify('success', this.vpMaster.DsgName + ' - Vacancy Saved!');
        this.closeWindow();
      }
    }, err => {
      this.handleError(err);
    });
  }
  public sortChange(sort: SortDescriptor[]): void {
    if (sort[0].field == 'asd') {
      return;
    }
    this.sort = sort;
    this.loadVacancy();
  }
  public getVacancy(code: string): void {
    this.loading = true;
    this._healthFacilityService.getHFVacancy(code).subscribe((data: any) => {
      debugger
      if (data) {
        this.vacancy = data;
        this.vacancyOrigional = this.vacancy;
        this.loadVacancy();
      }
    }, err => {
      this.handleError(err);
    });
  }
  public getSuggestedProfile(vpMaster_Id: number, hFId: number, scale: number): void {
    this.profileLoading = true;
    this._healthFacilityService.getSuggestedProfile(vpMaster_Id, hFId, scale).subscribe((data: any) => {
      if (data) {
        this.suggestedProfiles = data;
      }
      this.profileLoading = false;
    }, err => {
      this.handleError(err);
      this.profileLoading = false;
    });
  }
  /* public getVpProfiles(vpMaster_Id: number): void {
    this.profileLoading = true;
    this._healthFacilityService.getVpProfiles(vpMaster_Id).subscribe((data: any) => {
      if (data) {
        this.suggestedProfiles = data;
      }
      this.profileLoading = false;
    }, err => {
      this.handleError(err);
      this.profileLoading = false;
    });
  } */
  public setVacancyAggregates() {
    this.vacancyAggregates = [
      { field: 'TotalSanctioned', aggregate: 'sum' },
      { field: 'TotalSanctionedPHMC', aggregate: 'sum' },
      { field: 'TotalWorking', aggregate: 'sum' },
      { field: 'PHFMC', aggregate: 'sum' },
      { field: 'Regular', aggregate: 'sum' },
      { field: 'Adhoc', aggregate: 'sum' },
      { field: 'Contract', aggregate: 'sum' },
      { field: 'Vacant', aggregate: 'sum' },
      { field: 'WorkingProfiles', aggregate: 'sum' }
    ];
    if (this.vacancy.length > 0) {
      this.vacancyTotal = aggregateBy(this.vacancy, this.vacancyAggregates);
      this.showVacancy = true;
    }
    this.saving = false;
    this.loading = false;

  }
  public shiftProfileToPSH(dataItem: any) {
    if (confirm("Are you sure?")) {
      dataItem.shifting = true;
      this._healthFacilityService.shiftProfileToPSH(dataItem.Id).subscribe((res: any) => {
        if (res) {
          this.getVacancy(this.hfmisCode);
          dataItem.shifting = false;
          this.closeWindow();
          this._notificationService.notify('success', 'Profile Shifted');
        }
      }, err => console.log(err));
    }
  }
  public searchVacancy(query: string) {
    this.loading = true;
    if (!query) {
      this.vacancy = this.vacancyOrigional;
      this.loadVacancy();
      return;
    }
    this.vacancy = this.vacancyOrigional.filter(x => x.DsgName.toLowerCase().indexOf(query.toLowerCase()) != -1);
    this.loadVacancy();
  }
  private loadVacancy(): void {
    this.gridView = {
      data: orderBy(this.vacancy, this.sort),
      total: this.vacancy.length
    };
    this.setVacancyAggregates();
  }
  public closeWindow() {
    debugger
    if (this.vacancyBefore != this.vpMaster.TotalWorking || this.vacancySanctionedBefore != this.vpMaster.TotalSanctioned) {
      this.vacancyChanged = true;
      this.getVacancy(this.hfmisCode);
    } else {
      this.saving = false;
    }
    this.showProfile = false;
    this.showLog = false;
    this.sumOrders = 1;
    this.profilesWindow.dialogOpened = false;
    this.profilesWindow.data = null;
    this.employementModes.forEach(empMode => {
      empMode.totalWorking = undefined;
      empMode.profiles = undefined;
    });
    this.vpMaster = new VpMProfileView();
    this.checkFilledExceed();
  }
  public loadDetails(item) {
    console.log(item);
  }
  public fillProfile(index, profile) {
    this.vpMaster.VPProfiles.push({ VPMaster_Id: this.vpMaster.Id, Profile_Id: profile.Id });
    let tempProfile = this.suggestedProfiles.find(x => x.Id == profile.Id);
    if (tempProfile) {
      tempProfile.filled = true;
    }
  }

  public removeProfile(profile) {
    let tempVPProfileIndex = this.vpMaster.VPProfiles.findIndex(x => x.Profile_Id == profile.Id);
    this.vpMaster.VPProfiles.splice(tempVPProfileIndex, 1);
    let tempProfile = this.suggestedProfiles.find(x => x.Id == profile.Id);
    if (tempProfile) {
      tempProfile.filled = false;
    }
  }


  public openWindow(dataItem, column?: string) {
    this.loadingVpMaster = true;
    this.showVacancyControls = true;
    this.profilesWindow.dialogOpened = true;
    this.profilesWindow.data = dataItem;
    // this.getSuggestedProfile(dataItem.Id, dataItem.HF_Id, dataItem.BPS);
    this._healthFacilityService.getVpDProfileViews(dataItem.Id).subscribe((res: any) => {
      if (res) {
        this.vpMaster = res.vpMaster;
        if (this.currentUser.HfmisCode.length > 3 && this.vpMaster.HFAC != 2 && this.currentUser.RoleName != 'District Computer Operator' && this.currentUser.RoleName != 'Health Facility') {
          this.showVacancyControls = false;
        }
        if (this.currentUser.RoleName == 'DG Health') {
          this.showVacancyControls = false;
        }
        /* if ((this.vpMaster.Desg_Id == 1320 || this.vpMaster.Desg_Id == 2404) && this.vpMaster.HFAC != 2) {
          this.showVacancyControls = false;
        }
        if ((this.vpMaster.Desg_Id == 1320 || this.vpMaster.Desg_Id == 2404) && (this.currentUser.RoleName == 'SDP' || this.currentUser.UserName == 'dpd')) {
          this.showVacancyControls = true;
        } */
        debugger
        this.vacancySanctionedBefore = this.vpMaster.TotalSanctioned;
        this.vacancyBefore = this.vpMaster.TotalWorking;
        this.vpMaster.VPDetails = res.vpDetails;
        this.vpMaster.VPProfiles = res.vpProfiles;
        if (!this.vpMaster.VPProfiles) {
          this.vpMaster.VPProfiles = [];
        }
        if (res.vpMasterLogs)
          this.vpMasterLogs = res.vpMasterLogs;
        if (res.vpDetailLogs)
          this.vpDetailLogs = res.vpDetailLogs;
        if (res.emls) {
          res.emls.forEach(eml => {
            if (eml.Description == 'Order Generated') eml.orderSum = this.sumOrders++;
          });
          this.entityModifiedLogs = res.emls;
        }

        this.setEmpModeDetail();
        this._healthFacilityService.getProfilesAgainstVacancy(this.profilesWindow.data.HF_Id, this.profilesWindow.data.Desg_Id).subscribe(data => {
          this.profilesWindow.profiles = data;
          if (column && column == 'profile') {
            this.showProfileClicked(0);
          }
        });
      }
    });
  }
  public getSum() {
    return this.sumOrders++;
  }
  public removeVacancy() {
    if (confirm('Remove ' + this.vpMaster.DsgName + ' - Vacancy!')) {
      this.removing = true;
      this._healthFacilityService.rmvHFVacancy(this.vpMaster.Id).subscribe((response: any) => {
        if (response == true) {
          this.removing = false;
          this._notificationService.notify('success', this.vpMaster.DsgName + ' - Vacancy Removed!');
          this.getVacancy(this.hfmisCode);
          this.closeWindow();
        }
      }, err => {
        this.removing = false;
        console.log(err);
      });
    }
  }
  public handleSearchEvents() {
    this.searchSubcription = this.searchEvent.pipe(
      debounceTime(400)).subscribe((x: any) => {
        console.log(x);
        if (x.filter == 'TotalSanctioned') {
          this.checkFilledExceed();
        } else if (x.filter == 'employementMode') {
          console.log(x);
          this.empModeValueChanged(x.Id, +x.event.target.value);
        } else if (x.filter == 'vacancy') {
          this.searchVacancy(x.event);
        }
      });
  }

  public empModeValueChanged(id: number, total: number) {
    let vpDetail = this.vpMaster.VPDetails.find(x => x.EmpMode_Id == id);
    if (vpDetail) {
      vpDetail.TotalWorking = total;
    } else {
      vpDetail = new VpDProfileView();
      vpDetail.EmpMode_Id = id;
      vpDetail.TotalWorking = total;
      this.vpMaster.VPDetails.push(vpDetail);
    }
    this.checkFilledExceed();
  }
  public checkFilledExceed() {
    let totalFilled = this.vpMaster.TotalWorking;
    this.vpMaster.TotalWorking = 0;
    this.vpMaster.VPDetails.forEach(vpDetail => {
      this.vpMaster.TotalWorking += vpDetail.TotalWorking ? vpDetail.TotalWorking : 0;
    });
    this.filledExceed = this.vpMaster.TotalWorking > this.vpMaster.TotalSanctioned ? true : false;
    if (this.filledExceed) {
      this.vpMaster.TotalWorking = totalFilled;
    }
  }
  public setEmpModeDetail() {
    this.vpMaster.VPDetails.forEach(vpDetail => {
      let empMode = this.employementModes.find(x => x.Id == vpDetail.EmpMode_Id);
      if (empMode) {
        empMode.totalWorking = vpDetail.TotalWorking;
        empMode.profiles = vpDetail.WorkingProfiles;
      }
    });
    this.loadingVpMaster = false;
  }
  public action(status) {
    this.profilesWindow.dialogOpened = false;
  }
  public showProfileClicked(empModeId: number) {
    this.profiles = [];
    this.showProfileEmpMode_Id = empModeId;
    if (this.showProfileEmpMode_Id == 0) {
      this.profiles = this.profilesWindow.profiles.slice();
    } else {
      this.profiles = this.profilesWindow.profiles.filter(x => x.EmpMode_Id == this.showProfileEmpMode_Id).slice();
    }
    this.showProfile = !this.showProfile;
  }
  public showLogClicked() {
    this.showProfile = false;
    this.showLog = !this.showLog;
  }
  public loadPhoto(src: string) {
    let photo = new Image();
    photo.src = src;
    photo.onload = () => {
      return true;
    }
  }

  public onTabSelect(e) {
    console.log(e);
    //this.selectedTab = e.heading;
  }

  public openInNewTab(link: string) {
    window.open('/' + link, '_blank');
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
  private handleError(err: any) {
    this.loading = false;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }
  ngOnDestroy(): void {
    this.searchSubcription.unsubscribe();
  }
}
