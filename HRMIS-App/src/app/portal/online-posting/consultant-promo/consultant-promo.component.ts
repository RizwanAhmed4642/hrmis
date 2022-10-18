import { Component, OnInit } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { PageChangeEvent } from '@progress/kendo-angular-grid';
// import { NotificationService } from '@progress/kendo-angular-notification';
import { orderBy, SortDescriptor } from '@progress/kendo-data-query';
import { Subject, Subscription } from 'rxjs-compat';
import { debounceTime } from 'rxjs/operators';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { KGridHelper } from '../../../_helpers/k-grid.class';
import { AuthenticationService } from '../../../_services/authentication.service';
import { RootService } from '../../../_services/root.service';
import { OnlinePostingService } from '../online-porting.service';

type NewType = Subject<any>;

@Component({
  selector: 'app-consultant-promo',
  templateUrl: './consultant-promo.component.html',
})
export class ConsultantPromoComponent implements OnInit {

  public kGrid: KGridHelper = new KGridHelper();
  public dropDowns: DropDownsHR = new DropDownsHR();
  public inputChange: NewType;
  public searchQuery: string = '';
  public ActiveDesignationId: number = 0;
  public designationId: number = 0;
  public currentUser: any;
  public filters: any = {};
  public user: any = {};
  public meritPosting: any = {};
  public esr: any = {};
  public profile: any = {};
  public summary: any = {};
  public selectedMerit: any;
  public selectedMeritPreferences: any[] = [];
  public hfOpened: any[] = [];
  public selectedHF: any = {};
  public actualPostingHF_Id: number = 0;
  public actualPostingHFMISCode: string = '0';
  public selectedVacancy: any[] = [];
  private inputChangeSubscription: NewType;

  public loadingPreferences: boolean = false;
  public showOrder: boolean = false;
  public loadingVacancy: boolean = false;
  public noVacancy: boolean = false;
  public savingOrder: boolean = false;
  public showVacancy: boolean = false;
  public preferencesWindow: boolean = false;
  public searchingHfs: boolean = false;
  public downloadingAcceptanceLetter: boolean = false;
  public acceptanceLetterDownloaded: boolean = false;
  public view: string = 'grid';
  public dateNow: string = '';
  public joiningDate: string = '';
  public offerDate: string = '';
  public imgSrc: string = '';
  public offerMonth: string = '';
  public searchEvent = new Subject<any>();
  public searchSubcription: Subscription = null;


  constructor(
    private sanitized: DomSanitizer, 
    // public _notificationService: NotificationService,
    private _rootService: RootService,
    private _onlinePostingService: OnlinePostingService,
    private _authenticationService: AuthenticationService
  ) { }

  ngOnInit() {
    this.getPromotedCandidates();
    this.getDesignations();
    this.user = this._authenticationService.getUser();
  }

