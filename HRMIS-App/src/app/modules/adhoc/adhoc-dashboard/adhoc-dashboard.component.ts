import { Component, OnInit } from '@angular/core';
import { PageChangeEvent } from '@progress/kendo-angular-grid';
import { orderBy, SortDescriptor } from '@progress/kendo-data-query';
import { KGridHelper } from '../../../_helpers/k-grid.class';
import { LocalService } from '../../../_services/local.service';
import { AdhocService } from '../adhoc.service';
import { aggregateBy } from '@progress/kendo-data-query';

@Component({
  selector: 'app-adhoc-dashboard',
  templateUrl: './adhoc-dashboard.component.html'
})
export class AdhocDashboardComponent implements OnInit {
  public user: any = {};
  public kGrid: KGridHelper = new KGridHelper();
  public summary: any = {};
  public selectedItem: any = {};
  public applicants: any[] = [];
  public adhocPendencySummary: any[] = [];
  public applicantsSummary: any[] = [];
  public applicationsDesig: any[] = [];
  public applicationsDesignation: any = { Id: String, Approved: Number, Designation: String, New: Number, OrderBy: Number, Rejected: Number, Submitted: Number, TotalApplicatons: Number };
  public aggregates: any[] = []
  public aggregatesTotal: any[] = []
  public TotalApplications: any[] = [];
  public documents: any[] = [];
  public total: any;
  public totalSum: any = {
    New: 0,
    Approved: 0,
    Rejected: 0,
    Submitted: 0,
    TotalGrievance: 0,
    ApprovedGrievance: 0,
    RejectedGrievance: 0
  };
  public designations: any[] = [
    { Id: 362, Designation: 'Consultant Anaesthetist' },
    { Id: 365, Designation: 'Consultant Cardiologist' },
    { Id: 368, Designation: 'Consultant Dermatologist' },
    { Id: 369, Designation: 'Consultant ENT Specialist' },
    { Id: 373, Designation: 'Consultant Gynaecologist' },
    { Id: 374, Designation: 'Consultant Ophthalmologist' },
    { Id: 375, Designation: 'Consultant Orthopaedic' },
    { Id: 381, Designation: 'Consultant Paediatrician' },
    { Id: 382, Designation: 'Consultant Pathologist' },
    { Id: 383, Designation: 'Consultant Physician' },
    { Id: 384, Designation: 'Consultant Psychiatrist / Neuro Psychiatrist' },
    { Id: 385, Designation: 'Consultant Radiologist' },
    { Id: 387, Designation: 'Consultant Surgeon' },
    { Id: 390, Designation: 'Consultant Urologist' },
    { Id: 1594, Designation: 'Consultant Nephrologist' },
    { Id: 1598, Designation: 'Consultant Neurologist' },
    { Id: 2136, Designation: 'Consultant TB/Chest Specialist' },
    { Id: 2313, Designation: 'Consultant Pharmacist' },
    { Id: 21, Designation: 'Additional Principal Medical Officer' },
    { Id: 22, Designation: 'Additional Principal Women Medical Officer' },
    { Id: 802, Designation: 'Medical Officer' },
    { Id: 1085, Designation: 'Senior Medical Officer' },
    { Id: 932, Designation: 'Principal Medical Officer' },
    { Id: 936, Designation: 'Principal Women Medical Officer' },
    { Id: 1157, Designation: 'Senior Women Medical Officer' },
    { Id: 1320, Designation: 'Women Medical Officer' },
    { Id: 2255, Designation: 'Medical Officer / Women Medical Officer (Hard Areas)' },
    { Id: 2404, Designation: 'Medical Officer / Women Medical Officer' },
    { Id: 302, Designation: 'Charge Nurse' },
    { Id: 431, Designation: 'Dental Surgeon' }
  ];
  public totalApp: any;
  public DesignationName: string = '';
  public dateNow: string = '';

  public max: any = 12345;

  constructor(private _localService: LocalService, private _adhocService: AdhocService) { }
  public getUser() {
    return this._localService.get('ussr');
  }

