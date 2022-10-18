import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';

import { SortDescriptor, orderBy } from '@progress/kendo-data-query';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { AuthenticationService } from '../../_services/authentication.service';
import { RootService } from '../../_services/root.service';
import { DropDownsHR } from '../../_helpers/dropdowns.class';
import { VacancyPositionService } from './vacancy-position.service';
import { aggregateBy } from '@progress/kendo-data-query';
import { Subscription } from 'rxjs/Subscription';
import { trigger } from '@angular/animations';
import { ActivatedRoute } from '@angular/router';


@Component({
  selector: 'app-vacancy-position',
  templateUrl: './vacancy-position.component.html',
  styles: []
})
export class VacancyPositionComponent implements OnInit, OnDestroy {

  public loading = true;
  public vacancyReport: any[] = [];
  public vacancyReportDetail: any[] = [];
  public vacancyReportGrandTotal: any = {};
  public vacancyDynamicReport: any[] = [];
  public vacancyDynamicReportGrandTotal: any[] = [];
  public vacancyTotal: any;
  public vacancyDetailTotal: any;
  public vacancyAggregates: any[] = [];
  public gridView: GridDataResult;
  //sorting variable
  public multiple = false;
  public allowUnsort = true;
  public sort: SortDescriptor[] = [];
  public showFilters: boolean = false;
  public hfmisCode: string = '0';
  public detailReportType: string = '';
  public hfTypeCodes: any[] = [];
  public dropDowns: DropDownsHR = new DropDownsHR();
  private subscription: Subscription;
  public selectedFiltersModel: any;
  public currentUser: any;
  public vacancyDetail: any = {};
  constructor(private _rootService: RootService, private _vacancyPositionService: VacancyPositionService,
    private _authenticationService: AuthenticationService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.currentUser = this._authenticationService.getUser();
    this.fetchParams();
    this.loadDropdownValues();

  }
  private fetchParams() {
    this.loading = true;
    /* this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('geo')) {
          this.vacancyDetail.GeoColumn = params['geo'];
          if (params.hasOwnProperty('geoname')) {
            this.vacancyDetail.GeoLevel = params['geoname'];
          }
          let dto = { column: this.vacancyDetail.GeoColumn, geoLevelName: this.vacancyDetail.GeoLevel, type: 'DesignationFacility' }
          this._vacancyPositionService.getVacancyReportDetail(dto).subscribe((res: any) => {
            if (res) {
              console.log(res);
              this.vacancyReportDetail = res;
              this.setVacancyDetailAggregates();
              this.gridView = { data: this.vacancyReportDetail, total: this.vacancyReportDetail.length };
              this.loading = false;
            }
          }, err => {
            console.log(err);
          });
        } else {
          this.getReport(this.currentUser.HfmisCode);
        }
      }
    ); */
    this.subscription = this.route.queryParams.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('g')) {
          this.vacancyDetail.GeoColumn = params['g'];

          if (params.hasOwnProperty('l')) {
            this.vacancyDetail.GeoLevel = params['l'];
          }
          if (params.hasOwnProperty('d')) {
            this.vacancyDetail.DesingationIds = params['d'];
            if (this.vacancyDetail.DesingationIds) {
              let s = this.vacancyDetail.DesingationIds.split(',');
              this.vacancyDetail.DesingationIdsLength = s.length;
              if (this.vacancyDetail.DesingationIdsLength > 1) {
                this.detailReportType = 'DesignationFacility';
              }
            } else {
              this.detailReportType = 'Facility';
            }
          } else {
            this.detailReportType = 'Facility';
          }
          if (params.hasOwnProperty('c')) {
            this.vacancyDetail.clickType = params['c'];
          }
          let dto = {
            column: this.vacancyDetail.GeoColumn, geoLevelName: this.vacancyDetail.GeoLevel, type: this.detailReportType,
            designationIds: this.vacancyDetail.DesingationIds ? this.vacancyDetail.DesingationIds.split(',') : [],
            clickType: this.vacancyDetail.clickType
          };
          this._vacancyPositionService.getVacancyReportDetail(dto).subscribe((res: any) => {
            if (res) {
              this.vacancyReportDetail = res;
              console.log(this.vacancyReportDetail);
              this.setVacancyDetailAggregates();
              this.gridView = { data: this.vacancyReportDetail, total: this.vacancyReportDetail.length };
              this.gridView.data.forEach(element => {
                if (element.TotalProfile > 0) {
                  this.getProfiles(element.HfId, element.DesignationID, element);
                }
              });
              this.loading = false;
            }
          }, err => {
            console.log(err);
          });
        } else {
          this.getReport(this.currentUser.HfmisCode);
        }
      }
    );
  }
  public getProfiles = (hfId, desigId, dataItem) => {
    this._vacancyPositionService.getProfilesAgainstVacancy(hfId, desigId).subscribe((data: any) => {
      if (data) {
        dataItem.profiles = data;
      }
    }, err => {
      console.log(err);

    })
  }
  public openInNewTab(link: string) {
    window.open('/' + link, '_blank');
  }
  private loadDropdownValues = () => {
    this.getDivisions(this.currentUser.HfmisCode);
    this.getDistricts(this.currentUser.HfmisCode);
    this.getTehsils(this.currentUser.HfmisCode);
    this.getHFTypes();
  }
  private getReport = (code: string) => {
    this._rootService.getVacancyReport(code).subscribe((res: any) => {
      this.vacancyReport = res;
      this.setVacancyAggregates();
      /*   this.vacancyReport = res.reportModel;
        this.vacancyReportGrandTotal.Sanctioned = res.GrandTotalSanctioned;
        this.vacancyReportGrandTotal.Working = res.GrandTotalWorking;
        this.vacancyReportGrandTotal.Profiles = res.GrandTotalProfiles;
        this.vacancyReportGrandTotal.Vacant = res.GrandTotalVacant;
        console.log(res); */

      this.gridView = { data: this.vacancyReport, total: this.vacancyReport.length };
      this.loading = false;
    },
      err => { this.handleError(err); }
    );
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
  private getHFTypes = () => {
    this._rootService.getHFTypes().subscribe((res: any) => {
      this.dropDowns.hfTypes = res;
      this.dropDowns.hfTypesData = this.dropDowns.hfTypes.slice();
    },
      err => { this.handleError(err); }
    );
  }
  public dropdownValueChanged = (value, filter) => {
    if (!value || !value.Code) {
      return;
    }
    if (filter == 'division') {
      this.hfmisCode = value.Code;
      this.resetDrops(filter);
      this.getDistricts(value.Code);
      this.getTehsils(value.Code);
    }
    if (filter == 'district') {
      this.hfmisCode = value.Code;
      this.resetDrops(filter);
      this.getTehsils(value.Code);
    }
    if (filter == 'tehsil') {
      this.hfmisCode = value.Code;
    }
    if (filter == 'hfType') {
      this.hfmisCode += value.Code;
    }
  }
  private getHFCatgetCategoryegory = () => {

  }

  public sortDetailChange(sort: SortDescriptor[]): void {
    if (sort[0].field == 'asd') { return; }
    this.sort = sort;
    this.sortDetailData();
  }
  public sortChange(sort: SortDescriptor[]): void {
    if (sort[0].field == 'asd') { return; }
    this.sort = sort;
    this.sortData();
  }
  private sortData() {
    this.gridView = {
      data: orderBy(this.vacancyReport, this.sort),
      total: this.vacancyReport.length
    };
  }
  public onSearch = () => {

  }
  private sortDetailData() {
    this.gridView = {
      data: orderBy(this.vacancyReportDetail, this.sort),
      total: this.vacancyReportDetail.length
    };
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
      this.dropDowns.divisionsData = this.dropDowns.divisions.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
    if (filter == 'district') {
      this.dropDowns.districtsData = this.dropDowns.districts.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
    if (filter == 'tehsil') {
      this.dropDowns.tehsilsData = this.dropDowns.tehsils.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
  }
  private handleError(err: any) {
    this.loading = false;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }
  public setVacancyAggregates() {
    this.vacancyAggregates = [
      { field: 'TotalSanctioned', aggregate: 'sum' },
      { field: 'TotalWorking', aggregate: 'sum' },
      { field: 'TotalVacant', aggregate: 'sum' },
      { field: 'TotalProfile', aggregate: 'sum' }

    ];
    this.vacancyTotal = aggregateBy(this.vacancyReport, this.vacancyAggregates);
  }

  public setVacancyDetailAggregates() {
    this.vacancyAggregates = [
      { field: 'TotalSanctioned', aggregate: 'sum' },
      { field: 'TotalWorking', aggregate: 'sum' },
      { field: 'TotalVacant', aggregate: 'sum' },
      { field: 'TotalProfile', aggregate: 'sum' }

    ];
    this.vacancyDetailTotal = aggregateBy(this.vacancyReportDetail, this.vacancyAggregates);
  }
  public numberWithCommas(x) {
    if (x) {
      return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }
  }
  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
