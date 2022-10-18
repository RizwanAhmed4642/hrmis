import { Component, OnInit } from '@angular/core';
import { SortDescriptor, orderBy } from '@progress/kendo-data-query';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { ProfileService } from '../profile.service';

@Component({
  selector: 'app-senority',
  templateUrl: './senority.component.html',
})
export class SenorityComponent implements OnInit {

  public loading = false;
  public searching = false;
  //grid data
  public gridView: GridDataResult;
  public profilesAll: any[] = [];
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
  public senorityList: any[] = [];
  constructor(private _profileService: ProfileService,) { }

  ngOnInit() {
    this.getSenority();
  }

  private getSenority = () => {
    this._profileService.getSenority({ Skip: this.skip, PageSize: this.pageSize }).subscribe((res: any) => {
      this.senorityList = [];
      this.senorityList = res.List;
      this.totalRecords = res.Count;
      this.gridView = { data: this.senorityList, total: this.totalRecords };
      this.loading = false;
      console.log('senorityList: ', this.senorityList);
    },
      err => { err; }
    );
  }

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

  public sortChange(sort: SortDescriptor[]): void {
    if (sort[0].field == 'asd') { return; }
    this.sort = sort;
    this.sortData();
  }
  private sortData() {
    this.gridView = {
      data: orderBy([], this.sort),
      total: this.totalRecords
    };
  }
  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
  }
  public changePagesize(value: any) {
    this.pageSize = +value;
    this.skip = 0;
  }

}
