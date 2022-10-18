import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { KGridHelper } from '../../_helpers/k-grid.class';
import { DropDownsHR } from '../../_helpers/dropdowns.class';
import { Subject } from 'rxjs/Subject';
import { NotificationService } from '../../_services/notification.service';
import { AuthenticationService } from '../../_services/authentication.service';
import { RootService } from '../../_services/root.service';
import { CitizenPortalService } from './citizen-portal.service';
import { PageChangeEvent } from '@progress/kendo-angular-grid';
import { DomSanitizer } from '@angular/platform-browser';
import { Subscription } from 'rxjs/Subscription';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-citizen-portal',
  templateUrl: './citizen-portal.component.html',
  styles: []
})
export class CitizenPortalComponent implements OnInit {
  public loaded: boolean = true;
  public removingApplication: boolean = false;
  public kGrid: KGridHelper = new KGridHelper();
  public user: any;
  public dropDowns: DropDownsHR = new DropDownsHR();
  public inputChange: Subject<any>;
  public searchQuery: string = '';
  public trackingNumber: number = 0;
  public barcode: any;
  public office_Id: number = 0;
  public typeId: number = 0;
  public statusId: number = 0;

  @ViewChild('popup', {static: false}) calendarpopup;
  public applicationTypeWindow: any = {
    dialogOpened: false,
    data: null,
    applicationTypes: [],
  }

  public officer: string = '';
  public type: string = '';
  public officerId: number = 0;
  public forwardingOfficerId: number = 0;
  public pandSOfficers: any[] = [];
  public range = { start: null, end: null };
  private subscription: Subscription;
  private inputChangeSubscription: Subscription;

  constructor(private sanitizer: DomSanitizer,
    private _rootService: RootService,
    private _cpService: CitizenPortalService,
    private _authenticationService: AuthenticationService,
    private _notificationService: NotificationService, private route: ActivatedRoute, private router: Router) { }

