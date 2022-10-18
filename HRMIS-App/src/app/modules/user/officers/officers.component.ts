import { Component, OnInit } from '@angular/core';
import { KGridHelper } from '../../../_helpers/k-grid.class';
import { User } from '../../../_models/user.class';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { Subject } from 'rxjs/Subject';
import { Subscription } from 'rxjs/Subscription';
import { NotificationService } from '../../../_services/notification.service';
import { RootService } from '../../../_services/root.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { ActivatedRoute } from '@angular/router';
import { SortDescriptor, orderBy } from '@progress/kendo-data-query';
import { UserService } from '../user.service';
import { debounceTime } from 'rxjs/operators/debounceTime';

@Component({
  selector: 'app-officers',
  templateUrl: './officers.component.html',
  styles: []
})
export class OfficersComponent implements OnInit {
  public currentUser: User;
  public currentOfficer: any;
  public userId: string = '';
  public officerId: number = 0;
  public kGrid: KGridHelper = new KGridHelper();
  public dropDowns: DropDownsHR = new DropDownsHR();
  public inputChange: Subject<any>;
  public searchQuery: string = '';
  public searchFilters: any = { Query: '', Designation_Id: 0, OfficerId: 0 };
  public subscription: Subscription;
  private inputChangeSubscription: Subscription;
  constructor(private route: ActivatedRoute,
    public _notificationService: NotificationService,
    private _rootService: RootService,
    private _authenticationService: AuthenticationService,
    private _userService: UserService) { }

  ngOnInit() {
    this.getAllPandSOfficers();
    this.subscribeInputChange();

  }
  public getAllPandSOfficers() {
    this._rootService.getAllPandSOfficers(this.searchFilters).subscribe((response: any) => {
      if (response && response.data) {
        this.kGrid.data = response.data;
        this.kGrid.totalRecords = this.kGrid.data.length;
        this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords };
      }
      this.kGrid.loading = false;
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
  private subscribeInputChange() {
    this.inputChange = new Subject();
    this.inputChangeSubscription = this.inputChange.pipe(debounceTime(300)).subscribe((query) => {
      this.searchQuery = query;
      if (this.searchQuery.length <= 1 && this.searchQuery.length != 0) {
        return;
      }
      this.getAllPandSOfficers();
    });
  }
  public dashifyCNIC(cnic: string) {
    cnic = cnic.replace('-', '').replace('-', '');
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
