import { Component, OnInit, Input } from '@angular/core';
import { SortDescriptor, orderBy } from '@progress/kendo-data-query';
import { PageChangeEvent, GridDataResult } from '@progress/kendo-angular-grid';
import { HealthFacilityService } from '../../../health-facility.service';
import { RootService } from '../../../../../_services/root.service';
import { AuthenticationService } from '../../../../../_services/authentication.service';
import { NotificationService } from '../../../../../_services/notification.service';

@Component({
  selector: 'app-hf-services',
  templateUrl: './hf-services.component.html',
  styles: [`
  .table th, .table td{
    border: none !important;
    vertical-align: middle !important;
  }
  
  .table tbody tr {
    border-top: 1px solid #c8ced3 !important;
    border-bottom: 1px solid #c8ced3 !important;
  }
  .table tbody tr:last-child
{
   border-bottom: none !important;
}

  `]
})
export class HFServicesComponent implements OnInit {
  public hfServices: any[] = [];
  @Input() public hfmisCode: string = '';
  public hfTypeCode: string = '';
  public currentUser: any;
  public multiple = false;
  public allowUnsort = true;
  public loading = true;
  public sort: SortDescriptor[] = [];
  public gridView: GridDataResult;
  public pageSize = 5;
  public skip = 0;
  constructor(private _rootService: RootService, private _notificationService: NotificationService, private _authenticationService: AuthenticationService, private _healthFacilityService: HealthFacilityService) { }

  ngOnInit() {
    this.currentUser = this._authenticationService.getUser();
    this.loadHFServices();
  }
  public changePagesize(value: any) {
    this.pageSize = +value;
    this.loadHFServices();
  }
  public dropValChanged(event: any) {
    console.log(event);
  }
  public sortChange(sort: SortDescriptor[]): void {
    if (sort[0].field == 'asd') {
      return;
    }
    this.sort = sort;
    this.loadHFServices();
  }
  private loadHFServices(): void {
    this._healthFacilityService.getHFServices(this.hfmisCode).subscribe((res: any) => {
      if (res) {
        this.gridView = {
          data: orderBy(res, this.sort),
          total: res.length
        };
      }
    }, err => {
      console.log(err);
    });
  }
  public removeHFService(hfId: number, serviceId: number) {
    if (confirm('Confirm Remove Service?')) {
      this._healthFacilityService.rmvHFService(hfId, serviceId).subscribe((res: any) => {
        this._notificationService.notify('warning', 'Service Removed!');
        this.loadHFServices();
      }, err => {
        console.log(err);
      });
    }
  }
  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadHFServices();
  }
  public onPageChange(state: any): void {
    this.pageSize = state.take;
  }
}