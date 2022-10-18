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
  selector: "app-adhoc-merit-posting",
  templateUrl: "./adhoc-merit-posting.component.html",
  styleUrls: ['./adhoc-merit-posting.component.scss']
})
export class AdhocMeritPostingComponent implements OnInit, OnDestroy {
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
  public runningPP: boolean = false;
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
  public districtCount: number = 0;
  public designationCount: number = 0;
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
    this.getAdhocJobs();
    this.getDistricts();
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
    this._adhocService.getAdhocMeritLocked(this.filters.Designation_Id, this.filters.DistrictCode).subscribe((res: any) => {
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
        let postings = res.adhocPostings;
        if (this.kGrid.data && this.kGrid.data.length > 0) {
          this.messageLoading = `Loading Merit List of (${this.kGrid.data.length}}) loaded...`;
          this.postingDetails.TotalPosted = 0;
          var myCurrentDate = new Date('04/24/2022');
          var yesterDay = new Date(myCurrentDate);
          yesterDay.setDate(yesterDay.getDate());
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
            a.posting = postings.find(x => x.Applicant_Id == a.Applicant_Id);
            this.checkMeritlistValues(a);
            if (a.posting && a.posting.PostingDistrict == this.adhocInterview.DistrictName) {
              this.postingDetails.TotalPosted++;
            }
            console.log(a.posting);
            console.log(yesterDay);
            console.log(new Date(a.DateTime));
            console.log(new Date(a.DateTime) < yesterDay);
            console.log(new Date(a.DateTime) > yesterDay);
            if (a.posting) {
              if (new Date(a.posting.DateTime) > yesterDay) {
                a.posting.coloring = true;
                console.log(a.posting.coloring);
              }
            }

          });
          this.messageLoading = `Merit List Loaded`;
          this.kGrid.loading = false;
          this.calculating = false;
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
      this.photoFile = [];
      let inputValue = event.target;
      this.photoFile = inputValue.files;
      if (obj) {
        obj.attached = true;
        obj.error = false;
      }
    }
  }
  public activePosting() {

    this.runningPP = true;

    this._adhocService.activePosting().subscribe((adhocInterview: any) => {
      setTimeout(() => {
        this.runningPP = false;
        this.getAdhocMerit();
      }, 9000);
    }, err => {
      this.runningPP = false;
      console.log(err);
    });
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
  public getAdhocVacantDesignations() {
    this.jobVacancy = {};
    this._adhocService.getAdhocVacantDesignations('facilities', this.filters.Designation_Id).subscribe((res: any) => {
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
    this.getApplicantPMC();
    this.openMeritDialog();
    this.selectedBatchApplication.loadingPrefs = true;
    this._adhocService.getApplicationPreference(this.selectedBatchApplication.Application_Id).subscribe((x: any) => {
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
  public closeMeritDialog() {
    this.selectedBatchApplication = {};
    this.meritVerificationDialog = false;
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
