import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { VpMProfileView, VpDProfileView, VPDetail } from '../vacancy-position.class';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { RootService } from '../../../_services/root.service';
import { VacancyPositionService } from '../vacancy-position.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from '../../../_services/authentication.service';
import { Subscription } from 'rxjs/Subscription';
import { Subject } from 'rxjs/Subject';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { HealthFacility } from '../../health-facility/health-facility.class';
import { NotificationService } from '../../../_services/notification.service';

@Component({
  selector: 'app-vacancy-add-edit',
  templateUrl: './add-edit.component.html',
  styles: []
})
export class AddEditComponent implements OnInit, OnDestroy {

  @Input() public isComponent: boolean = false;
  public loading: boolean = true;
  public saving: boolean = false;
  public duplicate: boolean = false;
  public duplicateString: string = '';
  public searchingHfs: boolean = false;
  public filledExceed: boolean = false;
  @Input() public hfmisCode: string = '0000000';
  private subscription: Subscription;
  public searchEvent = new Subject<any>();
  public searchSubcription: Subscription = null;
  public healthFacility: HealthFacility;
  public hfsList: any[] = [];
  @Input() public vpMaster: VpMProfileView;
  public dropDowns: DropDownsHR = new DropDownsHR();
  constructor(private _vacancyPositionService: VacancyPositionService, public _notificationService: NotificationService, private _authenticationService: AuthenticationService, private _rootService: RootService, _vacancyService: VacancyPositionService, private router: Router, private route: ActivatedRoute) { }

  ngOnInit() {
    if (this.isComponent) {
      this.fetchData();
    } else {
      this.handleSearchEvents();
      this.fetchParams();
    }
    this.loadDropdownValues();
  }
  public saveVacancy() {
    this.checkFilledExceed();
    if (this.filledExceed) return;
    this.saving = true;
    this._vacancyPositionService.saveVacancy(this.vpMaster).subscribe((res: any) => {
      if (res && res.Id) {
        this._notificationService.notify('success', 'Vacancy Saved!');
        if (this.healthFacility) {
          this.router.navigate(['/health-facility/' + this.healthFacility.HFMISCode]);
        } else {
          this.router.navigate(['/vacancy-position']);
        }
      } else if (res == 'Duplicate') {
        this.duplicate = true;
        this.duplicateString = this.vpMaster.DsgName + ' is already added.';
        this._notificationService.notify('danger', this.vpMaster.DsgName + ' is already added.');
      }
      this.saving = false;
    }, err => {
      this.handleError(err);
    });
  }
  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('hfcode')) {
          this.hfmisCode = params['hfcode'];
          this.fetchData();
        } else {
          this.initializeProps();
        }
      }
    );
  }
  private fetchData() {
    this._rootService.searchHealthFacilities(this.hfmisCode).subscribe(
      (res: any) => {
        if (res) {
          this.healthFacility = res[0] as HealthFacility;
          if (this.isComponent) {
            this.loading = false;
          } else {
            this.initializeProps();
          }
        }
      },
      err => {
        this.handleError(err);
      }
    );
  }
  public handleSearchEvents() {
    this.searchSubcription = this.searchEvent.pipe(
      debounceTime(400)).subscribe((x: any) => {
        console.log(x);
        if (x.filter == 'hfs') {
          this.search(x.event, x.filter);
        } else if (x.filter == 'TotalSanctioned') {
          this.checkFilledExceed();
        } else if (x.filter == 'employementMode') {
          this.empModeValueChanged(x.Id, +x.event.target.value);
        }
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
    this.duplicate = false;
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
  public initializeProps() {
    this.vpMaster = new VpMProfileView();
    this.vpMaster.TotalSanctioned = 0;
    this.vpMaster.TotalWorking = 0;
    if (this.healthFacility) {
      this.vpMaster.HF_Id = this.healthFacility.Id;
      this.vpMaster.HFMISCode = this.healthFacility.HFMISCode;
    }
    this.vpMaster.VPDetails = [];
    this.loading = false;
  }
  public dropdownValueChanged = (value, filter) => {
    this.duplicate = false;
    if (!value) {
      return;
    }
    if (filter == 'hfs') {
      this._rootService.searchHealthFacilities(value).subscribe((data) => {
        let hfs = data[0] as HealthFacility;
        this.vpMaster.HF_Id = hfs.Id;
        this.vpMaster.HFMISCode = hfs.HFMISCode;
      });
    }
    if (filter == 'postType') {
      this.vpMaster.PostType_Id = value.Id;
    }
    if (filter == 'post') {
      this.vpMaster.Desg_Id = value.Id;
      this.vpMaster.DsgName = value.Name;
      this.vpMaster.Cadre_Id = value.Cadre_Id;
      this.vpMaster.CadreName = value.Cadre_Name;
      this.vpMaster.BPS = value.Scale;
    }
  }
  private loadDropdownValues = () => {
    this.getPostTypes();
    this.getDesignations();
    this.getEmploymentModes();
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
  private handleError(err: any) {
    this.loading = false;
    this.duplicate = false;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }
  public numberWithCommas(x) {
    if (x) {
      return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }
  }
  ngOnDestroy(): void {
    this.subscription.unsubscribe();
    this.searchSubcription.unsubscribe();
  }
}
