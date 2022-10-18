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
import { DatePipe } from "@angular/common";
//import { ActivatedRoute } from '@angular/router';
//import { LocalService } from '../../../_services/local.service';

@Component({
  selector: "app-adhoc-applicant-scrutiny",
  templateUrl: "./adhoc-applicant-scrutiny.component.html",
  styleUrls: ['./adhoc-applicant-scrutiny.component.scss']
})
export class AdhocApplicantScrutinyComponent implements OnInit, OnDestroy {
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
  public showMarksFlag: boolean = false;
  public decision: string = '';
  public decisionObj: any = {};
  public portalClosed: boolean = false;
  public closureDate: any = new Date('12-11-2021');
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
    private ngZone: NgZone,
    private datePipe: DatePipe
  ) { }

  public ngOnInit(): void {
    this.checkPortalClosure();
    this.user = this._authenticationService.getUser();
    this.isAdmin = this.user.UserName == 'dpd' ? true : false;
    this.fetchParams();
    this.getAdhocScrutinyReasons();
  }

  public checkPortalClosure() {
    this.portalClosed = false;
    /*  let currentDate: any = new Date();
     currentDate = this.datePipe.transform(currentDate, 'dd-MM-yyyy');
     this.closureDate = this.datePipe.transform(this.closureDate, 'dd-MM-yyyy');
     if (this.closureDate === currentDate) {
       this.portalClosed = false;
     } */
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
  public showMarks(dataItem: any) {
    let degreeId: number = dataItem.Required_Degree_Id;
    this.showMarksFlag = !(degreeId == Degree.FCPSPartI || degreeId == Degree.FCPSPartII || degreeId == Degree.MCPSPartI || degreeId == Degree.MCPSPartII
      || degreeId == Degree.DiplomainAnesthesia || degreeId == Degree.DiplomainCardiology || degreeId == Degree.DiplomainDermatology || degreeId == Degree.DLO || degreeId == Degree.DGO
      || degreeId == Degree.DGO || degreeId == Degree.DOMS || degreeId == Degree.DOMS || degreeId == Degree.DCH
      || degreeId == Degree.DCH || degreeId == Degree.DCP || degreeId == Degree.MPhill || degreeId == Degree.DCP
      || degreeId == Degree.MPhill || degreeId == Degree.DMRD || degreeId == Degree.DMRD || degreeId == Degree.MSUrology
      || degreeId == Degree.MSUrology || degreeId == Degree.DiplomainNephrology || degreeId == Degree.DiplomainNephrology
      || degreeId == Degree.DTCD || degreeId == Degree.DTCD || degreeId == Degree.DPM || degreeId == Degree.DPM);
    return this.showMarksFlag;
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
            let temp: any[] = [];
            this.applicant.AdhocScrutinies.forEach(scrutiny => {
              if (scrutiny.IsRejected) {
                temp.push(scrutiny);
              }
            });
            this.applicant.AdhocScrutiniesRejected = temp;
            this.checkScrutiny();
          }
          this.scrutinyDone = true;
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
    if (this.scrutinyComplete && this.application.Status_Id != 2 && this.application.Status_Id != 3) {
      /*   if (this.scrutinyRejected) {
          this.changeFinalStatus('RejectedFinal');
        } else {
          this.changeFinalStatus('AcceptedFinal');
        } */
    }
  }
  public verifyPMC() {
    if (this.applicant.PMDCNumber && !this.applicant.PMDCNumber.trim().toLowerCase().endsWith('-p')) {
      if (!this.applicant.PMDCNumber.toLowerCase().endsWith('-d')) {
        this.applicant.PMDCNumber = this.applicant.PMDCNumber + '-P';
      }
    }
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
  public undoAdhocScrutiny(item) {
    if (!item.scrutiny) return;
    item.scrutiny.saving = true;
    this._adhocService.undoAdhocScrutiny(item.scrutiny.Id).subscribe((res: any) => {
      item.scrutiny.saving = !res;
      if (res) {
        this.closeScrutinyDialog();
        this.getAdhocApplicationApplicant();
      }
    }, err => {
      console.log(err);
    });
  }
  public changeApplicationStatus(statusId: number) {
    this.changingStatus = true;
    this.applicationLog.StatusId = statusId;
    this.applicationLog.Application_Id = this.application.Id;
    this._adhocService.changeApplicationStatus(this.applicationLog).subscribe((res) => {
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
  public saveAdhocScrutiny(accepted: boolean) {
    this.changingStatus = true;
    this.adhocScrutiny.Application_Id = this.application.Id;
    if (this.decisionObj.Name) {
      this.adhocScrutiny.DocName = this.decisionObj.Name;
    }
    else if (this.decisionObj.DegreeName) {
      this.adhocScrutiny.DocName = this.decisionObj.DegreeName;
      this.adhocScrutiny.Qualification_Id = this.decisionObj.Id;
      this.adhocScrutiny.UploadPath = this.decisionObj.UploadPath;
    }
    else if (this.decisionObj.JobTitle) {
      this.adhocScrutiny.DocName = this.decisionObj.JobTitle + ', ' + this.decisionObj.Organization
      this.adhocScrutiny.Experience_Id = this.decisionObj.Id;
      this.adhocScrutiny.UploadPath = this.decisionObj.UploadPath;
    }
    this.adhocScrutiny.IsAccepted = accepted;
    this.adhocScrutiny.IsRejected = this.adhocScrutiny.IsAccepted ? false : true;
    this._adhocService.saveAdhocScrutiny(this.adhocScrutiny).subscribe((res) => {
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
  public changeStatus(obj: any, status: string) {
    this.decisionObj = obj;
    this.decision = status;
    if (this.decisionObj.Name) {
      if (this.decisionObj.Name == 'Domicile') {
        this.scrutinyReasons = this.scrutinyReasonsOrigional.filter(x => x.Id == 3 || x.Id == 9);
        this.adhocScrutiny.UploadPath = this.applicant.DomicileDoc;
      }
      else if (this.decisionObj.Name == 'PMC' || this.decisionObj.Name == 'PNC') {
        this.scrutinyReasons = this.scrutinyReasonsOrigional.filter(x => x.Id == 8 || x.Id == 10 || x.Id == 9);
        this.adhocScrutiny.UploadPath = this.applicant.PMDCDoc;
      }
      else if (this.decisionObj.Name == 'Hafiz-e-Quran') {
        this.scrutinyReasons = this.scrutinyReasonsOrigional.filter(x => x.Id == 11 || x.Id == 9);
        this.adhocScrutiny.UploadPath = this.applicant.HifzDocument;
      }
    }
    else if (this.decisionObj.DegreeName) {
      this.scrutinyReasons = this.scrutinyReasonsOrigional.filter(x => x.Id == 1 || x.Id == 2 || x.Id == 9);
    }
    else if (this.decisionObj.JobTitle) {
      this.scrutinyReasons = this.scrutinyReasonsOrigional.filter(x => x.Id == 2 || x.Id == 4 || x.Id == 5 || x.Id == 6 || x.Id == 7 || x.Id == 9);
    }
    this.openScrutinyDialog();
  }
  public undoStatus(obj: any) {
    this.decisionObj = obj;
    this.decision = 'Undo';
    this.openScrutinyDialog();
  }
  public changeFinalStatus(status: string) {
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

enum Degree {
  Matriculation = 58,
  FirstProfessionalMBBSI = 62,
  FSCPreMedical = 65,
  OLevel = 93,
  ALevel = 94,
  FirstProfessionalBDS = 113,
  FirstProfessionalMBBSII = 116,
  SecondProfessionalMBBS = 117,
  ThirdProfessionalMBBS = 118,
  FinalProfessionalMBBS = 119,
  FCPSPartI = 122,
  FCPSPartII = 123,
  MCPSPartI = 124,
  MSMDPartI = 125,
  MSMDPartII = 126,
  MCPSPartII = 127,
  MastersinPublicHealth = 128,
  BSNursing = 63,
  GeneralNursing1stYear = 129,
  DiplomainAnesthesia = 130,
  GeneralNursing2ndYear = 131,
  GeneralNursing3rdYear = 132,
  DiplomainGeneralNursingandMidwifery = 133,
  SecondProfessionalBDS = 134,
  ThirdProfessionalBDS = 135,
  FinalProfessionalBDS = 136,
  DMRD = 137,
  DiplomainCardiology = 138,
  DiplomainDermatology = 139,
  DLO = 140,
  DOMS = 141,
  DCH = 142,
  DCP = 143,
  MSUrology = 144,
  DiplomainNephrology = 145,
  DTCD = 146,
  DGO = 147,
  MPhill = 120,
  DPM = 148
}