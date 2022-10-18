import { Component, OnInit, Input } from '@angular/core';
import { SortDescriptor, orderBy, aggregateBy } from '@progress/kendo-data-query';
import { PageChangeEvent, GridDataResult } from '@progress/kendo-angular-grid';
import { HealthFacilityService } from '../../../health-facility.service';
import { AuthenticationService } from '../../../../../_services/authentication.service';
import { NotificationService } from '../../../../../_services/notification.service';
import { RootService } from '../../../../../_services/root.service';
import { DropDownsHR } from '../../../../../_helpers/dropdowns.class';

@Component({
  selector: 'app-hf-wards-new',
  templateUrl: './hf-wards-new.component.html',
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
export class HFWardsNewComponent implements OnInit {

  public dropDowns: DropDownsHR = new DropDownsHR();
  public hfWards: any[] = [];
  @Input() public hfmisCode: string = '';
  @Input() public HF_Id: number = 0;
  public hfTypeCode: string = '';
  public multiple = true;
  public allowUnsort = true;
  public loadingWards = true;
  public savingWard = false;
  public removingWard = false;
  public aggregates: any[] = [];
  public totalSums: any;
  public sort: SortDescriptor[] = [];
  public gridView: GridDataResult;
  public currentUser: any;
  public pageSize = 5;
  public skip = 0;
  public oldValGB = 0;
  public oldValSB = 0;
  public oldValSanctioned = 0;
  public oldValDonated = 0;
  public newWard: any = {};
  public selectedWard: any;
  public wardDialogOpened: boolean = false;
  public addingWard: boolean = false;
  constructor(private _rootService: RootService, private _hfService: HealthFacilityService, private _notificationService: NotificationService, private _healthFacilityService: HealthFacilityService, private _authenticationService: AuthenticationService) {
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
  public dropdownValueChanged = (value, filter) => {
    if (!value) {
      return;
    }
    if (filter == 'WardName') {
      this.newWard.TotalSanctioned = this.newWard.TotalSanctioned ? this.newWard.TotalSanctioned : 0;
      this.newWard.TotalGB = this.newWard.TotalGB ? this.newWard.TotalGB : 0;
      this.newWard.TotalSB = this.newWard.TotalSB ? this.newWard.TotalSB : 0;
      this.newWard.TotalDonated = this.newWard.TotalDonated ? this.newWard.TotalDonated : 0;
    }

  }
  public addHFWard() {
    this.addingWard = true;
    this.newWard.HF_Id = this.HF_Id;
    this._hfService.addHFWardBeds(this.newWard).subscribe((res: any) => {
      if (res && res.Id) {
        this.getWards();
        this._notificationService.notify('success', 'Ward Info Added');
      } else {
        this._notificationService.notify('warning', 'Duplicate Ward Info!');
      }
      this.newWard = {};
      this.addingWard = false;
    },
      err => {
        console.log(err);
      });
  }
  private loadWards(): void {
    this.gridView = {
      data: orderBy(this.hfWards, this.sort),
      total: this.hfWards.length
    };
    this.setTotalAggregates();
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
    this._rootService.getWardsCustom(this.HF_Id).subscribe((data: any) => {
      if (data && data.wards) {
        this.dropDowns.wards = data.wards;
        this.hfWards = data.hfWardsBeds;
        this.loadWards();
      } else {
        this.loadingWards = false;
      }
    }, err => {
      console.log(err);
    });
  }

  public setTotalAggregates() {
    this.aggregates = [
      { field: 'TotalGB', aggregate: 'sum' },
      { field: 'TotalSB', aggregate: 'sum' },
      { field: 'TotalDonated', aggregate: 'sum' },
      { field: 'TotalSanctioned', aggregate: 'sum' },
      { field: 'Total', aggregate: 'sum' },
    ];
    this.totalSums = aggregateBy(this.hfWards, this.aggregates);
    this.loadingWards = false;
  }
  private getHFWards() {
    this.loadingWards = true;
    this._healthFacilityService.getHFWardsBeds(this.hfmisCode).subscribe((data: any) => {
      if (data) {
        this.hfWards = data;
        this.loadWards();
      } else {
        this.loadingWards = false;
      }
    }, err => {
      this.loadingWards = false;
      console.log(err);
    });
  }
  public editWard() {
    this.savingWard = true;
    this._healthFacilityService.addHFWardBeds(this.selectedWard).subscribe((response: any) => {
      if (response) {
        this._notificationService.notify('success', this.selectedWard.Name + ' - Saved!');
        this.closeWardWindow();
      }
      this.savingWard = false;
    }, err => {
      this.savingWard = false;
      console.log(err);
    });
  }
  public removeWard(id: number, wardName: string) {
    if (confirm('Confirm Remove Ward?')) {
      this.removingWard = true;
      this._healthFacilityService.rmvHFWardsBed(id).subscribe((response: any) => {
        if (response == true) {
          this.removingWard = false;
          this._notificationService.notify('success', wardName + ' - Removed!');
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
    this.oldValGB = 0;
    this.oldValSB = 0;
    this.oldValSanctioned = 0;
    this.oldValDonated = 0;
    this.getWards();
    this.wardDialogOpened = false;
  }
  public openWardWindow(ward: any) {
    this.selectedWard = ward;
    this.oldValGB = this.selectedWard.TotalGB;
    this.oldValSB = this.selectedWard.TotalSB;
    this.oldValSanctioned = this.selectedWard.TotalSanctioned;
    this.oldValDonated = this.selectedWard.TotalDonated;
    this.wardDialogOpened = true;
  }
}