  ngOnInit() {
    this.user = this._authenticationService.getUser();
    this.kGrid.loading = true;
    this.fetchParams();
    this.subscribeInputChange();

  }
  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('statusId') && +params['statusId']) {
          this.statusId = +params['statusId'];
        }
        this.officer = localStorage.getItem('officKeyRi');
        localStorage.setItem('officKeyRi', '');
        this.initializeProps();
        this.loadDropDownValues();
      }
    );
  }
  public getApplications() {
    this.kGrid.loading = true;
    this._cpService.getApplications(this.kGrid.skip,
      this.kGrid.pageSize,
      this.searchQuery,
      6,
      this.office_Id,
      false,
      this.typeId,
      this.statusId, this.range.start, this.range.end, this.forwardingOfficerId).subscribe((res: any) => {
        if (res) {
          this.kGrid.data = [];
          this.kGrid.data = res.List;
          console.log(this.kGrid.data);
          this.kGrid.totalRecords = res.Count;
          this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords };
        }
        this.kGrid.loading = false;
        this.kGrid.firstLoad = false;
      }, err => {
        this.kGrid.loading = false;
        this.kGrid.firstLoad = false;
        console.log(err);
      });
  }
  private getApplicationTypes() {
    this._rootService.getApplicationTypesActive().subscribe(
      (response: any) => {
        if (response) {
          this.dropDowns.applicationTypes = response;
          this.dropDowns.applicationTypesData = this.dropDowns.applicationTypes;
          /* if (this.type) {
            let ts = this.dropDowns.applicationTypes as any[];
            let t = ts.find(x => x.Name == this.type);
            if (t) {
              this.dropDowns.selectedFiltersModel.applicationType = t;
              this.typeId = t.Id;
            }
          } else {
            this.typeId = 0;
          } */
          this.getApplications();
        }
      },
      err => this.handleError(err)
    );
  }
  private subscribeInputChange() {
    this.inputChange = new Subject();
    this.inputChangeSubscription = this.inputChange.pipe(debounceTime(300)).subscribe((query) => {
      this.searchQuery = query;
      if (this.searchQuery.length <= 1 && this.searchQuery.length != 0) {
        return;
      }
      this.getApplications();
    });
  }
  private initializeProps() {
    this.kGrid.loading = true;
    this.kGrid.pageSize = 50;
    this.hideCalendarOnClickOutside();
  }
  private loadDropDownValues() {
    this.getPandSOfficers('all');
    this.getApplicationStatus();
    this.getApplicationTypes();
  }
  private getPandSOfficers = (type: string) => {
    this.pandSOfficers = [];
    this._rootService.getPandSOfficers(type).subscribe((res: any) => {
      if (res) {
        this.pandSOfficers = res;
        let pso = this.pandSOfficers as any[];
        let o = pso.find(x => x.DesignationName == this.officer);
        if (o) {
          this.office_Id = o.Id;
          this.dropDowns.selectedFiltersModel.officer = o;
        }
        this.getApplications();
      }
    },
      err => { this.handleError(err); }
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
  public dropdownValueChanged = (value, filter) => {
    if (filter == 'office') {
      this.forwardingOfficerId = value.Id;
    }
    if (filter == 'status') {
      this.statusId = value.Id;
    } if (filter == 'types') {
      this.typeId = value.Id;
    }
    this.getApplications();

  }
  public removeApplication(application) {
    if (confirm('Remove Tracking # ' + application.TrackingNumber + '?')) {
      this.removingApplication = true;
      this.kGrid.loading = true;
      let applicationLog = { Application_Id: application.Id, Action_Id: 17 };
      this._cpService.removeApplication(application.Id, application.TrackingNumber, applicationLog).subscribe((res: any) => {
        if (res) {
          this.removingApplication = false;
          this.getApplications();
        }
      }, err => {
        this.removingApplication = false;
        console.log(err);
      });
    }
  }
  public generateBars(code: number) {
    this.trackingNumber = code;
    this._rootService.generateBarcodeRI(this.trackingNumber).subscribe((x) => {
      if (x) {
        this.barcode = x.barCode;
        setTimeout(() => {
          this.printBarcode();
        }, 1000);
      }
    }, err => {
      console.log(err);

    });
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
  public openInNewTab(link: string) {
    window.open('/' + link, '_blank');
  }
  public dashifyCNIC(cnic: string) {
    if (!cnic) return;
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
  public barcodeSrc() {
    return this.sanitizer.bypassSecurityTrustUrl(this.barcode);
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
  printBarcode() {
    let html = document.getElementById('barcodeFileBars').innerHTML;
    var mywindow = window.open('', 'PRINT', 'height=600,width=900');
    if (mywindow) {
      mywindow.document.write(`<html><head>
      <style>
      button.print {
        padding: 10px 40px;
        font-size: 21px;
        position: absolute;
        margin-left: 40%;
        background: #46a23f;
        background-image: linear-gradient(rgba(63, 162, 79, 0), rgba(63, 162, 79, 0.2));
        cursor: pointer;
        border: none;
        color: #ffffff;
        z-index: 9999;
      }
      @media print {
        button.print {
          display: none;
        }
      }
            </style>
      <title>File</title>`);
      mywindow.document.write('</head><body >');
      mywindow.document.write('<button onclick="printFunc()" class="print">Print</button>');
      mywindow.document.write(html);
      mywindow.document.write(`
                <script>
          function printFunc() {
            window.print();
          }
          </script>
      `);
      mywindow.document.write('</body></html>');
      //show upload signed copy input chooser

      mywindow.document.close(); // necessary for IE >= 10
      mywindow.focus(); // necessary for IE >= 10
      /*    mywindow.print();
        mywindow.close(); */
    }
  }
  ngOnDestroy() {
    this.subscription.unsubscribe();
    this.inputChangeSubscription.unsubscribe();
  }
}
