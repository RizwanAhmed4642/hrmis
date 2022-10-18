import { Observable } from "rxjs/Observable";
import { Component, OnInit, Inject, ViewChild, NgZone } from "@angular/core";
import { Validators, FormBuilder, FormGroup } from "@angular/forms";
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

import { DatabaseService } from "../database.service";
import { RootService } from "../../../_services/root.service";
import { KGridHelper } from "../../../_helpers/k-grid.class";
import { take } from "rxjs/operators/take";
import { DropDownsHR } from "../../../_helpers/dropdowns.class";
import {
  DesignationFilters,
  Designation,
  HrDesignation
} from "./designations.class";
import { DBColumn } from "../database.class";
@Component({
  selector: "app-designations",
  templateUrl: "./designations.component.html",
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
export class DesignationsComponent implements OnInit {
  @ViewChild("grid", { static: false }) public grid: GridComponent;
  public kGrid: KGridHelper = new KGridHelper();
  public dropDowns: DropDownsHR = new DropDownsHR();
  public designationDTO: any = {};
  public designationFilters: DesignationFilters = new DesignationFilters();
  public designation: HrDesignation = new HrDesignation();
  public designations: any[] = [];
  public columns: DBColumn[] = [];
  public elcColumns: DBColumn[] = [
    { name: "id", value: false },
    { name: "Name", value: false },
    { name: "dateTime", value: false },
    { name: "isActive", value: false }
  ];
  public allColumns: boolean = false;
  public showFilters = true;
  public showCheckBoxes = false;
  public showIds = false;
  public windowOpened = false;
  public windowWidth = 100;
  public windowState = "maximized";
  public mySelection: any[] = [];
  public dialogOpened = false;
  public editDialogOpened = false;
  public viewDialogOpened = false;
  public values: any;
  public selectedDesig: any;
  public DesigDialogOpened: boolean = false;
  public addingDesig = false;

  public savingDesig = false;
  //Pagination variables
  public pageSizes = [5, 10, 25, 50];
  public totalRecords = 0;
  public pageSize = 10;
  public skip = 0;

  public gridView: GridDataResult;

  constructor(
    private _rootService: RootService,
    private ngZone: NgZone,
    private _databaseService: DatabaseService,
    public _notificationService: NotificationService
  ) {
    this.kGrid.loading = false;
    this.kGrid.pageSize = 50;
    this.kGrid.pageSizes = [50, 100];
  }

  public ngOnInit(): void {
    this.loadDesignations();
    this.getCadres();
    this.getScales();
    this.windowWidth = window.innerWidth;
  }

  public getCadres() {
    //debugger;
    this.dropDowns.cadres = [];
    this.dropDowns.cadreData = [];
    this._rootService.getCadres().subscribe(
      (res: any) => {
        console.log(res);
        this.dropDowns.cadres = res;
        this.dropDowns.cadresData = this.dropDowns.cadres.slice();
        // debugger;

        if (this.selectedDesig) {
          this.dropDowns.selectedFiltersModel.cadre.Id = this.selectedDesig.Cadre_Id;
          this.dropDowns.selectedFiltersModel.cadre.Name = this.selectedDesig.Cadre_Name;
        }
      },
      err => {
        this.handleError(err);
      }
    );
  }

  public getScales() {
    //    debugger;
    this.dropDowns.scales = [];
    this.dropDowns.scalesData = [];
    //  debugger;
    this._rootService.getScales().subscribe(
      (res: any) => {
        this.dropDowns.scales = res;
        this.dropDowns.scalesData = this.dropDowns.scales.slice();
      },
      err => {
        this.handleError(err);
      }
    );
  }

  public valueChanges(event, type) {
    console.log(event);
    if (type == "_idOperator") {
      this.designationFilters._idOperator = event;
    } else if (type == "nameOperator") {
      this.designationFilters.nameOperator = event;
    } else if (type == "_cadreOperator") {
      this.designationFilters._cadreOperator = event;
    } else if (type == "_scaleOperator") {
      this.designationFilters._scaleOperator = event;
    } else if (type == "gazattedOperator") {
      this.designationFilters.gazattedOperator = event;
    }
  }
  public removeDesig() {
    debugger;
    this._databaseService.removeDesignation(this.selectedDesig).subscribe(
      (response: any) => {
        if (response) {
          this.savingDesig = false;
          this.closeDesigWindow();
          this._notificationService.notify(
            "success",
            "Designation Removed!"
          );
          this.selectedDesig = "";
          this.loadDesignations();
        }
      },
      err => {
        console.log(err);
      }
    );
  }
  public setColumns(obj) {
    this.columns = [];
    let keys = Object.keys(obj);
    keys.forEach(key => {
      this.columns.push({ name: key, value: false });
    });
  }
  public makeObject() {
    this.designationFilters.columns = this.columns;
    console.log(this.designationFilters);
  }
  public columnSelected(event: any, isAll?: boolean, isELCColumn?: boolean) {
    let checkBoxValue = event.target.checked;
    //      debugger;
    if (isELCColumn) {
      if (isAll) {
        this.elcColumns.forEach(elcColumn => {
          elcColumn.value = checkBoxValue;
        });
      } else if (!isAll) {
        let falseValueExist = this.elcColumns.find(x => x.value == false);
        this.designationFilters.allLogColumns = falseValueExist ? false : true;
      }
    } else {
      if (isAll) {
        this.columns.forEach(column => {
          column.value = checkBoxValue;
        });
      } else {
        let falseValueExist = this.columns.find(x => x.value == false);
        this.designationFilters.allColumns = falseValueExist ? false : true;
      }
    }
  }

  public dropdownValueChanged = (value, filter) => {
    debugger;
    if (!value) {
      return;
    }
    if (filter == "cadre") {
      //
      this.designation.Cadre_Id = value.id;
      this.selectedDesig.Cadre_Id = +value.id;
      // console.log(this.selectedDesig.Cadre_Id);
      /*  this.selectedDesig.Cadre_Id = +value.Id;
      this.designation.Cad = value.Id; */
    }
    if (filter == "scale") {
      //
      this.designation.HrScale_Id = value.Id;
      this.selectedDesig.HrScale_Id = +value.Id;
    }
  };
  private loadDropdownValues = () => {
    this.getCadres();
    this.getScales();
  };
  public close(component) {
    this.dialogOpened = false;
    this.editDialogOpened = false;
    this.viewDialogOpened = false;
    if (component == "window") {
      this.windowOpened = false;
    }
  }
  public onSubmit(val) {
    console.log(val);
    //this.loadDesignations();
  }
  public open(component) {
    this.selectedDesig = component;
    this.windowOpened = false;
    this.editDialogOpened = false;
    this.viewDialogOpened = false;
    this.dialogOpened = true;
    this[component + "DialogOpened"] = true;
  }

  public openDesigWindow(designation: any) {
    this.editDialogOpened = true;
    this.DesigDialogOpened = true;
    this.dialogOpened = true;
    this.viewDialogOpened = false;
    this.loadDropdownValues();
    //     debugger;
    console.log(designation);

    this.selectedDesig = designation;
    console.log(this.selectedDesig);
  }
  public closeDesigWindow() {
    this.selectedDesig = null;
    this.DesigDialogOpened = false;
    this.viewDialogOpened = false;
  }

  public addDesignation() {
    this.addingDesig = true;
    this._databaseService.addDesignation(this.designation).subscribe(
      (res: any) => {
        //console.log(res);
        if (res && res.Id) {
          this._notificationService.notify("success", "Designation Saved!");
          this.dropDowns.selectedFiltersModel.cadre = this.dropDowns.defultFiltersModel.cadre;
          this.dropDowns.selectedFiltersModel.Scale = this.dropDowns.defultFiltersModel.Scale;
          this.designation = new HrDesignation();
          this.loadDesignations();
          this.addingDesig = false;
        } else if (res == "Duplicate") {
          this._notificationService.notify("danger", "Duplication Name!");
          this.addingDesig = false;
        }
      },
      err => {
        console.log(err);
      }
    );
  }

  public editDesig() {
    debugger;
    this.selectedDesig.HrScale_Id = this.selectedDesig.Scale;
    this._databaseService.editDesignation(this.selectedDesig).subscribe(
      (response: any) => {
        if (response) {
          this.savingDesig = false;
          this.closeDesigWindow();
          this._notificationService.notify(
            "success",
            "Designation is Updated!"
          );
          this.selectedDesig = "";
          this.loadDesignations();
        }
      },
      err => {
        console.log(err);
      }
    );
  }

  public action(status) {
    console.log(`Dialog result: ${status}`);

    if (status == "yes") {
      this.windowOpened = false;
    }

    this.dialogOpened = false;
    this.editDialogOpened = false;
    this.viewDialogOpened = false;
  }
  public selectionChange(event) {
    console.log(event);
    this.loadDesignations();
  }
  public controlChange(event) {
    console.log(event);
    this[event] = !this[event];
    if (event == "showCheckBoxes" || event == "showIds") {
    }
  }
  private loadDesignations() {
    this.kGrid.loading = true;
    this._databaseService
      .getDesignations(this.kGrid.skip, this.kGrid.pageSize)
      .subscribe(
        response => this.handleResponse(response),
        err => this.handleError(err)
      );
  }

  private handleResponse(response: any) {
    console.log(response);
    this.kGrid.data = [];
    this.kGrid.data = response.List;
    this.kGrid.totalRecords = response.Count;
    this.kGrid.gridView = {
      data: this.kGrid.data,
      total: this.kGrid.totalRecords
    };
    this.kGrid.data[0] && this.columns.length == 0
      ? this.setColumns(this.kGrid.data[0])
      : [];
    this.kGrid.loading = false;
  }
  private handleError(err: any) {
    this.kGrid.loading = false;
  }

  public sortChange(sort: SortDescriptor[]): void {
    if (sort[0].field == "asd") {
      return;
    }
    console.log(sort);

    this.kGrid.sort = sort;
    this.sortData();
  }
  private sortData() {
    this.kGrid.gridView = {
      data: orderBy(this.kGrid.data, this.kGrid.sort),
      total: this.kGrid.totalRecords
    };
  }
  public pageChange(event: PageChangeEvent): void {
    this.kGrid.skip = event.skip;
    this.loadDesignations();
  }
  public changePagesize(value: any) {
    this.kGrid.pageSize = +value;
    this.kGrid.skip = 0;
    this.loadDesignations();
  }
}
