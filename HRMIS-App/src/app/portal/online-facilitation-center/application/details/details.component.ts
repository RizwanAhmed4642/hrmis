import { Component, OnInit } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { ApplicationAttachment, ApplicationDocument, ApplicationLog, ApplicationLogView } from '../../../../modules/application-fts/application-fts';
import { DropDownsHR } from '../../../../_helpers/dropdowns.class';
import { RootService } from '../../../../_services/root.service';
import { OnlineFacilitationCenterService } from '../../online-facilitation-center.service';

@Component({
  selector: 'app-details',
  templateUrl: './details.component.html',
})
export class DetailsComponent implements OnInit {

  public today = new Date();
  public count = 3;
  public barcodeImgSrc: any = {};
  public exist: boolean = true;
  public loading: boolean = false;
  public trackingNo: number = 132789;

  public application: any = {
      };

  public logRefreshing: boolean = true;
  public logAscOrder: boolean = true;
  public applicationId: number = 0;
  public applicationtracking: number = 0;
  public applicationOlds: any[] = [];
  public ddsFile: any;
  public personAppeared: any;
  public fileRequisitions: any[] = [];
  private subscription: Subscription;

  public lastLogId: number = 0;
  public applicationDocuments: ApplicationDocument[] = [];
  public applicationLog: ApplicationLog = new ApplicationLog();
  public applicationLogs: ApplicationLogView[] = [];
  public applicationAttachments: ApplicationAttachment [] = [];
  public dropDowns: DropDownsHR = new DropDownsHR();
  public divisions: any[] = [];
  public districts: any[] = [];
  public tehsils: any[] = [];

  

  constructor(
    private sanitizer: DomSanitizer,
    private _onlineFacilitationService: OnlineFacilitationCenterService,
    private _rootService: RootService,
    private route: ActivatedRoute,
    private router: Router) { }

  ngOnInit() {
    this.dashifyCNIC();
    this.fetchParams();
  }

  // public printApplication()
  // {

  // }

  
  public dashifyCNIC()
  {
    return this.application.CNIC;
  }

  public barcodeSrc() {
    return this.sanitizer.bypassSecurityTrustUrl(this.barcodeImgSrc);
  }

  private getApplicationStatus() {
    this._rootService.getApplicationStatus().subscribe(
      (response: any) => {
        if (response) {
          let statuses = [];
          response.forEach(element => {
            if (element.Id == 1 || element.Id == 2 || element.Id == 3 || element.Id == 8 || element.Id == 10) {
              statuses.push(element);
            }
            if (element.Id == 4 && this.application.FileRequested && this.application.FileRequestStatus_Id == 3) {
              statuses.push(element);
            }
            if (element.Id == 4 && this.application.ApplicationSource_Id == 5) {
              statuses.push(element);
            }
          });
          this.dropDowns.applicationStatus = statuses;
          this.dropDowns.applicationStatusData = this.dropDowns.applicationStatus;
        }

      },
      err => console.log(err)
      
    );
  }

  private resetDropsBelow = (filter: string) => {
    this.application.toHealthFacility = '';
    this.dropDowns.selectedFiltersModel.healthFacilityForTransfer = { Name: 'Select Health Facility', Id: 0 };
    if (filter == 'division') {
      this.dropDowns.selectedFiltersModel.districtForTransfer = { Name: 'Select District', Code: '0' };
      this.dropDowns.selectedFiltersModel.tehsilForTransfer = { Name: 'Select Tehsil', Code: '0' };
    }
    if (filter == 'district') {
      this.dropDowns.selectedFiltersModel.tehsilForTransfer = { Name: 'Select Tehsil', Code: '0' };
    }
  }


  private getAll = (code: string) => {
    if (code.length <= 1) {
      this._rootService.getDivisions(code).subscribe((res: any) => {
        this.divisions = res;
      },
        err => { console.log(err);
         }
      );
    }
    if (code.length <= 3) {
      this.resetDropsBelow('division');
      this._rootService.getDistricts(code).subscribe((res: any) => {
        this.districts = res;
      },
        err => { console.log(err);
         }
      );
    }
    if (code.length <= 6) {
      this.resetDropsBelow('district');
      this._rootService.getTehsils(code).subscribe((res: any) => {
        this.tehsils = res;
      },
        err => { console.log(err); }
      );
    }
  }

  private getDivisions = (code: string) => {
    this.dropDowns.divisions = [];
    this.dropDowns.divisionsData = [];
    this._rootService.getDivisions(code).subscribe((res: any) => {

      this.dropDowns.divisions = res;
      this.dropDowns.divisionsData = this.dropDowns.divisions.slice();
      if (this.application && this.application.HfmisCode && this.application.HfmisCode.length == 19) {
        let division = this.dropDowns.divisionsData.find(x => x.Code == this.application.HfmisCode.substring(0, 3));
        if (division) {
          this.dropDowns.selectedFiltersModel.division = { Code: division.Code, Name: division.Name };
        }
      }
    },
      err => { console.log(err);
       }
    );
  }

