import { Observable } from "rxjs/Observable";
import {
  Component,
  OnInit,
  Inject,
  ViewChild,
  NgZone,
  OnDestroy
} from "@angular/core";
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

import { KGridHelper } from "../../../_helpers/k-grid.class";
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
import { AdhocService } from "../adhoc.service";
import { ActivatedRoute } from "@angular/router";
//import { ActivatedRoute } from '@angular/router';
//import { LocalService } from '../../../_services/local.service';

@Component({
  selector: "app-adhoc-applicant-grievance",
  templateUrl: "./adhoc-applicant-grievance.component.html",
  styleUrls: ['./adhoc-applicant-grievance.component.scss']
})
export class AdhocApplicantGrievanceComponent implements OnInit, OnDestroy {
  // @ViewChild("grid") public grid: GridComponent;
  @ViewChild('photoRef', { static: false }) public photoRef: any;
  public kGrid: KGridHelper = new KGridHelper();
  public inputChange: Subject<any>;
  public user: any = {};
  public reason: any = {};
  public applicant: any = {};
  public pmcVerification: any;
  public pmcQualification: any;
  public documents: any[] = [];
  public application: any = {};
  public applicationLog: any = {};
  public adhocScrutiny: any = {};
  public subscription = null;
  public scrutinyDialog: boolean = false;
  public scrutinyDone: boolean = false;
  public scrutinyRejected: boolean = false;
  public scrutinyComplete: boolean = false;
  public isAdmin: boolean = false;
  public changingStatus: boolean = false;
  public acceptingGrievance: boolean = false;
  public rejectingGrievance: boolean = false;
  public decision: string = '';
  public decisionObj: any = {};
  public grievanceKeywords: string[] = [
    'mistake', 'pmc', 'expire', 'valid', 'my mistake'
  ];
  public rejectReasons: any[] = [
    { Id: 1, Reason: 'PMDC document not valid' },
    { Id: 2, Reason: 'Hafiz-e-Quran certificate not valid' },
    { Id: 3, Reason: 'Qualification incomplete' },
    { Id: 5, Reason: 'Domicile not from Punjab' }

  ];
  public scrutinyReasonsOrigional: any[] = [];
  public scrutinyReasons: any[] = [];
  public urdu: any = {
    info: `درخواست گزار کے کوائف کی جانچ پرٹال کیلئے مندرجہ ذیل ٹیبز کو کھولئے اور تمام دی گئی معلومات /اسناد کی جانچ کریں۔`,
    info2: `معلومات /اسناد کو مسترد کرنے کی صورت میں درخواست گزار کو بذریعہ ایس ایم ایس ، ای میل اور پورٹل سے مطلع کیا جائے گا۔ لہذا مسترد کرنے کی وجوہات کو صحیح اور مناسب طریقے سے منتخب کریں۔`,
    info3: `ہا ؤس جاب یا نان میڈیکل کے تجبرہ کو مسترد کیا جائے۔`,
    infoeng: 'House job will not be considered as an experience to apply for adhoc',
    infoeng1: 'Non-medical experience will not be considered for an adhoc application',
    infoeng2: 'You only have to verify the experience certificate, the system will automatically calculate the post qualification tenure of experience'
  };
  constructor(
    private _adhocService: AdhocService,
    public _notificationService: NotificationService,
    public _rootService: RootService,
    private route: ActivatedRoute,
    private _authenticationService: AuthenticationService,
    private ngZone: NgZone
  ) { }

