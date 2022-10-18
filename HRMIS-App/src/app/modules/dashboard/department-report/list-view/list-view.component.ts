import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { KGridHelper } from '../../../../_helpers/k-grid.class';
import { DropDownsHR } from '../../../../_helpers/dropdowns.class';
import { Subject } from 'rxjs/Subject';
import { Subscription } from 'rxjs/Subscription';
import { RootService } from '../../../../_services/root.service';
import { DialogService } from '../../../../_services/dialog.service';
import { ApplicationFtsService } from '../../../application-fts/application-fts.service';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthenticationService } from '../../../../_services/authentication.service';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { SortDescriptor, orderBy } from '@progress/kendo-data-query';
import { PageChangeEvent } from '@progress/kendo-angular-grid';
import { DashboardService } from '../../dashboard.service';

@Component({
  selector: 'app-list-view',
  templateUrl: './list-view.component.html',
  styleUrls: ['./list-view.component.scss']
})
export class ListViewComponent implements OnInit, OnDestroy {
  @ViewChild('popup', {static: false}) calendarpopup;
  public applicationTypeWindow: any = {
    dialogOpened: false,
    data: null,
    applicationTypes: [],
  }
  public kGrid: KGridHelper = new KGridHelper();
  public dropDowns: DropDownsHR = new DropDownsHR();
  public inputChange: Subject<any>;
  public searchQuery: string = '';
  public type: string = '';
  public typeId: number = 0;
  public statusId: number = 0;
  public officer: string = '';
  public officerId: number = 0;
  public pandSOfficers: [] = [];
  public pandSOfficer: any;
  public range = { start: null, end: null };
  private subscription: Subscription;
  private inputChangeSubscription: Subscription;
  constructor(private _rootService: RootService, private _dashboardService: DashboardService, private _dialogService: DialogService,
    private route: ActivatedRoute, private router: Router, private _authenticationService: AuthenticationService) { }

  ngOnInit() {

    this.fetchParams();
    this.subscribeInputChange();
    this.initializeProps();
    this.loadDropDownValues();
  }
  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('type')) {
          /*  this.officer = params['office'];
           console.log(this.officer);
  */
          this.type = params['type'];
          if (params.hasOwnProperty('statusId') && +params['statusId']) {
            this.statusId = +params['statusId'];
          }
        }
        console.log(localStorage.getItem('officKey'));
        this.officer = localStorage.getItem('officKey');
        localStorage.setItem('officKey', '');
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
    this.kGrid.loading = false;
    this.kGrid.pageSize = 150;
    this.kGrid.pageSizes = [50, 100, 200, 300, 500, 1000];
    this.hideCalendarOnClickOutside();
  }
  private loadDropDownValues() {
    this.getPandSOfficers('all');
    this.getApplicationTypes();
    this.getApplicationStatus();
  }
  private getPandSOfficers = (type: string) => {
    this.pandSOfficers = [];
    this._rootService.getPandSOfficers(type).subscribe((res: any) => {
      if (res) {
        this.pandSOfficers = res;
        let pso = this.pandSOfficers as any[];
        let o = pso.find(x => x.DesignationName == this.officer);
        if (o) {
          this.pandSOfficer = o;
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
  public getApplications() {
    this.kGrid.loading = true;
    this._dashboardService.getApplications(this.kGrid.skip, this.kGrid.pageSize,
      this.searchQuery, this.typeId, this.statusId, this.officerId,
      this.range.start, this.range.end).subscribe(
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
  public openInNewTab(link: string) {
    window.open('https://hrmis.pshealthpunjab.gov.pk/' + link, '_blank');

  }
  public dropdownValueChanged = (value, filter) => {
    if (filter == 'types') {
      this.typeId = value.Id;
    }
    if (filter == 'status') {
      this.statusId = value.Id;
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
    this._dialogService.openDialog(type);
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

