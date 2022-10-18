import { Component, OnInit } from '@angular/core';
import { KGridHelper } from '../../../_helpers/k-grid.class';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { Subject } from 'rxjs/Subject';
import { NotificationService } from '../../../_services/notification.service';
import { RootService } from '../../../_services/root.service';
import { FileTrackingSystemService } from '../../../file-tracking-system/file-tracking-system.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { orderBy, SortDescriptor } from '@progress/kendo-data-query';
import { PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subscription } from 'rxjs/Subscription';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { OnlinePostApplyService } from '../online-post-apply.service';

@Component({
  selector: 'app-reporting-smo',
  templateUrl: './reporting.component.html',
  styles: []
})
export class ReportingSMOComponent implements OnInit {

  public kGrid: KGridHelper = new KGridHelper();
  public dropDowns: DropDownsHR = new DropDownsHR();
  public inputChange: Subject<any>;
  public searchQuery: string = '';
  public ActiveDesignationId: number = 36;
  public currentUser: any;
  public filters: any = {};
  public selectedMerit: any;
  public selectedMeritPreferences: any[] = [];
  public selectedVacancy: any[] = [];
  private inputChangeSubscription: Subscription;

  public loadingPreferences: boolean = false;
  public loadingVacancy: boolean = false;
  public showVacancy: boolean = false;
  public preferencesWindow: boolean = false;

  constructor(public _notificationService: NotificationService,
    private _rootService: RootService,
    private _onlinePostApplyService: OnlinePostApplyService,
    private _authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.initializeProps();
    this.getMerits();
    this.subscribeInputChange();
  }
  public getMerits() {
    this.kGrid.loading = true;
    this._onlinePostApplyService.getPromotionApply({
      Query: this.searchQuery,
      Skip: this.kGrid.skip,
      PageSize: this.kGrid.pageSize
    }).subscribe((response: any) => {
      this.kGrid.data = [];
      this.kGrid.data = response.List;
      this.kGrid.totalRecords = response.Count;
      this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords };
      this.kGrid.loading = false;
    }, err => {
      console.log(err);
    });
  }

  getOfferLetterLink(cnic) {
    this._onlinePostApplyService.getDownloadLink('offer', cnic).subscribe((link) => {
      if (link) {
        window.open('http://localhost:8913' + link, '_blank');
      }
    });
  }
  public getPreferencesOpenWindow(item: any) {
    this.selectedMerit = item;
    this.loadingPreferences = true;
    this.preferencesWindow = true;
    this._onlinePostApplyService.getPreferencesAll(this.selectedMerit.Id).subscribe((response: any) => {
      if (response) {
        this.selectedMeritPreferences = response;
        this.loadingPreferences = false;
      }
    }, err => {
      console.log(err);
      this.loadingPreferences = false;
    });
  }
  public getPreferencesVacancy(hrmisCode: any) {
    this.loadingVacancy = true;
    this.showVacancy = false;
    this._onlinePostApplyService.getPreferenceVacancy(hrmisCode).subscribe((response: any) => {
      if (response) {
        this.selectedVacancy = response;
        this.loadingVacancy = false;
        this.showVacancy = true;
      }
    }, err => {
      console.log(err);
      this.loadingPreferences = false;
    });
  }
  public closePreferencesWindow() {
    this.preferencesWindow = false;
  }
  private subscribeInputChange() {
    this.inputChange = new Subject();
    this.inputChangeSubscription = this.inputChange.pipe(debounceTime(300)).subscribe((query) => {
      this.searchQuery = query;
      if (this.searchQuery.length <= 2 && this.searchQuery.length != 0) {
        return;
      }
      this.getMerits();
    });
  }
  public dropdownValueChanged = (value, filter) => {
    if (filter == 'types') {
      this.filters.typeId = value.Id;
    }
    if (filter == 'status') {
      this.filters.statusId = value.Id;
    }
    if (filter == 'officer') {
      this.filters.officerId = value.Id;
    }
    if (filter == 'source') {
      this.filters.sourceId = value.Id;
    }
  }
  public openInNewTab(type: string, item: any) {
    if (type == 'acceptance') {
      window.open('https://hrmis.pshealthpunjab.gov.pk/Uploads/Acceptances/' + item.Id + '_OfferLetter.jpg', '_blank');
    } else if (type == 'offer') {
      this._onlinePostApplyService.getDownloadLink('offer', item.Cnic).subscribe((link) => {
        if (link) {
          window.open('' + link, '_blank');
        }
      });
    } else if (type == 'preferences') {
      this._onlinePostApplyService.getDownloadLink('offer', item.Cnic).subscribe((link) => {
        if (link) {
          window.open('' + link, '_blank');
        }
      });
    }
  }
  private initializeProps() {
    this.kGrid.loading = true;
    this.kGrid.pageSize = 50;
    this.kGrid.pageSizes = [50, 100, 200, 300, 500];
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
    this.getMerits();
  }
  public changePagesize(value: any) {
    this.kGrid.pageSize = +value;
    this.kGrid.skip = 0;
    this.getMerits();
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
    this.inputChangeSubscription.unsubscribe();
  }
}