  ngOnInit() {
    let today = new Date();
    let dd: any = today.getDate(), mm: any = today.getMonth() + 1, yyyy = today.getFullYear();
    if (dd < 10) dd = '0' + dd; if (mm < 10) mm = '0' + mm;
    this.dateNow = dd + '/' + mm + '/' + yyyy;
    this.getAdhocApplicants();
    this.getAdhocApplicantsSummary();
    this.getAdhocCounts();
    this.getAdhocPendencySummary();
  }
  public getAdhocApplicantsSummary() {
    this._adhocService.getAdhocApplicantsSummary().subscribe((response: any) => {
      this.applicantsSummary = response;
      console.log('summary: ', this.applicantsSummary);
      this.kGrid.loading = false;
    }, err => {
      console.log(err);
    });
  }
  public getAdhocCounts() {
    this._adhocService.getAdhocDashboardCounts().subscribe((response: any) => {
      //this.applicationsDesig = response;
      let oldreport = response.res;
      let grvReport = response.grv;
      this.totalSum = {
        New: 0,
        Approved: 0,
        Rejected: 0,
        Submitted: 0,
        TotalGrievance: 0,
        ApprovedGrievance: 0,
        RejectedGrievance: 0
      };
      let totalsums = 0;
      let s: string = ``;
      grvReport.forEach(grv => {
        s += `{ Designation:'${grv.Designation}', Apps:${grv.Rejected} },`;
      });
      console.log(s);
      
      oldreport.forEach(element => {
        this.applicationsDesignation = {};
        this.applicationsDesignation.Id = element.Id;
        this.applicationsDesignation.Approved = element.Approved;
        this.applicationsDesignation.Designation = element.Designation;
        this.applicationsDesignation.New = element.New;
        this.applicationsDesignation.OrderBy = element.OrderBy;
        this.applicationsDesignation.Rejected = element.Rejected;
        this.applicationsDesignation.Submitted = element.Submitted;
        this.applicationsDesignation.Grievance = grvReport.find(x => x.Designation == element.Designation);

        this.totalSum.New += this.applicationsDesignation.New;
        this.totalSum.Approved += this.applicationsDesignation.Approved;
        this.totalSum.Rejected += this.applicationsDesignation.Rejected;
        this.totalSum.Submitted += this.applicationsDesignation.Submitted;
        this.totalSum.TotalGrievance += this.applicationsDesignation.Grievance.Total_Greivance;
        this.totalSum.ApprovedGrievance += this.applicationsDesignation.Grievance.Approved;
        this.totalSum.RejectedGrievance += this.applicationsDesignation.Grievance.Rejected;

        this.applicationsDesignation.TotalApplications = (element.New == null ? 0 : element.New) + (element.Approved == null ? 0 : element.Approved) + (element.Submitted == null ? 0 : element.Submitted);
        this.applicationsDesig.push(this.applicationsDesignation);
      });
      this.getAggregations();
      //this.getAggregationsTotalApplication();
      this.kGrid.loading = false;
    }, err => {
      console.log(err);
    });
  }
  public openWithDesignation(deisgnation: string) {
    console.log(deisgnation);
    var d = this.designations.find(x => x.Designation == deisgnation);
    console.log(d);
    if (d) {
      this.openInNewTab('adhoc-applications/job-detail/' + d.Id);
    }
  }
  public getAdhocPendencySummary() {
    this._adhocService.getAdhocPendencySummary().subscribe((response: any) => {
      this.adhocPendencySummary = response;
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
    item.loading = true;
    this.documents = [];
    let obj = {
      Skip: this.kGrid.skip,
      PageSize: 10000000,
      applicantId: 0,
      hfmisCode: this.user.HfmisCode,
      Query: null,
      DesignationName: item.Designation
    };
    this._adhocService.getAdhocApplicationScrutiny(obj).subscribe((res: any) => {
      if (res) {
        this.documents = res.List;
        setTimeout(() => {
          this.printApplication();
          item.loading = false;
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
  /*  public getMinutes(item: any) {
     
     this.getAdhocApplication();
   } */
  public getAggregations() {
    this.aggregates = [
      { field: 'Submitted', aggregate: 'sum' },
      { field: 'Approved', aggregate: 'sum' },
      { field: 'Rejected', aggregate: 'sum' },
      { field: 'New', aggregate: 'sum' },
      { field: 'TotalApplications', aggregate: 'sum' }]
    this.total = aggregateBy(this.applicationsDesig, this.aggregates);
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
        }
        
        table.doc {
            font-family: sans-serif, Arial, Verdana, "Trebuchet MS";
            border-collapse: collapse;
            width: 100%;
        }
        
        table.doc td {
            border: 1px solid #000000;
            text-align: center;
            padding: 8px;
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
      bottom: 0;    left: 27%;color:#e3e3e3;">Powered by Health Information and
        Service Delivery Unit</div>
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
