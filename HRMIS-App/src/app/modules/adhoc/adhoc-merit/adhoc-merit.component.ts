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
import { MeritList, MeritListCounts, MeritListValues } from "../../../_models/merit-list.class";
//import { ActivatedRoute } from '@angular/router';
//import { LocalService } from '../../../_services/local.service';
declare var moment: any;

@Component({
  selector: "app-adhoc-merit",
  templateUrl: "./adhoc-merit.component.html",
  styleUrls: ['./adhoc-merit.component.scss']
})
export class AdhocMeritComponent implements OnInit, OnDestroy {
  // @ViewChild("grid") public grid: GridComponent;
  @ViewChild('photoRef', { static: false }) public photoRef: any;
  public kGrid: KGridHelper = new KGridHelper();
  public inputChange: Subject<any>;
  public hfOpens: any[] = [];
  public vpProfileStatus: any[] = [];
  public hfsList: any[] = [];
  public jobHFs: any[] = [];
  public documents: any[] = [];
  public jobApplications: any[] = [];
  public user: any = {};
  public job: any = {};
  public jobHF: any = {};
  public jobDocument: any = {};
  public vpDetail: any[] = [];
  public vpDetailTotal: number = 0;
  public reason: any = {};
  public applicantLog: any = {};
  public selectedApplicant: any = {};
  public jobDocuments: any[] = [];
  public jobApplicants: any[] = [];
  public dateNow: string = '';
  public hfName: string = '';
  public running: boolean = false;
  public isAdmin: boolean = false;
  public loading: boolean = false;
  public changingStatus: boolean = false;
  public printing: boolean = false;
  public saving: boolean = false;
  public showHF: boolean = true;
  public printEnable: boolean = false;
  public decision: string = '';
  public messageLoading: string = '';
  private inputChangeSubscription: Subscription;
  public scrutinyComplete: boolean = false;
  public scrutinyDonwloaded: boolean = false;
  public scrutinyUpDonwloaded: boolean = false;
  public meritVerificationDialog: boolean = false;
  public searchingHfs: boolean = false;
  public preferencesWindow: boolean = false;
  public savingAdhocMerit: boolean = false;
  public calculating: boolean = false;
  public searchEvent = new Subject<any>();
  public searchSubcription: Subscription;
  public dropDowns: DropDownsHR = new DropDownsHR();
  public filters: any = {};
  public adhocInterview: any = {};
  public meritList: MeritList;
  public fontSize: number = 12;
  public count: number = 0;
  public totalCandidatesInterview: number = 0;
  public totalRejectedCandidatesInterview: number = 0;
  public totalAcceptedCandidatesInterview: number = 0;
  public districtCount: number = 0;
  public designationCount: number = 0;
  public currentDate: Date;
  public interview: any = {};
  public selectedBatchApplication: any = {};
  public interviewBatch: any[] = [];
  public interviewBatchCommittees: any[] = [];
  public interviewBatchApplications: any[] = [];
  public jobVacancy: any = {};
  public ApplicantInfo: any = {};
  public hfs: any[] = [];
  public photoFile: any[] = [];
  public hfsOrigional: any[] = [];
  public rejectReasons: any[] = [
    { Id: 1, Reason: 'PMDC document not valid' },
    { Id: 2, Reason: 'Hafiz-e-Quran certificate not valid' },
    { Id: 3, Reason: 'Qualification incomplete' },
    { Id: 4, Reason: 'Experience incomplete' },
  ];
  public designations: any[] = [];
  public designationsOrigional: any[] = [];
  constructor(
    private _adhocService: AdhocService,
    public _notificationService: NotificationService,
    public _rootService: RootService,
    private _authenticationService: AuthenticationService,
    private ngZone: NgZone
  ) { }

