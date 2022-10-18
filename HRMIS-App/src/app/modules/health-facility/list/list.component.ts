import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';

import { SortDescriptor, orderBy } from '@progress/kendo-data-query';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { AuthenticationService } from '../../../_services/authentication.service';
import { RootService } from '../../../_services/root.service';
import { from } from 'rxjs/observable/from';
import { delay } from 'rxjs/operators/delay';
import { switchMap } from 'rxjs/operators/switchMap';
import { map } from 'rxjs/operators/map';

import { HealthFacilityService } from '../health-facility.service';
import { HealthFacility } from '../health-facility.class';
import { Subscription } from 'rxjs/Subscription';
import { ActivatedRoute } from '@angular/router';
import { LocalService } from '../../../_services/local.service';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styles: [`
  .k-grid-header{
    display: none !important;
  }
  .k-grid {
    border: 1px solid #e9e9e9 !important;
  }
  `]
})
export class ListComponent implements OnInit, OnDestroy {

  //template variables
  @ViewChild('hfTypeList', {static: true}) hfTypeList;
  @ViewChild('hfCategoryList', {static: false}) hfCategoryList;
  @ViewChild('hfacList', {static: true}) hfacList;
  @ViewChild('excelexportVacancy', {static: false}) excelexportVacancy;
  //loading flag
  public loading = false;
  //grid data
  public gridView: GridDataResult;
  public currentUser: any;
  public userViewType: number = 3;
  public viewTypes: number[] = [1, 2, 3];
  public healthFacilitiesAll: any[] = [];
  public healthFacilities: any[] = [];
  public vacancy: any[] = [];
  private subscription: Subscription;
  //sorting variable
  public multiple = false;
  public allowUnsort = true;
  public sort: SortDescriptor[] = [];
  //Pagination variables
  public pageSizes = [5, 10, 25, 50];
  public totalRecords = 0;
  public pageSize = 10;
  public skip = 0;
  //dropdowns variables
  public hfmisCode: string;
  public hfStatus: string;
  public hfmisCodeParam: string;
  public hfTypeCodes: any[] = [];
  public hfCategoryCodes: any[] = [];
  public hfACs: any[] = [];
  //dropdowns : need two same arrays for search/filter purpose
  public divisions: Array<{ text: string, value: string }>;
  public divisionsData: Array<{ text: string, value: string }>;
  public districts: Array<{ text: string, value: string }>;
  public districtsData: Array<{ text: string, value: string }>;
  public tehsils: Array<{ text: string, value: string }>;
  public tehsilsData: Array<{ text: string, value: string }>;
  public hfTypes: Array<{ text: string, value: string }>;
  public hfTypesData: Array<{ text: string, value: string }>;
  public hfCategories: Array<{ text: string, value: string }>;
  public hfCategoriesData: Array<{ text: string, value: string }>;
  public hfacs: Array<{ text: string, value: string }>;
  public hfacsData: Array<{ text: string, value: string }>;
  //selected object for drop downs
  public selectedFiltersModel: any = {
    division: { Name: 'Select Division', Code: '0' },
    district: { Name: 'Select District', Code: '0' },
    tehsil: { Name: 'Select Tehsil', Code: '0' }
  };
  public selectedVacancy: any = {};
  constructor(private _localService: LocalService, private _rootService: RootService, private route: ActivatedRoute, private _hfService: HealthFacilityService, private _authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.currentUser = this._authenticationService.getUser();
    this.hfmisCode = this.currentUser.HfmisCode;
    this.fetchParams();
  }
  private fetchParams() {
    this.subscription = this.route.queryParams.subscribe(
      (queryParam: any) => {
        if (queryParam['h']) {
          this.hfmisCodeParam = queryParam['h'];
          this.setParamValues();
          if (queryParam['t']) {
            let typeCode = queryParam['t'];
            if (typeCode != '0') {
              this.hfTypeCodes.push(queryParam['t']);
            }
          }
        }
        this.initializeProps();
      }
    );
  }
  private initializeProps() {
    this.getSetSavedView();
    this.loadDropdownValues();
    this.loadHealthFacilities();
    this.multiDropFilter();
  }
  private loadDropdownValues = () => {
    this.getDivisions();
    this.getDistricts();
    this.getTehsils();
    this.getHFTypes();
    this.getHFCategories();
    this.getHFAC();
  }
  private getDivisions = () => {
    this.divisions = [];
    this.divisionsData = [];
    this._rootService.getDivisions(this.hfmisCode).subscribe((res: any) => {
      this.divisions = res;
      this.divisionsData = this.divisions.slice();
      if (this.divisions.length == 1) {
        this.selectedFiltersModel.division = this.divisionsData[0];
      }
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
      if (this.districts.length == 1) {
        this.selectedFiltersModel.district = this.districtsData[0];
      }
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
      if (this.tehsils.length == 1) {
        this.selectedFiltersModel.tehsil = this.tehsilsData[0];
      }
    },
      err => { this.handleError(err); }
    );
  }
  public setParamValues() {
    if (this.hfmisCodeParam.length == 3) {
      this._rootService.getDivisions(this.hfmisCodeParam).subscribe(_ => this.selectedFiltersModel.division = _[0]);
    } else if (this.hfmisCodeParam.length == 6) {
      this._rootService.getDistricts(this.hfmisCodeParam).subscribe(_ => this.selectedFiltersModel.district = _[0]);
    } else if (this.hfmisCodeParam.length == 9) {
      this._rootService.getTehsils(this.hfmisCodeParam).subscribe(_ => this.selectedFiltersModel.tehsil = _[0]);
    }
  }
  private getHFTypes = () => {
    this._rootService.getHFTypes().subscribe((res: any) => {
      this.hfTypes = res;
      this.hfTypesData = this.hfTypes.slice();

    },
      err => { this.handleError(err); }
    );
  }
  private getHFCategories = () => {
    this._rootService.getHFCategories().subscribe((res: any) => {
      this.hfCategories = res;
      this.hfCategoriesData = this.hfCategories.slice();
    },
      err => { this.handleError(err); }
    );
  }
  debugger
  private getHFAC = () => {
    this._rootService.getHFAC().subscribe((res: any) => {
      this.hfacs = res;
      this.hfacsData = this.hfacs.slice();
    },
      err => { this.handleError(err); }
    );
  }
  private loadHealthFacilities() {
    this.loading = true;
    this._hfService.getHealthFacilities(this.skip, this.pageSize, this.hfmisCodeParam ? this.hfmisCodeParam : this.hfmisCode, this.hfTypeCodes,
      this.hfCategoryCodes, this.hfACs, this.hfStatus).subscribe(
      response => this.handleResponse(response),
      err => this.handleError(err)
    );
  }
  private handleResponse(response: any) {
    this.healthFacilities = [];
    this.healthFacilities = response.List;

    this.totalRecords = response.Count;
    this.gridView = { data: this.healthFacilities, total: this.totalRecords };
    this.loading = false;
    this._hfService.getHealthFacilities(0, this.totalRecords, this.hfmisCodeParam ? this.hfmisCodeParam : this.hfmisCode, this.hfTypeCodes,
      this.hfCategoryCodes, this.hfACs, this.hfStatus).subscribe(
      (response: any) => {
        this.healthFacilitiesAll = [];
        this.healthFacilitiesAll = response.List;
      },
      err => this.handleError(err)
    );
  }
  private handleError(err: any) {
    this.loading = false;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }
  public getPhoto(id) {
    //this._hfService.getHFPhoto(id).subscribe(_ => { return _; });
  }
  public dropdownValueChanged = (value, filter) => {
    if (!value || !value.Code) {
      return;
    }
    this.hfmisCodeParam = undefined;
    this.hfmisCode = value.Code;

    if (filter == 'division') {
      this.selectedFiltersModel.district.Code = this.hfmisCode;
      this.resetDrops(filter);
      this.getDistricts();
      this.getTehsils();
    }
    if (filter == 'district') {
      this.selectedFiltersModel.tehsil.Code = this.hfmisCode;
      this.resetDrops(filter);
      this.getTehsils();
    }
  }
  private resetDrops = (filter: string) => {
    if (filter == 'division') {
      this.selectedFiltersModel.district = { Name: 'Select District', Code: this.currentUser.HfmisCode };
      this.selectedFiltersModel.tehsil = { Name: 'Select Tehsil', Code: this.currentUser.HfmisCode };
    }
    if (filter == 'district') {
      this.selectedFiltersModel.tehsil = { Name: 'Select Tehsil', Code: this.currentUser.HfmisCode };
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
  private multiDropFilter = () => {
    this.hfTypeList.filterChange.asObservable().pipe(
      switchMap(value => from([this.hfTypes]).pipe(
        delay(300),
        map((data) => data.filter(contains(value)))
      ))
    ).subscribe(x => {
      this.hfTypesData = x;
    });
    this.hfacList.filterChange.asObservable().pipe(
      switchMap(value => from([this.hfacs]).pipe(
        delay(300),
        map((data) => data.filter(contains(value)))
      ))
    ).subscribe(x => {
      this.hfacsData = x;
    });
   /*  this.hfCategoryList.filterChange.asObservable().pipe(
      switchMap(value => from([this.hfCategories]).pipe(
        delay(300),
        map((data) => data.filter(contains(value)))
      ))
    ).subscribe(x => {
      this.hfCategoriesData = x;
      this.onSearch();
    }); */
    const contains = value => (s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1;

  }
  public onSearch() {
    this.hfmisCodeParam = undefined;
    this.skip = 0;
    this.loadHealthFacilities();
  }
  public getVacancy(dataItem: any) {
    this.selectedVacancy = dataItem;
    dataItem.loading = true;
    this.vacancy = [];
    this._hfService.getHFVacancy(this.selectedVacancy.HFMISCode).subscribe((data: any) => {
      debugger
      if (data) {
        console.log(data);
        this.selectedVacancy.vacancy = data;
        this.vacancy = data;
        this.excelexportVacancy.save();
        dataItem.loading = false;
        this.selectedVacancy = null;
      }
    }, err => {
      this.handleError(err);
    });
  }
  public sortChange(sort: SortDescriptor[]): void {
    if (sort[0].field == 'asd') { return; }
    this.sort = sort;
    this.sortData();
  }
  private sortData() {
    this.gridView = {
      data: orderBy(this.healthFacilities, this.sort),
      total: this.totalRecords
    };
  }
  public changeView(value: number) {
    this._localService.set('hfViewType', value);
    this.getSetSavedView();
  }
  public getSetSavedView() {
    let savedView = this._localService.get('hfViewType');
    this.userViewType = +savedView ? +savedView : 3;
  }
  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadHealthFacilities();
  }
  public changePagesize(value: any) {
    this.pageSize = +value;
    this.skip = 0;
    this.loadHealthFacilities();
  }
  public hfClicked(HFMISCode: string, type: number) {
    type === 1 ?
      window.open('/health-facility/' + HFMISCode + '/edit', '_blank') :
      window.open('/health-facility/' + HFMISCode, '_blank');
  }
  // helpers
  get showInitialPageSize() {
    return this.pageSizes
      .filter(num => num === Number(this.pageSize))
      .length === 0;
  }
  public numberWithCommas(x) {
    if (x) {
      return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }
  }
  ngOnDestroy() {
    this.hfmisCodeParam = undefined;
    this.subscription.unsubscribe();
  }
}
