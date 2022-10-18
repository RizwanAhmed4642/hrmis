import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from '../../../_services/authentication.service';
import { NotificationService } from '../../../_services/notification.service';
import { RootService } from '../../../_services/root.service';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { Subject } from 'rxjs/Subject';
import { KGridHelper } from '../../../_helpers/k-grid.class';
import { PageChangeEvent } from '@progress/kendo-angular-grid';
import { VacancyPositionService } from '../vacancy-position.service';
import { VPHolderView } from '../vacancy-position.class';

@Component({
  selector: 'app-vacancy-status',
  templateUrl: './vacancy-status.component.html',
  styles: []
})
export class VacancyStatusComponent implements OnInit {
  public loaded: boolean = true;
  public holderWindow: boolean = false;
  public saving: boolean = false;
  public vpHolder: VPHolderView = new VPHolderView();
  public kGrid: KGridHelper = new KGridHelper();
  public currentUser: any;
  public dropDowns: DropDownsHR = new DropDownsHR();
  public inputChange: Subject<any>;
  public searchQuery: string = '';
  public office_Id: number = 0;
  public typeId: number = 0;
  public statusId: number = 0;
  constructor(private _rootService: RootService,
    private _authenticationService: AuthenticationService,
    private _vacancyService: VacancyPositionService,
    private _notificationService: NotificationService) { }

  ngOnInit() {
    this.currentUser = this._authenticationService.getUser();
    this.getVpHolders();
  }
  public getVpHolders() {
    this._vacancyService.getVpHolders({
      Skip: this.kGrid.skip,
      PageSize: this.kGrid.pageSize,
    }).subscribe((res: any) => {
      if (res) {
        this.kGrid.data = [];
        this.kGrid.data = res.List;
        this.kGrid.totalRecords = res.Count;
        this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords };
      }
      this.kGrid.loading = false;
      this.kGrid.firstLoad = false;
    }, err => {
      console.log(err);
    });
  }
  public openHolderWindow(vpHolder: VPHolderView) {
    this.vpHolder = vpHolder;
    this.holderWindow = true;
  }
  public closeHolderWindow() {
    this.holderWindow = false;
  }
  public saveVacancyHolder() {
    this.saving = true;
    this._vacancyService.saveVacancyHolder(this.vpHolder).subscribe((res) => {
      if (res) {
        this.saving = false;
        this.closeHolderWindow();
        this._notificationService.notify('success', 'Vacancy Status Updated!');
      }
    }, err => {
      this.saving = false;
      console.log(err);
    });
  }
  public pageChange(event: PageChangeEvent): void {
    this.kGrid.skip = event.skip;
    this.getVpHolders();
  }
  public changePagesize(value: any) {
    this.kGrid.pageSize = +value;
    this.kGrid.skip = 0;
    this.getVpHolders();
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
}
