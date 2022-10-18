import { GridDataResult } from "@progress/kendo-angular-grid";
import { SortDescriptor, orderBy } from "@progress/kendo-data-query";

export class KGridHelper {
  //loading flag
  public firstLoad = false;
  public loading = false;
  //grid data
  public dataOrigional: any[] = [];
  public data: any[] = [];

  public gridView: GridDataResult;
  //filter
  public filter: any;
  //sorting variable
  public multiple = false;
  public allowUnsort = true;
  public sort: SortDescriptor[] = [];
  //Pagination variables
  public pageSizes = [10, 50, 100, 200, 500, 1000];
  public totalRecords = 0;
  public pageSize = 10;
  public skip = 0;
  public get showInitialPageSize() {
    return this.pageSizes
      .filter(num => num === Number(this.pageSize))
      .length === 0;
  }

  public numberWithCommas(x) {
    if (x) {
      return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }
  }
}