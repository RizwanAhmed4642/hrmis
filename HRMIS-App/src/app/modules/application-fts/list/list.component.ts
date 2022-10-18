import { Component, OnInit, ViewChild, OnDestroy, Input } from '@angular/core';
import { KGridHelper } from '../../../_helpers/k-grid.class';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { RootService } from '../../../_services/root.service';
import { ApplicationFtsService } from '../application-fts.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { orderBy, SortDescriptor } from '@progress/kendo-data-query';
import { PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs/Subject';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { DialogService } from '../../../_services/dialog.service';

@Component({
  selector: 'app-application-list',
  templateUrl: './list.component.html',
  styles: []
})
export class ListComponent implements OnInit, OnDestroy {
  @ViewChild('popup', { static: false }) calendarpopup;
  @ViewChild('excelexport', { static: false }) excelExport;
  public applicationTypeWindow: any = {
    dialogOpened: false,
    data: null,
    applicationTypes: [],
  }

  public kGrid: KGridHelper = new KGridHelper();
  public dropDowns: DropDownsHR = new DropDownsHR();
  public inputChange: Subject<any>;
  public searchQuery: string = '';
  @Input() public type: string = '';
  @Input() public typeId: number = 0;
  @Input() public statusId: number = 0;
  @Input() public sourceId: number = 0;
  public officer: string = '';
  @Input() public officerId: number = 0;
  public pandSOfficers: [] = [];
  public applications: [] = [];
  public range = { start: null, end: null };
  public currentUser: any;
  public dateNow: Date;
  public date10: Date;
  public date20: Date;
  public exporting: boolean = false;
  private subscription: Subscription;
  private inputChangeSubscription: Subscription;
  constructor(private _rootService: RootService, private _dialogService: DialogService,
    private _applicationFtsService: ApplicationFtsService, private route: ActivatedRoute, private router: Router,
    private _authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.dateNow = new Date();
    this.date10 = new Date();
    this.date10.setDate(this.dateNow.getDate() + 10);
    this.date20 = new Date();
    this.date20.setDate(this.dateNow.getDate() + 20);
    this.currentUser = this._authenticationService.getUser();
    this.fetchParams();
    this.subscribeInputChange();
    this.initializeProps();
    this.loadDropDownValues();
    this.kGrid.pageSizes = [50, 100, 200, 300, 500, 1000];
  }
  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('type')) {
          this.officer = params['office'];
          this.type = params['type'];
          if (params.hasOwnProperty('statusId') && +params['statusId']) {
            this.statusId = +params['statusId'];
          }
        }
        this.initializeProps();
      }
    );
  }
  private subscribeInputChange() {
    this.inputChange = new Subject();
    this.inputChangeSubscription = this.inputChange.pipe(debounceTime(300)).subscribe((query) => {
      this.searchQuery = query;
      if (this.searchQuery.length <= 2 && this.searchQuery.length != 0) {
        return;
      }
      this.getApplications();
    });
  }
  private initializeProps() {
    this.kGrid.loading = true;
    this.kGrid.pageSize = 50;
    this.kGrid.pageSizes = [50, 100, 200, 300, 500, 1000, 2000];
    this.hideCalendarOnClickOutside();
  }
  private loadDropDownValues() {
    this.getPandSOfficers('fts');
    this.getApplicationTypes();
    this.getApplicationStatus();
    this.getApplicationSource();
  }
  private getPandSOfficers = (type: string) => {
    this.pandSOfficers = [];
    this._rootService.getPandSOfficers(type).subscribe((res: any) => {
      if (res) {
        this.pandSOfficers = res;
        let pso = this.pandSOfficers as any[];
        let o = pso.find(x => x.DesignationName == this.officer);
        if (o) {
          this.officerId = o.Id;
        }
        this.getApplications();
      }
    },
      err => { this.handleError(err); }
    );
  }
  private getApplicationTypes() {
    this._rootService.getApplicationTypes().subscribe(
      (response: any) => {
        if (response) {
          this.dropDowns.applicationTypes = response;
          this.dropDowns.applicationTypesData = this.dropDowns.applicationTypes;
          let ts = this.dropDowns.applicationTypes as any[];
          let t = ts.find(x => x.Name == this.type);
          if (t) {
            this.dropDowns.selectedFiltersModel.applicationType = t;
            this.typeId = t.Id;
          }
          //this.getApplications();
        }
      },
      err => this.handleError(err)
    );
  }
  private getApplicationStatus() {
    this._rootService.getApplicationStatus().subscribe(
      (response: any) => {
        if (response) {
          this.dropDowns.applicationStatus = response;
          this.dropDowns.applicationStatusData = this.dropDowns.applicationStatus;
          let sts = this.dropDowns.applicationStatus as any[];
          let st = sts.find(x => x.Id == this.statusId);
          if (st) {
            this.dropDowns.selectedFiltersModel.applicationStatus = st;
          }
        }
      },
      err => this.handleError(err)
    );
  }
  private getApplicationSource() {
    this._rootService.getApplicationSources().subscribe(
      (response: any) => {
        if (response) {
          this.dropDowns.applicationSources = response;
        }
      },
      err => this.handleError(err)
    );
  }

  public getApplications() {
    this.kGrid.loading = true;
    this._applicationFtsService.getApplications(
      this.kGrid.skip,
      this.kGrid.pageSize,
      this.searchQuery,
      this.typeId,
      this.statusId,
      this.officerId,
      this.range.start,
      this.range.end,
      this.sourceId).subscribe(
        (response: any) => {
          this.kGrid.data = [];
          this.kGrid.data = response.List;
          this.kGrid.totalRecords = response.Count;
          this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords };
          this.kGrid.loading = false;
          if (this.calendarpopup) {
            this.calendarpopup.toggle(false);
          }
        },
        err => this.handleError(err)
      );
  }
  public exportApplications() {
    this.exporting = true;
    this.applications = [];
    this._applicationFtsService.getApplications(
      0,
      100000,
      this.searchQuery,
      this.typeId,
      this.statusId,
      this.officerId,
      this.range.start,
      this.range.end,
      this.sourceId).subscribe(
        (response: any) => {
          this.applications = response.List;
          console.log(this.applications);

          if (this.calendarpopup) {
            this.calendarpopup.toggle(false);
          }
          setTimeout(() => {
            this.exporting = false;
            this.excelExport.save();
          }, 800);
        },
        err => {
          this.handleError(err);
          this.exporting = false;

        }
      );
  }
  public dropdownValueChanged = (value, filter) => {
    if (filter == 'types') {
      this.typeId = value.Id;
    }
    if (filter == 'status') {
      this.statusId = value.Id;
    }
    if (filter == 'officer') {
      this.officerId = value.Id;
    }
    if (filter == 'source') {
      this.sourceId = value.Id;
    }
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
  public pageChange(event: PageChangeEvent): void {
    this.kGrid.skip = event.skip;
    this.getApplications();
  }
  public changePagesize(value: any) {
    this.kGrid.pageSize = +value;
    this.kGrid.skip = 0;
    this.getApplications();
  }
  private handleError(err: any) {
    this.kGrid.loading = false;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }
  private hideCalendarOnClickOutside() {
    let element = document.getElementById('popupcal');
    if (element) {
      const outsideClickListener = event => {
        if (!element.contains(event.target) && this.calendarpopup) {
          this.calendarpopup.toggle(false);
          removeClickListener();
        }
      }
      const removeClickListener = () => {
        document.removeEventListener('click', outsideClickListener);
      }
      document.addEventListener('click', outsideClickListener);
    }
  }
  public newDialog(type) {
    this._dialogService.openDialog({ type: type });
  }
  public openWindow(dataItem) {
    this.applicationTypeWindow.data = dataItem;
    this.applicationTypeWindow.dialogOpened = true;
    this._rootService.getApplicationTypes().subscribe(data => {
      this.applicationTypeWindow.applicationTypes = data;
      this.applicationTypeWindow.dialogOpened = true;
    });
  }
  public closeWindow() {
    this.applicationTypeWindow.dialogOpened = false;
  }
  public parseDate(input) {
    var parts = input.match(/(\d+)/g);
    // new Date(year, month [, date [, hours[, minutes[, seconds[, ms]]]]])
    return new Date(parts[0], parts[1] - 1, parts[2]); // months are 0-based
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
  ngOnDestroy() {
    this.subscription.unsubscribe();
    this.inputChangeSubscription.unsubscribe();
  }
}
