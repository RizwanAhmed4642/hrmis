import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { SortDescriptor, orderBy } from '@progress/kendo-data-query';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { AuthenticationService } from '../../../../_services/authentication.service';
import { RootService } from '../../../../_services/root.service';
import { from } from 'rxjs/observable/from';
import { delay } from 'rxjs/operators/delay';
import { switchMap } from 'rxjs/operators/switchMap';
import { map } from 'rxjs/operators/map';
import { ProfileService } from '../../../profile/profile.service';
import { ProfileCompactView } from '../profile.class';
import { Subscription } from 'rxjs/Subscription';
import { ActivatedRoute } from '@angular/router';
import { LocalService } from '../../../../_services/local.service';
import { Subject } from 'rxjs/Subject';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { DropDownsHR } from '../../../../_helpers/dropdowns.class';
import { AttandanceService } from "../../attandance.service";

@Component({
  selector: 'app-emp-list',
  templateUrl: './emp-list.component.html',
  styles: []
})
export class EmpListComponent implements OnInit, OnDestroy {
  @ViewChild("cadresList", { static: true }) cadresList;
  @ViewChild("designationsList", { static: true }) designationsList;
  @ViewChild("statusList", { static: true }) statusList;
  @ViewChild("employementModesList", { static: true }) employementModesList;
  @ViewChild("qualificationsList", { static: true }) qualificationsList;
  @ViewChild("specializationList", { static: true }) specializationList;

  public currentUser: any;
  private subscription: Subscription;
  //loading flag
  public loading = false;
  public searching = false;
  //grid data
  public gridView: GridDataResult;
  public profiles: ProfileCompactView[] = [];
  public userViewType: number = 2;
  public viewTypes: number[] = [1, 2, 3];
  //sorting variable
  public multiple = false;
  public allowUnsort = true;
  public sort: SortDescriptor[] = [];
  //Pagination variables
  public pageSizes = [50, 100, 200, 500];
  public totalRecords = 0;
  public pageSize = 50;
  public skip = 0;
  //dropdowns variables
  public dropDowns: DropDownsHR = new DropDownsHR();
  public param: string = '';
  public hfmisCode: string = '000000';
  public searchTerm: string = '';
  public selectedCadres: any[] = [];
  public selectedDesignations: any[] = [];
  public selectedStatuses: any[] = [];
  public selectedEmployementModes: any[] = [];
  //dropdowns : need two same arrays for search/filter purpose
  public divisions: Array<{ text: string, value: string }>;
  public divisionsData: Array<{ text: string, value: string }>;
  public districts: Array<{ text: string, value: string }>;
  public districtsData: Array<{ text: string, value: string }>;
  public tehsils: Array<{ text: string, value: string }>;
  public tehsilsData: Array<{ text: string, value: string }>;
  public cadres: Array<{ text: string, value: string }>;
  public cadresData: Array<{ text: string, value: string }>;
  public designations: Array<{ text: string, value: string }>;
  public designationsData: Array<{ text: string, value: string }>;
  //selected object for drop downs
  public selectedFiltersModel: any = {
    division: { Name: 'Select Division', Code: '0' },
    district: { Name: 'Select District', Code: '0' },
    tehsil: { Name: 'Select Tehsil', Code: '0' }
  };
  public isPool: boolean = false;
  public isInactive: boolean = false;
  public inputChange: Subject<any>;
  private searchSubscription: Subscription;
  constructor(private _localService: LocalService, private route: ActivatedRoute, private _attandanceService: AttandanceService, private _rootService: RootService, private _profileService: ProfileService, private _authenticationService: AuthenticationService) { }

