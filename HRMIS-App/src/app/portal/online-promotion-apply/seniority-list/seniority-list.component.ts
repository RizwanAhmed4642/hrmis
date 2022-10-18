import { Component, NgZone, OnInit } from '@angular/core';
import { Subject, Subscription } from 'rxjs-compat';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { KGridHelper } from '../../../_helpers/k-grid.class';
import { AuthenticationService } from '../../../_services/authentication.service';
import { RootService } from '../../../_services/root.service';
import { OnlinePromotionApplyService } from '../online-promotion-apply.service';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { CookieService } from '../../../_services/cookie.service';
@Component({
  selector: 'app-seniority-list',
  templateUrl: './seniority-list.component.html',
  styleUrls: ['./seniority-list.component.scss']
})

export class SeniorityListComponent implements OnInit {

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
  public applicant: any = {};
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
  public downloadClicked: boolean = false;
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
  public urdu: any = {
    first: 'اپنے شناختی کارڈ نمبر کا اندراج کیجئے',
    second1: `
    آپ کا شناختی کارڈ نمبر پہلے سے درج ہے لہذا اپنے پاس ورڈ کا اندراج کیجئے
    `,
    second2: `Forgot Password?`,
    second3: `
     کا انتخاب کریں۔
    `,
    secondeng: 'Your CNIC is already registered, please enter your password. You can reset password by clicking on Forgot Password!',
    third: `
    آپ کے شناختی کارڈ نمبر کا پہلے سےاندراج موجود نہیں ہے۔ لہذا اپنے موبائل نمبر اور ای میل ایڈریس کا اندراج کر کے خود کو رجسٹر کروائیں ۔ موبائل نمبر اور ای میل ایڈریس پر موصول ہونے والے پاس ورڈ سے لاگ اِن کریں۔
    `,
    thirdeng: 'Your CNIC is not registered. Please enter your mobile number and email address. You will recieve password on your mobile number and email address',
    fourth: `
کسی بھی قسم کی دشواری کی صورت میں ہیلپ لائن نمبر 1033سے رہنمائی حاصل کی جاسکتی ہے۔
`, hdnama: `ہدایات نامہ`,
    fifth: `اپنا موبائل نمبر یا ای میل ایڈریس تبدیل کرنے کے لیے براہ کرم 04299206173 پر رابطہ کریں کیونکہ ویب سائٹ میں لاگ ان کرنے کا پن کوڈ آپ کے رجسٹرڈ موبائل نمبر اور ای میل پر بھیجا جائے گا۔`,
    fiftheng: 'To change your mobile number or email address please contact at 042-99206173 because the pin code for login the portal will be sent on your registered mobile number and email.',
    difurdu: `ویب سائٹ کو لاگ ان کرنے میں کسی مشکل کی صورت میں آپ سے گزارش ہے کہ   04299206173  پر صبح 10 بجے سے شام 4 بجے (پیر سے جمعہ) رابطہ کریں۔`,
    desc1: `سپیشلسٹ ڈاکٹرز کی عارضی سنیارٹی لسٹ کا جائزہ لینے کے لیے اپنے شناختی کارڈ نمبر کا اندراج کرتے ہوئے لاگ اِن کریں۔`,
    desc2: ` :لاگ ان کرنے کے بعد آپ کو درج ذیل مراحل پر عمل کرنا ہوگا`,
    desc3: `۔(۱).اگر آپ کا ریکارڈ عارضی سنیارٹی لسٹ  میں درست ہے تو مطلوبہ دستاویزات اپ لوڈ کریں۔`,
    desc4: `۔(۲).	اگر آپ کا ریکارڈ عارضی سنیارٹی لسٹ میں غلط ہے تو اپنا ریکارڈ درست کریں  اور مطلوبہ دستاویزات اپلوڈ کریں۔`,
    desc5: `۔(۳).	اگر آپ کا ریکارڈ عارضی سنیارٹی لسٹ میں موجود نہ ہے  تو اپنے ریکارڈ کا اندراج کریں اور مطلوبہ دستاویزات اپلوڈ کریں۔`,
  };
  public designations: any[] = [];
  public designationsOrigional: any[] = [];
  constructor(
    private _onlinePromotionApplyService: OnlinePromotionApplyService,
    private _cookieService: CookieService,
    public _rootService: RootService,

    private ngZone: NgZone
  ) { }
  ngOnInit() {
    //this.filters.cnic = this._cookieService.getCookie('cnicussrpromotion');
    this.filters.searchTerm = this.filters.cnic;
    this.filters.CategoryId = 1;
    this.getSeniorityList();
    this.subscribeToFilter();
    this.getConsultantDesignations();
  }
  private subscribeToFilter() {
    this.inputChange = new Subject();
    this.inputChangeSubscription = this.inputChange.pipe(debounceTime(800)).subscribe((query: boolean) => {
      this.filters.CategoryId = 1;
      this.getSeniorityList();
    });
  }
  public getSeniorityList = () => {
    this.messageLoading = `Fetching Seniority List`;
    this.calculating = false;
    this.runningPP = true;
    this.loading = true;
    this.promotionCount = 0;
    this.ppscCount = 0;
    this.contractCount = 0;
    this._onlinePromotionApplyService.getSeniorityListFixed(this.filters).subscribe((res: any[]) => {
      if (res) {
        this.verifiedCount = 0;
        this.notVerifiedCount = 0;
        this.kGrid.dataOrigional = res;
        this.kGrid.data = this.kGrid.dataOrigional;
        this.kGrid.dataOrigional.forEach(element => {
          this.totalCount++;
          if (this.filters.cnic == element.CNIC) {
            this.applicant = element;
            console.log(this.applicant);

          }
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
    this._onlinePromotionApplyService.generateSeniorityList(this.filters).subscribe((res: any[]) => {
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
  public downlaodSld() {
    window.open('/Uploads/Files/senority_list_2022_Consultants.pdf', '_blank');
    this.downloadClicked = true;
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

  public dropdownValueChanged = (value, filter) => {

    if (filter == 'designation') {
      this.filters.Designation_Id = value.Id;
      this.getSeniorityList();
    }
  }
}
