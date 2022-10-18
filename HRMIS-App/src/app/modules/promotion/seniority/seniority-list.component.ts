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
import { PromotionService } from "../promotion.service";
import { MeritList, MeritListCounts, MeritListValues } from "../../../_models/merit-list.class";
//import { ActivatedRoute } from '@angular/router';
//import { LocalService } from '../../../_services/local.service';

@Component({
  selector: "app-seniority-list",
  templateUrl: "./seniority-list.component.html",
  styleUrls: ['./seniority-list.component.scss']
})
export class SeniorityListComponent implements OnInit, OnDestroy {
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
  public seniorityApplication: any = {};
  public selectedApplicant: any = {};
  public jobDocuments: any[] = [];
  public jobApplicants: any[] = [];
  public dateNow: string = '';
  public hfName: string = '';
  public loadingApplication: boolean = false;
  public showPassword: boolean = false;
  public accepting: boolean = false;
  public locking: boolean = false;
  public rejecting: boolean = false;
  public runningPP: boolean = false;
  public running: boolean = false;
  public isAdmin: boolean = false;
  public isSdp: boolean = false;
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
  public meritList: any;
  public fontSize: number = 11;
  public count: number = 0;
  public districtCount: number = 0;
  public promotionCount: number = 0;
  public ppscCount: number = 0;
  public contractCount: number = 0;
  public designationCount: number = 0;
  public totalCount: number = 0;
  public verifiedCount: number = 0;
  public notVerifiedCount: number = 0;
  public currentDate: Date;
  public interview: any = {};
  public postingDetails: any = { TotalPosted: 0 };
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
    private _promotionService: PromotionService,
    public _notificationService: NotificationService,
    public _rootService: RootService,
    private _authenticationService: AuthenticationService,
    private ngZone: NgZone
  ) { }

  public ngOnInit(): void {
    this.user = this._authenticationService.getUser();
    this.isSdp = this.user.UserName && this.user.UserName.startsWith('sdp') ? true : false;
    let today = new Date();
    let dd: any = today.getDate(), mm: any = today.getMonth() + 1, yyyy = today.getFullYear();
    if (dd < 10) dd = '0' + dd; if (mm < 10) mm = '0' + mm;
    this.dateNow = dd + '/' + mm + '/' + yyyy;

    this.kGrid.multiple = true;
    this.kGrid.loading = false;
    this.kGrid.pageSize = 122220;
    this.kGrid.pageSizes = [50, 100, 200, 300, 500, 1000];
    this.getConsultantDesignations();
    this.getDistricts();
    this.filters.CategoryId = 1;
    //this.getNewSeniorityList();
    this.getSeniorityList();
    //this.getSeniorityListFixed();

    this.getPostingModes();
    this.getDivisions();
  }
  public getSeniorityList = () => {
    this.messageLoading = `Fetching Seniority List`;
    this.calculating = false;
    this.runningPP = true;
    this.loading = true;
    this.promotionCount = 0;
    this.ppscCount = 0;
    this.contractCount = 0;
    this._promotionService.getSeniorityList(this.filters).subscribe((res: any[]) => {
      if (res && res.length > 0) {
        this.kGrid.dataOrigional = res;
        this.kGrid.data = this.kGrid.dataOrigional;
        this.kGrid.dataOrigional.forEach(element => {
          if (element.ModeId == 1) {
            this.promotionCount++;
          }
          if (element.ModeId == 2) {
            this.ppscCount++;
          }
          if (element.ModeId == 3) {
            this.contractCount++;
          }

          if (element.IsVerified) {
            this.verifiedCount++;
          }
          if (!element.IsVerified) {
            this.notVerifiedCount++;
          }
          this.totalCount++;
        });
        this.kGrid.loading = false;
      } else {
        this.messageLoading = `Generating New Seniority List`;
        this.generateSeniorityList();
      }
      this.loading = false;
      this.runningPP = false;
    },
      err => {
        console.log(err);
        this.loading = false;
        this.runningPP = false;
      }
    );
  }

  public getNewSeniorityList = () => {
    this.messageLoading = `Fetching Seniority List`;
    this.calculating = false;
    this.runningPP = true;
    this.loading = true;
    this.promotionCount = 0;
    this.ppscCount = 0;
    this.contractCount = 0;
    this._promotionService.getNewSeniorityList(this.filters).subscribe((res: any[]) => {
      if (res) {
        this.verifiedCount = 0;
        this.notVerifiedCount = 0;
        this.kGrid.dataOrigional = res;
        this.kGrid.data = this.kGrid.dataOrigional;
        this.kGrid.dataOrigional.forEach(element => {
          if (element.ModeId == 1 || element.PromotionJoiningDate != null) {
            this.promotionCount++;
            element.color = 'promotion';
          }
          if ((element.ModeId == 2 || element.ModeId == 4 || element.ModeId == 5 || element.ModeId == 13) && element.PromotionJoiningDate == null) {
            this.ppscCount++;
            element.color = 'ppsc';
          }
          if (element.ModeId == 3 || element.ModeId == 6 || element.ModeId == 11 || element.ModeId == 12) {
            this.contractCount++;
            element.color = 'contract';
          }

          if (element.IsVerified) {
            this.verifiedCount++;
          }
          if (!element.IsVerified) {
            this.notVerifiedCount++;
          }
          this.totalCount++;
        });
        this.kGrid.loading = false;
      }
      this.loading = false;
      this.runningPP = false;
    },
      err => {
        console.log(err);
        this.loading = false;
        this.runningPP = false;
      }
    );
  }
  public getSeniorityListFixed = () => {
    this.messageLoading = `Fetching Seniority List`;
    this.calculating = false;
    this.runningPP = true;
    this.loading = true;
    this.promotionCount = 0;
    this.ppscCount = 0;
    this.contractCount = 0;
    this._promotionService.getSeniorityListFixed(this.filters).subscribe((res: any[]) => {
      if (res) {
        this.verifiedCount = 0;
        this.notVerifiedCount = 0;
        this.kGrid.dataOrigional = res;
        this.kGrid.data = this.kGrid.dataOrigional;
        this.kGrid.dataOrigional.forEach(element => {
          if (element.ModeId == 1 || element.PromotionJoiningDate != null) {
            this.promotionCount++;
            element.color = 'promotion';
          }
          if ((element.ModeId == 2 || element.ModeId == 4 || element.ModeId == 5 || element.ModeId == 13) && element.PromotionJoiningDate == null) {
            this.ppscCount++;
            element.color = 'ppsc';
          }
          if (element.ModeId == 3 || element.ModeId == 6 || element.ModeId == 11 || element.ModeId == 12) {
            this.contractCount++;
            element.color = 'contract';
          }

          if (element.IsVerified) {
            this.verifiedCount++;
          }
          if (!element.IsVerified) {
            this.notVerifiedCount++;
          }
          this.totalCount++;
        });
        this.kGrid.loading = false;
      }
      this.loading = false;
      this.runningPP = false;
    },
      err => {
        console.log(err);
        this.loading = false;
        this.runningPP = false;
      }
    );
  }
  public generateSeniorityList = () => {
    if (!this.isAdmin) return;
    this.calculating = true;
    this.loading = true;
    this._promotionService.generateSeniorityList(this.filters).subscribe((res: any[]) => {
      if (res && res.length > 0) {
        this.kGrid.data = res;
        this.kGrid.loading = false;
      }
      this.loading = false;
      this.calculating = false;
    },
      err => {
        console.log(err);
        this.loading = false;
        this.calculating = false;
      }
    );
  }
  public filterSeniorityList(modeId: number) {
    this.messageLoading = `Fetching Seniority List`;
    this.calculating = false;
    this.runningPP = true;
    this.loading = true;
    this.kGrid.data = [];
    setTimeout(() => {
      this.kGrid.data = this.kGrid.dataOrigional.filter(x => x.ModeId == modeId);
      this.runningPP = false;
      this.loading = false;
    }, 900);
  }

  public filterVerifiedSeniorityList(isVerified: boolean) {
    this.messageLoading = `Fetching Seniority List`;
    this.calculating = false;
    this.runningPP = true;
    this.loading = true;
    this.kGrid.data = [];
    setTimeout(() => {
      if (isVerified) {
        this.kGrid.data = this.kGrid.dataOrigional.filter(x => x.IsVerified == isVerified);
      } else {
        this.kGrid.data = this.kGrid.dataOrigional.filter(x => !x.IsVerified);
      }
      this.runningPP = false;
      this.loading = false;
    }, 900);
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
      this.photoFile = [];
      let inputValue = event.target;
      this.photoFile = inputValue.files;
      if (obj) {
        obj.attached = true;
        obj.error = false;
      }
    }
  }
  public getAdhocInterview() {

    this.kGrid.loading = true;
    this.messageLoading = `Checking Interviews!`;
    this.adhocInterview = {};
    this.interviewBatch = [];
    this._promotionService.getAdhocInterview(this.filters.DistrictCode, this.filters.Designation_Id).subscribe((adhocInterview: any) => {
      if (adhocInterview && adhocInterview.Id > 0) {
        this.adhocInterview = adhocInterview;
        let batchIdForCommittee = 0;
        this.messageLoading = `Interview Found!`;
      } else {
        this.kGrid.loading = false;
      }
    }, err => {
      this.kGrid.loading = false;
      console.log(err);
    });
  }
  public getApplicantPMC() {
    console.log(this.selectedBatchApplication);
    this._promotionService.getAdhocApplicantPMC(this.selectedBatchApplication.Applicant_Id).subscribe((r: any) => {
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
        this._promotionService.verifyPMCGetQualifications(this.selectedBatchApplication.PMDCNumber).subscribe((res) => {
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
            this._promotionService.saveAdhocApplicantPMC(obj).subscribe((r) => {
              console.log(this.selectedBatchApplication.Applicant_Id);
              this._promotionService.getAdhocApplicantPMC(this.selectedBatchApplication.Applicant_Id).subscribe((p: any) => {
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
  public getAdhocVacantDesignations() {
    this.jobVacancy = {};
    this._promotionService.getAdhocVacantDesignations('facilities', this.filters.Designation_Id).subscribe((res: any) => {
      if (res) {
        this.jobVacancy = res;

        this.jobVacancy.districts;
        console.log(this.jobVacancy);

        this.vpDetail = this.jobVacancy.districts.filter(x => x.Code == this.filters.DistrictCode);
        this.vpDetailTotal = 0;
        this.vpDetail.forEach(vp => {
          if (vp) {
            if (vp.DesignationId != 1085 && vp.DesignationId != 1157 && vp.DesignationId != 2404) {
              this.vpDetailTotal += vp.Vacant;
            }
            if (vp.DesignationId == 2404) {
              this.vpDetailTotal += (vp.Vacant % 2) == 1 ? ((vp.Vacant - 1) / 2) : (vp.Vacant / 2);
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
  public viewApplicantMerit(obj) {
    this.selectedBatchApplication = obj;
    this.openMeritDialog();
    this.selectedBatchApplication.loadingPrefs = true;
    this._promotionService.getApplicationPreference(this.selectedBatchApplication.Application_Id).subscribe((x: any) => {
      if (x) {
        this.selectedBatchApplication.Preferences = x;
        this.selectedBatchApplication.loadingPrefs = false;
      }
    }, err => {
      this.selectedBatchApplication.loadingPrefs = false;
      console.log(err);
    });
  }
  public openMeritDialog() {
    this.meritVerificationDialog = true;
  }
  public openDialog(cnic: string) {
    this.meritVerificationDialog = true;
    this.loadingApplication = true;
    this._promotionService.getSeniorityApplicant(cnic).subscribe((x: any) => {
      if (x) {
        console.log(x);

        this.seniorityApplication = x;
        if (this.seniorityApplication && this.seniorityApplication.application)
          if (this.seniorityApplication.application.ModeId == 2 || this.seniorityApplication.application.ModeId == 3) {
            this.seniorityApplication.application.entry = 101;
          }
      }
      this.loadingApplication = false;
    }, err => {
      this.loadingApplication = false;
      console.log(err);
    });
  }
  public changeStatus(statusId: number, dataItem: any) {
    if (statusId == 1) {
      this.accepting = true;
    }
    if (statusId == 2) {
      this.rejecting = true;
    }
    if (statusId == 3) {
      this.locking = true;
    }
    if (dataItem) {
      this._promotionService.changeSeniorityApplicationStatus(dataItem.Id, statusId).subscribe((res: any) => {
        if (statusId == 1) {
          this.accepting = false;
          this.openDialog(dataItem.CNIC);
        }
        if (statusId == 2) {
          this.rejecting = false;
          this.closeMeritDialog();
        }
        if (statusId == 3) {
          this.locking = false;
          this.closeMeritDialog();
        }
      },
        err => {
          console.log(err);
          this.loading = false;
        }
      );
    }
  }
  public closeMeritDialog() {
    this.seniorityApplication = {};
    this.meritVerificationDialog = false;
  }
  private getConsultantDesignations = () => {
    this._rootService.getConsultantDesignations().subscribe((res: any) => {
      console.log(res);

      if (res && res.List) {
        this.dropDowns.adhocJobs = res.List;
        this.dropDowns.adhocJobsData = this.dropDowns.adhocJobs;
      }
    },
      err => {
        console.log(err);
        this.loading = false;
      }
    );
  }
  private getPostingModes = () => {
    this.dropDowns.postingModes = [];
    this._rootService.GetJobPostingMode().subscribe((res: any) => {
      this.dropDowns.postingModes = res;
      this.dropDowns.postingModes = this.dropDowns.postingModes.filter(x => x.Id == 2 || x.Id == 3 || x.Id == 4 || x.Id == 5 || x.Id == 6 || x.Id == 11 || x.Id == 12 || x.Id == 13);
      if (this.dropDowns.postingModes.length == 8) {
        this.dropDowns.postingModes[8] = { Name: 'Promotion', Id: 1 }
      }
    },
      err => { console.log(err); }
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
  private getDivisions = () => {
    this.dropDowns.divisions = [];
    this._rootService.getDivisions(this.user.HfmisCode).subscribe((res: any) => {
      this.dropDowns.divisions = res;
      if (this.dropDowns.divisions.length == 1) {
        this.dropDowns.selectedFiltersModel.division = this.dropDowns.divisions[0];
        this.filters.DivisionCode = this.user.HfmisCode;
      }
    },
      err => { console.log(err); }
    );
  }
  public verify(obj: any) {
    obj.saving = true;
    if (confirm('Verify and Lock?')) {
      this._promotionService.verifyProfileForPromotion(obj.Profile_Id).subscribe((res: any) => {
        if (res) {
          this.getNewSeniorityList();
        }
        obj.saving = false;
      }, err => {
        console.log(err);
        obj.saving = false;
      });
    }
  }
  public openInNewTab(link: string) {
    window.open('' + link, '_blank');
  }
  public dropdownValueChanged = (value, filter) => {
    if (filter == 'category') {
      this.filters.CategoryId = value.Id;
      this.getSeniorityList();
    }
    if (filter == 'designation') {
      this.filters.Designation_Id = value.Id;
      this.getSeniorityList();
    }
    if (filter == 'postingMode') {
      this.filters.PostingModeId = value.Id;
      this.getSeniorityList();
    }
    if (filter == 'designation') {
      this.filters.Designation_Id = value.Id;
      this.getSeniorityList();
    }
    if (filter == 'applicationStatus') {
      this.filters.ApplicationStatusId = value.Id;
      this.getSeniorityList();
    }
    if (filter == 'district') {
      this.filters.DistrictCode = value.Code;
    }
    if (filter == 'division') {
      this.filters.DivisionCode = value.Code;
      this.getSeniorityList();
    }
    /* if (this.filters.Designation_Id && this.filters.Designation_Id > 0 && this.filters.DistrictCode) {
      this.getAdhocVacantDesignations();
    } */
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
    this._promotionService.getAdhocApplicant(this.selectedApplicant.Id).subscribe((res: any) => {
      if (res) {
        this.ApplicantInfo = res;
        console.log('ApplicantInfoAd: ', this.ApplicantInfo);
      }
    }, err => {
      console.log(err);
    });
  }

  ngOnDestroy() {
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