  private getDistricts = (code: string) => {
    this.dropDowns.districts = [];
    this.dropDowns.districtsData = [];
    this._rootService.getDistricts(code).subscribe((res: any) => {
      this.dropDowns.districts = res;
      this.dropDowns.districtsData = this.dropDowns.districts.slice();
      if (this.application && this.application.HfmisCode && this.application.HfmisCode.length == 19) {

        let district = this.dropDowns.districtsData.find(x => x.Code == this.application.HfmisCode.substring(0, 6));
        if (district) {
          this.dropDowns.selectedFiltersModel.district = { Code: district.Code, Name: district.Name };
        }

      }
    },
      err => { console.log(err);
       }
    );
  }
  private getTehsils = (code: string) => {
    this.dropDowns.tehsils = [];
    this.dropDowns.tehsilsData = [];
    this._rootService.getTehsils(code).subscribe((res: any) => {
      this.dropDowns.tehsils = res;
      this.dropDowns.tehsilsData = this.dropDowns.tehsils.slice();
      if (this.application && this.application.HfmisCode && this.application.HfmisCode.length == 19) {
        let tehsil = this.dropDowns.tehsilsData.find(x => x.Code == this.application.HfmisCode.substring(0, 9));
        if (tehsil) {
          this.dropDowns.selectedFiltersModel.tehsil = { Code: tehsil.Code, Name: tehsil.Name };
        }
      }
    },
      err => { console.log(err);
       }
    );
  }

