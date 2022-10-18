import { Component, OnInit, OnDestroy } from '@angular/core';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { Subscription } from 'rxjs/Subscription';
import { ActivatedRoute, Router } from '@angular/router';
import { RootService } from '../../../_services/root.service';
import { CookieService } from '../../../_services/cookie.service';
import { LocalService } from '../../../_services/local.service';
import { OnlineAdhocService } from '../online-adhoc.service';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-apply-now',
  templateUrl: './apply-now.component.html',
  styleUrls: ['./apply-now.component.scss']
})
export class ApplyNowComponent implements OnInit, OnDestroy {

  public dropDowns: DropDownsHR = new DropDownsHR();
  private subscription: Subscription;
  public cnic: string = '';
  public type: string = '';
  public merit: any = {};
  public meritActiveDesignation: any = {};
  public profile: any = null;
  public hfmisCode: string = null;
  public hf_Id: number = 0;
  public desigId: number = 0;
  public applicant: any = {};
  public application: any = {};
  public preference: any = {};
  public jobVacancy: any = {};
  public hfs: any[] = [];
  public districts: any[] = [];
  public experiences: any[] = [];
  public documents: any[] = [];
  public hfsOrigional: any[] = [];
  public designations: any[] = [];
  public designationsOrigional: any[] = [];
  public preferencesOrigional: any[] = [];
  public preferences: any[] = [];
  public excludeDistrictsOrigional: string[] = ['Lahore', 'Islamabad'];
  public excludeDistricts: string[] = [];
  public meritPreferences: any[] = [];
  public loading: boolean = false;
  public locking: boolean = false;
  public applied: boolean = false;
  public addingPrefs: boolean = false;
  public addingAllPrefs: boolean = false;
  public showOther: boolean = false;
  public proceedApplication: boolean = false;
  public alreadyApplied: boolean = false;
  public portalClosed: boolean = false;
  public closureDate: any = new Date('12-11-2021');

  public urdu: any = {
    info1: `صوبہ بھر میں تمام اضلاع کی خالی آسامیوں پر اپلائی کیا جا سکتا ہے۔ اپنی ترجیحات کو ترتیب میں منتخب کریں۔ترجیحات کو تبدیل کرنے کے لیے `,
    info2: `جائزہ ترجیح `,
    infoeng: 'You can apply for any vacant seats in all districts of Punjab. Please select your preferences in order. Click Review Preferences to change the preferences order',
    info3: ` پر کلک کریں۔  `,
    infoLock: `درخواست مکمل ہو گئی ہے۔
    حتمی جمع کرانے اور پرنٹ کرنے کے بعد آپ کی درخواست یہاں مقفل کر دی جائے گی۔پرنٹ لینے کے بعد براہ کرم نیچے تصدیقی بٹن پر کلک کریں۔`
  };
  constructor(private _rootService: RootService,
    private _cookieService: CookieService, private router: Router,
    private _localService: LocalService,
    private _onlineAdhocService: OnlineAdhocService,
    private route: ActivatedRoute, 
    private datePipe: DatePipe) { }

  ngOnInit() {
    this.cnic = this._cookieService.getCookie('cnicussradhoc');
    this.application.Id = +this._cookieService.getCookie('cnicussradhocapp');
    this.fetchParams();
    this.checkPortalClosure();
  }

  public checkPortalClosure() {
    this.portalClosed = false;
    /* let currentDate: any = new Date();
    currentDate = this.datePipe.transform(currentDate, 'dd-MM-yyyy');
    this.closureDate = this.datePipe.transform(this.closureDate, 'dd-MM-yyyy');
    if (this.closureDate === currentDate) {
      this.portalClosed = false;
    } */
  }

  private getApplicant() {
    this.loading = true;
    this._onlineAdhocService.getApplicant(this.cnic).subscribe((res3: any) => {
      if (res3) {
        this.applicant = res3;
        if (this.desigId != 0) {
          this.getApplication();
        }
        this.applicant.appliedFor = this._localService.get('desigaplname');
        this.applicant.DOB = new Date(this.applicant.DOB);
        this.calculate_age(this.applicant.DOB);
        this.getExperiences();
        this.getapplicantDocuments();
      }

    }, err2 => {
      this.loading = true;
      console.log(err2);
    });
  }

