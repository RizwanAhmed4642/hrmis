import { Component, OnInit, OnDestroy } from '@angular/core';
import { NotificationService } from '../../../_services/notification.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { RootService } from '../../../_services/root.service';
import { VacancyPositionService } from '../vacancy-position.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Subject } from 'rxjs/Subject';
import { Subscription } from 'rxjs/Subscription';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { VpMProfileView, VpDProfileView, VPHolder, VPHolderView } from '../vacancy-position.class';

@Component({
  selector: 'app-vp-holdings',
  templateUrl: './vp-holdings.component.html',
  styles: []
})
export class VpHoldingsComponent implements OnInit, OnDestroy {


  public searchEvent = new Subject<any>();
  public searchSubcription: Subscription = null;
  public hfsList: any[] = [];
  public searchingHfs: boolean = false;
  public loading: boolean = false;
  public vpHold: any = {};
  public dropDowns: DropDownsHR = new DropDownsHR();
  public vpMaster: VpMProfileView;
  public vpDetails: VpDProfileView[];
  public vpMasterLog: any;
  public vpHolder: VPHolderView = new VPHolderView();
  public vpMasterLogs: any[];
  public holderWindow: boolean = false;
  constructor(public _notificationService: NotificationService,
    private _authenticationService: AuthenticationService,
    private _rootService: RootService,
    private _vacancyService: VacancyPositionService,
    private router: Router,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.handleSearchEvents();
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
        this.vpHold.HealthFacility_Id = item.Id;
        this.getDesignations();
        this.vpHold.HfmisCode = item.HFMISCode;
      }
    }
  }
  public saveVacancyHolder() {
    this._vacancyService.saveVacancyHolder(this.vpHolder).subscribe((res) => {
      if (res) {
        this.closeHolderWindow();
        this._notificationService.notify('success', 'Vacancy Holded');
      }
    }, err => {
      console.log(err);
    });
  }
  private getVpMaster = () => {
    this.loading = true;
    this.vpMaster = null;
    this.vpHolder = new VPHolderView();
    this.vpDetails = null;
    this.vpMasterLog = null;
    this.vpMasterLogs = null;
    this._vacancyService.getVpMaster({
      HealthFacility_Id: this.vpHold.HealthFacility_Id,
      Designation_Id: this.vpHold.Designation_Id,
    }).subscribe((res: any) => {
      if (res) {
        this.vpMaster = res.vpMaster;
        this.getVpHolder();
        this.vpDetails = res.vpDetails;
        this.vpMasterLog = res.vpMasterLog;
        this.vpMasterLogs = res.vpMasterLogs;
      }
      this.loading = false;
    },
      err => { this.handleError(err); }
    );
  }
  private getVpHolder = () => {
    this.vpHolder = new VPHolderView();
    this._vacancyService.getVpHolder({
      Id: this.vpMaster.Id
    }).subscribe((res: any) => {
      if (res) {
        this.vpHolder = res;
      }
      this.loading = false;
    },
      err => { this.handleError(err); }
    );
  }
  private getDesignations = () => {
    this._rootService.getDesignationsFiltered({ HF_Id: this.vpHold.HealthFacility_Id }).subscribe((res: any) => {
      if (res) {
        this.dropDowns.designations = res;
        this.dropDowns.designationsData = this.dropDowns.designations.slice();
      }
    },
      err => { this.handleError(err); }
    );
  }
  public dropdownValueChanged = (value, filter) => {
    if (filter == 'designation') {
      this.vpHold.Designation_Id = value.Id;
      this.vpHold.designationName = value.Name;
      if (this.vpHold.HealthFacility_Id) {
        this.getVpMaster();
      }
    }
  }
  public openHolderWindow(vpDetailId: number) {
    this.vpHolder.VpMaster_Id = this.vpMaster.Id;
    this.vpHolder.TotalSeats = this.vpMaster.TotalSanctioned;
    this.vpHolder.TotalSeatsVacant = this.vpMaster.TotalSanctioned - this.vpMaster.TotalWorking;
    this.vpHolder.VpDetail_Id = vpDetailId;
    this.holderWindow = true;
  }
  public closeHolderWindow() {
    this.holderWindow = false;
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
