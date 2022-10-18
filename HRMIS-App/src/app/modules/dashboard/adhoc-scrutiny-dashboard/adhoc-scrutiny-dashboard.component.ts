import { Component, OnInit } from '@angular/core';
import { PageChangeEvent } from '@progress/kendo-angular-grid';
import { orderBy, SortDescriptor } from '@progress/kendo-data-query';
import { KGridHelper } from '../../../_helpers/k-grid.class';
import { LocalService } from '../../../_services/local.service';

import { aggregateBy } from '@progress/kendo-data-query';
import { DashboardService } from '../dashboard.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { NotificationService } from '../../../_services/notification.service';

@Component({
  selector: 'app-adhoc-scrutiny-dashboard',
  templateUrl: './adhoc-scrutiny-dashboard.component.html',
})
export class AdhocScrutinyDashboardComponent implements OnInit {

  public user: any = {};
  public kGrid: KGridHelper = new KGridHelper();
  public summary: any = {};
  public selectedItem: any = {};
  public applicants: any[] = [];
  public applicantsSummary: any[] = [];
  public applicationsDesig: any[] = [];
  public applicationsDesignation: any = { Id: String, Approved: Number, Designation: String, New: Number, OrderBy: Number, Rejected: Number, Submitted: Number, TotalApplicatons: Number };
  public aggregates: any[] = []
  public aggregatesTotal: any[] = []
  public TotalApplications: any[] = [];
  public photoFile: any[] = [];
  public documents: any[] = [];
  public report: any = {};
  public total: any = {};
  public adhocScrutinyCommittee: any = {};
  public totalApp: any;

  public toggle: boolean = true;
  public max: any = 12345;
  public dateNow: string = '';
  public districtName: string = '';
  public saving: boolean = false;
  constructor(private _localService: LocalService, private _notificationService: NotificationService, private _authenticationService: AuthenticationService, private _adhocService: DashboardService) { }
  public getUser() {
    return this._localService.get('ussr');
  }

  ngOnInit() {
    this.user = this._authenticationService.getUser();
    let today = new Date();
    let dd: any = today.getDate(), mm: any = today.getMonth() + 1, yyyy = today.getFullYear();
    if (dd < 10) dd = '0' + dd; if (mm < 10) mm = '0' + mm;
    this.dateNow = dd + '/' + mm + '/' + yyyy;
    this.getAdhocApplicants();
    this.getAdhocApplicantsSummary();
    this.getAdhocCounts();
    this.getAdhocScrutinyCommittee();
    this._authenticationService.userLevelNameEmitter.subscribe((x: any) => {
      this.districtName = x;
      this.districtName = this.districtName ? this.districtName.replace(' District', '') : 'District';
    });
  }
  public getAdhocApplicantsSummary() {
    this._adhocService.getAdhocApplicantsSummary().subscribe((response: any) => {
      this.applicantsSummary = response;
      this.kGrid.loading = false;
    }, err => {
      console.log(err);
    });
  }

  public getAdhocScrutinyCommittee() {
    this._adhocService.getAdhocScrutinyCommittee(this.user.HfmisCode).subscribe((response: any) => {
      if (response) {
        if (response.NotificationDated) {
          response.NotificationDated = new Date(response.NotificationDated);
        }
        if (response.MeetingDate) {
          response.MeetingDate = new Date(response.MeetingDate);
        }
        this.adhocScrutinyCommittee = response;
      }
      else {
        this.adhocScrutinyCommittee = {};
        this.toggle = true;
      }
    }, err => {
      console.log(err);
    });
  }
  public saveAdhocScrutinyCommittee() {
    this.saving = true;
    if (this.adhocScrutinyCommittee.NotificationDated) {
      this.adhocScrutinyCommittee.NotificationDated = this.adhocScrutinyCommittee.NotificationDated.toDateString();
    }
    if (this.adhocScrutinyCommittee.MeetingDate) {
      this.adhocScrutinyCommittee.MeetingDate = this.adhocScrutinyCommittee.MeetingDate.toDateString();
    }
    this.adhocScrutinyCommittee.DistrictCode = this.user.HfmisCode;
    this.adhocScrutinyCommittee.District = this.districtName;

    this._adhocService.saveAdhocScrutinyCommittee(this.adhocScrutinyCommittee).subscribe((response: any) => {
      if (response) {
        if (response.NotificationDated) {
          response.NotificationDated = new Date(response.NotificationDated);
        }
        if (response.MeetingDate) {
          response.MeetingDate = new Date(response.MeetingDate);
        }
        this.adhocScrutinyCommittee = response;

        if (this.adhocScrutinyCommittee.Id > 0 && this.photoFile.length > 0) {
          this._adhocService.uploadCommitteeNotification(this.photoFile, this.adhocScrutinyCommittee.Id).subscribe((res4: any) => {
            this.saving = false;
            this.photoFile = [];
            this._notificationService.notify('success', 'Scrutiny Committee Saved');
          }, err => {
            console.log(err);
          });
        } else {
          this.saving = false;
          this._notificationService.notify('success', 'Scrutiny Committee Saved');
        }
      }
      else {
        this.adhocScrutinyCommittee = {};
      }
    }, err => {
      console.log(err);
    });
  }
  public getAdhocCounts() {
    this._adhocService.getAdhocDashboardDistrict(this.user.HfmisCode).subscribe((response: any) => {
      //this.applicationsDesig = response;
      console.log(response);

      response.forEach(element => {
        let totalsums = 0;
        this.applicationsDesignation = {};
        this.applicationsDesignation.Id = element.Id;
        this.applicationsDesignation.Designation_Id = element.Designation_Id;
        this.applicationsDesignation.Approved = element.Approved;
        this.applicationsDesignation.Designation = element.Designation;
        this.applicationsDesignation.New = element.New;
        this.applicationsDesignation.OrderBy = element.OrderBy;
        this.applicationsDesignation.Rejected = element.Rejected;
        this.applicationsDesignation.Submitted = element.Submitted;
        this.applicationsDesignation.TotalApplications = (element.New == null ? 0 : element.New) + (element.Approved == null ? 0 : element.Approved) + (element.Submitted == null ? 0 : element.Submitted);
        this.applicationsDesig.push(this.applicationsDesignation);
      });
      console.log('applicationsDesig: ', this.applicationsDesig);
      this.getAggregations();
      //this.getAggregationsTotalApplication();
      this.kGrid.loading = false;
    }, err => {
      console.log(err);
    });
  }

