import { Component, OnInit, OnDestroy } from '@angular/core';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { Subject } from 'rxjs/Subject';
import { Subscription } from 'rxjs/Subscription';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { RootService } from '../../../_services/root.service';
import { NotificationService } from '../../../_services/notification.service';
import { AuthenticationService } from '../../../_services/authentication.service';

@Component({
  selector: 'app-covid-facilities',
  templateUrl: './covid-facilities.component.html',
  styleUrls: ['./covid-facilities.component.scss']
})
export class CovidFacilitiesComponent implements OnInit, OnDestroy {
  public dropDowns: DropDownsHR = new DropDownsHR();
  public user: any = {};
  public newFacility: any = {};
  public healthFacilityFullName = '';
  public hfsList: any[] = [];
  public covidFacilities: any[] = [];
  public searchEvent = new Subject<any>();
  public searchSubcription: Subscription = null;
  public searchingHfs: boolean = false;
  public saving: boolean = false;
  public hfmisCode: string = '0';
  constructor(private _rootService: RootService,
    private _authenticationService: AuthenticationService,
    private _notificationService: NotificationService) { }

  ngOnInit() {
    this.user = this._authenticationService.getCurrentOfficer();
    this.hfmisCode = this._authenticationService.getUserHfmisCode();
    this.handleSearchEvents();
    this.getTypes();
    this.getCovidFacilities();
  }
  public submit() {
    this.saving = true;
    this._rootService.saveCovidFacility(this.newFacility).subscribe((res: any) => {
      if (res.Id > 0) {
        this.newFacility = {};
        this._notificationService.notify('success', 'Saved Successfully!');
        this.getCovidFacilities();
      } else {
        this._notificationService.notify('danger', 'Not Saved!');
      }
      this.saving = false;
    }, err => {
      this.saving = false;
      this._notificationService.notify('danger', 'Not Saved!');
      console.log(err);
    });
  }
  public dropdownValueChanged = (value, filter) => {
    if (!value) {
      return;
    }
    if (filter == 'type') {
      this.newFacility.TypeId = value.Id;
      this.newFacility.Name = value.Name;
    }
  }
  public getTypes() {
    this._rootService.getCovidFacilityTypes().subscribe((res: any) => {
      if (res) {
        this.dropDowns.covidFacilityTypes = res;
      }
    }, err => {
      console.log(err);
    });
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
  
  public removeCovidFacility(dataItem: any) {
    if (confirm('Confirm Remove?')) {
      dataItem.removing = true;
      this._rootService.removeCovidFacility(dataItem.Id).subscribe((res: any) => {
        if (res) {
          this.getCovidFacilities();
        }
        dataItem.removing = false;
      }, err => {
        console.log(err);
        dataItem.removing = false;
      });
    }
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
  }
  public searchClicked(FullName, filter) {

    if (filter == 'hfs') {
      let item = this.hfsList.find(x => x.FullName === FullName);
      if (item) {
        this.newFacility.HF_Id = item.Id;
      }
    }
  }
  ngOnDestroy() {
    if (this.searchSubcription) {
      this.searchSubcription.unsubscribe();
    }
  }

}
