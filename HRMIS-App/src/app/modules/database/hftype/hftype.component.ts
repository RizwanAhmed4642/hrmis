import {
  Component,
  OnInit,
  Inject,
  ViewChild,
  NgZone,
  OnDestroy
} from "@angular/core";
import { Observable } from "rxjs/Observable";
import { Subscription } from "rxjs/Subscription";
import { NotificationService } from "../../../_services/notification.service";

import {
  GridDataResult,
  PageChangeEvent,
  GridComponent
} from "@progress/kendo-angular-grid";
import {
  State,
  process,
  SortDescriptor,
  orderBy
} from "@progress/kendo-data-query";
import { KGridHelper } from "../../../_helpers/k-grid.class";
import { DropDownsHR } from "../../../_helpers/dropdowns.class";

import { RootService } from "../../../_services/root.service";
import { DatabaseService } from "../database.service";

import { Validators, FormBuilder, FormGroup } from "@angular/forms";
import { AuthenticationService } from "../../../_services/authentication.service";
import { take } from "rxjs/operators/take";
import { DBColumn } from "../database.class";
import { from } from "rxjs/observable/from";
import { delay } from "rxjs/operators/delay";
import { switchMap } from "rxjs/operators/switchMap";
import { map } from "rxjs/operators/map";

import { HfType } from "./hftype.class";

@Component({
  selector: "app-hftype",
  templateUrl: "./hftype.component.html",
  styles: []
})
export class HFTypeComponent implements OnInit, OnDestroy {
  //ViewChild('grid') public grid: GridComponent;
  public kGrid: KGridHelper = new KGridHelper();

  public dropDowns: DropDownsHR = new DropDownsHR();
  public hftype: HfType;
  public loading = true;
  private subscription: Subscription;
  public addingHft = false;
  //Pagination variables
  public pageSizes = [5, 10, 25, 50];
  public totalRecords = 0;
  public pageSize = 10;
  public skip = 0;
  //gridview
  public hftypes: any[] = [];
  public currentUser: any;
  public hfmisCode: string;
  public windowWidth = 100;
  //
  //sorting variable
  public multiple = false;
  public allowUnsort = true;
  public sort: SortDescriptor[] = [];

  public gridView: GridDataResult;

  public selectedHfType: any;
  public HfTypeDialogOpened: boolean = false;
  public savingHfType = false;

  constructor(
    private _rootService: RootService,
    private _databaseService: DatabaseService,
    private _authenticationService: AuthenticationService,
    private ngZone: NgZone,
    public _notificationService: NotificationService
  ) {}
  // grid
  public sortChange(sort: SortDescriptor[]): void {
    if (sort[0].field == "asd") {
      return;
    }
    this.sort = sort;
    this.sortData();
  }

  private sortData() {
    this.gridView = {
      data: orderBy(this.hftypes, this.sort),
      total: this.totalRecords
    };
  }
  //
  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadHfTypes();
  }
  public changePagesize(value: any) {
    this.pageSize = +value;
    this.skip = 0;
    this.loadHfTypes();
  }

  private loadHfTypes() {
    this.loading = true;
    this._databaseService
      .getHfTypes(this.skip, this.pageSize)
      .subscribe(
        response => this.handleResponse(response),
        err => this.handleError(err)
      );
  }

  private getHFCategory = () => {
    this.dropDowns.hfCategory = [];
    this.dropDowns.hfCategoryData = [];
    this._rootService.getHFCategories().subscribe(
      (res: any) => {
        this.dropDowns.hfCategory = res;
        this.dropDowns.hfCategoryData = this.dropDowns.hfCategory.slice();
      },
      err => {
        this.handleError(err);
      }
    );
  };

  get showInitialPageSize() {
    return (
      this.pageSizes.filter(num => num === Number(this.pageSize)).length === 0
    );
  }

  private loadDropdownValues = () => {
    this.getHFCategory();
  };

  private handleResponse(response: any) {
    //debugger;
    console.log(response);
    this.hftypes = [];
    this.hftypes = response.List;
    //  console.log(this.hftypes);
    this.totalRecords = response.Count;
    this.gridView = { data: this.hftypes, total: this.totalRecords };
    this.loading = false;
  }

  private handleError(err: any) {
    this._notificationService.notify("danger", "Error!");
    this.loading = false;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }
  ngOnDestroy() {
    // this.subscription.unsubscribe();
  }

  ngOnInit() {
    this.hftype = new HfType();
    this.loadDropdownValues();
    this.currentUser = this._authenticationService.getUser();
    this.hfmisCode = this.currentUser.HfmisCode;
    this.loadHfTypes();
    this.windowWidth = window.innerWidth;
  }

  public addHfType() {
    this.addingHft = true;
    this._databaseService.addHfType(this.hftype).subscribe((res: any) => {
        //console.log(res);
        if (res) {
          debugger;
          let hftype = this.hftype;
          // if (this.savingHfType) {
          this._notificationService.notify(
            "success",
            "Health Facility Type Saved!"
          );
          this.loadHfTypes();
          this.addingHft = false;
          // }
          this.dropDowns.selectedFiltersModel.hfCategory = this.dropDowns.defultFiltersModel.hfCategory;
          this.hftype.Name = "";
          this.loadHfTypes();
        }
         else if(res == 'Invalid') {
          this._notificationService.notify(
            "danger",
            "Cannot save duplicate name."
          );
          this.addingHft = false;
        }
        this.hftype = new HfType();
      },
      err => {
        console.log(err);
      }
    );
  }
  public getHFCategories() {
    this._rootService.getHFCategories().subscribe(
      (res: any) => {
        if (res) {
          this.dropDowns.hfCategory = res;
          this.dropDowns.hfCategoryData = this.dropDowns.hfCategory;
        }
      },
      err => {
        this.handleError(err);
      }
    );
  }
  public openHfTypeWindow(hftype: any) {
    this.selectedHfType = hftype;
    this.bindValues();
    this.HfTypeDialogOpened = true;
  }

  private bindValues() {
    let categories = this.dropDowns.hfCategory as any[];
    let category = categories.find(x => x.Id == this.selectedHfType.HFCat_Id);
    if (category) {
      this.dropDowns.selectedFiltersModel.hfCategory = {
        Id: category.Id,
        Code: category.Code,
        Name: category.Name
      };
    }
  }
  public dropdownValueChanged = (value, filter) => {
    if (!value) {
      return;
    }
    if (filter == "hfCategory") {
      this.hftype.HFCat_Id = value.Id;
    }
  };

  public editHfType() {
    debugger;
    this._databaseService
      .editHfType(this.selectedHfType, this.selectedHfType.Id)
      .subscribe(
        (response: any) => {
          console.log(response);
          if (response) {
            this.savingHfType = false;
            this.closeHfTypeWindow();
            this._notificationService.notify(
              "success",
              "Health Facility Type is Updated!"
            );
            this.selectedHfType = "";
            this.loadHfTypes();
          }
        },
        err => {
          console.log(err);
        }
      );
  }
  public closeHfTypeWindow() {
    this.selectedHfType = null;
    this.HfTypeDialogOpened = false;
  }
  public removeHfTypeWindow()
  {
    
  }
  
}
