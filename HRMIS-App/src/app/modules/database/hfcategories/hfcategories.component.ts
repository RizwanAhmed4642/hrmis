import { Observable} from "rxjs/Observable";
import { Component, OnInit, Inject, ViewChild, NgZone } from "@angular/core";
import { Validators, FormBuilder, FormGroup } from "@angular/forms";
import { NotificationService } from "../../../_services/notification.service";
import { 
        GridDataResult,
        PageChangeEvent,
        GridComponent } from "@progress/kendo-angular-grid";
import {
        State,
        process,
        SortDescriptor,
        orderBy
} from "@progress/kendo-data-query";
import { DatabaseService } from "../database.service";
import { KGridHelper } from "../../../_helpers/k-grid.class";
import { RootService } from "../../../_services/root.service";
import { HfCategories } from "../hfcategories/hfcategories.class";
import { AuthenticationService } from "../../../_services/authentication.service";

@Component({
  selector: 'app-hfcategories',
  templateUrl: './hfcategories.component.html',
  styles: []
})
export class HFCategoriesComponent implements OnInit {

  public hfcategory : HfCategories;
  public gridView: GridDataResult;
  public loading = true;
  public skip = 0;
  public pageSize = 10;
  public hfcategories : any[] = [];
  public totalRecords = 0;
  public currentUser : any;
  public selectedHfCategory: any;
  public HfCatDialogOpened : boolean = false;

  constructor(
    private _rootService: RootService,
    private _databaseService: DatabaseService,
    private _authenticationService: AuthenticationService,
    public _notificationService: NotificationService

  ) { }

  ngOnInit() {
    this.hfcategory = new HfCategories;
    this.currentUser = this._authenticationService.getUser();
    this.loadHfCategories();

  }
  private loadHfCategories() {
    debugger;
    this.loading = true;
    this._databaseService
    .getHfCategories(this.skip, this.pageSize)
    .subscribe(
      response => this.handleResponse(response),
      err => this.handleError(err)
    );

  }
  private handleResponse(response: any){
    this.hfcategories = [];
    this.hfcategories = response.List;
    this.totalRecords = response.Count;
    this.gridView = { data: this.hfcategories, total: this.totalRecords };
    this.loading = false;
  }

  private handleError(err: any){
    this._notificationService.notify("danger","Error!");
    this.loading = false;
    if (err.status == 403){
      this._authenticationService.logout();
    }
  }

  public openHfCatWindow(hfcategory: any){
    this.selectedHfCategory = hfcategory;
   // this.bindValues();
    this.HfCatDialogOpened = true;
  }

  private bindValues(){

  }

  public closeHfCatWindow(){
    this.selectedHfCategory = null;
    this.HfCatDialogOpened = false;
  }

  public editHfCategory(){

  }

 
}
