import { Component, OnDestroy, OnInit } from '@angular/core';
import { KGridHelper } from '../../../_helpers/k-grid.class';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { Subject } from 'rxjs/Subject';
import { NotificationService } from '../../../_services/notification.service';
import { RootService } from '../../../_services/root.service';
import { FileTrackingSystemService } from '../../../file-tracking-system/file-tracking-system.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { orderBy, SortDescriptor } from '@progress/kendo-data-query';
import { PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subscription } from 'rxjs/Subscription';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { DomSanitizer } from '@angular/platform-browser';
import { AdhocService } from '../adhoc.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-adhoc-posting',
  templateUrl: './adhoc-posting.component.html',
  styleUrls: ['./adhoc-posting.component.scss']
})
export class AdhocPostingComponent implements OnInit, OnDestroy {

  public kGrid: KGridHelper = new KGridHelper();
  public dropDowns: DropDownsHR = new DropDownsHR();
  public inputChange: Subject<any>;
  public skipChange: Subject<any>;
  public searchQuery: string = '';
  public totalPreferences: number = 0;
  public ActiveDesignationId: number = 68;
  public designationId: number = 1320;
  public selectedJobId: number = 0;
  public statusId: number = 0;
  public interviewMarks: number;
  public currentUser: any;
  public filters: any = {};
  public user: any = {};
  public meritPosting: any = {};
  public esr: any = {};
  public profile: any = {};
  public selectedApplicant: any;
  public selectedApplicantPreferences: any[] = [];
  public hfOpened: any[] = [];
  public selectedHF: any = {};
  public applicant: any = {};
  public meritMarks: any[] = [];
  public interviewVerifications: any[] = [];
  public application: any = {};
  public pmcVerification: any;
  public pmcQualification: any;
  public job: any = {};
  public ApplicantInfo: any = {};
  public actualPostingHF_Id: number = 0;
  public actualPostingHFMISCode: string = '0';
  public selectedVacancy: any[] = [];
  public jobBatches: any[] = [];
  public designations: any[] = [];
  public documents: any[] = [];
  public designationsData: any[] = [];
  public applications: any[] = [];
  public leaveRecord: any[] = [];
  public qualifications: any[] = [];
  public serviceHistory: any[] = [];
  public jobApplicants: any[] = [];
  public jobApplications: any[] = [];
  private inputChangeSubscription: Subscription;
  private skipChangeSubscription: Subscription;
  public subscription: Subscription;

