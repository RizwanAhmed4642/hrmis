import { Component, OnInit, OnDestroy } from '@angular/core';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { Subject } from 'rxjs/Subject';
import { Subscription } from 'rxjs/Subscription';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { RootService } from '../../../_services/root.service';
import { NotificationService } from '../../../_services/notification.service';
import { AuthenticationService } from '../../../_services/authentication.service';

@Component({
  selector: 'app-covid-staff',
  templateUrl: './covid-staff.component.html',
  styleUrls: ['./covid-staff.component.scss']
})
export class CovidStaffComponent implements OnInit, OnDestroy {
  public dropDowns: DropDownsHR = new DropDownsHR();
  public user: any = {};
  public newFacility: any = {};
  public newStaff: any = {};
  public attachedPersonName = '';
  public hfsList: any[] = [];
  public covidStaff: any[] = [];
  public covidFacilities: any[] = [];
  public covidStaffList: any[] = [];
  public profileList: any[] = [];
  public selectedFacility: any = {};
  public profile: any = {};
  public addNewStaff: boolean = false;
  public windowOpen: boolean = false;
  public searchEvent = new Subject<any>();
  public searchSubcription: Subscription = null;
  public searchingHfs: boolean = false;
  public saving: boolean = false;
  public searchingProfile: boolean = false;
  public hfmisCode: string = '0';
  constructor(private _rootService: RootService,
    private _authenticationService: AuthenticationService,
    private _notificationService: NotificationService) { }

  ngOnInit() {
    this.user = this._authenticationService.getUser();
    this.hfmisCode = this._authenticationService.getUserHfmisCode();
    this.handleSearchEvents();
    this.getCovidFacilities();
    this.getEmploymentModes();
  }
  public saveStaff() {
    this.saving = true;
    this.newStaff.CovidFacilityId = this.selectedFacility.Id;
    this._rootService.saveCovidStaff(this.newStaff).subscribe((res: any) => {
      if (res.Id > 0) {
        this.addNewStaff = false;
        this.newStaff = {};
        this._notificationService.notify('success', 'Saved Successfully!');
        this.getCovidStaff();
      } else {
        this._notificationService.notify('danger', 'Not Saved!');
      }
      this.saving = false;
      this.addNewStaff = true;
    }, err => {
      this.saving = false;
      this._notificationService.notify('danger', 'Not Saved!');
      console.log(err);
    });
  }
  public openWindow(dataItem: any, addNewStaff: boolean) {
    this.selectedFacility = dataItem;
    this.addNewStaff = addNewStaff;
    this.getCovidStaff();
    this.windowOpen = true;
  }
  public closeWindow() {
    this.windowOpen = false;
    this.addNewStaff = false;
    this.selectedFacility = {};
  }
  public dropdownValueChanged = (value, filter) => {
    if (!value) {
      return;
    }
    if (filter == 'employementMode') {
      this.newStaff.EmpModeId = value.Id;
    }
  }
  private getEmploymentModes = () => {
    this.dropDowns.employementModes = [];
    this.dropDowns.employementModesData = [];
    this._rootService.getEmploymentModes().subscribe((res: any) => {
      this.dropDowns.employementModes = res;
      this.dropDowns.employementModesData = this.dropDowns.employementModes;
    },
      err => { console.log(err); }
    );
  }
  public getCovidFacilities() {
    this._rootService.getCovidFacilities(this.hfmisCode).subscribe((res: any) => {
      if (res) {
        this.covidFacilities = res;
      }
    }, err => {
      console.log(err);
    });
  }
  public removeCovidStaff(dataItem: any) {
    if (confirm('Confirm Remove?')) {
      dataItem.removing = true;
      this._rootService.removeCovidStaff(dataItem.Id).subscribe((res: any) => {
        if (res) {
          this.getCovidStaff();
        }
        dataItem.removing = false;
      }, err => {
        console.log(err);
        dataItem.removing = false;
      });
    }
  }
  public getCovidStaff() {
    this._rootService.getCovidStaff(this.selectedFacility.Id).subscribe((res: any) => {
      if (res) {
        this.covidStaffList = res;
      }
    }, err => {
      console.log(err);
    });
  }
  public handleSearchEvents() {
    this.searchSubcription = this.searchEvent.pipe(
      debounceTime(400)).subscribe((x: any) => {
        this.search(x.event, x.filter);
      });
  }
  public search(value: string, filter: string) {
    if (filter == 'cnic') {
      this.profileList = [];
      if (value.length > 2) {
        this.profile = {};
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
    if (filter == 'cnic') {
      let prf = this.profileList.find(x => x.EmployeeName == FullName);
      if (prf) {
        this.newStaff.ProfileId = prf.Id;
        this.newStaff.CNIC = prf.CNIC;
        this.profile = prf;
      }
    }
  }
  ngOnDestroy() {
    if (this.searchSubcription) {
      this.searchSubcription.unsubscribe();
    }
  }

}
