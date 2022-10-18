import { Component, NgZone, OnInit } from '@angular/core';
import { KGridHelper } from '../../../_helpers/k-grid.class';
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

import { take } from "rxjs/operators/take";
import { DropDownsHR } from "../../../_helpers/dropdowns.class";

import { from } from "rxjs/observable/from";
import { delay } from "rxjs/operators/delay";
import { switchMap } from "rxjs/operators/switchMap";
import { map } from "rxjs/operators/map";
import { Subscription } from "rxjs/Subscription";
import { cstmdummyActiveStatus } from "../../../_models/cstmdummydata";
import { RootService } from "../../../_services/root.service";
import { Subject } from "rxjs/Subject";
import { debounceTime } from "rxjs/operators/debounceTime";
import { AdhocService } from '../adhoc.service';

@Component({
  selector: 'app-adhoc-hf-opened',
  templateUrl: './adhoc-hf-opened.component.html',
})
export class AdhocHfOpenedComponent implements OnInit {
  public kGrid: KGridHelper = new KGridHelper();
  public hfOpens: any[] = [];
  public vpProfileStatus: any[] = [];
  public hfsList: any[] = [];
  public hfOpened: any = {};
  public hfName: string = '';
  public saving: boolean = false;
  public searchingHfs: boolean = false;
  public laddaflag: boolean = false;
  public searchEvent = new Subject<any>();
  public searchSubcription: Subscription;
  public dropDowns: DropDownsHR = new DropDownsHR();

  constructor(
    private _adhocService: AdhocService,
    public _notificationService: NotificationService,
    public _rootService: RootService,
    private _authenticationService: AuthenticationService,
    private ngZone: NgZone
  ) { }

  ngOnInit() {
    this.laddaflag = true;
    this.getHFOpened();
    this.handleSearchEvents();
    this.getDesignations();
  }

  public getHFOpened() {
    this.hfOpens = [];
    this._adhocService.getOpenHF().subscribe((res: any) => {
      if (res) {
        this.hfOpens = res;
      }
    }, err => {
      console.log(err);
    });

  }
  public removeOpenedHF(Id: number) {
    if (confirm('Are you sure?')) {
      this._adhocService.removeOpenHF(Id).subscribe((res: any) => {
        if (res) {
          this._notificationService.notify('success', 'Removed');
          this.getHFOpened();
        }
      }, err => {
        console.log(err);
      });
    }
  }


  public saveOpenHF() {
    this.saving = true;
    this.hfOpened.Designation_Id = 1320;
    this._adhocService.saveHFOpen(this.hfOpened).subscribe((data) => {
      if (data) {
        this.getHFOpened();
        this.hfOpened = {};
        this.hfName = '';
        this.dropDowns.selectedFiltersModel.designation = this.dropDowns.defultFiltersModel.designation;
        this._notificationService.notify('success', 'Saved');
        this.saving = false;
      }
    });
  }
  public search(value: string, filter: string) {
    if (filter == 'hfs') {
      this.hfsList = [];
      if (value.length > 2) {
        this.searchingHfs = true;
        this._rootService.searchHealthFacilitiesAll(value).subscribe((data) => {
          this.hfsList = data as any[];
          this.searchingHfs = false;
        });
      }
    }
  }
  public searchClicked(FullName, filter) {

    if (filter == 'hfs') {
      let item = this.hfsList.find(x => x.FullName === FullName);
      if (item) {
        this.hfOpened.HF_Id = item.Id;
      }
    }
  }
  private getDesignations = () => {
    this.dropDowns.designations = [];
    this.dropDowns.designationsData = [];
    this._rootService.getConsultantDesignations().subscribe((res: any) => {
      if (res && res.List) {
        this.dropDowns.designations = res.List;
        this.dropDowns.designationsData = this.dropDowns.designations.slice();
      }
    },
      err => { console.log(err); }
    );
  }
  public dropdownValueChanged = (value, filter) => {
    if (!value) {
      return;
    }
    if (filter == 'designation') {
      this.hfOpened.Designation_Id = value.Id;
    }
  }
  public handleSearchEvents() {
    this.searchSubcription = this.searchEvent.pipe(
      debounceTime(400)).subscribe((x: any) => {
        this.search(x.event, x.filter);
      });
  }


  public removeVpProfileStatus(status_Id: number) {
    if (confirm('Are you sure?')) {
      this._adhocService.removeVpProfileStatus(status_Id).subscribe((res: any) => {
        if (res) {
          this._notificationService.notify('success', 'Link Removed');
          //this.getVpProfileStatus();
        }
      }, err => {
        console.log(err);
      });
    }
  }

}
