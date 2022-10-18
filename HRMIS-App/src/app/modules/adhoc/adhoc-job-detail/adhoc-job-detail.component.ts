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
import { ActivatedRoute, Router } from "@angular/router";
//import { ActivatedRoute } from '@angular/router';
//import { LocalService } from '../../../_services/local.service';

declare var moment: any;

@Component({
  selector: "app-adhoc-job-detail",
  templateUrl: "./adhoc-job-detail.component.html",
  styleUrls: ['./adhoc-job-detail.component.scss']
})
export class AdhocJobDetailComponent implements OnInit, OnDestroy {
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
  public grievanceScrutiny: any[] = [];
  public approvedApplications: any[] = [];
  public approvedApplicationGrievanceScrutiny: any[] = [];
  public meritMarks: any[] = [
    { Id: 1, Name: 'Matriculation' }
    , { Id: 2, Name: 'Intermediate' }
    , { Id: 3, Name: 'O Level' }
    , { Id: 4, Name: 'A Level' }
    , { Id: 5, Name: 'Masters' }
    , { Id: 6, Name: 'First Stage Higher' }
    , { Id: 7, Name: 'Second Stage Higher' }
    , { Id: 8, Name: 'Third Stage Higher' }
    , { Id: 9, Name: 'Hafiz-e-Quran' }
    , { Id: 10, Name: 'Experience' }
    , { Id: 11, Name: 'Interview' }
    , { Id: 12, Name: '1st Position' }
    , { Id: 13, Name: '2nd Position' }
    , { Id: 14, Name: '3rd Position' }
    , { Id: 15, Name: 'Total' }
    , { Id: 16, Name: 'Graduation' }
  ];
  public photoFile: any[] = [];
  public minutesFile: any[] = [];
  public grievanceMinutesFile: any[] = [];
  public job: any = {};
  public job2: any = {};
  public application: any = {};
  public applicationLog: any = {};
  public scrutinyMinutes: any = {};
  public grievanceScrutinyMinutes: any = {};
  public adhocScrutiny: any = {};
  public subscription = null;
  public scrutinyDialog: boolean = false;
  public savingAdhocMerit: boolean = false;
  public scrutinyDone: boolean = false;
  public scrutinyRejected: boolean = false;
  public scrutinyComplete: boolean = false;
  public isAdmin: boolean = false;
  public changingStatus: boolean = false;
  public meritVerificationDialog: boolean = false;
  public printing: boolean = false;
  public minutesUploading: boolean = false;
  public sendingSMS: boolean = false;
  public checkingSMS: boolean = false;
  public marksLoaded: boolean = false;
  public decision: string = '';
  public decisionObj: any = {};
  public adhocScrutinyCommittee: any = {};
  public report: any = {};
  public reportGS: any = {};
  public totalReport: any = {};
  public totalReportGS: any = {};
  public adhocInterview: any = {};
  public interview: any = {};
  public selectedBatchApplication: any = {};
  public interviewBatch: any[] = [];
  public interviewBatchCommittees: any[] = [];
  public selectedBatch: any = {};
  public interviewBatchCommittee: any = {};
  public interviewBatchApplications: any[] = [];
  public rejectReasons: any[] = [
    { Id: 1, Reason: 'PMDC document not valid' },
    { Id: 2, Reason: 'Hafiz-e-Quran certificate not valid' },
    { Id: 3, Reason: 'Qualification incomplete' },
    { Id: 5, Reason: 'Domicile not from Punjab' }
  ];
  public toggle: boolean = true;
  public loading: boolean = false;
  public fontSize: number = 11;
  public max: any = 12345;
  public totalP: any = 282;
  public approved: any = 69;
  public count: any = 0;
  public verificationsRequired: number = 0;
  public verificationsDone: number = 0;
  public rejectedGrievanceBackup: any = {};
  public meritVerificationsObj: any = { Done: 0, Pending: 0 };
  public rejectedGrievance: any = 0;
  public dateNow: string = '';
  public districtName: string = '';
  public saving: boolean = false;
  public calculating: boolean = false;
  public oldBackCounts: any[] = [{
    Designation: 'Medical Officer',
    Apps: 664
  }, {
    Designation: 'Women Medical Officer',
    Apps: 790
  }, {
    Designation: 'Consultant Anaesthetist',
    Apps: 1
  }, {
    Designation: 'Consultant Cardiologist',
    Apps: 1
  }, {
    Designation: 'Consultant Dermatologist',
    Apps: 5
  }, {
    Designation: 'Consultant ENT Specialist',
    Apps: 2
  }, {
    Designation: 'Consultant Gynaecologist',
    Apps: 19
  }, {
    Designation: 'Consultant Nephrologist',
    Apps: 0
  }, {
    Designation: 'Consultant Neurologist',
    Apps: 0
  }, {
    Designation: 'Consultant Ophthalmologist',
    Apps: 2
  }, {
    Designation: 'Consultant Orthopaedic',
    Apps: 5
  }, {
    Designation: 'Consultant Paediatrician',
    Apps: 10
  }, {
    Designation: 'Consultant Pathologist',
    Apps: 7
  }, {
    Designation: 'Consultant Physician',
    Apps: 9
  }, {
    Designation: 'Consultant Psychiatrist / Neuro Psychiatrist',
    Apps: 0
  }, {
    Designation: 'Consultant Radiologist',
    Apps: 0
  }, {
    Designation: 'Consultant Surgeon',
    Apps: 8
  }, {
    Designation: 'Consultant TB/Chest Specialist',
    Apps: 0
  }, {
    Designation: 'Consultant Urologist',
    Apps: 0
  }, {
    Designation: 'Charge Nurse',
    Apps: 251
  }, {
    Designation: 'Dental Surgeon',
    Apps: 208
  }];
  public currentDate: Date;
  constructor(
    private _adhocService: AdhocService,
    public _notificationService: NotificationService,
    public _rootService: RootService,
    private route: ActivatedRoute,
    private router: Router,
    private _authenticationService: AuthenticationService,
    private ngZone: NgZone
  ) { }