  public getAdhocApplicants() {
    this.kGrid.loading = true;
    this._adhocService.getAdhocApplicantsDash({
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

  public getAdhocApplication = (item) => {
    this.selectedItem = item;
    /*   if ((this.selectedItem.Submitted ? this.selectedItem.Submitted : 0) != 0 || !this.adhocScrutinyCommittee.MSName
        || !this.adhocScrutinyCommittee.MSHFName
        || !this.adhocScrutinyCommittee.MSRole
        || !this.adhocScrutinyCommittee.FPName
        || !this.adhocScrutinyCommittee.FPHFName
        || !this.adhocScrutinyCommittee.FPRole
        || !this.adhocScrutinyCommittee.NotificationDated
        || !this.adhocScrutinyCommittee.DHOName
        || !this.adhocScrutinyCommittee.NotificationNumber
        || !this.adhocScrutinyCommittee.DHOHFName
        || !this.adhocScrutinyCommittee.MeetingDate
        || !this.adhocScrutinyCommittee.NotificationPath
        || !this.adhocScrutinyCommittee.DHORole) {
        return;
      } */
    item.loading = true;
    this.documents = [];
    let obj = {
      hfmisCode: this.user.HfmisCode,
      Designation_Id: item.Designation_Id,
      Status_Id: 100
    };

    this._adhocService.gGetAdhocApplicationScrutinyPrint(obj).subscribe((res: any) => {
      if (res) {
        if (res.applications) {
          this.documents = res.applications;
          let adhocScrutinies = res.adhocScrutinies;
          console.log(adhocScrutinies);

          this.documents.forEach(doc => {
            if (doc.Status_Id == 3) {
              doc.adhocScrutinies = adhocScrutinies.filter(x => x.Application_Id == doc.Id);
              console.log(doc.adhocScrutinies);
            }
          });
          this.report.Designation = this.documents[0] ? this.documents[0].DesignationName : '';
        }
        if (res.report) {
          let temp = res.report;
          this.report.Total = temp.find(x => x.StatusId == 4);
          if (!this.report.Total) this.report.Total = { Total: 0 };
          this.report.Approved = temp.find(x => x.StatusId == 2);
          if (!this.report.Approved) this.report.Approved = { Total: 0 };
          this.report.Rejected = temp.find(x => x.StatusId == 3);
          if (!this.report.Rejected) this.report.Rejected = { Total: 0 };
          if (this.report.Total && this.report.Approved && this.report.Rejected) {
            this.report.Total.Total = this.report.Total.Total + this.report.Approved.Total + this.report.Rejected.Total
          }
        }
        setTimeout(() => {
          item.loading = false;
          this.printApplication();
        }, 1500);
        /* this.kGrid.totalRecords = res.Count; */
        /*   this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords }; */
      }
    },
      err => {
        console.log(err);
      }
    );
  }

  public readUrl(event: any, obj: any) {
    if (event.target.files && event.target.files[0]) {
      this.photoFile = [];
      let inputValue = event.target;
      this.photoFile = inputValue.files;
      if (obj) {
        obj.attached = true;
        obj.error = false;
      }
    }
  }
  public getAggregations() {
    this.aggregates = [
      { field: 'Submitted', aggregate: 'sum' },
      { field: 'Approved', aggregate: 'sum' },
      { field: 'Rejected', aggregate: 'sum' },
      { field: 'New', aggregate: 'sum' },
      { field: 'TotalApplications', aggregate: 'sum' }]
    this.total = aggregateBy(this.applicationsDesig, this.aggregates);
  }

  public capitalize(val: string) {
    if (!val) return '';
    return val.toUpperCase();
  }
  private sortData() {
    this.kGrid.gridView = {
      data: orderBy(this.kGrid.data, this.kGrid.sort),
      total: this.kGrid.totalRecords
    };
  }
  public pageChange(event: PageChangeEvent): void {
    this.kGrid.skip = event.skip;
    // this.getMerits();
  }
  public changePagesize(value: any) {
    this.kGrid.pageSize = +value;
    this.kGrid.skip = 0;
    // this.getMerits();
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
  public phonifyNumber(phonenumber: string) {
    if (!phonenumber) return '';
    if (phonenumber.length != 11) return '';
    return phonenumber[0] +
      phonenumber[1] +
      phonenumber[2] +
      phonenumber[3] +
      '-' +
      phonenumber[4] +
      phonenumber[5] +
      phonenumber[6] +
      phonenumber[7] +
      phonenumber[8] +
      phonenumber[9] +
      phonenumber[10];
  }
  public sortChange(sort: SortDescriptor[]): void {
    if (sort[0].field == 'asd') { return; }
    this.kGrid.sort = sort;
    this.sortData();
  }

  public openInNewTab(link: string) {
    window.open('/' + link, '_blank');
  }
  printApplication() {
    let html = document.getElementById('phfmchr').innerHTML;
    var mywindow = window.open('', 'PRINT', 'height=600,width=900');
    if (mywindow) {
      mywindow.document.write(`<html><head>
          <style>
          body {
            font-family: -apple-system, system-ui, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;
         margin: 25px !important;
        }
     
        table { page-break-inside:auto }
    tr    { page-break-inside:avoid; page-break-after:auto }
    thead { display:table-header-group }
    tfoot { display:table-footer-group }
   p { margin: 0px !important;}
          p.heading {
            text-align: center;
            margin-top: 5px;
            margin-bottom: 5px;
        }
        
        p.normalPara {
            margin-left: 5px;
            font-family: sans-serif, Arial, Verdana, "Trebuchet MS";
            font-weight: bold;
        }
        
        p.normalPara u {
            font-weight: 100 !important;
        }
        
        p.normalPara span.left {
            float: left;
        }
        
        p.normalPara span.right {
            float: right;
        }
        
        .candidate-photo {
            width: 150px;
            height: 175px;
        }
        
        .candidate-photo img {
            width: 100%;
            height: 100%;
            border: 1px solid #000000;
        }
        
        table.info {
            font-family: sans-serif, Arial, Verdana, "Trebuchet MS";
            width: 100%;
        }
        
        table.info td {
            border: none;
        }
        
        table.info th {
            border: none;
    font-size: 14px !important;
  }
        
        table.doc {
            border-collapse: collapse;
            width: 100%;
        }
        
        table.doc td {
            border: 1px solid #000000;
            text-align: center;
            padding: 8px;
    font-size: 12px !important;
        }
        
        table.doc th {
            border: 1px solid #000000;
            text-align: center;
            padding: 8px;
        }
          button.print {
            padding: 10px 40px;
            font-size: 21px;
            position: absolute;
            margin-left: 40%;
            background: #424242;
            background-image: linear-gradient(rgba(63, 162, 79, 0), rgba(63, 162, 79, 0.2));
            cursor: pointer;
            border: none;
            color: #ffffff;
            z-index: 9999;
          }
        
  
          .pr-5 { padding-right: 3rem !important; }
          .pr-3 { padding-right: 1rem !important; }
          .pr-2 { padding-right: 0.5rem !important; }
          .pl-2 { padding-left: 0.5rem !important; }
          .m-0 { margin: 0px !important; }
          @media print {
            .page{
              padding: 40px !important;
            }
            button.print {
              display: none;
            }
          }
                </style>
                <title>Application</title>`);
      mywindow.document.write('</head><body >');
      //mywindow.document.write('<button onclick="printFunc()" class="print">Print</button>');
      mywindow.document.write(html);
      mywindow.document.write(`
      <div class="divFooter" style="position: fixed;
      bottom: 0;    left: 27%;color:#e3e3e3;">Powered by Health Information and
        Service Delivery Unit</div>
                  <script>
            function printFunc() {
              window.print();
            }
            </script>
        `);
      mywindow.document.write('</body></html>');

      /*  mywindow.print();
       mywindow.close(); */
    }
  }
}