  public apply(item) {
    item.loading = true;
    this.preference.JobApplicant_Id = this.applicant.Id;
    this.preference.JobApplication_Id = this.application.Id;
    this.preference.JobHF_Id = item.Id;
    this._onlineAdhocService.saveAdhocPreference(this.preference).subscribe((res3: any) => {
      if (res3 == 'Already') {
        item.applied = true;
        this.applied = true;
      }
      if (res3 && res3.Id) {
        item.prefId = res3.Id;
        item.applied = true;
        this.applied = true;
      }
      item.loading = false;
    }, err2 => {
      item.loading = false;
      console.log(err2);
    });
  }
  public sendConfimationSMS(applicationId: number) {
    this._onlineAdhocService.sendConfimationSMS(applicationId).subscribe((res: any) => {
      if (res) {
        console.log('sms res: ', res);
        this.router.navigate(['/adhoc/home']);
      }
    }, err2 => {
      console.log(err2);
    });
  }
  public lockApplication() {
    this.locking = true;
    this._onlineAdhocService.lockApplication(this.application.Id).subscribe((res: any) => {
      if (res) {
        console.log('app: ', this.application);
        console.log('response: ', res);
        //this.sendConfimationSMS(this.application.applicantId)
        this.locking = false;
      }
      this.router.navigate(['/adhoc/home']);
    }, err2 => {
      console.log(err2);
    });
  }
  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('desigId')) {
          let param = params['desigId'];
          if (+param) {
            this.desigId = +params['desigId'] as number;

            if (this.desigId == 1) {
              this.desigId = +this._cookieService.getCookie('cnicussradhocdesig');
              this.proceedApplication = true;
            }
            if (this.desigId == 11) {
              this.desigId = +this._cookieService.getCookie('cnicussradhocdesig');
              this.proceedApplication = true;
              this.alreadyApplied = true;
            }
            this.type = 'facilities';
          }
        } else {
          this.desigId = 0;
          this.type = 'designations';
        }
        this.getApplicant();
      }
    );
  }
  public proceed() {
    this.proceedApplication = true;
  }
  public getJobPreferences() {
    this._onlineAdhocService.getJobPreferences(this.hf_Id).subscribe((res: any) => {
      if (res) {
        this.hfs = res;
      }
    }, err => {
      console.log(err);
    })
  }
  public searchDesignations(query: string) {
    if (!query) {
      this.designations = this.designationsOrigional;
    }
    this.designations = this.designationsOrigional.filter(x => x.Name.toLowerCase().indexOf(query.toLowerCase()) !== -1);
  }
  public applyNow(dsgId: number) {
    if (dsgId == 0) {
      this.hfs = this.hfsOrigional;
    }
    this.hfs = this.hfsOrigional.filter(x => x.Desg_Id == dsgId);
  }

  public removeHF(item) {
    item.loading = true;
    this._onlineAdhocService.removeApplicantPreference(item.prefId).subscribe((res: any) => {
      if (res) {
        item.applied = false;
      }
      item.loading = false;
    }, err => {
      item.loading = false;
      console.log(err);
    });
  }
  public getExperiences() {
    this.loading = true;
    this._onlineAdhocService.getExperiences(this.applicant.Id).subscribe((res: any) => {
      if (res) {
        this.experiences = res;
        this.applicant.exp = 0;
        this.experiences.forEach(exp => {
          this.applicant.exp += this.diff_years(exp.FromDate, exp.ToDate);
        });
        this.loading = false;
      }
    }, err => {
      console.log(err);
    });
  }
  public getApplication() {
    this.loading = true;
    this._onlineAdhocService.getApplicationSingle(this.applicant.Id, this.desigId).subscribe((res: any) => {
      if (res) {
        this.application = res;
        this.getAdhocApplicationPref();
        this.loading = false;
      } else {
        this.applied = false;
      }
    }, err => {
      this.loading = false;
      console.log(err);
    });
  }
  public getapplicantDocuments() {
    this.loading = true;
    this._onlineAdhocService.getApplicantQualification(this.applicant.Id).subscribe((res: any) => {
      if (res) {
        this.documents = res;
        this.loading = false;
      }
    }, err => {
      console.log(err);
    });
  }
  public getAdhocVacants() {
    this.jobVacancy = {};
    this._onlineAdhocService.getAdhocVacants(this.type, this.desigId).subscribe((res: any) => {
      if (res) {
        this.jobVacancy = res;
        if (this.desigId == 0) {
          this.designationsOrigional = this.jobVacancy.designations;
          this.designations = this.designationsOrigional;
          this.hfsOrigional = [];
          this.districts = [];
        } else {
          this.designationsOrigional = [];
          this.designations = [];
          this.hfsOrigional = this.jobVacancy.hfs;
          this.applicant.Preferences.forEach(pref => {
            let prefGivenHF = this.hfsOrigional.find(x => x.Id == pref.JobHF_Id);
            if (prefGivenHF) {
              prefGivenHF.applied = true;
            }
          });
          this.districts = this.jobVacancy.districts;
          this.districts.forEach(dist => {
            dist.hfs = this.hfsOrigional.filter(x => x.HFMISCode.startsWith(dist.Code))
          });
        }
      }
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = true;
    })
  }

  public fetchMeritInfo() {
    this._onlineAdhocService.getMeritProfile(this.cnic).subscribe((res: any) => {
      if (res) {
        this._onlineAdhocService.getMeritActiveDesignation(res.MeritsActiveDesignationId).subscribe((data: any) => {
          this.merit = res;
          this.meritActiveDesignation = data;
          if (data.IsActive == 'Y') {
            this.getPreferences();
            this.getVacantPlaces();
            this._rootService.getProfileByCNIC(this.cnic).subscribe((x) => {
              if (x) {
                this.profile = x;
              }
            }, err => {
              console.log(err);
            });
          } else {
            this.router.navigate(['/ppsc/review']);
          }
        }, err => {
          console.log(err);
        });
      }
    }, err => {
      console.log(err);
    });
  }

  public handleFilter = (value, filter) => {
    if (filter == 'preferences') {
      this.hfs = this.hfsOrigional.filter((s: any) => s.FullName.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
  }
  public getDistricts() {
    this._rootService.getDistricts('0').subscribe((res: any) => {
      if (res) {
        this.hfsOrigional = res;
        this.hfs = this.hfsOrigional;
        //this.dropDowns.districtsData = this.dropDowns.districts;
      }
    }, err => {
      console.log(err);
    });
  }
  public getVacantPlaces() {
    this._rootService.getVacantPlaces(this.merit.Designation_Id, this.profile ? this.profile.Id ? 1 : 0 : 0).subscribe((res: any) => {
      if (res) {
        this.hfsOrigional = res;
        this.hfs = this.hfsOrigional;
        /*  this.dropDowns.districts = res;
         this.dropDowns.districtsData = this.dropDowns.districts; */
      }
    }, err => {
      console.log(err);
    });
  }
  public getPreferences() {
    this._onlineAdhocService.getPreferences(this.merit.Id).subscribe((res: any) => {
      if (res) {
        this.preferencesOrigional = res;
        this.preferences = this.preferencesOrigional;

        this.addingPrefs = false;
        this.addingAllPrefs = false;

        //this.setDistricts();
      }
    }, err => {
      console.log(err);
    });
  }
  public setDistricts() {
    this.excludeDistricts = ['Lahore', 'Islamabad'];
    this.preferences.forEach((elem) => {
      this.excludeDistricts.push(elem);
    });
    let districts = this.dropDowns.districtsData;
    this.dropDowns.districts = this.dropDowns.districtsData.filter(
      function (e) {
        return this.indexOf(e.Name) < 0;
      },
      this.excludeDistricts
    );
    console.log(this.excludeDistricts);
  }
  public removePreferences(HfmisCode) {
    console.log({ HfmisCode: HfmisCode, MeritId: this.merit.Id });

    if (confirm('Are you sure?')) {
      this._onlineAdhocService.removePreferences({ HfmisCode: HfmisCode, MeritId: this.merit.Id }).subscribe((res: any) => {
        if (res) {
          this.getPreferences();
          //this.preferences = res;
        }
      }, err => {
        console.log(err);
      });
    }
  }

  public dropdownValueChanged = (value, filter) => {
    if (value) {
      if (value.Code == '0') return;
    }
    if (filter == 'district') {
      this.hf_Id = value.HF_Id;
      this.hfmisCode = value.HFMISCode;
    }
  }
  public addPreference() {
    if (this.hfmisCode && this.hfmisCode != '0') {
      this.addingPrefs = true;
      this._onlineAdhocService.addPreferences({ MeritId: this.merit.Id, hfmisCode: this.hfmisCode, desigId: this.hf_Id }).subscribe((res: any) => {
        if (res) {
          this.hfmisCode = null;
          this.dropDowns.selectedFiltersModel.district = this.dropDowns.defultFiltersModel.district;
          this.getPreferences();
        }
      }, err => {
        console.log(err);
      });
    }
  }

  public getAdhocApplicationPref() {
    this._onlineAdhocService.getApplicationPref(this.application.Id).subscribe((x: any) => {
      if (x) {
        this.applicant.Preferences = x;
        if (this.applicant.Preferences.length > 0) {
          this.applied = true;
        }
        this.getAdhocVacants();
      }
    }, err => {
      console.log(err);
    });
  }
  public saveAllPreferences() {
    this.addingAllPrefs = true;
    this._onlineAdhocService.saveAllPreferences({ MeritId: this.merit.Id }).subscribe((res: any) => {
      if (res) {
        this.hfmisCode = null;
        this.dropDowns.selectedFiltersModel.district = this.dropDowns.defultFiltersModel.district;
        this.getPreferences();
      }
    }, err => {
      console.log(err);
    });
  }
  diff_years(dt2, dt1) {
    dt1 = new Date(dt1);
    dt2 = new Date(dt2);
    var diff = (dt2.getTime() - dt1.getTime()) / 1000;
    diff /= (60 * 60 * 24);
    return Math.abs(Math.round(diff / 365.25));
  }
  calculate_age(dob) {
    var diff_ms = Date.now() - dob.getTime();
    var age_dt = new Date(diff_ms);
    this.applicant.age = Math.abs(age_dt.getUTCFullYear() - 1970);
  }
  public dashifyCNIC(cnic: string) {
    if (!cnic) {
      return;
    }
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
  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
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
