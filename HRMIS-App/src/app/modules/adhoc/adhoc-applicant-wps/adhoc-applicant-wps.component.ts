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
  selector: "app-adhoc-applicant-wps",
  templateUrl: "./adhoc-applicant-wps.component.html",
  styleUrls: ['./adhoc-applicant-wps.component.scss']
})
export class AdhocApplicantWPSComponent implements OnInit, OnDestroy {
  // @ViewChild("grid") public grid: GridComponent;
  @ViewChild('photoRef', { static: false }) public photoRef: any;
  public kGrid: KGridHelper = new KGridHelper();
  public inputChange: Subject<any>;
  public user: any = {};
  public reason: any = {};
  public applicant: any = {};
  public interviewBatch: any = {};
  public pmcVerification: any;
  public pmcQualification: any;
  public documents: any[] = [];
  public interviewBatches: any[] = [];
  public meritMarks: any[] = [];
  public application: any = {};
  public selectedValue: any = {};
  public applicationLog: any = {};
  public adhocScrutiny: any = {};
  public adhocInterview: any = {};
  public subscription = null;
  public scrutinyDialog: boolean = false;
  public editDialog: boolean = false;
  public scrutinyDone: boolean = false;
  public scrutinyRejected: boolean = false;
  public scrutinyComplete: boolean = false;
  public isAdmin: boolean = false;
  public changingStatus: boolean = false;
  public decision: string = '';
  public districtName: string = '';
  public decisionObj: any = {};
  public count: number = 0;
  public viewAs: number = 2;
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
    // this.getAdhocScrutinyReasons();
    this._authenticationService.userLevelNameEmitter.subscribe((x: any) => {
      this.districtName = x;
      this.districtName = this.districtName ? this.districtName.replace(' District', '') : 'District';
    });
  }

  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('desigId')) {
          this.adhocInterview.Designation_Id = +params['desigId'];

          if (params.hasOwnProperty('batchId')) {
            this.interviewBatch.Id = +params['batchId'];
            this.getAdhocInterviewBatches();
          }
        }
      }
    );
  }
  public getAdhocInterviewBatches() {
    this._adhocService.getAdhocInterview(this.user.HfmisCode, this.adhocInterview.Designation_Id).subscribe((res: any) => {
      if (res && res.Id > 0) {
        this.adhocInterview = res;
        console.log(this.adhocInterview.Id);
        this._adhocService.getAdhocInterviewBatches(this.adhocInterview.Id).subscribe((batches: any[]) => {
          if (batches && batches.length > 0) {
            this.interviewBatches = batches;
            console.log(this.interviewBatches);
            let batch = this.interviewBatches.find(x => x.Id == this.interviewBatch.Id);
            console.log(batch);
            this.selectBatch(batch);
          }
        }, err => {
          console.log(err);
        });
      }
    }, err => {
      console.log(err);
    });
    this.interviewBatches = [];
  }
  public selectBatch(item) {
    this.interviewBatch = item;
    this.viewAs = 2;
    if (item) {
      this._adhocService.getAdhocInterviewBatchCommittee(this.interviewBatch.Id).subscribe((res: any[]) => {
        if (res && res.length > 0) {
          this.interviewBatch.AdhocInterviewBatchCommittees = res;
        }
      }, err => {
        console.log(err);
      });
      this.count = 0;
      this._adhocService.getAdhocInterviewBatchApplications(this.interviewBatch.Id).subscribe((applications: any[]) => {
        if (applications && applications.length > 0) {
          this.interviewBatch.Applications = applications;
          this.count = 0;
          for (let app of this.interviewBatch.Applications) {
            this._adhocService.getAdhocApplicationApplicant(app.Applicant_Id, app.Application_Id).subscribe((a: any) => {
              if (a) {
                app.applicant = a.applicant;
                app.application = a.application;
                app.list = {};
                app.list.Id = app.application.Id;
                app.list.Name = app.applicant.Name;
                app.list.Age = app.applicant.Age;
                app.list.FatherName = app.applicant.FatherName;
                app.list.CNIC = this.dashifyCNIC(app.applicant.Name);
                app.list.Domicile = app.applicant.DomicileName;
                app.list.PMDCValidUpto = app.applicant.PMDCValidUpto;
                app.list.Hafiz = app.applicant.Hafiz ? 'Yes' : '-';
                app.documents = [];
                let doc: any = { Name: 'CNIC', UploadPath: this.applicant.CNICDoc };
                app.documents.push(doc);
                doc = { Name: 'Domicile', SubName: app.applicant.DomicileName, UploadPath: app.applicant.DomicileDoc };
                app.documents.push(doc);
                if (app.applicant.Hafiz) {
                  doc = { Name: 'Hafiz-e-Quran', UploadPath: app.applicant.HifzDocument };
                  app.documents.push(doc);
                }
                doc = {
                  Name: app.application.Desgination_Id == 302 ? 'PNC' : 'PMC', Valid: app.applicant.PMDCValidUpto, PMDCNumber: app.applicant.PMDCNumber,
                  UploadPath: app.applicant.PMDCDoc
                };
                app.documents.push(doc);
                this._adhocService.getApplicantDocuments(app.applicant.Id).subscribe((x: any) => {
                  if (x) {
                    app.applicant.Qualifications = x;
                    /*   app.applicant.Qualifications.forEach(q => {
                     
                      }); */
                  }
                  this._adhocService.getExperiences(app.applicant.Id).subscribe((x1: any) => {
                    if (x1) {
                      app.applicant.ApplicantExperiences = x1;
                      app.applicant.ApplicantExperiences.forEach(exp => {
                      });
                    }
                    this._adhocService.getAdhocScrutiny(app.Application_Id).subscribe((x2: any) => {
                      if (x2) {
                        app.applicant.AdhocScrutinies = x2;
                        let temp: any[] = [];
                        app.applicant.AdhocScrutinies.forEach(scrutiny => {
                          if (scrutiny.IsRejected) {
                            temp.push(scrutiny);
                          }
                        });
                        app.applicant.AdhocScrutiniesRejected = temp;
                        app.documents.forEach(doc => {
                          doc.scrutiny = app.applicant.AdhocScrutinies.find(x => x.DocName == doc.Name);
                        });
                        app.applicant.Qualifications.forEach(qualification => {
                          qualification.scrutiny = app.applicant.AdhocScrutinies.find(x => x.Qualification_Id == qualification.Id);
                          if (qualification.scrutiny && qualification.scrutiny.IsAccepted)
                            app.list.higherone = '';
                          if (qualification.Required_Degree_Id == Degree.Matriculation || qualification.Required_Degree_Id == Degree.OLevel) {
                            app.list.matric = qualification.ObtainedMarks + '/' + qualification.TotalMarks;
                          } else if (qualification.Required_Degree_Id == Degree.FSCPreMedical || qualification.Required_Degree_Id == Degree.ALevel || qualification.Required_Degree_Id == Degree.FA) {
                            app.list.inter = qualification.ObtainedMarks + '/' + qualification.TotalMarks;
                          } else if (qualification.Required_Degree_Id == Degree.FirstProfessionalMBBSI || qualification.Required_Degree_Id == Degree.FirstProfessionalBDS || qualification.Required_Degree_Id == Degree.GeneralNursing1stYear) {
                            app.list.mbbs1 = qualification.ObtainedMarks + '/' + qualification.TotalMarks;
                            app.list.mbbsi = qualification.Institute ? qualification.Institute : '';
                          } else if (qualification.Required_Degree_Id == Degree.FirstProfessionalMBBSII || qualification.Required_Degree_Id == Degree.SecondProfessionalBDS || qualification.Required_Degree_Id == Degree.GeneralNursing2ndYear) {
                            app.list.mbbs2 = qualification.ObtainedMarks + '/' + qualification.TotalMarks;
                            app.list.mbbsi = qualification.Institute ? qualification.Institute : '';
                          } else if (qualification.Required_Degree_Id == Degree.SecondProfessionalMBBS || qualification.Required_Degree_Id == Degree.SecondProfessionalBDS || qualification.Required_Degree_Id == Degree.GeneralNursing3rdYear) {
                            app.list.mbbs3 = qualification.ObtainedMarks + '/' + qualification.TotalMarks;
                            app.list.mbbsi = qualification.Institute ? qualification.Institute : '';
                          } else if (qualification.Required_Degree_Id == Degree.ThirdProfessionalMBBS || qualification.Required_Degree_Id == Degree.FinalProfessionalBDS || qualification.Required_Degree_Id == Degree.DiplomainGeneralNursingandMidwifery) {
                            app.list.mbbs4 = qualification.ObtainedMarks + '/' + qualification.TotalMarks;
                            app.list.mbbsi = qualification.Institute ? qualification.Institute : '';
                          } else if (qualification.Required_Degree_Id == Degree.FinalProfessionalMBBS) {
                            app.list.mbbs5 = qualification.ObtainedMarks + '/' + qualification.TotalMarks;
                            app.list.mbbsi = qualification.Institute ? qualification.Institute : '';
                          } else if (qualification.Required_Degree_Id == Degree.FCPSPartI) {
                            app.list.higherone += qualification.DegreeName;
                          } else if (qualification.Required_Degree_Id == Degree.FCPSPartII) {
                            app.list.higherone += ', ' + qualification.DegreeName;
                          } else if (qualification.Required_Degree_Id == Degree.MCPSPartI) {
                            app.list.higherone += qualification.DegreeName;
                          } else if (qualification.Required_Degree_Id == Degree.MCPSPartII) {
                            app.list.higherone += ', ' + qualification.DegreeName;
                          }
                        });
                        let tempexp = [];

                        app.applicant.ApplicantExperiences.forEach(experience => {
                          app.list.exp = 0;
                          experience.scrutiny = app.applicant.AdhocScrutinies.find(x => x.Experience_Id == experience.Id);
                          if (experience.scrutiny) {
                            if (!experience.scrutiny.IsRejected) {
                              app.list.exp += this.diff_years(experience.FromDate, experience.ToDate);
                              tempexp.push(experience);
                            }
                          }
                        });
                        app.applicant.ApplicantExperiences = tempexp;
                      }
                      app.scrutinyDone = true;
                      this.count++;
                      if (this.count == this.interviewBatch.Applications.length) {
                        this.interviewBatch.loaded = true;
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
              }
            }, err => {
              console.log(err);
            });
          }
        }
      }, err => {
        console.log(err);
      });
    }
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
  public getMeritMarks() {
    this.meritMarks = [];
    this._adhocService.getMeritMarks().subscribe((res: any) => {
      if (res) {
        this.meritMarks = res;
        this._adhocService.getAdhocApplicationMarks(this.application.Id).subscribe((marks: any) => {
          if (marks) {
            this.application.marks = marks;
            this.meritMarks.forEach(meritMark => {
              meritMark.applicationMarks = this.application.marks.find(x => x.Marks_Id == meritMark.Id);
            });
            this.application.matricMerit = this.application.marks.find(x => x.Marks_Id == 1);
            this.application.interMerit = this.application.marks.find(x => x.Marks_Id == 2);
            this.application.masterMerit = this.application.marks.find(x => x.Marks_Id == 5);
            this.application.firstStageMerit = this.application.marks.find(x => x.Marks_Id == 6);
            this.application.secondStageMerit = this.application.marks.find(x => x.Marks_Id == 7);
            this.application.thirdStageMerit = this.application.marks.find(x => x.Marks_Id == 8);
            this.application.hifzMerit = this.application.marks.find(x => x.Marks_Id == 9);
            this.application.expMerit = this.application.marks.find(x => x.Marks_Id == 10);
            this.application.interviewMerit = this.application.marks.find(x => x.Marks_Id == 10);
            this.application.firstPositionMerit = this.application.marks.find(x => x.Marks_Id == 12);
            this.application.secondPositionMerit = this.application.marks.find(x => x.Marks_Id == 13);
            this.application.thirdPositionMerit = this.application.marks.find(x => x.Marks_Id == 14);
            this.application.totalMerit = this.application.marks.find(x => x.Marks_Id == 15);
            this.application.bachelors = null;
          }
        }, err => {
          console.log(err);
        });
      }
    }, err => {
      console.log(err);
    });
  }
  public setAdhocMerit() {
    this._adhocService.setAdhocMerit(this.application.Id).subscribe((marks: any) => {
      if (marks) {
        this.application.marks = marks;
        this.meritMarks.forEach(meritMark => {
          meritMark.applicationMarks = this.application.marks.find(x => x.Marks_Id == meritMark.Id);
        });
      }
      this.getAdhocApplicationApplicant();
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
        this.getMeritMarks();
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
    let tempexp = [];

    this.applicant.ApplicantExperiences.forEach(experience => {
      experience.scrutiny = this.applicant.AdhocScrutinies.find(x => x.Experience_Id == experience.Id);
      if (experience.scrutiny) {
        if (!experience.scrutiny.IsRejected) {
          tempexp.push(experience);
        }
      }
    });
    this.applicant.ApplicantExperiences = tempexp;
    this.scrutinyDone = true;
    this.scrutinyComplete = this.applicant.AdhocScrutinies.length >= (2 + this.applicant.Qualifications.length + this.applicant.ApplicantExperiences.length);
    let scrutinyRejectedObj = this.applicant.AdhocScrutinies.find(x => x.IsRejected == true && x.Experience_Id == null);
    if (scrutinyRejectedObj) {
      this.scrutinyRejected = true;
    } else {
      this.scrutinyRejected = false;
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
  public openEditDialog(item: any, type: string) {
    if (type == 'Qualification') {
      item.PassingYear = new Date(item.PassingYear);
    }
    if (type == 'Experience') {
      item.FromDate = new Date(item.FromDate);
      item.ToDate = new Date(item.ToDate);
    }
    this.selectedValue = { T: type, ...item };
    this.editDialog = true;
  }
  public closeScrutinyDialog() {
    this.decisionObj = {};
    this.application.Agree = false;
    this.application.Agree2 = false;
    this.decision = '';
    this.adhocScrutiny = {};
    this.scrutinyDialog = false;
  }
  public closeEditDialog() {
    this.selectedValue = {};
    this.editDialog = false;
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
    let css = this.viewAs == 2 ? '@page { size: landscape; }' : '@page { size: potrait; }';
    let head = document.head || document.getElementsByTagName('head')[0],
      style: any = document.createElement('style');

    /*   style.type = 'text/css';
      style.media = 'print'; */

    if (style.styleSheet) {
      style.styleSheet.cssText = css;
    } else {
      style.appendChild(document.createTextNode(css));
    }
    head.appendChild(style);

    window.print();
    return;
    let html = document.getElementById('formPrint').innerHTML;
    var mywindow = window.open('', 'PRINT', 'height=600,width=900');
    if (mywindow) {
      mywindow.document.write(`<html><head>
     
    <style>
   
    body {
      padding: 30px 50px 0 50px;
      font-family: -apple-system, system-ui;
    }

    p {
      margin-top: 0;
      margin-bottom: 1rem !important;
    }

    .mt-2 {
      margin-top: 0.5rem !important;
    }

    .mb-0 {
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


    .table.border-c tr {
      border-bottom: 1px solid grey !important;
    }


    .table-merits {
      width: 100%;
    }

    .table-merits th,
    .table-merits td {
      border: 1px solid grey;
      margin-left: 4px;
      padding: 4px;
      font-family: 'Roboto-Bold';
      font-weight: 100;
    }

    .table-merits td .regul-text {
      font-family: 'Roboto-Regular' !important;
    }

    .image--cover {
      width: 150px;
      height: 150px;

    }

    .zoom {
      transition: transform .2s;
      border: 1px solid black;
    }

    .zoom:hover {
      transform: scale(1.08);
      box-shadow: 0px 0px 5px black !important;
    }

    .info-custom-box {
      font-size: 20px;
      padding: 10px;
      background: white;
      border-radius: 15px;
      list-style: none;
      box-shadow: 0px 0px 2px grey;
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
  printList() {
    let html = document.getElementById('listt').innerHTML;
    var mywindow = window.open('', 'PRINT', 'height=600,width=900');
    if (mywindow) {
      mywindow.document.write(`<html><head>
          <style>
          body {
            font-family: -apple-system, system-ui, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;
         margin: 55px !important;
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
        .list-unstyled {
          padding-left: 0;
          list-style: none;
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
            } .bottom-heading {
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
      bottom: 0;    left: 27%;color:#e3e3e3;">
      <small style="margin-top: 10px;">
      <u>This document is electronically system generated</u>
    </small>
      <br>
      Powered by Health Information and
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


enum Degree {
  Matriculation = 58,
  FA = 98,
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
