import { Component, OnInit, OnDestroy } from '@angular/core';

import { SortDescriptor, orderBy } from '@progress/kendo-data-query';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { AuthenticationService } from '../../../_services/authentication.service';
import { RootService } from '../../../_services/root.service';
import { from } from 'rxjs/observable/from';
import { delay } from 'rxjs/operators/delay';
import { switchMap } from 'rxjs/operators/switchMap';
import { map } from 'rxjs/operators/map';
import { UserService } from '../user.service';
import { User } from '../../../_models/user.class';
import { Subject } from 'rxjs/Subject';
import { Subscription } from 'rxjs/Subscription';
import { debounceTime } from 'rxjs/operators/debounceTime';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styles: []
})
export class ListComponent implements OnInit, OnDestroy {

  //loading flag
  public loading = false;
  public loadingUserName = false;
  //grid data
  public gridView: GridDataResult;
  public users: any[] = [];
  public userRights: any[] = [];
  //sorting variable
  public multiple = false;
  public allowUnsort = true;
  public sort: SortDescriptor[] = [];
  //Pagination variables
  public pageSizes = [10, 50, 100, 200, 300, 500];
  public totalRecords = 0;
  public pageSize = 10;
  public skip = 0;
  public currenUser: User;
  public userName: string = '';
  public inputChange: Subject<any>;
  private userNameSubscription: Subscription;
  constructor(private _rootService: RootService, private _userService: UserService, private _authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.currenUser = this._authenticationService.getUser();
    this.loadUsers();
    this.subscribeUserName();
  }
  public subscribeUserName() {
    this.inputChange = new Subject();
    this.userNameSubscription = this.inputChange.pipe(debounceTime(300)).subscribe((query) => {
      this.loadingUserName = true;
      this.userName = query as string;
      this.loadUsers();
    });
  }
  private loadUsers() {
    this.loading = true;
    this._userService.getUsers(this.skip, this.pageSize, this.userName, '').subscribe(
      response => this.handleResponse(response),
      err => this.handleError(err)
    );
  }
  private handleResponse(response: any) {
    this.users = [];
    this.users = response.List;
    this.totalRecords = response.Count;
    this.gridView = { data: this.users, total: this.totalRecords };
    this.loading = false;
    this.loadingUserName = false;
    this.getUserRights();
  }
  private handleError(err: any) {
    this.loading = false;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }
  public onSearch() {

  }
  public sortChange(sort: SortDescriptor[]): void {
    if (sort[0].field == 'asd') { return; }
    this.sort = sort;
    this.sortData();
  }
  private sortData() {
    this.gridView = {
      data: orderBy(this.users, this.sort),
      total: this.totalRecords
    };
  }
  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadUsers();
  }
  public changePagesize(value: any) {
    this.pageSize = +value;
    this.skip = 0;
    this.loadUsers();
  }
  public getUserRights() {
    this._userService.getUserRight().subscribe((res: any) => {
      if (res) {
        this.userRights = res;
        this.userRights.forEach(userRight => {
          let user = this.users.find(x => x.Id == userRight.User_Id);
          if(user){
            user.Vacancy = userRight.EditVacancy;
          }
        });
      }
    }, err => {
      console.log(err);
    });
  }
  public switchValChange(dataItem) {
    if (dataItem) {
      let userRight: any = {};
      userRight.User_Id = dataItem.Id;
      userRight.AddVacancy = dataItem.Vacancy;
      userRight.EditVacancy = dataItem.Vacancy;
      userRight.RemoveVacancy = dataItem.Vacancy;
      this._userService.setUserRights(userRight).subscribe((res: any) => {
        if (res) {

        }
      }, err => {
        console.log(err);
      })
    }
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
  ngOnDestroy(): void {
    this.userNameSubscription.unsubscribe();
  }
  // helpers
  get showInitialPageSize() {
    return this.pageSizes
      .filter(num => num === Number(this.pageSize))
      .length === 0;
  }
}
