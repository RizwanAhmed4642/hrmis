import { Component, OnInit } from '@angular/core';
import { PageChangeEvent } from '@progress/kendo-angular-grid';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { RootService } from '../../_services/root.service';
import { FileTrackingSystemService } from '../file-tracking-system.service';
import { DropDownsHR } from '../../_helpers/dropdowns.class';
import { KGridHelper } from '../../_helpers/k-grid.class';
import { ApplicationFtsService } from '../../modules/application-fts/application-fts.service';
import { AuthenticationService } from '../../_services/authentication.service';
import { Subject } from 'rxjs/Subject';

@Component({
  selector: 'app-scanned-documents',
  templateUrl: './scanned-documents.component.html',
  styles: []
})
export class ScannedDocumentsComponent implements OnInit {
  public kGrid: KGridHelper = new KGridHelper();
  public dropDowns: DropDownsHR = new DropDownsHR();
  public inputChange: Subject<any>;
  public searchQuery: string = '';
  public office_Id: number = 0;
  public typeId: number = 0;
  public statusId: number = 0;
  public recieveFiles: boolean = false;
  public fromDate: Date = new Date();
  public selectedApplications: number[] = [];
  public range = { start: null, end: null };
  constructor(private _rootService: RootService, private _fileTrackingSystemService: FileTrackingSystemService, private _applicationFtsService: ApplicationFtsService, private _authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.kGrid.firstLoad = true;
    this.kGrid.pageSizes = [50, 100, 200, 300, 500, 1000];
    this.inputChange = new Subject();
    this.inputChange.pipe(debounceTime(300)).subscribe((query) => {
      this.searchQuery = query;
      if (this.searchQuery.length <= 2 && this.searchQuery.length != 0) {
        return;
      }
      this.getApplications();
    });
    this.initializeProps();
    this.loadDropDownValues();
    this.getApplications();
  }
  private initializeProps() {
    this.kGrid.loading = false;
    this.kGrid.pageSize = 50;
    this.kGrid.pageSizes = [50, 100];
  }
  private loadDropDownValues() {
    this.getApplicationTypes();
    this.getApplicationStatus();
    //this.getInboxOffices();
  }
  private getApplicationTypes() {
    this._rootService.getApplicationTypes().subscribe(
      (response: any) => {
        this.dropDowns.applicationTypes = response;
        this.dropDowns.applicationTypesData = this.dropDowns.applicationTypes;
      },
      err => this.handleError(err)
    );
  }
  private getApplicationStatus() {


    this._rootService.getApplicationStatus().subscribe(
      (response: any) => {
        this.dropDowns.applicationStatus = response;
        this.dropDowns.applicationStatusData = this.dropDowns.applicationStatus;
      },
      err => this.handleError(err)
    );
  }
  public dropdownValueChanged = (value, filter) => {
    if (filter == 'types') {
      this.typeId = value.Id;
    }
    if (filter == 'status') {
      this.statusId = value.Id;
    }
    if (filter == 'office') {
      this.office_Id = value.Id;
    }
  }
  private getApplications() {
    this.kGrid.loading = true;
    this._fileTrackingSystemService.getScannedDocuments(this.kGrid.skip, this.kGrid.pageSize, this.searchQuery, this.office_Id, this.typeId, this.statusId,
      this.range.start, this.range.end).subscribe(
        (response: any) => {
          this.kGrid.data = [];
          this.kGrid.data = response.List;
          this.kGrid.totalRecords = response.Count;
          this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords };
          this.kGrid.loading = false;
          this.kGrid.firstLoad = false;
        },
        err => this.handleError(err)
      );
  }
  public generateSlip = () => {
    this.recieveFiles = true;
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
}
