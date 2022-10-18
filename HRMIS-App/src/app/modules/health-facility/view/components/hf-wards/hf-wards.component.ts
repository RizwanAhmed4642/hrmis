import { Component, OnInit, Input } from '@angular/core';
import { SortDescriptor, orderBy } from '@progress/kendo-data-query';
import { PageChangeEvent, GridDataResult } from '@progress/kendo-angular-grid';
import { HealthFacilityService } from '../../../health-facility.service';
import { AuthenticationService } from '../../../../../_services/authentication.service';
import { NotificationService } from '../../../../../_services/notification.service';

@Component({
  selector: 'app-hf-wards',
  templateUrl: './hf-wards.component.html',
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
export class HFWardsComponent implements OnInit {

  public hfWards: any[] = [];
  @Input() public hfmisCode: string = '';
  public hfTypeCode: string = '';
  public multiple = false;
  public allowUnsort = true;
  public loadingWards = false;
  public savingWard = false;
  public removingWard = false;
  public sort: SortDescriptor[] = [];
  public gridView: GridDataResult;
  public currentUser: any;
  public pageSize = 5;
  public skip = 0;
  public selectedWard: any;
  public wardDialogOpened: boolean = false;
  constructor(private _notificationService: NotificationService, private _healthFacilityService: HealthFacilityService, private _authenticationService: AuthenticationService) {
  }
  ngOnInit() {
    this.currentUser = this._authenticationService.getUser();
    this.hfTypeCode = this.hfmisCode.substring(15, 12);
    this.getWards();
  }
  public changePagesize(value: any) {
    this.pageSize = +value;
    this.loadWards();
  }
  public dropValChanged(event: any) {
    console.log(event);
  }
  public sortChange(sort: SortDescriptor[]): void {
    if (sort[0].field == 'asd') {
      return;
    }
    this.sort = sort;
    this.loadWards();
  }
  private loadWards(): void {
    this.gridView = {
      data: orderBy(this.hfWards, this.sort),
      total: this.hfWards.length
    };
    this.loadingWards = false;
  }
  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadWards();
  }
  public onPageChange(state: any): void {
    this.pageSize = state.take;
  }
  private getWards() {
    this.loadingWards = true;
    this._healthFacilityService.getHFWards(this.hfmisCode).subscribe((data: any) => {
      if (data) {
        this.hfWards = data;
        this.loadWards();
      }
    }, err => {
      this.loadingWards = false;
      console.log(err);
    });
  }

  public editWard() {
    this.savingWard = true;
    this._healthFacilityService.editHfWard(this.selectedWard, this.selectedWard.Id).subscribe((response: any) => {
      console.log(response);
      if (response == true) {
        this.savingWard = false;
        this._notificationService.notify('success', this.selectedWard.Name + ' - Saved!');
        this.closeWardWindow();
      }
    }, err => {
      this.savingWard = false;
      console.log(err);
    });
  }
  public removeWard() {
    if (confirm('Confirm Remove Ward?')) {
      this.removingWard = true;
      this._healthFacilityService.rmvHFWard(this.selectedWard.Id).subscribe((response: any) => {
        if (response == true) {
          this.removingWard = false;
          this._notificationService.notify('success', this.selectedWard.Name + ' - Removed!');
          this.closeWardWindow();
        }
      }, err => {
        this.removingWard = false;
        console.log(err);
      });
    }
  }
  public closeWardWindow() {
    this.selectedWard = null;
    this.getWards();
    this.wardDialogOpened = false;
  }
  public openWardWindow(ward: any) {
    console.log(ward);
    this.selectedWard = ward;
    this.wardDialogOpened = true;
  }
}