  public showFilters: boolean = true;
  public scrutinyDone: boolean = false;
  public scrutinyComplete: boolean = false;
  public saving: boolean = false;
  public rejecting: boolean = false;
  public absenting: boolean = false;
  public locking: boolean = false;
  public reseting: boolean = false;
  public positioning: boolean = false;
  public presenting: boolean = false;
  public scrutinyRejected: boolean = false;
  public loadingPreferences: boolean = false;
  public showOrder: boolean = false;
  public loadingVacancy: boolean = false;
  public noVacancy: boolean = false;
  public savingOrder: boolean = false;
  public showVacancy: boolean = false;
  public preferencesWindow: boolean = false;
  public searchingHfs: boolean = false;
  public checkingVacancy: boolean = false;
  public downloadingAcceptanceLetter: boolean = false;
  public acceptanceLetterDownloaded: boolean = false;
  public downloadingOfferLetter: boolean = false;
  public positionSaved: boolean = false;
  public loadingApplications: boolean = false;
  public loadingApplication: boolean = false;
  public verificationComplete: boolean = false;
  public rejectedVerification: boolean = false;
  public specialQuota: boolean = false;
  public view: string = 'grid';
  public dateNow: string = '';
  public joiningDate: string = '';
  public offerDate: string = '';
  public imgSrc: string = '';
  public offerMonth: string = '';
  public searchEvent = new Subject<any>();
  public searchSubcription: Subscription = null;
  public adhocInterview: any = {};
  public interview: any = {};
  public interviewBatches: any[] = [];
  public adhocInterviews: any[] = [];
  public interviewBatchCommittees: any[] = [];
  public selectedBatch: any = {};
  public interviewBatchApplication: any = {};
  public interviewBatchCommittee: any = {};
  public interviewBatchApplications: any[] = [];
  constructor(private sanitized: DomSanitizer, public _notificationService: NotificationService,
    private _rootService: RootService,
    private route: ActivatedRoute,
    private _adhocService: AdhocService,
    private _authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.user = this._authenticationService.getUser();
    this.kGrid.pageSizes = [5, 10, 25, 50];
    this.kGrid.totalRecords = 0;
    this.kGrid.pageSize = 100;
    this.filters.Skip = 0;
    this.filters.PageSize = this.kGrid.pageSize;
    this.filters.hfmisCode = this.user.HfmisCode;
    let today = new Date();
    let dd: any = today.getDate(), mm: any = today.getMonth() + 1, yyyy = today.getFullYear();
    if (dd < 10) dd = '0' + dd; if (mm < 10) mm = '0' + mm;
    this.dateNow = dd + '/' + mm + '/' + yyyy;
    this.interview.DistrictCode = this.user.HfmisCode;
    this.dropDowns.candidateStatus = [
      { Id: 5, Name: 'Pending' },
      { Id: 1, Name: 'Present' },
      { Id: 2, Name: 'Absent' },
      { Id: 3, Name: 'Marks Added' },
      { Id: 4, Name: 'Rejected' },
    ];
    this.getAdhocInterviews();
    this.fetchParams();
    this.subscribeInputChange();
  }
  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('desigId')) {
          this.filters.Designation_Id = +params['desigId'];
          this.getAdhocInterview();
        }
      }
    );
  }
  private subscribeInputChange() {
    this.inputChange = new Subject();
    this.inputChangeSubscription = this.inputChange.pipe(debounceTime(300)).subscribe((query) => {
      this.loadingApplications = true;
      this.filters.query = query;
      this._adhocService.searchAdhocInterviewBatchApplications({ batchId: this.selectedBatch.Id, Query: this.filters.query }).subscribe((applications: any[]) => {
        if (applications) {
          this.selectedBatch.Applications = applications;
        }
        this.loadingApplications = false;
      }, err => {
        console.log(err);
        this.loadingApplications = false;
      });
    });
  }
  public searchAdhocInterviewBatchApplications(statusId: number) {
    this.applicant = {};
    this.loadingApplications = true;
    this._adhocService.searchAdhocInterviewBatchApplications({ batchId: this.selectedBatch.Id, Status_Id: statusId }).subscribe((applications: any[]) => {
      if (applications) {
        this.selectedBatch.Applications = applications;
      }
      this.loadingApplications = false;
    }, err => {
      console.log(err);
      this.loadingApplications = false;
    });
  }
  public getAdhocApplication = () => {
    this.kGrid.loading = true;
    this.kGrid.data = [];
    let obj = {
      Skip: this.kGrid.skip,
      PageSize: this.kGrid.pageSize,
      applicantId: 0,
      hfmisCode: this.user.HfmisCode,
      Query: this.filters.query,
      Designation_Id: this.filters.Designation_Id,
      BatchId: this.filters.BatchId
    };
    this._adhocService.getAdhocJobApplications(obj).subscribe((res: any) => {
      if (res) {
        this.kGrid.data = res.List;
        this.kGrid.totalRecords = res.Count;
        this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords };
        this.kGrid.loading = false;
      }
      console.log(this.kGrid.data);
    },
      err => {
        console.log(err);
        this.kGrid.loading = false;
      }
    );
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
  private getAdhocJobs = () => {
    this._adhocService.getAdhocJobs().subscribe((res: any) => {
      if (res) {
        this.dropDowns.adhocJobs = res;
        this.dropDowns.adhocJobsData = this.dropDowns.adhocJobs;
      }
    },
      err => {
        console.log(err);
      }
    );
  }

  public getAdhocApplicationApplicant() {
    this._adhocService.getAdhocApplicationApplicant(this.applicant.Id, this.application.Id).subscribe((res: any) => {
      if (res) {
        this.applicant = res.applicant;
        this.applicant.PositionHolder = "No";
        this.application = res.application;
        this.getApplicationData();
      }
    }, err => {
      console.log(err);
    });
  }

  public getApplicationData() {
    this.documents = [];
    let doc: any = { Id: 1, Name: 'CNIC', UploadPath: this.applicant.CNICDoc };
    this.documents.push(doc);
    doc = { Id: 2, Name: 'Domicile', SubName: this.applicant.DomicileName, UploadPath: this.applicant.DomicileDoc };
    this.documents.push(doc);
    if (this.applicant.Hafiz) {
      doc = { Id: 4, Name: 'Hafiz-e-Quran', UploadPath: this.applicant.HifzDocument };
      this.documents.push(doc);
    }
    doc = {
      Id: 3,
      Name: this.application.Desgination_Id == 302 ? 'PNC' : 'PMC', Valid: this.applicant.PMDCValidUpto, PMDCNumber: this.applicant.PMDCNumber,
      UploadPath: this.applicant.PMDCDoc
    };
    this.documents.push(doc);
    this.applicant.Qualifications = [];
    this.applicant.ApplicantExperiences = [];
    this.loadingApplication = false;
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
        this.getAdhocInterviewVerifications();
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
    this.applicant.Preferences = [];
    this._adhocService.getApplicationPref(this.applicant.Id).subscribe((x: any) => {
      if (x) {
        this.applicant.Preferences = x;
      }
    }, err => {
      console.log(err);
    });
  }
  public getAdhocInterviewBatchApplications(item) {
    this.loadingApplications = true;
    this.positionSaved = false;
    this.selectedBatch = {};
    this.selectedBatch = item;
    this.applicant = {};
    if (this.selectedBatch.Id) {
      this._adhocService.getAdhocInterviewBatchCommittee(this.selectedBatch.Id).subscribe((res: any[]) => {
        if (res && res.length > 0) {
          this.selectedBatch.AdhocInterviewBatchCommittees = res;
        }
      }, err => {
        console.log(err);
      });
      this._adhocService.getAdhocInterviewBatchApplications(this.selectedBatch.Id).subscribe((applications: any[]) => {
        if (applications && applications.length > 0) {
          this.selectedBatch.Applications = applications;
          let present = 0, absent = 0, marksadded = 0, rejected = 0, pending = 0, pendingLock = 0, locked = 0;
          this.selectedBatch.Applications.forEach(app => {
            if (app.IsPresent == true) {
              present++;
            }
            if (app.IsPresent == false) {
              absent++;
            }
            if (app.InterviewMarks > 0) {
              marksadded++;
            }
            if (app.IsRejected == true) {
              rejected++;
            }
            if (app.IsPresent == null) {
              pending++;
            }
            if (!app.IsLocked) {
              pendingLock++;
            }
            if (app.IsLocked) {
              locked++;
            }
          });
          this.dropDowns.candidateStatus = [
            { Id: 5, Name: 'Pending (' + pending + ')' },
            { Id: 1, Name: 'Present (' + present + ')' },
            { Id: 2, Name: 'Absent (' + absent + ')' },
            { Id: 3, Name: 'Marks Added (' + marksadded + ')' },
            { Id: 4, Name: 'Rejected (' + rejected + ')' },
            { Id: 6, Name: 'Locked (' + locked + ')' },
            { Id: 7, Name: 'Lock Pending (' + pendingLock + ')' },
          ];
        }
        this.loadingApplications = false;
      }, err => {
        console.log(err);
        this.loadingApplications = false;
      });
    }
  }
  public getAdhocInterviews() {
    this.adhocInterviews = [];
    this._adhocService.getAdhocInterviews(this.user.HfmisCode).subscribe((res: any) => {
      if (res) {
        this.adhocInterviews = res;
      }
    }, err => {
      console.log(err);
    });
  }
  public getAdhocInterview() {
    this.adhocInterview = {};
    this.interviewBatches = [];
    this._adhocService.getAdhocInterview(this.user.HfmisCode, this.filters.Designation_Id).subscribe((res: any) => {
      if (res && res.Id > 0) {
        this.adhocInterview = res;
        this.interview = {};
        this.interview = this.adhocInterview;
        this.dropDowns.selectedFiltersModel.designationInterview = this.adhocInterview;
        this.interview.DesignationName = this.job.DesignationName;
        this._adhocService.getAdhocInterviewBatches(this.adhocInterview.Id).subscribe((batches: any[]) => {
          if (batches && batches.length > 0) {
            this.interviewBatches = batches;
          }
        }, err => {
          console.log(err);
        });
      }
    }, err => {
      console.log(err);
    });
  }
  public getAdhocJob() {
    this._adhocService.getAdhocJob(this.filters.Designation_Id).subscribe((res: any) => {
      if (res) {
        this.job = res.job;
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
  public changeStatus(item, s) {

  }
  public checkScrutiny() {
    this.documents.forEach(doc => {
      doc.scrutiny = this.applicant.AdhocScrutinies.find(x => x.DocName == doc.Name);
    });
    this.applicant.Qualifications.forEach(qualification => {
      qualification.scrutiny = this.applicant.AdhocScrutinies.find(x => x.Qualification_Id == qualification.Id);
    });
    this.applicant.ApplicantExperiences.forEach(experience => {
      experience.scrutiny = this.applicant.AdhocScrutinies.find(x => x.Experience_Id == experience.Id && x.IsAccepted == true);
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
  public verifyPMC() {
    if (this.applicant.PMDCNumber && !this.applicant.PMDCNumber.trim().toLowerCase().endsWith('-p')) {
      if (!this.applicant.PMDCNumber.toLowerCase().endsWith('-d')) {
        this.applicant.PMDCNumber = this.applicant.PMDCNumber + '-P';
      }
    }
    this._adhocService.verifyPMC(this.applicant.PMDCNumber).subscribe((res) => {
      this.pmcVerification = res;
      // this.openScrutinyDialog();
    }, err => {
      console.log(err);
    });
    this._adhocService.verifyPMCGetQualifications(this.applicant.PMDCNumber).subscribe((res) => {
      this.pmcQualification = res;
    }, err => {
      console.log(err);
    });
  }
  public getAdhocInterviewVerifications() {
    this._adhocService.getAdhocInterviewVerifications(this.application.Id, this.interviewBatchApplication.Id).subscribe((res: any[]) => {
      this.interviewVerifications = res;
      this.interviewVerifications.forEach(iv => {
        if (iv.Doc_Id > 0) {
          let doc = this.documents.find(x => x.Id == iv.Doc_Id);
          doc.verification = iv;
        }
        if (iv.Qualification_Id > 0) {
          let qual = this.applicant.Qualifications.find(x => x.Id == iv.Qualification_Id);
          qual.verification = iv;
        }
        if (iv.Experience_Id > 0) {
          let exp = this.applicant.ApplicantExperiences.find(x => x.Id == iv.Experience_Id);
          exp.verification = iv;
        }
      });
      this.verificationComplete = this.interviewVerifications.length >= this.documents.length + this.applicant.Qualifications.length;
      let rejected = this.interviewVerifications.filter(x => x.Verified == false && x.Qualification_Id != null);
      if (rejected && rejected.length > 0) {
        this.rejectedVerification = true;
      }
      if (rejected && rejected.length == 0) {
        this.rejectedVerification = false;
      }
    }, err => {
      console.log(err);
    });
  }
  public getAdhocInterviewBatchApplication() {
    this._adhocService.getAdhocInterviewBatchApplication(this.interviewBatchApplication.Id).subscribe((res: any) => {
      this.interviewBatchApplication = res;
      if (this.interviewBatchApplication.PositionHolder != null) {
        this.positionSaved = true;
      } else {
        this.positionSaved = false;
      }
    }, err => {
      console.log(err);
    });
  }
  public saveAdhocInterviewBatchApplication() {
    this.saving = true;
    this.interviewBatchApplication.InterviewMarks = this.interviewMarks;
    this.interviewBatchApplication.IsPresent = true;
    this._adhocService.saveAdhocInterviewBatchApplication(this.interviewBatchApplication).subscribe((res: any) => {
      this.interviewBatchApplication = res;
      this.getAdhocInterviewBatchApplication();
      this.saving = false;
    }, err => {
      console.log(err);
    });
  }
  public absentAdhocInterviewBatchApplication() {
    this.absenting = true;
    this.interviewBatchApplication.IsPresent = false;
    this._adhocService.saveAdhocInterviewBatchApplication(this.interviewBatchApplication).subscribe((res: any) => {
      this.interviewBatchApplication = res;
      if (this.interviewBatchApplication.PositionHolder != null) {
        this.positionSaved = true;
      } else {
        this.positionSaved = false;
      }
      this.getAdhocInterviewVerifications();
      this.absenting = false;
    }, err => {
      console.log(err);
    });
  }
  public resetAdhocInterviewBatchApplication() {
    this.reseting = true;
    this.interviewBatchApplication.IsActive = false;
    this._adhocService.saveAdhocInterviewBatchApplication(this.interviewBatchApplication).subscribe((res: any) => {
      this._adhocService.getAdhocInterviewBatchApplication(this.interviewBatchApplication.Id).subscribe((res: any) => {
        this.interviewBatchApplication = res;
        this.interviewMarks = null;
        this.verificationComplete = false;
        this.positionSaved = false;
        this.interviewBatchApplication.PositionHolder = null;
        this.getAdhocInterviewVerifications();
        this.reseting = false;
      }, err => {
        console.log(err);
      });



    }, err => {
      console.log(err);
    });
  }
  public lockAdhocInterviewBatchApplication() {
    this.locking = true;
    this.interviewBatchApplication.IsLocked = true;
    this._adhocService.saveAdhocInterviewBatchApplication(this.interviewBatchApplication).subscribe((res: any) => {
      this.interviewBatchApplication = res;
      this.getAdhocInterviewBatchApplication();
      this.locking = false;
    }, err => {
      console.log(err);
    });
  }
  public positionAdhocInterviewBatchApplication() {
    this.positioning = true;
    if (this.interviewBatchApplication.PositionHolder == 'Yes') {
      this.interviewBatchApplication.PositionHolder = true;
    } else if (this.interviewBatchApplication.PositionHolder == 'No') {
      this.interviewBatchApplication.PositionHolder = false;
    }
    if (this.interviewBatchApplication.PositionHolder == true && !this.interviewBatchApplication.PositionDoc && this.interviewBatchApplication.photoFile && this.interviewBatchApplication.photoFile.length > 0) {
      this._adhocService.uploadApplicantPositionDoc(this.interviewBatchApplication.photoFile, this.interviewBatchApplication.Applicant_Id).subscribe((res4: any) => {
        this.interviewBatchApplication.photoFile = [];
        this._adhocService.saveAdhocInterviewBatchApplication(this.interviewBatchApplication).subscribe((res: any) => {
          this.interviewBatchApplication = res;
          this.getAdhocInterviewBatchApplication();
          this.positionSaved = true;
          this.positioning = false;
        }, err => {
          console.log(err);
        });
      }, err => {
        console.log(err);
      });
    } else {
      this._adhocService.saveAdhocInterviewBatchApplication(this.interviewBatchApplication).subscribe((res: any) => {
        this.interviewBatchApplication = res;
        this.getAdhocInterviewBatchApplication();
        this.positionSaved = true;
        this.positioning = false;
      }, err => {
        console.log(err);
      });
    }

  }
  public changeAdhocApplicantQualification(obj: any) {
    obj.savings = true;
    this._adhocService.changeAdhocApplicantQualification(obj).subscribe((res: any) => {
      this._adhocService.getApplicantDocuments(this.applicant.Id).subscribe((x: any) => {
        if (x) {
          this.applicant.Qualifications = x;
          this.checkScrutiny();
        }
      }, err => {
        console.log(err);
      });
    }, err => {
      console.log(err);
    });
  }
  public presentAdhocInterviewBatchApplication() {
    this.presenting = true;
    this.interviewBatchApplication.IsPresent = true;

    this._adhocService.saveAdhocInterviewBatchApplication(this.interviewBatchApplication).subscribe((res: any) => {
      this.interviewBatchApplication = res;
      this.getAdhocInterviewBatchApplication();
      this.presenting = false;
    }, err => {
      console.log(err);
    });
  }
  public rejectAdhocInterviewBatchApplication() {
    this.rejecting = true;
    this.interviewBatchApplication.IsPresent = true;
    this.interviewBatchApplication.IsRejected = true;
    this._adhocService.saveAdhocInterviewBatchApplication(this.interviewBatchApplication).subscribe((res: any) => {
      this.interviewBatchApplication = res;
      this.getAdhocInterviewBatchApplication();
      this.rejecting = false;
    }, err => {
      console.log(err);
    });
  }
  public saveAdhocApplicationVerification(accepted: boolean, decisionObj: any, active: boolean) {
    if (!active) {
      decisionObj.undoing = true;
    } else if (accepted && active) {
      decisionObj.saving = true;
    } else if (!accepted && active) {
      if (confirm('Are you sure you want to reject?')) {
        decisionObj.rejecting = true;
      }
    }
    let adhocApplicationVerification: any = {};
    if (active) {
      adhocApplicationVerification.BatchApplication_Id = this.interviewBatchApplication.Id;
      adhocApplicationVerification.Application_Id = this.application.Id;
      adhocApplicationVerification.DistrictCode = this.user.HfmisCode;
      adhocApplicationVerification.DocName = this.application.Id;
      if (decisionObj.Name) {
        adhocApplicationVerification.DocName = decisionObj.Name;
        adhocApplicationVerification.Doc_Id = decisionObj.Id;
      }
      else if (decisionObj.DegreeName) {
        adhocApplicationVerification.DocName = decisionObj.DegreeName;
        adhocApplicationVerification.Qualification_Id = decisionObj.Id;
      }
      else if (decisionObj.JobTitle) {
        adhocApplicationVerification.DocName = decisionObj.JobTitle + ', ' + decisionObj.Organization
        adhocApplicationVerification.Experience_Id = decisionObj.Id;
      }
      adhocApplicationVerification.Verified = accepted;
      adhocApplicationVerification.IsActive = active;
    } else {
      adhocApplicationVerification.Id = decisionObj.verification.Id;
    }
    this._adhocService.saveAdhocApplicationVerification(adhocApplicationVerification).subscribe((res: any) => {
      this.getAdhocInterviewVerifications();
      if (!active) {
        decisionObj.undoing = false;
        decisionObj.verification = null;
      } else if (accepted && active) {
        decisionObj.saving = false;
      } else if (!accepted && active) {
        decisionObj.rejecting = false;
      }
    }, err => {
      console.log(err);
      if (!active) {
        decisionObj.undoing = false;
      } else if (accepted && active) {
        decisionObj.saving = false;
      } else if (!accepted && active) {
        decisionObj.rejecting = false;
      }
    });
  }
  public dropdownValueChanged(value: any, filter: any) {
    if (filter == 'designation') {
      this.filters.Designation_Id = value.Designation_Id;
      this.getAdhocInterview();
    }
    if (filter == 'application') {
      this.loadingApplication = true;
      this.applicant = {};
      this.application = {};
      this.interviewBatchApplication = {};
      this.interviewBatchApplication = value;
      this.applicant.Id = value.Applicant_Id;
      this.application.Id = value.Application_Id;
      this.interviewMarks = null;
      this.verificationComplete = false;
      this.positionSaved = false;
      this._adhocService.getAdhocInterviewBatchApplication(this.interviewBatchApplication.Id).subscribe((res: any) => {
        this.getAdhocApplicationApplicant();
        this.interviewBatchApplication = res;
        if (this.interviewBatchApplication.PositionHolder != null) {
          this.positionSaved = true;
        } else {
          this.positionSaved = false;
        }
      }, err => {
        console.log(err);
      });
    }
  }
  public handleFilter = (value, filter) => {
    if (filter == 'designation') {
      this.designationsData = this.designations.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
  }
  public readUrl(event: any, obj: any) {
    if (event.target.files && event.target.files[0]) {
      obj.photoFile = [];
      let inputValue = event.target;
      obj.photoFile = inputValue.files;
      if (obj) {
        obj.attached = true;
        obj.error = false;
      }
    }
  }
  // public getAdhocApplications() {
  //   this.kGrid.loading = true;
  //   this._publicPortalService.getAdhocApplications(this.filters).subscribe((res: any) => {
  //     if (res) {
  //       this.jobApplicants = res.applicants;
  //       this.jobApplications = res.applications;

  //       this.jobApplicants.forEach(jobApplicant => {
  //         jobApplicant.applications = this.jobApplications.filter(x => x.Applicant_Id == jobApplicant.Id);
  //       });
  //       console.log(this.jobApplicants);

  //       this.kGrid.data = [];
  //       this.kGrid.data = this.jobApplicants;
  //       this.kGrid.totalRecords = res.totalRecords;
  //       this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords };
  //       this.kGrid.loading = false;
  //     }
  //     this.showFilters = false;
  //     this.kGrid.loading = false;
  //   }, err => {
  //     console.log(err);
  //   });
  // }


  public getDesignations() {
    this._rootService.getDesignations().subscribe((res: any) => {
      if (res) {
        this.designations = res;
        this.designationsData = this.designations;
      }
    }, err => {
      console.log(err);
    });
  }
  public selectApplicant(dataItem) {
    this.selectedApplicant = dataItem;
    console.log('selected applicant: ', this.selectedApplicant);

  }
  diff_years(dt2, dt1) {
    dt1 = new Date(dt1);
    dt2 = new Date(dt2);
    var diff = (dt2.getTime() - dt1.getTime()) / 1000;
    diff /= (60 * 60 * 24);
    return Math.abs(Math.round(diff / 365.25));
  }
  public openEditDialog(fieldName, fieldLabel) {

  }
  public openInNewTab(link: string) {
    window.open('' + link, '_blank');
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
  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
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