  ngOnInit() {
    //debugger;
    this.fetchParams();
    this.currentUser = this._authenticationService.getUser();
    console.log(this.currentUser);
    this.hfmisCode = this.currentUser.HfmisCode;
    this.initializeProps();
    this.subscribeToFilter();
  }
  private fetchParams() {
    //  debugger;

    this.subscription = this.route.params.subscribe(
      (params: any) => {

        if (params.hasOwnProperty('type')) {
          this.param = params['type'];
        }
        if (this.param == "InActive") {
          this.isInactive = true;
          this.loadProfiles();
        } else {
          this.isInactive = false;
          this.loadProfiles();
        }
      }
    );
  }
  private subscribeToFilter() {
    this.inputChange = new Subject();
    this.searchSubscription = this.inputChange.pipe(debounceTime(300)).subscribe((query: boolean) => {
      this.loadProfiles();
    });
  }
  private initializeProps() {
    this.fetchParams();
    this.loadDropdownValues();
    this.getSetSavedView();

    //   this.multiDropFilter();
  }
  private loadDropdownValues = () => {
    this.getDivisions();
    this.getDistricts();
    this.getTehsils();
    this.getCadres();
    this.getDesignations();
    /*   this.getStatuses();
      this.getEmploymentModes();
      this.getSpecializations();
      this.getQualifications(); */
  }
  private getDivisions = () => {
    this.divisions = [];
    this.divisionsData = [];
    this._rootService.getDivisions(this.hfmisCode).subscribe((res: any) => {
      this.divisions = res;
      this.divisionsData = this.divisions.slice();
    },
      err => { this.handleError(err); }
    );
  }
  private getDistricts = () => {
    this.districts = [];
    this.districtsData = [];
    this._rootService.getDistricts(this.hfmisCode).subscribe((res: any) => {
      this.districts = res;
      this.districtsData = this.districts.slice();
    },
      err => { this.handleError(err); }
    );
  }
  private getTehsils = () => {
    this.tehsils = [];
    this.tehsilsData = [];
    this._rootService.getTehsils(this.hfmisCode).subscribe((res: any) => {
      this.tehsils = res;
      this.tehsilsData = this.tehsils.slice();
    },
      err => { this.handleError(err); }
    );
  }
  private getCadres = () => {
    this._rootService.getCadres().subscribe((res: any) => {
      console.log(res);

      this.cadres = res;
      this.cadresData = this.cadres.slice();
    },
      err => { this.handleError(err); }
    );
  }
  private getDesignations = () => {
    this._rootService.getDesignations().subscribe((res: any) => {
      this.designations = res;
      this.designationsData = this.designations.slice();
    },
      err => { this.handleError(err); }
    );
  }
  private getStatuses = () => {
    this._rootService.getProfileStatuses().subscribe((res: any) => {
      this.dropDowns.profileStatuses = res;
      this.dropDowns.profileStatusesData = this.dropDowns.profileStatuses;
    },
      err => { this.handleError(err); }
    );
  }
  private getEmploymentModes = () => {
    this._rootService.getEmploymentModes().subscribe((res: any) => {
      this.dropDowns.employementModes = res;
      this.dropDowns.employementModesData = this.dropDowns.employementModes;
    },
      err => { this.handleError(err); }
    );
  }
  private getSpecializations = () => {
    this._rootService.getSpecializations().subscribe((res: any) => {
      this.dropDowns.specialization = res;
      this.dropDowns.specializationData = this.dropDowns.specialization;
    },
      err => { this.handleError(err); }
    );
  }
  private getQualifications = () => {
    this._rootService.getQualifications().subscribe((res: any) => {
      this.dropDowns.qualifications = res;
      this.dropDowns.qualificationsData = this.dropDowns.qualifications;
    },
      err => { this.handleError(err); }
    );
  }
  public loadProfiles() {
    //  debugger;
    this.loading = true;
    if (this.isInactive) {
      this._attandanceService.getProfilesInActive(this.skip, this.pageSize, this.hfmisCode, this.searchTerm).subscribe(
        response => this.handleResponse(response),
        err => this.handleError(err)
      );
    } else {
      this._attandanceService.getProfiles(this.skip, this.pageSize, this.hfmisCode, this.searchTerm).subscribe(
        response => this.handleResponse(response),
        err => this.handleError(err)
      );
    }
  }
  private handleResponse(response: any) {
    this.profiles = [];
    this.profiles = response.List;
    this.totalRecords = response.Count;
    this.gridView = { data: this.profiles, total: this.totalRecords };
    this.loading = false;
    this.searching = false;
  }
  private handleError(err: any) {
    this.loading = false;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }
  public openInNewTab(link: string) {
    //  debugger;
    //  console.log(link);
    window.open('http://hrmis.pshealthpunjab.gov.pk/' + link, '_blank');
    //  window.open('http://localhost:4200/' + link, '_blank');
  }
  public dropdownValueChanged = (value, filter) => {
    if (!value || !value.Code) {
      return;
    }
    this.hfmisCode = value.Code;

    if (filter == 'division') {
      this.resetDrops(filter);
      this.getDistricts();
      this.getTehsils();
    }
    if (filter == 'district') {
      this.resetDrops(filter);
      this.getTehsils();
    }
  }
  private resetDrops = (filter: string) => {
    if (filter == 'division') {
      this.selectedFiltersModel.district = { Name: 'Select District', Code: '0' };
      this.selectedFiltersModel.tehsil = { Name: 'Select Tehsil', Code: '0' };
    }
    if (filter == 'district') {
      this.selectedFiltersModel.tehsil = { Name: 'Select Tehsil', Code: '0' };
    }
  }
  public handleFilter = (value, filter) => {
    if (filter == 'division') {
      this.divisionsData = this.divisions.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
    if (filter == 'district') {
      this.districtsData = this.districts.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
    if (filter == 'tehsil') {
      this.tehsilsData = this.tehsils.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
  }
  public onSearch() {
    this.searching = true;
    this.skip = 0;
    this.loadProfiles();
  }
  public sortChange(sort: SortDescriptor[]): void {
    if (sort[0].field == 'asd') { return; }
    this.sort = sort;
    this.sortData();
  }
  private sortData() {
    this.gridView = {
      data: orderBy(this.profiles, this.sort),
      total: this.totalRecords
    };
  }
  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadProfiles();
  }
  public changePagesize(value: any) {
    this.pageSize = +value;
    this.skip = 0;
    this.loadProfiles();
  }
  public hfClicked(HFMISCode: string, type: number) {
    type === 1 ?
      window.open('/health-facility/' + HFMISCode + '/edit', '_blank') :
      window.open('/health-facility/' + HFMISCode, '_blank');
  }
  public changeView(value: number) {
    this._localService.set('profileViewType', value);
    this.getSetSavedView();
  }
  public getSetSavedView() {
    let savedView = this._localService.get('profileViewType');
    this.userViewType = +savedView ? +savedView : 1;
  }
  // helpers
  get showInitialPageSize() {
    return this.pageSizes
      .filter(num => num === Number(this.pageSize))
      .length === 0;
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
  public numberWithCommas(x) {
    if (x) {
      return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }
  }
  ngOnDestroy() {
    this.subscription.unsubscribe();
    this.searchSubscription.unsubscribe();
  }

}