  public ngOnInit(): void {
    this.user = this._authenticationService.getUser();
    this.isAdmin = this.user.UserName == 'dpd' ? true : false;
    this.fetchParams();
    this.getAdhocScrutinyReasons();
  }

  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('id')) {
          this.applicant.Id = +params['id'];
        } else {
          this.applicant = {};
        }
        if (params.hasOwnProperty('appId')) {
          this.application.Id = +params['appId'];
        } else {
          this.application = {};
        }
        this.getAdhocApplicationApplicant();
      }
    );
  }
  public getAdhocApplicationApplicant() {
    this.documents = [];
    this._adhocService.getAdhocApplicationApplicant(this.applicant.Id, this.application.Id).subscribe((res: any) => {
      if (res) {
        this.applicant = res.applicant;
        this.application = res.application;
        this.getApplicationData();

      }
    }, err => {
      console.log(err);
    });
  }
  public adhocApplicantContinueExp(item: any) {
    this._adhocService.adhocApplicantContinueExp(item.Id).subscribe((res: any) => {
      if (res) {
        item = res;
        this.closeScrutinyDialog();
        this.getAdhocApplicationApplicant();
      }
    }, err => {
      console.log(err);
    });
  }
  public getApplicationData() {
    let doc: any = { Name: 'CNIC', UploadPath: this.applicant.CNICDoc };
    this.documents.push(doc);
    doc = { Name: 'Domicile', SubName: this.applicant.DomicileName, UploadPath: this.applicant.DomicileDoc };
    this.documents.push(doc);
    if (this.applicant.Hafiz) {
      doc = { Name: 'Hafiz-e-Quran', UploadPath: this.applicant.HifzDocument };
      this.documents.push(doc);
    }
    doc = {
      Name: this.application.Desgination_Id == 302 ? 'PNC' : 'PMC', Valid: this.applicant.PMDCValidUpto, PMDCNumber: this.applicant.PMDCNumber,
      UploadPath: this.applicant.PMDCDoc
    };
    this.documents.push(doc);
    this._adhocService.getApplicantDocuments(this.applicant.Id).subscribe((x: any) => {
      if (x) {
        this.applicant.Qualifications = x;
      }
      this._adhocService.getExperiences(this.applicant.Id).subscribe((x1: any) => {
        if (x1) {
          this.applicant.ApplicantExperiences = x1;
          this.applicant.ApplicantExperiences.forEach(exp => {
            exp.exp = this.diff_years(exp.FromDate, exp.ToDate);
          });
        }
        this._adhocService.getAdhocScrutiny(this.application.Id).subscribe((x2: any) => {
          if (x2) {
            this.applicant.AdhocScrutinies = x2;

            this.checkScrutiny();
          }
          this.scrutinyDone = true;
          this._adhocService.getAdhocGrievance(this.application.Id).subscribe((res2: any) => {
            if (res2) {
              this.application.Grievance = res2;
            }
          }, err => {
            console.log(err);
          });
        }, err => {
          console.log(err);
        });

      }, err => {
        console.log(err);
      });
    }, err => {
      console.log(err);
    });
    this._adhocService.getApplicationPref(this.applicant.Id).subscribe((x: any) => {
      if (x) {
        this.applicant.Preferences = x;
      }
    }, err => {
      console.log(err);
    });
  }
  public checkScrutiny() {
    this.documents.forEach(doc => {
      doc.scrutiny = this.applicant.AdhocScrutinies.find(x => x.DocName == doc.Name);
    });
    this.applicant.Qualifications.forEach(qualification => {
      qualification.scrutiny = this.applicant.AdhocScrutinies.find(x => x.Qualification_Id == qualification.Id);
    });
    this.applicant.ApplicantExperiences.forEach(experience => {
      experience.scrutiny = this.applicant.AdhocScrutinies.find(x => x.Experience_Id == experience.Id);
    });
    this.scrutinyDone = true;
    this.scrutinyComplete = this.applicant.AdhocScrutinies.length >= (2 + this.applicant.Qualifications.length + this.applicant.ApplicantExperiences.length);
    let scrutinyRejectedObj = this.applicant.AdhocScrutinies.find(x => x.IsRejected == true && x.Experience_Id == null);
    if (scrutinyRejectedObj) {
      this.scrutinyRejected = true;
    } else {
      this.scrutinyRejected = false;
    }
    let temp: any[] = [];
    this.applicant.AdhocScrutinies.forEach(scrutiny => {
      if (scrutiny.IsRejected && !scrutiny.GrievanceAccepted) {
        scrutiny.detailData = this.documents.find(x => x.Name == scrutiny.DocName);
        if (!scrutiny.detailData) {
          scrutiny.detailData = this.applicant.Qualifications.find(x => x.Id == scrutiny.Qualification_Id);
        }
        temp.push(scrutiny);
      }
    });
    this.applicant.AdhocScrutiniesRejected = temp;
  }
  public checkGrievanceKeywords() {

  }
  public verifyPMC() {
    if (this.applicant.PMDCNumber && !this.applicant.PMDCNumber.trim().toLowerCase().endsWith('-p')) {
      if (!this.applicant.PMDCNumber.toLowerCase().endsWith('-d')) {
        this.applicant.PMDCNumber = this.applicant.PMDCNumber + '-P';
      }
    }
    this.decision = null;
    this._adhocService.verifyPMC(this.applicant.PMDCNumber).subscribe((res) => {
      this.pmcVerification = res;
      this.openScrutinyDialog();
    }, err => {
      console.log(err);
    });
    this._adhocService.verifyPMCGetQualifications(this.applicant.PMDCNumber).subscribe((res) => {
      this.pmcQualification = res;
    }, err => {
      console.log(err);
    });
  }
  public getAdhocScrutinyReasons() {
    this._adhocService.getAdhocScrutinyReasons().subscribe((res: any) => {
      this.scrutinyReasonsOrigional = res;
    }, err => {
      console.log(err);
    });
  }
  public changeApplicationGrievanceStatus(statusId: number) {
    if (!this.application.Grievance) return;
    if (statusId == 2) {
      this.acceptingGrievance = true;
    } else if (statusId == 3) {
      this.rejectingGrievance = true;
    }
    this.applicationLog.Id = this.application.Grievance.Id;
    this.applicationLog.StatusId = statusId;
    this.applicationLog.Application_Id = this.application.Id;
    this._adhocService.changeApplicationGrievanceStatus(this.applicationLog).subscribe((res) => {
      if (res) {
        this.closeScrutinyDialog();
        this.getAdhocApplicationApplicant();
      }
      if (statusId == 2) {
        this.acceptingGrievance = false;
      } else if (statusId == 3) {
        this.rejectingGrievance = false;
      }
    }, err => {
      console.log(err);
      if (statusId == 2) {
        this.acceptingGrievance = false;
      } else if (statusId == 3) {
        this.rejectingGrievance = false;
      }
    });

  }
  public saveAdhocScrutiny(scrutiny: any) {
    this.changingStatus = true;
    this._adhocService.editAdhocScrutiny(scrutiny).subscribe((res) => {
      if (res) {
        this.closeScrutinyDialog();
        this.getAdhocApplicationApplicant();
      }
      this.changingStatus = false;
    }, err => {
      console.log(err);
      this.changingStatus = false;
    });

  }
  public acceptAdhocGrievanceScrutiny() {
    this.changingStatus = true;
    this.adhocScrutiny.loading = true;
    this.adhocScrutiny.GrievanceAccepted = true;
    let temp = this.adhocScrutiny.detailData;
    this.adhocScrutiny.detailData = {};
    this._adhocService.acceptAdhocGrievanceScrutiny(this.adhocScrutiny).subscribe((res) => {
      if (res) {
        this.adhocScrutiny.detailData = temp;
        this.closeScrutinyDialog();
        this.getAdhocApplicationApplicant();
      }
      this.adhocScrutiny.loading = false;
      this.changingStatus = false;
    }, err => {
      console.log(err);
      this.changingStatus = false;
    });

  }
  public changeStatus(obj: any, status: string) {
    this.adhocScrutiny = obj;
    this.adhocScrutiny.Remarks = '';
    this.decision = status;
    this.openScrutinyDialog();
  }
  public dropValueChanged(filter: string, value: any) {
    if (filter == 'reason') {
      this.adhocScrutiny.Reason_Id = value.Id;
    }
    if (filter == 'reasonSpecific') {
      if (this.applicationLog.ReasonId == 3) {
        this.applicationLog.QualificationId = value.Id;
      }
      if (this.applicationLog.ReasonId == 4) {
        this.applicationLog.ExperienceId = value.Id;
      }
    }
  }
  public openScrutinyDialog() {
    this.scrutinyDialog = true;
  }
  public closeScrutinyDialog() {
    this.decisionObj = {};
    this.application.Agree = false;
    this.application.Agree2 = false;
    this.decision = '';
    this.adhocScrutiny = {};
    this.applicationLog = {};
    this.scrutinyDialog = false;
  }
  diff_years(dt2, dt1) {
    dt1 = new Date(dt1);
    dt2 = new Date(dt2);
    var diff = (dt2.getTime() - dt1.getTime()) / 1000;
    diff /= (60 * 60 * 24);
    return Math.abs(Math.round(diff / 365.25));
  }
  public dashifyCNIC(cnic: string) {
    if (cnic) {
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
  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
  public openInNewTab(link: string) {
    window.open(link, '_blank');
  }
  printApplication() {
    let html = document.getElementById('formPrint').innerHTML;
    var mywindow = window.open('', 'PRINT', 'height=600,width=900');
    if (mywindow) {
      mywindow.document.write(`<html><head>
     
    <style>
   
    body {
      padding: 30px 50px 0 50px;
      font-family: -apple-system,system-ui;
  }
  table.file-table, table.file-table th, table.file-table td {
    border: 1px solid black;
    border-collapse: collapse;
  }
  table.file-table td {
    padding: 0.5rem;
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
    <title>Application</title>`);
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
      /* this.dropDowns.selectedFiltersModel.inboxOfficers = { Name: 'Select Office', Id: 0 };
      this.dropdownValueChanged(this.dropDowns.selectedFiltersModel.inboxOfficers, 'office'); */
      //this.getFileMovements();
      /*     mywindow.document.close(); // necessary for IE >= 10
          mywindow.focus(); // necessary for IE >= 10
          mywindow.print();
          mywindow.close(); */
    }
  }
}