  public getPromotedCandidates() {
    this.view = 'grid';
    this.kGrid.loading = true;
    this._onlinePostingService.getPromotedCandidates({
      Query: this.searchQuery,
      Skip: this.kGrid.skip,
      DesignationId: this.designationId,
      PageSize: this.kGrid.pageSize
    }).subscribe((response: any) => {
      this.kGrid.data = [];
      this.kGrid.data = response.List;
      this.kGrid.totalRecords = response.Count;
      this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords };
      this.kGrid.loading = false;

      console.log('Candidates: ', this.kGrid.data);
    }, err => {
      console.log(err);
    });
  }
  public getMeritsSummary() {
    this.view = 'grid';
    this.kGrid.loading = true;
    this._onlinePostingService.getPostingPlan({
      Query: this.searchQuery,
      DesignationId: this.designationId,
      ActiveDesignationId: this.ActiveDesignationId,
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

  private getMeritActiveDesignations = () => {
    this.dropDowns.designations = [];
    this.dropDowns.designationsData = [];
    this._rootService.getMeritActiveDesignations().subscribe((res: any) => {
      if (res) {
        this.dropDowns.designations = res;
        this.dropDowns.designationsData = this.dropDowns.designations.slice();
      }
    },
      err => { console.log(err); }
    );
  }
  getOfferLetterLink(Id: number) {
    this._onlinePostingService.getDownloadLinkById(Id).subscribe((link) => {
      if (link) {
        window.open('' + link, '_blank');
      }
    });
  }
  public handleSearchEvents() {
    this.searchSubcription = this.searchEvent.pipe(
      debounceTime(400)).subscribe((x: any) => {
        this.search(x.event, x.filter);
      });
  }

  genOrder() {
    this.savingOrder = true;
    let html = document.getElementById('applicationPrintOld').innerHTML;
    this._onlinePostingService.meritOrder(this.selectedMerit.Id, html).subscribe((res: any) => {
      if (res) {
        this.imgSrc = res.imgSrc;
        this.esr = res.esr;
        this.profile = res.hrProfile;
        // this._notificationService.notify('success', 'Order Saved');
      } else {
        this.noVacancy = true;
        // this._notificationService.notify('danger', 'No Vacancy');
      }
      this.savingOrder = false;
    }, err => {
      console.log(err);
    });
  }

  public search(value: string, filter: string) {
    if (filter == 'hfs') {
      this.hfOpened = [];
      this.actualPostingHF_Id = 0;
      if (value.length > 2) {
        this.searchingHfs = true;
        this._rootService.searchHealthFacilitiesAll(value).subscribe((data) => {
          this.hfOpened = data as any[];
          this.searchingHfs = false;
        });
      }
    }

  }
  public getPreferencesOpenWindow(item: any) {
    this.selectedMeritPreferences = [];
    this.selectedMerit = item;
    this.loadingPreferences = true;
    this.preferencesWindow = true;
    this.noVacancy = false;
    {
      this._onlinePostingService.getConsultantPreferences(item.CNIC).subscribe((response: any) => {
        if (response) {
          this.selectedMeritPreferences = response;
          this.loadingPreferences = false;
          console.log('prefs: ', this.selectedMeritPreferences);
          
        }
      }, err => {
        console.log(err);
        this.loadingPreferences = false;
      });
    }
    // this.meritPosting = {};
    // this.showOrder = false;
    // this._onlinePostingService.getMeritPosting(this.selectedMerit.Id).subscribe((response: any) => {
    //   if (response) {
    //     this.meritPosting = response;
    //     this.showOrder = true;
    //   }
    // }, 
    (err => {
      console.log(err);
    });
  }
  public postMerit() {
    this._rootService.postMerit(0).subscribe((response: any) => {
      if (response) {
        console.log(response);
      }
    }, err => {
      console.log(err);
    });
  }
  public postMeritManually() {
    if (!this.actualPostingHF_Id) return;
    this.searchingHfs = true;
    this._rootService.postMeritManually(this.selectedMerit.Id, this.actualPostingHF_Id, this.actualPostingHFMISCode).subscribe((response: any) => {
      if (response) {
        this.loadingPreferences = true;
        this.preferencesWindow = true;
        this.noVacancy = false;
        if (this.selectedMerit && this.selectedMerit.Designation_Id == 373) {
          this.offerDate = '18';
          this.offerMonth = 'November';
        }
        this._onlinePostingService.getPreferences(this.selectedMerit.Id).subscribe((response: any) => {
          if (response) {
            this.selectedMeritPreferences = response;
            this.loadingPreferences = false;
          }
        }, err => {
          console.log(err);
          this.loadingPreferences = false;
        });
        this.meritPosting = {};
        this.showOrder = false;
        this._onlinePostingService.getMeritPosting(this.selectedMerit.Id).subscribe((response: any) => {
          if (response) {
            this.meritPosting = response;
            this.showOrder = true;
          }
        }, err => {
          console.log(err);
        });
      }
      this.searchingHfs = false;
    }, err => {
      console.log(err);
      this.searchingHfs = false;
    });
  }
  public getPostedMerits(dataItem: any) {
    dataItem.loading = true;
    this._onlinePostingService.getPostedMerits(dataItem.HFMISCode, this.selectedMerit.Designation_Id, this.selectedMerit.MeritsActiveDesignationId).subscribe((response: any) => {
      if (response && response.meritPosting) {
        console.log(response);
        dataItem.RemarksP = response.meritPosting;
        if (response.vpMaster) {
          dataItem.vpMaster = response.vpMaster;
        }
        if (response.vpDetails) {
          dataItem.vpDetails = response.vpDetails;
        }
      }
      dataItem.loading = false;
    }, err => {
      console.log(err);
    });
  }
  public getPreferencesVacancy(hrmisCode: any) {
    this.loadingVacancy = true;
    this.showVacancy = false;
    this._onlinePostingService.getPreferenceVacancy(hrmisCode).subscribe((response: any) => {
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
    this.imgSrc = '';
    this.esr = {};
    this.profile = {};
  }

  public searchClicked(FullName, filter) {
    if (filter == 'hfs') {
      let item = this.hfOpened.find(x => x.FullName === FullName);
      if (item) {
        this.actualPostingHF_Id = item.Id;
        this.actualPostingHFMISCode = item.HFMISCode;
      }
    }
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
    if (filter == 'designation') {
      this.designationId = value.Id;
      this.getPromotedCandidates();
    }
  }
  public getAcceptanceLetterLink() {
    this.downloadingAcceptanceLetter = true;
    this._onlinePostingService.getDownloadLink('acceptance', this.selectedMerit.CNIC).subscribe((link) => {
      if (link) {
        this.downloadingAcceptanceLetter = false;
        window.open('' + link, '_blank');
        this.acceptanceLetterDownloaded = true;
      } else {
        this.acceptanceLetterDownloaded = false;
      }
    });
  }
  public openInNewTab(type: string, item: any) {
    if (type == 'acceptance') {
      window.open('/Uploads/Acceptances/' + item.Id + '_OfferLetter.jpg', '_blank');
    } else if (type == 'offer') {
      this._onlinePostingService.getDownloadLink('offer', item.Cnic).subscribe((link) => {
        if (link) {
          window.open('' + link, '_blank');
        }
      });
    } else if (type == 'preferences') {
      this._onlinePostingService.getDownloadLink('offer', item.Cnic).subscribe((link) => {
        if (link) {
          window.open('' + link, '_blank');
        }
      });
    } else {
      window.open('' + type, '_blank');
    }
  }
  private initializeProps() {
    this.kGrid.loading = true;
    this.kGrid.pageSize = 50;
    this.kGrid.pageSizes = [50, 100, 200, 300, 500];

    let today = new Date();
    let dd: any = today.getDate(), mm: any = this.makeOrderMonth(today.getMonth() + 1), yyyy = today.getFullYear();

    this.dateNow = this.makeOrderDate(dd) + ', ' + mm + ' ' + yyyy;
    //this.offerDate = this.makeOrderDate(dd) + ', ' + mm + ' ' + yyyy;

    let joiningDay = new Date();
    joiningDay.setDate(joiningDay.getDate() + 15);
    let dd2: any = joiningDay.getDate(), mm2: any = this.makeOrderMonth(joiningDay.getMonth() + 1), yyyy2 = joiningDay.getFullYear();
    this.joiningDate = this.makeOrderDate(dd2) + ', ' + mm2 + ' ' + yyyy2;
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
    this.getPromotedCandidates();
  }
  public changePagesize(value: any) {
    this.kGrid.pageSize = +value;
    this.kGrid.skip = 0;
    this.getPromotedCandidates();
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
  transform(value) {
    return this.sanitized.bypassSecurityTrustHtml(value);
  }
  public barcodeSrc() {
    return this.sanitized.bypassSecurityTrustUrl(this.imgSrc);
  }
  makeOrderDate(day: number) {
    return day == 1 || day == 21 || day == 31 ? day + '<sup>st</sup>'
      : day == 2 || day == 22 ? day + '<sup>nd</sup>'
        : day == 3 || day == 23 ? day + '<sup>rd</sup>'
          : (day => 4 && day <= 20) || (day => 24 && day <= 30) ? day + '<sup>th</sup>' : '';
  }
  makeOrderMonth(month: number) {
    return month == 1 ? 'January'
      : month == 2 ? 'February'
        : month == 3 ? 'March'
          : month == 4 ? 'April'
            : month == 5 ? 'May'
              : month == 6 ? 'June'
                : month == 7 ? 'July'
                  : month == 8 ? 'August'
                    : month == 9 ? 'September'
                      : month == 10 ? 'October'
                        : month == 11 ? 'November'
                          : month == 12 ? 'December' : '';
  }

  printApplication() {
    let html = document.getElementById('applicationPrintOld').innerHTML;
    if (this.esr.Id) {
      this._onlinePostingService.UpdateOrderHTML(this.esr.Id, html).subscribe((res: any) => {
        if (res) {
          console.log(res);
        }
      }, err => {
        console.log(err);

      });
      var mywindow = window.open('', 'PRINT', 'height=600,width=900');
      if (mywindow) {
        mywindow.document.write(`<html><head>
          <style>
            
            body {
           
              font-family: sans-serif, Arial, Verdana, "Trebuchet MS";
              font-size: 13px;
              font-style: normal;
              font-variant: normal;
              font-weight: 400;
              line-height: 1.6;
              color: #383e4b;
              background-color: #fff;
          }
          p{
              word-wrap: break-word;
              font-family: sans-serif, Arial, Verdana, "Trebuchet MS";
              font-size: 13px;
              font-style: normal;
              font-variant: normal;
              font-weight: 400;
              line-height: 1.6;
          }
          .mt-2 {
            margin-top: 0.5rem !important;
          }.mb-0 {
            margin-bottom: 0 !important;
          }
          .ml-1 {
            margin-left: 0.25rem !important;
          }
          .mb-2 {
            margin-bottom: 0.5rem !important;
          }
       
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
        
  
          .w-20 { width: 20% !important; }
          .w-30 { width: 30% !important; }
          .w-50 { width: 50% !important; }
          .w-70 { width: 70% !important; }
          .w-80 { width: 80% !important; }
  
          .mt-10 { margin-top: 10px !important; }
          .mt-30 { margin-top: 30px  !important; }
          @media print {
            button.print {
              display: none;
            }
          }
                </style>
                <title>Application</title>`);


        mywindow.document.write('</head><body >');
        mywindow.document.write('<button onclick="printFunc()" class="print">Print</button>');
        mywindow.document.write(html);
        mywindow.document.write(`
      <div class="divFooter" style="position: fixed;
      bottom: 0;    left: 25%;color:#e3e3e3;">Powered by Health Information and
        Service Delivery Unit</div>
                  <script>
            function printFunc() {
              window.print();
            }
            </script>
        `);
        mywindow.document.write('</body></html>');

        /*    mywindow.document.close(); // necessary for IE >= 10
           mywindow.focus(); // necessary for IE >= 10
           mywindow.print();
           mywindow.close(); */
      }
    }
  }



}