  public ngOnInit(): void {
    this.user = this._authenticationService.getUser();
    this.isAdmin = this.user.UserName == 'dpd' ? true : false;
    this.scrutinyMinutes.DistrictCode = this.user.HfmisCode;
    this.grievanceScrutinyMinutes.DistrictCode = this.user.HfmisCode;

    this.interview.DistrictCode = this.user.HfmisCode;
    this.fetchParams();
    this.getAdhocScrutinyCommittee();
    this._authenticationService.userLevelNameEmitter.subscribe((x: any) => {
      this.districtName = x;
      this.districtName = this.districtName ? this.districtName.replace(' District', '') : 'District';
    });
  }

  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('deisgnationId')) {
          this.job.Designation_Id = +params['deisgnationId'];
          this.scrutinyMinutes.Designation_Id = this.job.Designation_Id;
          this.grievanceScrutinyMinutes.Designation_Id = this.job.Designation_Id;
          this.getAdhocJob();
          this.saveScrutinyMinutes();
        }
      }
    );
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
  public getCurrentDate() {
    this._rootService.getCurrentDate().subscribe((response: any) => {
      this.currentDate = new Date(response);
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
  public saveScrutinyMinutes() {
    this._adhocService.saveScrutinyMinutes(this.scrutinyMinutes).subscribe((response: any) => {
      if (response) {
        this.scrutinyMinutes = response;
      }
    }, err => {
      console.log(err);
    });
  }
  public saveInterview() {
    this.adhocInterview = {};
    this.saving = true;
    let tempBatch = this.selectedBatch;
    let tempBatchCommittees = this.selectedBatch.AdhocInterviewBatchCommittees;
    this.selectedBatch = {};
    tempBatch.AppIds = [];
    tempBatch.Applications.forEach(app => {
      tempBatch.AppIds.push(app.Id);
    });
    this.selectedBatch = {
      BatchNo: tempBatch.BatchNo,
      Candidates: tempBatch.Candidates,
      Datetime: tempBatch.Datetime,
      Venue: tempBatch.Venue
    };
    let adhocInterviewBatch = { AdhocInterviewBatch: this.selectedBatch, AdhocInterviewBatchCommittees: tempBatchCommittees, ApplicationIds: tempBatch.AppIds };
    this.adhocInterview = { AdhocInterview: this.interview, AdhocInterviewBatch: adhocInterviewBatch };
    this._adhocService.saveAdhocInterview(this.adhocInterview).subscribe((response: any) => {
      if (response) {
        this.adhocInterview = response;
        this.interview = this.adhocInterview.AdhocInterview;
        this.selectedBatch = this.adhocInterview.AdhocInterviewBatch;
        this.saving = false;
        this._notificationService.notify('success', 'Interview Batch Saved!');
        this.getAdhocInterview();
        //this.getAdhocJob();
      }
    }, err => {
      this.saving = false;
      this._notificationService.notify('danger', 'Something went wrong');
      console.log(err);
    });
  }
  public uploadScrutinyMinutes() {
    if (this.scrutinyMinutes.Id > 0 && this.minutesFile.length > 0) {
      this.minutesUploading = true;
      this._adhocService.uploadScrutinyMinutes(this.minutesFile, this.scrutinyMinutes.Id).subscribe((res4: any) => {
        this.minutesUploading = false;
        this.minutesFile = [];
        this._notificationService.notify('success', 'Scrutiny Minutes Uploaded');
        this.getScrutinyMinutes();
      }, err => {
        this._notificationService.notify('danger', 'Scrutiny Minutes Error');
        console.log(err);
      });
    }
  }
  public uploadGrievanceScrutinyMinutes() {
    if (this.scrutinyMinutes.Id > 0 && this.grievanceMinutesFile.length > 0) {
      this.minutesUploading = true;
      this._adhocService.uploadGrievanceScrutinyMinutes(this.grievanceMinutesFile, this.scrutinyMinutes.Id).subscribe((res4: any) => {
        this.minutesUploading = false;
        this.grievanceMinutesFile = [];
        this._notificationService.notify('success', 'Scrutiny Minutes Uploaded');
        this.getScrutinyMinutes();
      }, err => {
        this._notificationService.notify('danger', 'Scrutiny Minutes Error');
        console.log(err);
      });
    }
  }
  public getAdhocJob() {
    this.loading = true;
    this._adhocService.getAdhocJob(this.job.Designation_Id).subscribe((res: any) => {
      if (res) {
        this.job = res.job;
        this.job2 = res.job2;
        if (this.isAdmin) {
          this.getAdhocAcceptedGrievancePrint();
        } else {
          this.getAdhocApplicationScrutiny();
          this.getAdhocApplicationGrievanceScrutinyPrint();
        }
      }
    }, err => {
      console.log(err);
    });
  }
  public getAdhocApplicationScrutiny = () => {
    let obj = {
      hfmisCode: this.user.HfmisCode,
      Designation_Id: this.job.Designation_Id,
      Status_Id: 100
    };
    this._adhocService.getAdhocApplicationScrutinyPrint(obj).subscribe((res: any) => {
      if (res) {
        if (res.applications) {
          this.documents = res.applications;
          this.approvedApplications = [];
          let adhocScrutinies = res.adhocScrutinies;
          this.documents.forEach(doc => {
            doc.adhocScrutinies = adhocScrutinies.filter(x => x.Application_Id == doc.Id);
            if (doc.Status_Id == 2) {
              this.approvedApplications.push(doc);
            }
          });
          this.report.Designation = this.documents[0] ? this.documents[0].DesignationName : '';
        }
        if (res.totalReport) {
          this.totalReport = res.totalReport;
          this.totalReport.Total = this.totalReport.Approved + this.totalReport.Rejected;
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
          this.interview.Candidates = this.report.Approved.Total;
          if (this.interview.Candidates > 250) {
            this.interview.Batches = 5;
          }
          if (this.interview.Candidates > 100 && this.interview.Candidates < 250) {
            this.interview.Batches = 3;
          }
          this.getAdhocInterview();
        }
        this.loading = false;
      }
    },
      err => {
        console.log(err);
      }
    );
  }
  public getAdhocApplicationGrievanceScrutinyPrint = () => {
    let obj = {
      hfmisCode: this.user.HfmisCode,
      Designation_Id: this.job.Designation_Id,
      Status_Id: 100
    };
    this._adhocService.getAdhocApplicationGrievanceScrutinyPrint(obj).subscribe((res: any) => {
      if (res) {
        if (res.applications) {
          this.grievanceScrutiny = res.applications;
          this.approvedApplicationGrievanceScrutiny = [];
          let adhocScrutinies = res.adhocScrutinies;
          this.grievanceScrutiny.forEach(doc => {
            doc.adhocScrutinies = adhocScrutinies.filter(x => x.Application_Id == doc.Id);
            if (doc.Status_Id == 2) {
              this.approvedApplicationGrievanceScrutiny.push(doc);
            }
          });
          this.reportGS.Designation = this.grievanceScrutiny[0] ? this.grievanceScrutiny[0].DesignationName : '';
          this.rejectedGrievanceBackup = this.oldBackCounts.find(x => x.Designation == this.reportGS.Designation);
        }
        if (res.totalReport) {
          this.totalReportGS = res.totalReport;
          this.totalReportGS.Total = this.totalReportGS.Approved + this.totalReportGS.Rejected;
        }
        /*  if (res.report) {
           let temp = res.report;
           this.reportGS.Total = temp.find(x => x.StatusId == 4);
           if (!this.reportGS.Total) this.reportGS.Total = { Total: 0 };
           this.reportGS.Approved = temp.find(x => x.StatusId == 2);
           if (!this.reportGS.Approved) this.reportGS.Approved = { Total: 0 };
           this.reportGS.Rejected = temp.find(x => x.StatusId == 3);
           if (!this.reportGS.Rejected) this.reportGS.Rejected = { Total: 0 };
           if (this.reportGS.Total && this.reportGS.Approved && this.reportGS.Rejected) {
             this.reportGS.Total.Total = this.reportGS.Total.Total + this.reportGS.Approved.Total + this.reportGS.Rejected.Total
           }
         } */
        this.loading = false;
      }
    },
      err => {
        console.log(err);
      }
    );
  }
  public changeFontSize() {
    this.fontSize = this.fontSize == 8 ? 10 : this.fontSize == 10 ? 11 : this.fontSize == 11 ? 12 : this.fontSize == 12 ? 8 : 0;
  }
  public getAdhocAcceptedGrievancePrint = () => {
    let obj = {
      hfmisCode: this.user.HfmisCode,
      Designation_Id: this.job.Designation_Id,
      Status_Id: 100
    };
    this._adhocService.getAdhocAcceptedGrievancePrint(obj).subscribe((res: any) => {
      if (res) {
        if (res.applications) {
          this.documents = res.applications;
          this.approvedApplications = [];
          let adhocScrutinies = res.adhocScrutinies;
          this.documents.forEach(doc => {
            doc.adhocScrutinies = adhocScrutinies.filter(x => x.Application_Id == doc.Id);
            if (doc.Status_Id == 2) {
              this.approvedApplications.push(doc);
            }
            if (doc.Status_Id == 3) {
              this.rejectedGrievance++;
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
        this.loading = false;
      }
    },
      err => {
        console.log(err);
      }
    );
  }
  public getAdhocInterviewBatchApplications(item) {
    this.selectedBatch = item;
    if (this.selectedBatch.Id) {
      this.interviewBatchCommittees = [];
      this._adhocService.getAdhocInterviewBatchCommittee(this.selectedBatch.Id).subscribe((res: any[]) => {
        if (res) {
          this.interviewBatchCommittees = res;
        }
      }, err => {
        console.log(err);
      });
      this._adhocService.getAdhocInterviewBatchApplications(this.selectedBatch.Id).subscribe((applications: any[]) => {
        if (applications && applications.length > 0) {
          this.selectedBatch.Applications = applications;
        }
      }, err => {
        console.log(err);
      });
    }
  }
  public setAdhocMerit() {
    this.marksLoaded = false;
    this.calculating = true;
    this._adhocService.setAdhocMerit(this.interview.Id).subscribe((marks: any) => {
      this.getAdhocInterview();
      this.calculating = false;
    }, err => {
      console.log(err);
    });

  }

  public saveMeritVerification(item: any, type: number, verified: boolean) {
    if (item) {
      if (verified) {
        item.saving = true;
      } else {
        item.notsaving = true;
      }
    }
    let meritVerification: any = {};
    meritVerification.IsVerified = verified;
    meritVerification.ApplicantId = this.selectedBatchApplication.Applicant_Id;
    meritVerification.ApplicationId = this.selectedBatchApplication.Application_Id;
    meritVerification.BatchApplicationId = this.selectedBatchApplication.Id;
    if (type == 1) {
      meritVerification.DocId = 1;
      this.selectedBatchApplication.verifyingC = true;
      meritVerification.UploadPath = this.selectedBatchApplication.CNICDoc;
    } else if (type == 2) {
      meritVerification.QualificationId = item.Id;
      meritVerification.UploadPath = item.UploadPath;
    } else if (type == 3) {
      meritVerification.ExperienceId = item.Id;
      meritVerification.UploadPath = item.UploadPath;
    } else if (type == 4) {
      meritVerification.DocId = 2;
      meritVerification.UploadPath = this.selectedBatchApplication.HifzDocument;
      this.selectedBatchApplication.verifyingH = true;
      if (verified) {
        this.selectedBatchApplication.verifyingH = true;
      } else {
        this.selectedBatchApplication.verifyingHN = true;
      }
    }
    meritVerification.DistrictCode = this.user.HfmisCode;
    this._adhocService.saveMeritVerification(meritVerification).subscribe((res: any[]) => {
      if (res) {
        this.getMeritVerification();
        if (item) {
          if (verified) {
            item.saving = false;
          } else {
            item.notsaving = false;
          }
        } else {
          this.selectedBatchApplication.verifyingH = false;
          this.selectedBatchApplication.verifyingHN = false;
          this.selectedBatchApplication.verifyingC = false;
        }
      }
    }, err => {
      console.log(err);
    });
  }

  public getAdhocInterview() {
    this.meritVerificationsObj = { Done: 0, Pending: 0 };
    this.marksLoaded = false;
    this.adhocInterview = {};
    this.interviewBatch = [];
    this._adhocService.getAdhocInterview(this.user.HfmisCode, this.job.Designation_Id).subscribe((res: any) => {
      if (res && res.Id > 0) {
        this.adhocInterview = res;
        this.interview = {};
        this.interview = this.adhocInterview;
        this.interview.DesignationName = this.job.DesignationName;
        this._adhocService.getAdhocInterviewBatches(this.adhocInterview.Id).subscribe((batches: any[]) => {
          if (batches && batches.length > 0) {
            this.interviewBatch = batches;
            this.interviewBatchApplications = [];
            this.count = 0;
            let c = 0;

            this._rootService.getCurrentDate().subscribe((response: any) => {
              this.currentDate = new Date(response);
              this.interviewBatch.forEach(batch => {
                if (!batch.IdTemp) {
                  this._adhocService.getAdhocInterviewBatchApplications(batch.Id).subscribe((applications: any[]) => {
                    c++;
                    if (applications) {
                      this.interviewBatchApplications = [...this.interviewBatchApplications, ...applications];
                      if (c == this.interviewBatch.length) {
                        this.interviewBatchApplications = this.interviewBatchApplications.filter(x => x.IsPresent == true && x.IsLocked == true && !x.IsRejected);
                        this._adhocService.getAdhocDistrictMerit(this.job.Designation_Id, this.user.HfmisCode).subscribe((resss: any) => {
                          let marks = resss.marks;
                          let applicantQualifications = resss.applicantQualifications;
                          let applicantExperiences = resss.applicantExperiences;
                          if (marks) {
                            this.interviewBatchApplications.forEach(a => {
                              a.documents = [];
                              let dob = moment(this.currentDate);
                              let today = moment(a.DOB);
                              let years = dob.diff(today, 'year');
                              today.add(years, 'years');
                              let months = dob.diff(today, 'months');
                              today.add(months, 'months');
                              let days = dob.diff(today, 'days');
                              a.ddd = days;
                              a.mmm = months;
                              a.yyy = years;
                              a.marks = marks.filter(x => x.Application_Id == a.Application_Id);
                              a.qualifications = applicantQualifications.filter(x => x.Applicant_Id == a.Applicant_Id);
                              a.experiences = applicantExperiences.filter(x => x.Applicant_Id == a.Applicant_Id);

                              a.meritMarks = [
                                { Id: 1, Name: 'Matriculation' }
                                , { Id: 2, Name: 'Intermediate' }
                                , { Id: 3, Name: 'O Level' }
                                , { Id: 4, Name: 'A Level' }
                                , { Id: 5, Name: 'Masters' }
                                , { Id: 6, Name: 'First Stage Higher' }
                                , { Id: 7, Name: 'Second Stage Higher' }
                                , { Id: 8, Name: 'Third Stage Higher' }
                                , { Id: 9, Name: 'Hafiz-e-Quran' }
                                , { Id: 10, Name: 'Experience' }
                                , { Id: 11, Name: 'Interview' }
                                , { Id: 12, Name: '1st Position' }
                                , { Id: 13, Name: '2nd Position' }
                                , { Id: 14, Name: '3rd Position' }
                                , { Id: 15, Name: 'Total' }
                                , { Id: 16, Name: 'Graduation' }
                              ];
                              a.meritMarks.forEach(meritMark => {
                                meritMark.applicationMarks = a.marks.find(x => x.Marks_Id == meritMark.Id);
                              });

                              a.total = 0;
                              a.matricMerit = a.marks.find(x => x.Marks_Id == 1);
                              if (a.matricMerit) {
                                this.marksLoaded = true;
                              }
                              a.interMerit = a.marks.find(x => x.Marks_Id == 2);
                              a.grad = a.marks.find(x => x.Marks_Id == 16);
                              let mbbs1 = a.qualifications.find(x => x.Required_Degree_Id == 62);
                              let mbbs2 = a.qualifications.find(x => x.Required_Degree_Id == 116);
                              let mbbs3 = a.qualifications.find(x => x.Required_Degree_Id == 117);
                              let mbbs4 = a.qualifications.find(x => x.Required_Degree_Id == 118);
                              let mbbs5 = a.qualifications.find(x => x.Required_Degree_Id == 119);

                              let bds1 = a.qualifications.find(x => x.Required_Degree_Id == 113);
                              let bds2 = a.qualifications.find(x => x.Required_Degree_Id == 134);
                              let bds3 = a.qualifications.find(x => x.Required_Degree_Id == 135);
                              let bds4 = a.qualifications.find(x => x.Required_Degree_Id == 136);

                              let gn1 = a.qualifications.find(x => x.Required_Degree_Id == 129);
                              let gn2 = a.qualifications.find(x => x.Required_Degree_Id == 131);
                              let gn3 = a.qualifications.find(x => x.Required_Degree_Id == 132);
                              let gn4 = a.qualifications.find(x => x.Required_Degree_Id == 133);

                              if (this.job.Designation_Id == 431) {
                                a.grad.TotalMarks = ((bds1 ? bds1.TotalMarks : 0) + (bds2 ? bds2.TotalMarks : 0) + (bds3 ? bds3.TotalMarks : 0) +
                                  (bds4 ? bds4.TotalMarks : 0)).toFixed();
                                a.grad.ObtainedMarks = ((bds1 ? bds1.ObtainedMarks : 0) + (bds2 ? bds2.ObtainedMarks : 0) + (bds3 ? bds3.ObtainedMarks : 0) +
                                  (bds4 ? bds4.ObtainedMarks : 0)).toFixed();
                              } else if (this.job.Designation_Id == 302) {
                                a.grad.TotalMarks = ((gn1 ? gn1.TotalMarks : 0) + (gn2 ? gn2.TotalMarks : 0) + (gn3 ? gn3.TotalMarks : 0) +
                                  (gn4 ? gn4.TotalMarks : 0)).toFixed();
                                a.grad.ObtainedMarks = ((gn1 ? gn1.ObtainedMarks : 0) + (gn2 ? gn2.ObtainedMarks : 0) + (gn3 ? gn3.ObtainedMarks : 0) +
                                  (gn4 ? gn4.ObtainedMarks : 0)).toFixed();
                              } else {
                                a.grad.TotalMarks = ((mbbs1 ? mbbs1.TotalMarks : 0) + (mbbs2 ? mbbs2.TotalMarks : 0) + (mbbs3 ? mbbs3.TotalMarks : 0) +
                                  (mbbs4 ? mbbs4.TotalMarks : 0) + (mbbs5 ? mbbs5.TotalMarks : 0)).toFixed();
                                a.grad.ObtainedMarks = ((mbbs1 ? mbbs1.ObtainedMarks : 0) + (mbbs2 ? mbbs2.ObtainedMarks : 0) + (mbbs3 ? mbbs3.ObtainedMarks : 0) +
                                  (mbbs4 ? mbbs4.ObtainedMarks : 0) + (mbbs5 ? mbbs5.ObtainedMarks : 0)).toFixed();
                              }
                              a.master = a.marks.find(x => x.Marks_Id == 16);
                              a.firstStageMerit = a.marks.find(x => x.Marks_Id == 6);
                              a.secondStageMerit = a.marks.find(x => x.Marks_Id == 7);
                              a.thirdStageMerit = a.marks.find(x => x.Marks_Id == 8);
                              a.hifzMerit = a.marks.find(x => x.Marks_Id == 9);
                              a.expMerit = a.marks.find(x => x.Marks_Id == 10);
                              a.interviewMerit = a.marks.find(x => x.Marks_Id == 10);
                              a.firstPositionMerit = a.marks.find(x => x.Marks_Id == 12);
                              a.secondPositionMerit = a.marks.find(x => x.Marks_Id == 13);
                              a.thirdPositionMerit = a.marks.find(x => x.Marks_Id == 14);
                              a.totalMerit = a.marks.find(x => x.Marks_Id == 15);
                              if (a.matricMerit) { a.total += a.matricMerit.Marks; }
                              if (a.interMerit) { a.total += a.interMerit.Marks; }
                              if (a.grad) { a.total += a.grad.Marks; }
                              if (a.firstPositionMerit) { a.total += a.firstPositionMerit.Marks; }
                              if (a.secondPositionMerit) { a.total += a.secondPositionMerit.Marks; }
                              if (a.thirdPositionMerit) { a.total += a.thirdPositionMerit.Marks; }
                              if (a.firstStageMerit) { a.total += a.firstStageMerit.Marks; }
                              if (a.secondStageMerit) { a.total += a.secondStageMerit.Marks; }
                              if (a.thirdStageMerit) { a.total += a.thirdStageMerit.Marks; }
                              if (a.hifzMerit) { a.total += a.hifzMerit.Marks; }
                              if (a.expMerit) { a.total += a.expMerit.Marks; }
                              if (a.InterviewMarks) { a.total += a.InterviewMarks; }
                            });
                          }
                          // this.interviewBatchApplications = this.interviewBatchApplications.sort((a, b) => (a.total > b.total ? -1 : 1));

                          //this.interviewBatchApplications = this.interviewBatchApplications.filter(x => x.firstStageMerit != null || x.secondStageMerit != null);
                          this.interviewBatchApplications = this.interviewBatchApplications.sort((a, b) => ((a.grad ? a.grad.ObtainedMarks : 0) > (b.grad ? b.grad.ObtainedMarks : 0) ? -1 : 1));
                          this.interviewBatchApplications = this.interviewBatchApplications.sort((a, b) => (a.TotalDays > b.TotalDays ? -1 : 1));
                          this.interviewBatchApplications = this.interviewBatchApplications.sort((a, b) => (a.total > b.total ? -1 : 1));
                          this.getMeritVerifications();
                        }, err => {
                          console.log(err);
                        });
                      }
                    }
                  }, err => {
                    console.log(err);
                  });
                } else {
                  c++;
                }
              });
            }, err => {
              console.log(err);
            });
          }
          if (this.interview.Batches != this.interviewBatch.length) {
            this.setInterviewAndBatches();
          }
        }, err => {
          console.log(err);
        });
      } else {
        this.interview.Job_Id = this.job.Id;
        this.interview.Designation_Id = this.job.Designation_Id;
        this.interview.DesignationName = this.job.DesignationName;
        this.setInterviewAndBatches();
      }
    }, err => {
      console.log(err);
    });
  }
  public saveAdhocMerit(a: any) {
    this.savingAdhocMerit = true;
    let merit: any = {};

    merit.BatchApplication_Id = a.Id;
    merit.Applicant_Id = a.Applicant_Id;
    merit.Application_Id = a.Application_Id;
    merit.DistrictCode = this.user.HfmisCode;
    merit.MatricMarks_Id = a.matricMerit ? a.matricMerit.Id : null;
    merit.MatricTotal = a.matricMerit ? a.matricMerit.TotalMarks : null;
    merit.MatricObtained = a.matricMerit ? a.matricMerit.ObtainedMarks : null;
    merit.MatricPercent = a.matricMerit ? a.matricMerit.Percentage : null;
    merit.Matriculation = a.matricMerit ? a.matricMerit.Marks : null;

    merit.InterMarks_Id = a.interMerit ? a.interMerit.Id : null;
    merit.InterTotal = a.interMerit ? a.interMerit.TotalMarks : null;
    merit.InterObtained = a.interMerit ? a.interMerit.ObtainedMarks : null;
    merit.InterPercent = a.interMerit ? a.interMerit.Percentage : null;
    merit.Intermediate = a.interMerit ? a.interMerit.Marks : null;

    let mbbs1 = a.qualifications.find(x => x.Required_Degree_Id == 62);
    let mbbs2 = a.qualifications.find(x => x.Required_Degree_Id == 116);
    let mbbs3 = a.qualifications.find(x => x.Required_Degree_Id == 117);
    let mbbs4 = a.qualifications.find(x => x.Required_Degree_Id == 118);
    let mbbs5 = a.qualifications.find(x => x.Required_Degree_Id == 119);


    let bds1 = a.qualifications.find(x => x.Required_Degree_Id == 113);
    let bds2 = a.qualifications.find(x => x.Required_Degree_Id == 134);
    let bds3 = a.qualifications.find(x => x.Required_Degree_Id == 135);
    let bds4 = a.qualifications.find(x => x.Required_Degree_Id == 136);


    let gn1 = a.qualifications.find(x => x.Required_Degree_Id == 129);
    let gn2 = a.qualifications.find(x => x.Required_Degree_Id == 131);
    let gn3 = a.qualifications.find(x => x.Required_Degree_Id == 132);
    let gn4 = a.qualifications.find(x => x.Required_Degree_Id == 133);
    if (this.job.Designation_Id == 431) {
      merit.MasterTotal = ((bds1 ? bds1.TotalMarks : 0) + (bds2 ? bds2.TotalMarks : 0) + (bds3 ? bds3.TotalMarks : 0) +
        (bds4 ? bds4.TotalMarks : 0)).toFixed();
      merit.MasterObtained = ((bds1 ? bds1.ObtainedMarks : 0) + (bds2 ? bds2.ObtainedMarks : 0) + (bds3 ? bds3.ObtainedMarks : 0) +
        (bds4 ? bds4.ObtainedMarks : 0)).toFixed();
    } else if (this.job.Designation_Id == 302) {
      merit.MasterTotal = ((gn1 ? gn1.TotalMarks : 0) + (gn2 ? gn2.TotalMarks : 0) + (gn3 ? gn3.TotalMarks : 0) +
        (gn4 ? gn4.TotalMarks : 0)).toFixed();
      merit.MasterObtained = ((gn1 ? gn1.ObtainedMarks : 0) + (gn2 ? gn2.ObtainedMarks : 0) + (gn3 ? gn3.ObtainedMarks : 0) +
        (gn4 ? gn4.ObtainedMarks : 0)).toFixed();
    } else {
      merit.MasterTotal = ((mbbs1 ? mbbs1.TotalMarks : 0) + (mbbs2 ? mbbs2.TotalMarks : 0) + (mbbs3 ? mbbs3.TotalMarks : 0) +
        (mbbs4 ? mbbs4.TotalMarks : 0) + (mbbs5 ? mbbs5.TotalMarks : 0)).toFixed();
      merit.MasterObtained = ((mbbs1 ? mbbs1.ObtainedMarks : 0) + (mbbs2 ? mbbs2.ObtainedMarks : 0) + (mbbs3 ? mbbs3.ObtainedMarks : 0) +
        (mbbs4 ? mbbs4.ObtainedMarks : 0) + (mbbs5 ? mbbs5.ObtainedMarks : 0)).toFixed();
    }
    merit.MasterMarks_Id = a.grad ? a.grad.Id : null;
    merit.MasterPercent = a.grad ? a.grad.Percentage : null;
    merit.Master = a.grad ? a.grad.Marks : null;

    merit.FirstHigher = a.firstStageMerit ? a.firstStageMerit.Marks : null;
    merit.FirstHigherMarks_Id = a.firstStageMerit ? a.firstStageMerit.Id : null;
    merit.SecondHigher = a.secondStageMerit ? a.secondStageMerit.Marks : null;
    merit.SecondHigherMarks_Id = a.secondStageMerit ? a.secondStageMerit.Id : null;
    merit.ThirdHigher = a.thirdStageMerit ? a.thirdStageMerit.Marks : null;
    merit.ThirdHigherMarks_Id = a.thirdStageMerit ? a.thirdStageMerit.Id : null;
    merit.FirstPosition = a.firstPositionMerit ? a.firstPositionMerit.Marks : null;
    merit.FirstPositionMarks_Id = a.firstPositionMerit ? a.firstPositionMerit.Id : null;
    merit.SecondPosition = a.secondPositionMerit ? a.secondPositionMerit.Marks : null;
    merit.SecondPositionMarks_Id = a.secondPositionMerit ? a.secondPositionMerit.Id : null;
    merit.ThirdPosition = a.thirdPositionMerit ? a.thirdPositionMerit.Marks : null;
    merit.ThirdPositionMarks_Id = a.thirdPositionMerit ? a.thirdPositionMerit.Id : null;

    merit.OneYearExp = a.expMerit && a.expMerit.Marks >= 0 && a.expMerit.Marks < 2 ? a.expMerit.Marks : null;
    merit.TwoYearExp = a.expMerit && a.expMerit.Marks >= 2 && a.expMerit.Marks < 3 ? a.expMerit.Marks : null;
    merit.ThreeYearExp = a.expMerit && a.expMerit.Marks >= 3 && a.expMerit.Marks < 4 ? a.expMerit.Marks : null;
    merit.FourYearExp = a.expMerit && a.expMerit.Marks >= 4 && a.expMerit.Marks < 5 ? a.expMerit.Marks : null;
    merit.FivePlusYearExp = a.expMerit && a.expMerit.Marks >= 5 ? a.expMerit.Marks : null;
    merit.ExperienceMarks_Id = a.expMerit ? a.expMerit.Id : null;
    merit.Hafiz = a.hifzMerit ? a.hifzMerit.Marks : null;
    merit.HafizMarks_Id = a.hifzMerit ? a.hifzMerit.Id : null;
    merit.Interview = a.InterviewMarks;
    merit.Total = a.total;
    merit.IsValid = a.verificationsCompleted == true ? true : false;

    this._adhocService.saveAdhocMerit(merit).subscribe((res) => {
      if (res) {
        this.savingAdhocMerit = false;
        this.closeMeritDialog();
      }
    }, err => {
      this.savingAdhocMerit = false;
      console.log(err);
    });
  }
  public onTabSelect(e) {
    let selectedTab = e.heading;
    if (selectedTab == 'Merit List') {
      this.router.navigate(['/adhoc-merit-wp']);
    }
  }
  public setInterviewAndBatches() {
    for (let index = 0; index < this.interview.Batches; index++) {
      let btch = this.interviewBatch.find(x => x.BatchNo == 'Batch ' + (index + 1));
      if (!btch) {
        let cand = ((this.interview.Candidates / this.interview.Batches) as number).toFixed();
        this.interviewBatch.push({
          IdTemp: (index + 1),
          BatchNo: 'Batch ' + (index + 1),
          Candidates: this.approvedApplications.slice((index * (+cand)), ((index * (+cand)) + (+cand))).length,
          Applications: this.approvedApplications.slice((index * (+cand)), ((index * (+cand)) + (+cand)))
        });
      }
    }
    let count: number = 0;
    if (this.interview.Batches != this.interviewBatch.length) {
      this.interviewBatch.forEach(b => {
        if (b.Applications) {
          count += b.Applications.length;
        }
      });
      if (count < this.approvedApplications.length) {
        let index = this.interviewBatch.length;
        let btch = this.interviewBatch.find(x => x.BatchNo == 'Batch ' + (index + 1));
        if (!btch) {
          let cand = ((this.approvedApplications.length - count) as number).toFixed();
          this.interviewBatch.push({
            IdTemp: (index + 1),
            BatchNo: 'Batch ' + (index + 1),
            Candidates: +cand,
            Applications: this.approvedApplications.slice((index * (+cand)), ((index * (+cand)) + (+cand)))
          });
        }
      }
    }
    this.selectedBatch = {};
  }

  public setInterviewAndNewBatches() {
    this.interviewBatch = [];
    for (let index = 0; index < this.interview.Batches; index++) {
      let cand = ((this.interview.Candidates / this.interview.Batches) as number).toFixed();
      this.interviewBatch.push({
        IdTemp: (index + 1),
        BatchNo: 'Batch ' + (index + 1),
        Candidates: this.approvedApplications.slice((index * (+cand)), ((index * (+cand)) + (+cand))).length,
        Applications: this.approvedApplications.slice((index * (+cand)), ((index * (+cand)) + (+cand)))
      });
    }
    let count: number = 0;
    this.interviewBatch.forEach(b => {
      count += b.Applications.length;
    });
    if (count < this.approvedApplications.length) {
      let index = this.interviewBatch.length;
      let cand = ((this.approvedApplications.length - count) as number).toFixed();
      this.interviewBatch.push({
        IdTemp: (index + 1),
        BatchNo: 'Batch ' + (index + 1),
        Candidates: +cand,
        Applications: this.approvedApplications.slice((index * (+cand)), ((index * (+cand)) + (+cand)))
      });
    }
    this.selectedBatch = {};
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
  /* public setAdhocMerit(app) {
    this._adhocService.setAdhocMerit(this.application.Id).subscribe((marks: any) => {
      if (marks) {
        this.application.marks = marks;
        this.meritMarks.forEach(meritMark => {
          meritMark.applicationMarks = this.application.marks.find(x => x.Marks_Id == meritMark.Id);
        });
      }
    }, err => {
      console.log(err);
    });
  } */

  public getApplicantPMC() {
    console.log(this.selectedBatchApplication);
    this._adhocService.getAdhocApplicantPMC(this.selectedBatchApplication.Applicant_Id).subscribe((r: any) => {
      console.log(this.selectedBatchApplication.Applicant_Id);

      if (r && r.AdhocApplicantPMC) {
        this.selectedBatchApplication.pmcVerification = r.AdhocApplicantPMC;
        this.selectedBatchApplication.pmcQualification = r.Qualifications;
        if (this.selectedBatchApplication.pmcQualification && this.selectedBatchApplication.pmcQualification.length == 1) {
          this.selectedBatchApplication.OneD++;
        } else if (this.selectedBatchApplication.pmcQualification && this.selectedBatchApplication.pmcQualification.length == 2) {
          this.selectedBatchApplication.TwoD++;
        } else if (this.selectedBatchApplication.pmcQualification && this.selectedBatchApplication.pmcQualification.length == 3) {
          this.selectedBatchApplication.ThreeD++;
        }
      }
      else {
        this._adhocService.verifyPMCGetQualifications(this.selectedBatchApplication.PMDCNumber).subscribe((res) => {
          console.log(res);
          this.selectedBatchApplication.pmcQualification = res;
          if (this.selectedBatchApplication.pmcQualification && this.selectedBatchApplication.pmcQualification.data) {
            console.log(this.selectedBatchApplication.pmcQualification.data);
            let obj: any = {
              AdhocApplicantPMC: {

                Applicant_Id: this.selectedBatchApplication.Applicant_Id,
                Name: this.selectedBatchApplication.pmcQualification.data.Name,
                FatherName: this.selectedBatchApplication.pmcQualification.data.FatherName,
                Gender: this.selectedBatchApplication.pmcQualification.data.Gender,
                RegistrationDate: new Date(this.selectedBatchApplication.pmcQualification.data.RegistrationDate).toDateString(),
                RegistrationNo: this.selectedBatchApplication.pmcQualification.data.RegistrationNo,
                RegistrationType: this.selectedBatchApplication.pmcQualification.data.RegistrationType,
                Status: this.selectedBatchApplication.pmcQualification.data.Status,
                ValidUpto: new Date(this.selectedBatchApplication.pmcQualification.data.ValidUpto).toDateString()
              },
              Qualifications: this.selectedBatchApplication.pmcQualification.data.Qualifications
            };
            if (this.selectedBatchApplication.pmcQualification && this.selectedBatchApplication.pmcQualification.data.Qualifications.length == 1) {
              this.selectedBatchApplication.OneD++;
            } else if (this.selectedBatchApplication.pmcQualification && this.selectedBatchApplication.pmcQualification.data.Qualifications.length == 2) {
              this.selectedBatchApplication.TwoD++;
            } else if (this.selectedBatchApplication.pmcQualification && this.selectedBatchApplication.pmcQualification.data.Qualifications.length == 3) {
              this.selectedBatchApplication.ThreeD++;
            }
            this._adhocService.saveAdhocApplicantPMC(obj).subscribe((r) => {
              console.log(this.selectedBatchApplication.Applicant_Id);
              this._adhocService.getAdhocApplicantPMC(this.selectedBatchApplication.Applicant_Id).subscribe((p: any) => {
                console.log(p);
                if (p && p.AdhocApplicantPMC) {
                  this.selectedBatchApplication.pmcVerification = p.AdhocApplicantPMC;
                  this.selectedBatchApplication.pmcQualification = p.Qualifications;
                  if (this.selectedBatchApplication.pmcQualification && this.selectedBatchApplication.pmcQualification.length == 1) {
                    this.selectedBatchApplication.OneD++;
                  } else if (this.selectedBatchApplication.pmcQualification && this.selectedBatchApplication.pmcQualification.length == 2) {
                    this.selectedBatchApplication.TwoD++;
                  } else if (this.selectedBatchApplication.pmcQualification && this.selectedBatchApplication.pmcQualification.length == 3) {
                    this.selectedBatchApplication.ThreeD++;
                  }
                }
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
    }, err => {
      console.log(err);
    });
  }

  public sendAdhocInterviewSMS() {
    if (confirm('Please confirm send sms?')) {
      this.sendingSMS = true;
      this._adhocService.sendAdhocInterviewSMS(this.selectedBatch.Id).subscribe((res: any) => {
        setTimeout(() => {
          this.sendingSMS = false;
          this.checkingSMS = true;
          this._adhocService.checkAdhocInterviewSMS(this.selectedBatch.Id).subscribe((res: any) => {
            this.checkingSMS = false;
            this.getAdhocJob();
          }, err => {
            console.log(err);
          });
        }, 6000);
      }, err => {
        console.log(err);
      });
    }
  }
  public saveAdhocInterviewBatchCommittee() {
    this.interviewBatchCommittee.saving = true;
    /* if (!this.selectedBatch.AdhocInterviewBatchCommittees) {
      this.selectedBatch.AdhocInterviewBatchCommittees = [];
    }
    this.selectedBatch.AdhocInterviewBatchCommittees.push({ ...this.interviewBatchCommittee }); */
    /* this.interviewBatch.forEach(batch => {
      if (!batch.AdhocInterviewBatchCommittees) {
        batch.AdhocInterviewBatchCommittees = [];
        batch.AdhocInterviewBatchCommittees.push({ ...this.interviewBatchCommittee });
      }
      else if (batch.AdhocInterviewBatchCommittees && this.selectedBatch.IdTemp == 1) {
        batch.AdhocInterviewBatchCommittees.push({ ...this.interviewBatchCommittee });
      }
      else if (batch.AdhocInterviewBatchCommittees && batch.IdTemp != 1 && batch.IdTemp == this.selectedBatch.IdTemp) {
        batch.AdhocInterviewBatchCommittees.push({ ...this.interviewBatchCommittee });
      }
    }); */
    this.interviewBatchCommittee.saving = true;
    this.interviewBatchCommittee.InterviewBatch_Id = this.selectedBatch.Id;
    this._adhocService.saveAdhocInterviewBatchCommittee(this.interviewBatchCommittee).subscribe((res: any) => {
      if (res) {
        this.interviewBatchCommittee = {};
        this.interviewBatchCommittees = [];
        this._adhocService.getAdhocInterviewBatchCommittee(this.selectedBatch.Id).subscribe((res: any[]) => {
          if (res && res.length > 0) {
            this.interviewBatchCommittees = res;
          }
        }, err => {
          console.log(err);
        });
      }
    }, err => {
      console.log(err);
    });
  }
  public removeBatchCommittee(committee: any, index: number) {
    /* committee.removing = true;
    if (!this.selectedBatch.Id) {
      this.interviewBatch.forEach(batch => {
        if (batch.IdTemp == this.selectedBatch.IdTemp) {
          if (batch.AdhocInterviewBatchCommittees) {
            batch.AdhocInterviewBatchCommittees.splice(index, 1);
          }
        }
      });
      this.selectedBatch.AdhocInterviewBatchCommittees.splice(index, 1);
      committee.removing = false;
    } */
    committee.removing = true;
    let com: any = {};
    com.Id = committee.Id;
    com.InterviewBatch_Id = this.selectedBatch.Id;
    com.Name = committee.Name;
    com.Designation = committee.Designation;
    com.Role = committee.Role;
    com.Office = committee.Office;
    com.IsActive = false;
    this.interviewBatchCommittees = [];
    this._adhocService.saveAdhocInterviewBatchCommittee(com).subscribe((res: any) => {
      if (res) {
        this._adhocService.getAdhocInterviewBatchCommittee(this.selectedBatch.Id).subscribe((res: any[]) => {
          if (res) {
            this.interviewBatchCommittees = res;
          }
          committee.removing = false;
        }, err => {
          committee.removing = false;
          console.log(err);
        });
      }
    }, err => {
      console.log(err);
    });
  }
  public getScrutinyMinutes() {
    this._adhocService.getScrutinyMinutes(this.scrutinyMinutes).subscribe((res: any) => {
      if (res) {
        this.scrutinyMinutes = res;
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
  public readMinutesUrl(event: any, obj: any) {
    if (event.target.files && event.target.files[0]) {
      this.minutesFile = [];
      let inputValue = event.target;
      this.minutesFile = inputValue.files;
      if (obj) {
        obj.attached = true;
        obj.error = false;
      }
    }
  }
  public readGrievanceMinutesUrl(event: any, obj: any) {
    if (event.target.files && event.target.files[0]) {
      this.grievanceMinutesFile = [];
      let inputValue = event.target;
      this.grievanceMinutesFile = inputValue.files;
      if (obj) {
        obj.gattached = true;
        obj.gerror = false;
      }
    }
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
  }

  public getMeritVerifications() {
    this.meritVerificationsObj = { Done: 0, Pending: 0 };

    this.interviewBatchApplications.forEach(batchApp => {
      this._adhocService.getMeritVerification(batchApp.Id).subscribe((res: any[]) => {

        this._adhocService.getMeritVerificationAll(batchApp.Application_Id).subscribe((resAll: any[]) => {
          batchApp.meritVerificationsList = resAll;
          if (batchApp.meritVerificationsList && batchApp.meritVerificationsList.length > 0) {
            batchApp.hifzMeritVerifications = batchApp.meritVerificationsList.filter(x => x.DocId == 2);
            batchApp.experiences.forEach(expp => {
              expp.expMeritVerifications = batchApp.meritVerificationsList.filter(x => x.ExperienceId == expp.Id);
            });
            
          }

        }, err => {
          console.log(err);
        });

        if (res) {
          batchApp.verificationsRequired = 1;
          if (batchApp.Hafiz == true) {
            batchApp.verificationsRequired++;
          }
          batchApp.qualifications.forEach(e => batchApp.verificationsRequired++);
          batchApp.experiences.forEach(e => batchApp.verificationsRequired++);
          batchApp.verificationsDone = 0;
          let meritVerifications = res;
          batchApp.ageDone = meritVerifications.find(x => x.DocId == 1);
          if (batchApp.ageDone != null) {
            batchApp.verificationsDone++;
          }
          if (batchApp.Hafiz == true) {
            batchApp.hifzDone = meritVerifications.find(x => x.DocId == 2);
            if (batchApp.hifzDone != null) {
              batchApp.verificationsDone++;
            }
          }
          batchApp.qualifications.forEach(element => {
            element.meritVerification = meritVerifications.find(x => x.QualificationId == element.Id);
            if (element.meritVerification != null) {
              batchApp.verificationsDone++;
            }
          });
          batchApp.experiences.forEach(element => {
            element.meritVerification = meritVerifications.find(x => x.ExperienceId == element.Id);
            if (element.meritVerification != null) {
              batchApp.verificationsDone++;
            }
          });
          batchApp.verificationsCompleted = batchApp.verificationsDone >= batchApp.verificationsRequired;
          if (batchApp.verificationsCompleted == true) {
            this.meritVerificationsObj.Done++;
          }
          if (batchApp.verificationsCompleted == false) {
            this.meritVerificationsObj.Pending++;
          }
        }
      }, err => {
        console.log(err);
      });
    });
  }
  public getMeritVerification() {
    this.selectedBatchApplication.verificationsDone = 0;
    this._adhocService.getMeritVerification(this.selectedBatchApplication.Id).subscribe((res: any[]) => {
      if (res) {
        let meritVerifications = res;
        this.selectedBatchApplication.ageDone = meritVerifications.find(x => x.DocId == 1);
        if (this.selectedBatchApplication.ageDone != null) {
          this.selectedBatchApplication.verificationsDone++;
        }
        if (this.selectedBatchApplication.Hafiz == true) {
          this.selectedBatchApplication.hifzDone = meritVerifications.find(x => x.DocId == 2);
          if (this.selectedBatchApplication.hifzDone != null) {
            this.selectedBatchApplication.verificationsDone++;
          }
        }
        this.selectedBatchApplication.qualifications.forEach(element => {
          element.meritVerification = meritVerifications.find(x => x.QualificationId == element.Id);
          if (element.meritVerification != null) {
            this.selectedBatchApplication.verificationsDone++;
          }
        });
        this.selectedBatchApplication.experiences.forEach(element => {
          element.meritVerification = meritVerifications.find(x => x.ExperienceId == element.Id);
          if (element.meritVerification != null) {
            this.selectedBatchApplication.verificationsDone++;
          }
        });
        this.selectedBatchApplication.verificationsCompleted = this.selectedBatchApplication.verificationsDone >= this.selectedBatchApplication.verificationsRequired;
        if (this.selectedBatchApplication.verificationsCompleted == true) {
          this.meritVerificationsObj.Done++;
        }
        if (this.selectedBatchApplication.verificationsCompleted == false) {
          this.meritVerificationsObj.Pending++;
        }
      }
    }, err => {
      console.log(err);
    });
  }

  public viewApplicantMerit(obj) {
    this.selectedBatchApplication = obj;
    this.selectedBatchApplication.verificationsRequired = 1;
    if (this.selectedBatchApplication.Hafiz == true) {
      this.selectedBatchApplication.verificationsRequired++;
    }
    this.selectedBatchApplication.qualifications.forEach(element => {
      this.selectedBatchApplication.verificationsRequired++;
    });
    this.selectedBatchApplication.experiences.forEach(element => {
      this.selectedBatchApplication.verificationsRequired++;
    });
    this.getMeritVerification();
    this.getApplicantPMC();
    this.openMeritDialog();
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
  public changeApplicationStatus(statusId: number) {
    this.changingStatus = true;
    this.applicationLog.StatusId = statusId;
    this.applicationLog.Application_Id = this.application.Id;
    this._adhocService.changeApplicationStatus(this.applicationLog).subscribe((res) => {
      if (res) {
        this.closeScrutinyDialog();
      }
      this.changingStatus = false;
    }, err => {
      console.log(err);
      this.changingStatus = false;
    });

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
      }
      this.changingStatus = false;
    }, err => {
      console.log(err);
      this.changingStatus = false;
    });

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
  public openMeritDialog() {
    this.meritVerificationDialog = true;
  }
  public closeMeritDialog() {
    this.selectedBatchApplication = {};
    this.meritVerificationDialog = false;
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
    let html = document.getElementById('scrutinyMinutes').innerHTML;
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
  printGSApplication() {
    let html = document.getElementById('grievanceScrutinyMinutes').innerHTML;
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
  printAttendance() {
    let html = document.getElementById('attendance').innerHTML;
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
  printList() {
    let html = document.getElementById('list').innerHTML;
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