  public ngOnInit(): void {
    this.user = this._authenticationService.getUser();
    this.isAdmin = this.user.UserName == 'dpd' ? true : false;
    let today = new Date();
    let dd: any = today.getDate(), mm: any = today.getMonth() + 1, yyyy = today.getFullYear();
    if (dd < 10) dd = '0' + dd; if (mm < 10) mm = '0' + mm;
    this.dateNow = dd + '/' + mm + '/' + yyyy;

    this.kGrid.multiple = true;
    this.kGrid.loading = false;
    this.kGrid.pageSize = 122220;
    this.kGrid.pageSizes = [50, 100, 200, 300, 500, 1000];
    this.subscribeInputChange();
    this.getAdhocJobs();
    this.getDistricts();
  }
  private subscribeInputChange() {
    this.inputChange = new Subject();
    this.inputChangeSubscription = this.inputChange.pipe(debounceTime(300)).subscribe((query) => {
      if (query.length <= 2 && query.length != 0) {
        return;
      }
      this.filters.query = query;
    });
  }
  public getAdhocMerit = () => {
    this.kGrid.loading = true;
    this.messageLoading = 'Loading Merit List...';
    this.adhocInterview = {};
    this.kGrid.data = [];
    this.meritList = new MeritList();
    this.meritList.values = new MeritListValues();
    this.meritList.values.stats = new MeritListCounts();
    this.adhocInterview.AdhocInterviewBatchCommittees = {};
    this._adhocService.getAdhocMerit(this.filters.Designation_Id, this.filters.DistrictCode).subscribe((res: any) => {
      if (res) {
        this.kGrid.data = res.adhocMerits;
        this.adhocInterview = res.adhocInterview;
        let item = this.kGrid.data[0];
        if (item) {
          this._adhocService.getAdhocInterviewBatchCommittee(item.Batch_Id).subscribe((resrs: any[]) => {
            if (resrs && resrs.length > 0) {
              this.adhocInterview.AdhocInterviewBatchCommittees = resrs;
            }
          }, err => {
            console.log(err);
          });
        }
        let applicantQualifications = res.applicantQualifications;
        let applicantExperiences = res.applicantExperiences;
        this.totalCandidatesInterview = res.total;
        this.totalRejectedCandidatesInterview = res.rejected;
        this.totalAcceptedCandidatesInterview = res.accepted;
        if (this.kGrid.data && this.kGrid.data.length > 0) {
          this.messageLoading = `Loading Merit List of (${this.kGrid.data.length}}) loaded...`;
          this.kGrid.data.forEach(a => {
            let dob = moment(a.CurrentDate);
            let today = moment(a.DOB);
            let years = dob.diff(today, 'year');
            today.add(years, 'years');
            let months = dob.diff(today, 'months');
            today.add(months, 'months');
            let days = dob.diff(today, 'days');
            a.ddd = days;
            a.mmm = months;
            a.yyy = years;
            a.qualifications = applicantQualifications.filter(x => x.Applicant_Id == a.Applicant_Id);
            a.experiences = applicantExperiences.filter(x => x.Applicant_Id == a.Applicant_Id);
            this.checkMeritlistValues(a);
            this._adhocService.getMeritVerificationAll(a.Application_Id).subscribe((resAll: any[]) => {
              a.meritVerificationsList = resAll;
              if (a.meritVerificationsList && a.meritVerificationsList.length > 0) {
                a.hifzMeritVerifications = a.meritVerificationsList.filter(x => x.DocId == 2);
                a.experiences.forEach(expp => {
                  expp.expMeritVerifications = a.meritVerificationsList.filter(x => x.ExperienceId == expp.Id);
                });

              }

            }, err => {
              console.log(err);
            });
          });
          this.messageLoading = `Merit List Loaded`;
          this.kGrid.loading = false;
          this.calculating = false;
      /*     setTimeout(() => {
            //this.runAll();
          }, 2000); */
          //this.changeAdhocInterviewStatus();
        } else {
          this.messageLoading = `No Merit List Found!`;
          this.getAdhocInterview();
        }

      }
    },
      err => {
        console.log(err);
        this.loading = false;
      }
    );
  }
  public checkMeritlistValues(a: any) {
    if (this.meritList) {
      a.MatricMarks_Id ? this.meritList.values.stats.matricCounts++ : this.meritList.values.stats.matricErrors++;
      a.InterMarks_Id ? this.meritList.values.stats.interCounts++ : this.meritList.values.stats.interErrors++;
      a.MasterMarks_Id ? this.meritList.values.stats.gradCounts++ : this.meritList.values.stats.gradErrors++;
      a.FirstHigherMarks_Id ? this.meritList.values.stats.firstHigherCounts++ : '';
      a.SecondHigherMarks_Id ? this.meritList.values.stats.secondHigherCounts++ : '';
      a.ThirdHigherMarks_Id ? this.meritList.values.stats.thirdHigherCounts++ : '';
      a.FirstPositionMarks_Id ? this.meritList.values.stats.firstPositionCounts++ : '';
      a.SecondPositionMarks_Id ? this.meritList.values.stats.secondPositionCounts++ : '';
      a.ThirdPositionMarks_Id ? this.meritList.values.stats.thirdPositionCounts++ : '';
      a.OneYearExp ? this.meritList.values.stats.oneYearExpCounts++ : '';
      a.TwoYearExp ? this.meritList.values.stats.twoYearExpCounts++ : '';
      a.ThreeYearExp ? this.meritList.values.stats.threeYearExpCounts++ : '';
      a.FourYearExp ? this.meritList.values.stats.fourYearExpCounts++ : '';
      a.FivePlusYearExp ? this.meritList.values.stats.fivePlusYearExpCounts++ : '';
      a.Hafiz && a.Hafiz == 5 ? this.meritList.values.stats.hufazCounts++ : '';
      a.Interview ? this.meritList.values.stats.oneMarksinterviewCounts++ : '';
      this.meritList.values.stats.errors += (a.MatricMarks_Id && a.InterMarks_Id && a.MasterMarks_Id) ? 0 : 1;
    }
  }
  public readUrl(event: any, obj: any) {
    if (event.target.files && event.target.files[0]) {
      this.adhocInterview.photoFile = [];
      let inputValue = event.target;
      this.adhocInterview.photoFile = inputValue.files;
      if (obj) {
        obj.attached = true;
        obj.error = false;
      }
    }
  }
  public uploadMeritList() {
    if (this.adhocInterview.photoFile.length > 0) {
      this.adhocInterview.uploading = true;
      this._adhocService.uploadMeritList(this.adhocInterview.photoFile, this.adhocInterview.Id).subscribe((res4: any) => {
        this.adhocInterview.uploading = false;
        this.adhocInterview.photoFile = [];
      }, err => {
        console.log(err);
        this.adhocInterview.uploading = false;
      });
    }
  }
  public getAdhocInterview() {

    this.kGrid.loading = true;
    this.messageLoading = `Checking Interviews!`;
    this.adhocInterview = {};
    this.interviewBatch = [];
    this._adhocService.getAdhocInterview(this.filters.DistrictCode, this.filters.Designation_Id).subscribe((adhocInterview: any) => {
      if (adhocInterview && adhocInterview.Id > 0) {
        this.adhocInterview = adhocInterview;
        let batchIdForCommittee = 0;

        this.messageLoading = `Interview Found!`;
        this.messageLoading = `Calculating Merit...`;
        this._adhocService.setAdhocMerit(adhocInterview.Id).subscribe((aaaaa: any) => {
          this.messageLoading = `Merit Calculated.`;
          this._adhocService.getAdhocInterviewBatches(adhocInterview.Id).subscribe((batches: any[]) => {
            if (batches && batches.length > 0) {
              this.interviewBatch = batches;
              this.interviewBatchApplications = [];
              this.count = 0;
              let c = 0;
              this._rootService.getCurrentDate().subscribe((currentDate: any) => {
                currentDate = new Date(currentDate);
                this.interviewBatch.forEach(batch => {
                  if (batchIdForCommittee == 0) {
                    batchIdForCommittee = batch.Id;
                    this._adhocService.getAdhocInterviewBatchCommittee(batchIdForCommittee).subscribe((resrs: any[]) => {
                      if (resrs && resrs.length > 0) {
                        this.adhocInterview.AdhocInterviewBatchCommittees = resrs;
                      }
                    }, err => {
                      console.log(err);
                    });
                  }
                  if (!batch.IdTemp) {
                    this._adhocService.getAdhocInterviewBatchApplications(batch.Id).subscribe((applications: any[]) => {
                      c++;
                      if (applications) {
                        this.interviewBatchApplications = [...this.interviewBatchApplications, ...applications];
                        if (c == this.interviewBatch.length) {
                          this.messageLoading = `Loading Merits....`;
                          this._adhocService.getAdhocDistrictMerit(this.filters.Designation_Id, this.filters.DistrictCode).subscribe((resss: any) => {
                            this.messageLoading = `....`;
                            let marks = resss.marks;
                            let applicantQualifications = resss.applicantQualifications;
                            let applicantExperiences = resss.applicantExperiences;
                            if (marks) {
                              this.interviewBatchApplications = this.interviewBatchApplications.filter(x => x.IsPresent == true && x.IsLocked == true && !x.IsRejected);
                              this.interviewBatchApplications.forEach(a => {
                                a.documents = [];
                                let dob = moment(currentDate);
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
                                a.interMerit = a.marks.find(x => x.Marks_Id == 2);
                                a.grad = a.marks.find(x => x.Marks_Id == 16);

                                /*  if(a.grad){
                                   let mbbs1 = a.qualifications.filter(x => x.Required_Degree_Id == 62);
                                   let mbbs2 = a.qualifications.filter(x => x.Required_Degree_Id == 116);
                                   let mbbs3 = a.qualifications.filter(x => x.Required_Degree_Id == 117);
                                   let mbbs4 = a.qualifications.filter(x => x.Required_Degree_Id == 118);
                                   let mbbs5 = a.qualifications.filter(x => x.Required_Degree_Id == 119);
                                   
                                   a.grad.TotalMarks = (mbbs1 ? mbbs1.TotalMarks : 0) + (mbbs2 ? mbbs2.TotalMarks : 0) + (mbbs3 ? mbbs3.TotalMarks : 0) +
                                     (mbbs4 ? mbbs4.TotalMarks : 0) + (mbbs5 ? mbbs5.TotalMarks : 0);
     
                                   a.grad.ObtainedMarks = (mbbs1 ? mbbs1.ObtainedMarks : 0) + (mbbs2 ? mbbs2.ObtainedMarks : 0) + (mbbs3 ? mbbs3.ObtainedMarks : 0) +
                                     (mbbs4 ? mbbs4.ObtainedMarks : 0) + (mbbs5 ? mbbs5.ObtainedMarks : 0);
     
                                     console.log(a.grad);
                                     console.log(a.grad.ObtainedMarks);
                                 }
                             */


                                a.master = a.marks.find(x => x.Marks_Id == 16);
                                a.firstStageMerit = a.marks.find(x => x.Marks_Id == 6);
                                a.secondStageMerit = a.marks.find(x => x.Marks_Id == 7);
                                a.thirdStageMerit = a.marks.find(x => x.Marks_Id == 8);
                                a.hifzMerit = a.marks.find(x => x.Marks_Id == 9);
                                a.expMerit = a.marks.find(x => x.Marks_Id == 10);
                                if (a.Application_Id == 7979) {
                                  console.log(a.expMerit);
                                  console.log(a.marks);
                                }
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
                            this.kGrid.loading = false;
                            console.log(err);
                          });
                        }
                      }
                    }, err => {
                      this.kGrid.loading = false;
                      console.log(err);
                    });
                  } else {
                    c++;
                  }
                });
              }, err => {
                this.kGrid.loading = false;
                console.log(err);
              });
            }
            if (this.interview.Batches != this.interviewBatch.length) {
            }
          }, err => {
            this.kGrid.loading = false;
            console.log(err);
          });
        }, err => {
          this.kGrid.loading = false;
          console.log(err);
        });

      } else {
        this.kGrid.loading = false;
      }
    }, err => {
      this.kGrid.loading = false;
      console.log(err);
    });
  }
  public getMeritVerifications() {
    this.messageLoading = `....`;
    let co = 0;
    this.interviewBatchApplications.forEach(batchApp => {
      this.messageLoading = `....`;
      co++;
      this.saveAdhocMerit(co, batchApp);
      this._adhocService.getMeritVerificationAll(batchApp.Application_Id).subscribe((resAll: any[]) => {
        batchApp.meritVerificationsList = resAll;
        if (batchApp.meritVerificationsList && batchApp.meritVerificationsList.length > 0) {
          batchApp.hifzMeritVerifications = batchApp.meritVerificationsList.filter(x => x.DocId == 2);
          batchApp.experiences.forEach(expp => {
            console.log(expp);
            expp.expMeritVerifications = batchApp.meritVerificationsList.filter(x => x.ExperienceId == expp.Id);
          });
        }
      }, err => {
        console.log(err);
      });
      this._adhocService.getMeritVerification(batchApp.Id).subscribe((res: any[]) => {
        if (res) {
          this.messageLoading = `....`;
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
          }
          if (batchApp.verificationsCompleted == false) {
          }

        }
      }, err => {
        console.log(err);
      });
    });
  }
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
  public saveAdhocMerit(number: number, a: any) {
    this.messageLoading = `....`;
    this.savingAdhocMerit = true;
    let merit: any = {};

    merit.MeritNumber = number;
    merit.BatchApplication_Id = a.Id;
    merit.Applicant_Id = a.Applicant_Id;
    merit.Application_Id = a.Application_Id;
    merit.DistrictCode = this.filters.DistrictCode;
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

    if (this.filters.Designation_Id == 431) {
      merit.MasterTotal = ((bds1 ? bds1.TotalMarks : 0) + (bds2 ? bds2.TotalMarks : 0) + (bds3 ? bds3.TotalMarks : 0) +
        (bds4 ? bds4.TotalMarks : 0)).toFixed();
      merit.MasterObtained = ((bds1 ? bds1.ObtainedMarks : 0) + (bds2 ? bds2.ObtainedMarks : 0) + (bds3 ? bds3.ObtainedMarks : 0) +
        (bds4 ? bds4.ObtainedMarks : 0)).toFixed();
    } else if (this.filters.Designation_Id == 302) {
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
    merit.MasterPercent = a.grad ? a.grad.Percentage.toFixed(2) : null;
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
        this.messageLoading = `....`;
        this.savingAdhocMerit = false;
        this.closeMeritDialog();
        console.log(number);
        if (number == this.interviewBatchApplications.length) {
          this.getAdhocMerit();
        }
      }
    }, err => {
      this.savingAdhocMerit = false;
      this.kGrid.loading = false;
      console.log(err);
    });
  }
  public getAdhocVacantDesignations() {
    this.jobVacancy = {};
    this._adhocService.getAdhocVacantDesignations('facilities', this.filters.Designation_Id).subscribe((res: any) => {
      if (res) {
        this.jobVacancy = res;

        this.jobVacancy.districts;
        console.log(this.jobVacancy);

        this.vpDetail = this.jobVacancy.districts.filter(x => x.Code == this.filters.DistrictCode);
        console.log(this.vpDetail);
        this.vpDetailTotal = 0;
        this.vpDetail.forEach(vp => {
          if (vp) {
            if (vp.DesignationId == 2404) {
              this.vpDetailTotal += (vp.Vacant % 2) == 1 ? ((vp.Vacant - 1) / 2) : (vp.Vacant / 2);
            } else {
              this.vpDetailTotal += vp.Vacant
            }
          }
        });

        /*  this.districts.forEach(dist => {
           dist.hfs = this.hfsOrigional.filter(x => x.HFMISCode.startsWith(dist.Code))
         }); */
      }
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
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
    meritVerification.DistrictCode = this.filters.DistrictCode;
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
    console.log(this.selectedBatchApplication);
    this.getMeritVerification();
    this.getApplicantPMC();
    this.openMeritDialog();
  }
  public openMeritDialog() {
    this.meritVerificationDialog = true;
  }
  public closeMeritDialog() {
    this.selectedBatchApplication = {};
    this.meritVerificationDialog = false;
  }
  public changeAdhocInterviewStatus = () => {
    this.changingStatus = true;
    if (confirm('Are you sure merit list is finalized? Once locked, it cannot be changed! Please Confirm.')) {
      this._adhocService.changeAdhocInterviewStatus(this.adhocInterview.Id, 5).subscribe((res: any) => {
        if (res == true) {
          this.adhocInterview.Status_Id = 5;
        }
        this.changingStatus = false;
       /*  setTimeout(() => {
          this.runAll();
        }, 2000); */
      },
        err => {
          console.log(err);
          this.changingStatus = false;
          this.loading = false;
        }
      );
    } else {
      this.changingStatus = false;
    }
  }
  public runAll = () => {
    this.running = true;
    if (this.designationCount < this.dropDowns.adhocJobsData.length) {
      if (this.districtCount >= 0 && this.districtCount < 36) {
        this.dropDowns.selectedFiltersModel.adhocJob = this.dropDowns.adhocJobsData[this.designationCount];
        this.dropDowns.selectedFiltersModel.district = this.dropDowns.districts[this.districtCount];
        console.log(this.dropDowns.selectedFiltersModel.adhocJob);
        console.log(this.dropDowns.selectedFiltersModel.district);
        if (this.dropDowns.selectedFiltersModel.adhocJob && this.dropDowns.selectedFiltersModel.district) {
          this.filters.Designation_Id = this.dropDowns.selectedFiltersModel.adhocJob.Designation_Id;
          this.filters.DistrictCode = this.dropDowns.selectedFiltersModel.district.Code;
          if (this.filters.Designation_Id && this.filters.Designation_Id > 0 && this.filters.DistrictCode) {
            this.getAdhocMerit();
          }
        }
      }
      else if (this.districtCount == 36) {
        this.districtCount = -1;
        if (this.designationCount < this.dropDowns.adhocJobsData.length) {
          this.designationCount++;
        }
      }
    }
    this.districtCount++;
    if (this.designationCount >= this.dropDowns.adhocJobsData.length && this.districtCount >= 36) {
      this.running = false;
    }
  }
  private getAdhocJobs = () => {
    this._adhocService.getAdhocJobs().subscribe((res: any) => {
      if (res) {
        this.dropDowns.adhocJobs = res;
        this.dropDowns.adhocJobsData = this.dropDowns.adhocJobs;
      }
    },
      err => {
        console.log(err);
        this.loading = false;
      }
    );
  }
  private getDistricts = () => {
    this.dropDowns.districts = [];
    this._rootService.getDistricts(this.user.HfmisCode).subscribe((res: any) => {
      this.dropDowns.districts = res;
      if (this.dropDowns.districts.length == 1) {
        this.dropDowns.selectedFiltersModel.district = this.dropDowns.districts[0];
        this.filters.DistrictCode = this.user.HfmisCode;
      }
    },
      err => { console.log(err); }
    );
  }

  public openInNewTab(link: string) {
    window.open(link, '_blank');
  }
  public dropdownValueChanged = (value, filter) => {
    if (filter == 'designation') {
      this.filters.Designation_Id = value.Designation_Id;
    }
    if (filter == 'district') {
      this.filters.DistrictCode = value.Code;
    }
    if (this.filters.Designation_Id && this.filters.Designation_Id > 0 && this.filters.DistrictCode) {
      this.getAdhocMerit();
      this.getAdhocVacantDesignations();
    }
  }
  public changeFontSize() {
    this.fontSize = this.fontSize == 8 ? 10 : this.fontSize == 10 ? 11 : this.fontSize == 11 ? 12 : this.fontSize == 12 ? 8 : 0;
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

  public getAdhocApplicant(applicantId: number) {
    this._adhocService.getAdhocApplicant(this.selectedApplicant.Id).subscribe((res: any) => {
      if (res) {
        this.ApplicantInfo = res;
        console.log('ApplicantInfoAd: ', this.ApplicantInfo);
      }
    }, err => {
      console.log(err);
    });
  }

  ngOnDestroy() {
    this.inputChangeSubscription.unsubscribe();
  }
  public uploadBtn() {
    this.scrutinyUpDonwloaded = true;
    this.photoRef.nativeElement.click();
  }

  public numberWithCommas(x) {
    if (x) {
      return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }
  }

  printApplication() {
    this.printing = true;
    this.printEnable = true;
    let css = '@page { size: landscape; }';
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

    setTimeout(() => {
      this.printing = false;
      window.print();
    }, 1500);
    return;
  }
}