  private getApplicationDocuments = () => {
    // this.loadingDocs = true;
    this.applicationDocuments = [];
    this._rootService.getApplicationDocuments(this.application.ApplicationType_Id).subscribe((res: any[]) => {
      if (res) {

        let tempIds = [];
        this.applicationAttachments.forEach(x => {
          let doc = res.find(y => y.Id == x.Document_Id);
          if (doc) {
            tempIds.push(doc.Id);
          }
        });
        res.forEach(doc => {
          let id = tempIds.find(z => z == doc.Id);
          if (!id) {
            this.applicationDocuments.push(doc);
          }
        });
        /*       this.applicationDocuments = res.filter(doc => {
                return this.applicationAttachments.findIndex(m => m.Document_Id == doc.Id) < 0;
              }); */
      }

      // this.loadingDocs = false;
      // this.uploadingAttachments = false;
      // this.applicationAttachmentsUpload = [];
    },
      err => { console.log(err); }
    );
  }

  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('id') && +params['id'] && params.hasOwnProperty('tracking') && +params['tracking']) {
          this.applicationId = +params['id'];
          this.applicationtracking = +params['tracking'];
          this.getApplication(this.applicationId, this.applicationtracking);
        } else {
        }
      }
    );
  }

  private getApplication(id, tracking) {
    this._onlineFacilitationService.getApplication(+id, +tracking).subscribe((data: any) => {
      if (data) {
        this.application = data.application;
        this.application.DateOfBirth = new Date(this.application.DateOfBirth);
        this.application.FromDate = new Date(this.application.FromDate);
        this.application.ToDate = new Date(this.application.ToDate);
        this._rootService.generateBars(this.application.TrackingNumber).subscribe((res: any) => { this.barcodeImgSrc = res.barCode; }, err => { console.log(err);});
        this.applicationAttachments = data.applicationAttachments as ApplicationAttachment[];
        this.applicationLog.Application_Id = this.application.Id;
        this.dropDowns.selectedFiltersModel.applicationStatus = { Id: this.application.Status_Id, Name: this.application.StatusName };
        this.dropDowns.selectedFiltersModel.leaveType = { Id: this.application.LeaveType_Id, LeaveType1: this.application.leaveType };
        this.dropDowns.selectedFiltersModel.officer = { Id: this.application.PandSOfficer_Id, DesignationName: this.application.PandSOfficerName };
        this.getApplicationStatus();
        this.getAll('0');
        this.getDivisions('0');
        this.getDistricts('0');
        this.getTehsils('0');
 
        this.loading = false;
        // this.subscribeLiveApp();
        this.getApplicationData("logs");
        this.getApplicationData("oldlogs");
        this.getApplicationData("file");
        this.getApplicationData("filereqs");
        this.getApplicationData("parliamentarian");
        this.getApplicationDocuments();
        // if (this.application.ApplicationType_Id == 2 && (this.user.UserName == 'og1' || this.user.UserName == 'ds.admin' || this.user.UserName == 'so.toqeer' || this.user.UserName == 'ds.general' || this.user.UserName == 'ordercell')) {
        //   this.checkVacancy();
        //   this.getVpMaster();
        // }
        // this.closeWindow();
      }
    }, err => { console.log(err);
     }
    );
  }

  public capitalize(val: string) {
    if (!val) return '';
    if (!val.toLowerCase().endsWith('application')) val += ' Application';
    return val.toUpperCase();
  }

  public getApplicationLog(all: boolean) {
    this.logRefreshing = true;
    let lastLog = null;
    if (this.applicationLogs.length > 0) {
      lastLog = this.applicationLogs[this.applicationLogs.length - 1];
      this.lastLogId = lastLog ? lastLog.Id : 0;
    }
    if (!all) {
      this.lastLogId = 0;
    }
    this._onlineFacilitationService.getApplicationLogs(this.application.Id, this.lastLogId, this.logAscOrder).subscribe((data: any) => {
      if (data) {
        if (lastLog) {
          data.forEach(element => {
            if (this.logAscOrder) {
              this.applicationLogs.push(element);
            } else {
              this.applicationLogs.unshift(element);
            }
          });
        } else {
          this.applicationLogs = data as ApplicationLogView[];
        }
        this.logRefreshing = false;
      }
    }, err => { this.logRefreshing = false; console.log(err);
     }
    );
  }

  private getApplicationData(type) {
    this._onlineFacilitationService.getApplicationData(this.application.Id, type).subscribe((data: any) => {
      if (data) {
        if (type == "logs") {
          this.getApplicationLog(true);
        }
        else if (type == "oldlogs") {
          this.applicationOlds = data.applicationForwardLogs;
        }
        else if (type == "file") {
          this.ddsFile = data.File;
        }
        else if (type == "parliamentarian") {
          this.personAppeared = data.applicationPersonAppeared;
        }
        else if (type == "filereqs") {
          this.fileRequisitions = data.applicationFileRecositions;
          if (this.fileRequisitions.length > 0) {
            console.log(this.fileRequisitions);
            /*  this.fileRequstedBy */
          }
        }

        this.loading = false;
      }
    }, err => { console.log(err);
     }
    );
  }

  public openWindow()
  {

  }

  printApplication() {
    let html = document.getElementById('applicationPrint').innerHTML;
    var mywindow = window.open('', 'PRINT', 'height=600,width=900');
    if (mywindow) {
      mywindow.document.write(`<html><head>
        <style>
            body {
              font-family: -apple-system, BlinkMacSystemFont, 
            'Segoe UI', 'Roboto' , 'Oxygen' , 'Ubuntu' , 'Cantarell' , 'Fira Sans' , 'Droid Sans' , 'Helvetica Neue' ,
              sans-serif !important;
            }
          p {
            margin-top: 0;
            margin-bottom: 1rem !important;
        }.mt-2 {
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
        .application-page {

          padding: 50px;
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
        .watermark-hisdu {
          text-align: center;
          position: absolute;
          left: 0;
          width: 100%;
          opacity: 0.25;
        }

        .watermark-hisdu img {
          display: inline-block;
        }

        table.header-pshealth,
        .applicant-information,
        .application-type-detail-preview,
        .attached-document,
        .remarks-preview,
        .info-application-preview,
        table.pshealth {
          border-color: transparent !important;
          width: 100%;
        }

        table.header-pshealth td {
          border-color: transparent !important;
        }

        table.header-pshealth td.gop-logo-a4-header {
          text-align: left;
        }

        table.header-pshealth td.gop-logo-a4-header img {
          display: inline-block;
          width: 134px;
        }

        table.header-pshealth td.pshealth-right-a4-td-header {
          text-align: right;
        }

        table.header-pshealth td.pshealth-right-a4-td-header .pshealth-right-a4-text-header {
          display: inline-block;
          text-align: center;
        }

        table.header-pshealth td.app-type-preview {
          text-align: left;
          width: 100%;
        }

        /* Applicant Information */

        table.applicant-information {
          border-color: transparent !important;
          width: 100%;
        }

        table.applicant-information td.applicant-info-heading,
        table.application-type-detail-preview td.application-type-detail-preview-heading,
        table.remarks-preview td.remarks-heading,
        table.info-application-preview td.info-application-preview-heading,
        table.attached-document td.attached-document-heading {
          text-align: center;
          border: 1px solid black;
        }

        table.applicant-information td.applicant-info-detail-1 {
          width: 20% !important;
        }

        table.applicant-information td.applicant-info-detail-2 {
          width: 40% !important;
        }

        table.applicant-information td.applicant-info-detail-3 {
          width: 10% !important;
        }

        table.applicant-information td.applicant-info-detail-4 {
          width: 30% !important;
        }
        table.applicant-information td.applicant-info-detail-5 {
          width: 15% !important;
        }

        table.applicant-information td.applicant-info-detail-6 {
          width: 30% !important;
        }

        table.applicant-information td.applicant-info-detail-7 {
          width: 20% !important;
        }

        table.applicant-information td.applicant-info-detail-8 {
          width: 35% !important;
        }
        table.info-application-preview td.info-application-preview-left {
          border-left: 1px solid black;
        }

        table.info-application-preview td.info-application-preview-right {
          text-align: center;
          margin: 5px 5px !important;
          border-right: 1px solid black;
          border-left: 1px solid black;
        }

        table.application-route-detail {
          border-color: transparent !important;
          width: 100% !important;
          text-align: center;
        }

        table.application-route-detail td.application-route-detail-header {
          width: 50% !important;
          border: 1px solid black;
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
      // mywindow.document.write('<button onclick="printFunc()" class="print">Print</button>');
      mywindow.document.write(html);
      mywindow.document.write(`
                <script>
          function printFunc() {
            window.print();
          }
          </script>
      `);
      mywindow.document.write('</body></html>');
  
    }
  }

}
