import { Observable } from "rxjs/Observable";
import {
  Component,
  OnInit,
  Inject,
  ViewChild,
  NgZone,
  OnDestroy
} from "@angular/core";
import { Validators, FormBuilder, FormGroup } from "@angular/forms";
import { AuthenticationService } from "../../../_services/authentication.service";
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
import { NotificationService } from "../../../_services/notification.service";

import { DatabaseService } from "../database.service";
import { KGridHelper } from "../../../_helpers/k-grid.class";
import { take } from "rxjs/operators/take";
import { DropDownsHR } from "../../../_helpers/dropdowns.class";
import { Service } from "./services.class";
import { DBColumn } from "../database.class";

import { from } from "rxjs/observable/from";
import { delay } from "rxjs/operators/delay";
import { switchMap } from "rxjs/operators/switchMap";
import { map } from "rxjs/operators/map";
import { Subscription } from "rxjs/Subscription";
import { cstmdummyActiveStatus } from "../../../_models/cstmdummydata";
//import { ActivatedRoute } from '@angular/router';
//import { LocalService } from '../../../_services/local.service';

@Component({
  selector: "app-services",
  templateUrl: "./services.component.html",
  styles: [
    `
      .k-danger {
        color: #ff4c4b !important;
      }
      .k-danger:hover {
        background: #ff4c4b;
        color: white !important;
      }
      /*  .k-grid-content {
      overflow-y: scroll !important;
    } */
    `
  ]
})
export class ServicesComponent implements OnInit {
  //@Input() public addService: Service;
  @ViewChild("grid", {static: false}) public grid: GridComponent;
  public kGrid: KGridHelper = new KGridHelper();

  public loading: boolean = false;
  public addingService = false;
  public gridView: GridDataResult;
  public service: Service = new Service();
  public services: any[] = [];
  public columns: DBColumn[] = [];
  public elcColumns: DBColumn[] = [
    { name: "Id", value: false },
    { name: "Name", value: false },
    { name: "isActive", value: false }
  ];
  public allColumns: boolean = false;
  public windowOpened = false;
  public dialogOpened = false;
  public editDialogOpened = false;
  public viewDialogOpened = false;
  public windowWidth = 100;
  public values: any;
  public currentUser: any;
  public serviceDialogOpened: boolean = false;
  public selectedService: any;
  public savingService = false;
  //sorting variable
  public multiple = false;
  public allowUnsort = true;
  public sort: SortDescriptor[] = [];
  //Pagination variables
  public pageSizes = [5, 10, 25, 50];
  public totalRecords = 0;
  public pageSize = 10;
  public skip = 0;
  public hfmisCode: string;
  public dropDowns: DropDownsHR = new DropDownsHR();
  public newService: any = {
    Name: "",
    IsActive: ""
  };
  constructor(
    private _databaseService: DatabaseService,
    public _notificationService: NotificationService,
    private _authenticationService: AuthenticationService,
    private ngZone: NgZone
  ) {}

  public ngOnInit(): void {
    this.currentUser = this._authenticationService.getUser();
    this.hfmisCode = this.currentUser.HfmisCode;
    this.loadServices();
    this.windowWidth = window.innerWidth;
  }

  /* public setColumns(obj) {
        this.columns = [];
        let keys = Object.keys(obj);
        keys.forEach(key => {
            this.columns.push({ name: key, value: false });
        });
    }
 */
  public sortChange(sort: SortDescriptor[]): void {
    if (sort[0].field == "asd") {
      return;
    }
    this.sort = sort;
    this.sortData();
  }

  private sortData() {
    this.gridView = {
      data: orderBy(this.services, this.sort),
      total: this.totalRecords
    };
  }

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadServices();
  }
  public changePagesize(value: any) {
    this.pageSize = +value;
    this.skip = 0;
    this.loadServices();
  }

  /*     private fitColumns(): void {
        this.ngZone.onStable.asObservable().pipe(take(1)).subscribe(() => {
            this.grid.autoFitColumns();
        });
    }
     */
  private loadServices() {
    //  this.kGrid.loading = true;
    this.loading = true;
    this._databaseService
      .getServices(this.skip, this.pageSize)
      .subscribe(
        response => this.handleResponse(response),
        err => this.handleError(err)
      );
  }
  private handleResponse(response: any) {
    debugger;
    console.log(response);
    this.services = [];

    this.services = response.List;
    //  console.log(this.services);
    this.totalRecords = response.Count;

    this.gridView = { data: this.services, total: this.totalRecords };
    this.loading = false;
  }
  private handleError(err: any) {
    this.loading = false;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }
  public action(status) {
    console.log(`Dialog result: ${status}`);
    this.dialogOpened = false;
    this.editDialogOpened = false;
    this.viewDialogOpened = false;
  }

  public close(component) {
    this.dialogOpened = false;
    this.editDialogOpened = false;
    this.viewDialogOpened = false;
    if (component == "window") {
      this.windowOpened = false;
    }
  }
  // helpers
  get showInitialPageSize() {
    return (
      this.pageSizes.filter(num => num === Number(this.pageSize)).length === 0
    );
  }

  public onSubmit(val) {
    console.log(val);
    //this.loadDesignations();
  }
  public open(component) {
    this.windowOpened = false;
    this.editDialogOpened = false;
    this.viewDialogOpened = false;
    this.dialogOpened = true;
    this[component + "DialogOpened"] = true;
  }

  public addService() {
    debugger;
    this.addingService = true;
    this._databaseService.addService(this.newService).subscribe(
      (res: any) => {
        console.log(res);
        if (res) {
          if (res == "Invalid") {
            debugger;
            this._notificationService.notify(
              "danger",
              "Cannot save duplicate name."
            );
            this.loadServices();
            this.addingService = false;
          } else {
            debugger;
            let service = this.newService;
            this._notificationService.notify("success", "Service Saved!");
            this.loadServices();
            this.addingService = false;
          }
        }
      },
      err => {
        console.log(err);
      }
    );
  }
  public editService() {
    debugger;
    this._databaseService
      .editService(this.selectedService, this.selectedService.Id)
      .subscribe(
        (response: any) => {
          console.log(response);
          if (response) {
            this.savingService = false;
            this.closeServiceWindow();
          }
        },
        err => {
          console.log(err);
        }
      );
  }
  public closeServiceWindow() {
    this.selectedService = null;
    this.serviceDialogOpened = false;
  }
  public openServiceWindow(service: any) {
    this.selectedService = service;
    this.serviceDialogOpened = true;
  }

  private bindValues() {
    this.dropDowns.selectedFiltersModel.status = cstmdummyActiveStatus.find(
      x => x.Name == this.service._isActive
    );
  }

  public dropdownValueChanged = (value, filter) => {
    if (!value) {
      return;
    }
    if (filter == "Activestatus") {
      console.log(value);
      this.service._isActive = value.Name;
    }
  };
}
