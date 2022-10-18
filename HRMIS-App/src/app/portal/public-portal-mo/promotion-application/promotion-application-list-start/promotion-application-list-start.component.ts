import { Component, OnInit } from '@angular/core';
import { PublicPortalMOService } from '../../public-portal-mo.service';
import { KGridHelper } from '../../../../_helpers/k-grid.class';
import { SortDescriptor, orderBy } from '@progress/kendo-data-query';
import { PageChangeEvent } from '@progress/kendo-angular-grid';

@Component({
  selector: 'hrmis-promotion-application-list-start',
  templateUrl: './promotion-application-list-start.component.html',
  styles: [``]
})
export class PromotionApplicationListStartComponent implements OnInit {

  public isLoading: boolean = false;
  public promoApps: any[] = [];
  public gridView: any = { data: [], total: 0 };
  public kGrid: KGridHelper = new KGridHelper();
  public paging: any = { currentPage: 1, itemsPerPage: 100, maxLinkSize: 25, totalRecords: 0, rowsPerPageOptions: [100, 200, 300] };
  public openSearch = '';
  public dateStart: any = '';
  public dateEnd: any = '';
  constructor(private common: PublicPortalMOService) { }

  ngOnInit() {
    this.loadData();
  }

  paginate(event, paginator) {
    this.paging.currentPage = event.page + 1;
    this.loadData();
  }
  addDashes(f_val) { return f_val.slice(0, 5) + "-" + f_val.slice(5, 12) + "-" + f_val.slice(12); }
  loadData() {
    this.isLoading = true;
    this.promoApps = [];
    let skip = ((this.paging.currentPage - 1) * this.paging.itemsPerPage);
    let dateStartString = '', dateEndString = '';
    if (this.dateStart instanceof Date) {
      dateStartString = this.dateStart.toDateString();
    }
    if (this.dateEnd instanceof Date) {
      dateEndString = this.dateEnd.toDateString();
    }
    let url = '/promotionapplication/GetPromoApps?skip=' + skip + '&SearchPhrase=' + encodeURI(this.openSearch) + '&dateStart=' + (dateStartString) + '&endDate=' + dateEndString;
    this.common.getPromoApps(this.kGrid.skip, this.openSearch, dateStartString, dateEndString).subscribe((x: any) => {
      this.isLoading = false;
      if (x.Type === 'Success') {
        this.promoApps = x.Data;
        this.paging.totalRecords = x.TotalRecords;
        this.kGrid.gridView = { data: this.promoApps, total: x.TotalRecords };
      }
    });
  }
  public sortChange(sort: SortDescriptor[]): void {
    if (sort[0].field == 'asd') { return; }
    this.kGrid.sort = sort;
    this.sortData();
  }
  private sortData() {
    this.kGrid.gridView = {
      data: orderBy(this.kGrid.data, this.kGrid.sort),
      total: this.kGrid.totalRecords
    };
  }
  public pageChange(event: PageChangeEvent): void {
    this.kGrid.skip = event.skip;
    this.loadData();
  }
  public changePagesize(value: any) {
    this.kGrid.pageSize = +value;
    this.kGrid.skip = 0;
    this.loadData();
  }
  public dashifyCNIC(cnic: string) {
    if (!cnic) return '';
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
  clear() {
    this.dateStart = '';
    this.dateEnd = '';
    this.openSearch = '';
    this.loadData();
  }
}