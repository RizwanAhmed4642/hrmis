import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { SortDescriptor, orderBy } from '@progress/kendo-data-query';
import { Subject } from 'rxjs';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { AuthenticationService } from '../../../_services/authentication.service';
import { LocalService } from '../../../_services/local.service';
import { RootService } from '../../../_services/root.service';
import { ProfileCompactView } from '../../profile/profile.class';
import { OrderService } from '../order.service';

@Component({
  selector: 'app-phfmc-order-list',
  templateUrl: './phfmc-order-list.component.html',
})
export class PhfmcOrderListComponent implements OnInit {
  public currentUser: any;
  public inputChange: Subject<any>;
  public dropDowns: DropDownsHR = new DropDownsHR();
  public hfmisCode: string = '000000';
  public searchTerm: string = '';
  public selectedCadres: any[] = [];
  public selectedDesignations: any[] = [];
  public selectedStatuses: any[] = [];
  public selectedEmployementModes: any[] = [];
  public searching = false;

  //grid data
  public gridView: GridDataResult;
  public profiles: ProfileCompactView[] = [];
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
  public loading = false;


  constructor(
    private _localService: LocalService,
    private router: Router,
    private route: ActivatedRoute,
    private _rootService: RootService,
    private _orderService: OrderService,
    private _authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.currentUser = this._authenticationService.getUser();
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

  public onSearch() {
    this.searching = true;
    this.skip = 0;
    this.loadProfiles();
  }

  public loadProfiles() {
    this.loading = true;
    this._orderService.getOrderRequests(this.skip, this.pageSize).subscribe(
      response => this.handleResponse(response),
      err => this.handleError(err)
    );
  }
  public openOrder(orderRequestId: number, esrId: number) {
    this._localService.set('DORId', orderRequestId);
    this.router.navigate(['/order/editor-view/' + esrId + '/1']);
  }
  public downloadOrder(path: string) {
    window.open('/' + path, '_blank');
  }
  private handleResponse(response: any) {
    this.profiles = [];
    this.profiles = response.List;
    this.totalRecords = response.Count;
    this.gridView = { data: this.profiles, total: this.totalRecords };
    this.loading = false;
    this.searching = false;
  }
  public approveOrderRequest(id: number) {
    this.loading = true;
    this._orderService.approveOrderRequest(id).subscribe(
      (response) => {
        this.loadProfiles();
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

  get showInitialPageSize() {
    return this.pageSizes
      .filter(num => num === Number(this.pageSize))
      .length === 0;
  }

}
