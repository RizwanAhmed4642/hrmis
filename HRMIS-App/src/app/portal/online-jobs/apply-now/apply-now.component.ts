import { Component, OnInit, OnDestroy } from '@angular/core';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { Subscription } from 'rxjs/Subscription';
import { ActivatedRoute, Router } from '@angular/router';
import { OnlineJobsService } from '../online-jobs.service';
import { RootService } from '../../../_services/root.service';
import { CookieService } from '../../../_services/cookie.service';
import { LocalService } from '../../../_services/local.service';

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
  public applied: boolean = false;
  public addingPrefs: boolean = false;
  public addingAllPrefs: boolean = false;
  public showOther: boolean = false;
  constructor(private _rootService: RootService,
    private _cookieService: CookieService, private router: Router,
    private _localService: LocalService,
    private _onlineJobsService: OnlineJobsService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.cnic = this._cookieService.getCookie('cnicussrphfmc');
    this.fetchParams();
  }
  private getApplicant() {
    this.loading = true;
    this._onlineJobsService.getApplicant(this.cnic).subscribe((res3: any) => {
      if (res3) {
        this.applicant = res3;
        this.getApplication();
        this.applicant.appliedFor = this._localService.get('desigaplname');
        this.applicant.DOB = new Date(this.applicant.DOB);
        this.calculate_age(this.applicant.DOB);
        this.getPHFMCVacants();
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
    this.application.Applicant_Id = this.applicant.Id;
    this.application.Designation_Id = this.desigId;
    this.application.HF_Id = item.Id;
    this._onlineJobsService.saveApplication(this.application).subscribe((res3: any) => {
      if (res3 && res3.Id) {
        item.applied = true;
        this.applied = true;
      } else if (res3 && res3 == "limit") {
        item.applied = "limit";
        this.applied = true;
      }
      item.loading = false;
    }, err2 => {
      item.loading = false;
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
  public getJobPreferences() {
    this._onlineJobsService.getJobPreferences(this.hf_Id).subscribe((res: any) => {
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
  public getExperiences() {
    this.loading = true;
    this._onlineJobsService.getExperiences(this.applicant.Id).subscribe((res: any) => {
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
    this._onlineJobsService.getApplication(this.applicant.Id, this.desigId).subscribe((res: any) => {
      if (res) {
        this.application = res;
        this.applied = true;
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
    this._onlineJobsService.getApplicantDocuments(this.applicant.Id).subscribe((res: any) => {
      if (res) {
        this.documents = res;
        this.loading = false;
      }
    }, err => {
      console.log(err);
    });
  }
  public getPHFMCVacants() {
    this.jobVacancy = {};
    this._onlineJobsService.getPHFMCVacants(this.type, this.desigId).subscribe((res: any) => {
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
    this._onlineJobsService.getMeritProfile(this.cnic).subscribe((res: any) => {
      if (res) {
        this._onlineJobsService.getMeritActiveDesignation(res.MeritsActiveDesignationId).subscribe((data: any) => {
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
  /* 
    public fetchMeritInfo() {
      this._onlineJobsService.getMeritProfile(this.cnic).subscribe((res: any) => {
        if (res) {
          this._onlineJobsService.getMeritActiveDesignation(res.MeritsActiveDesignationId).subscribe((data: any) => {
            this.meritActiveDesignation = data;
            this.merit = res;
            this.getPreferences();
            this.getVacantPlaces();
            if (data.IsActive == 'Y') {
              this._rootService.getProfileByCNIC(this.cnic).subscribe((x) => {
                if (x) {
                  this.profile = x;
                }
              }, err => {
                console.log(err);
              });
            } else {
              //this.router.navigate(['/ppsc/review']);
            }
          }, err => {
            console.log(err);
          });
        }
      }, err => {
        console.log(err);
      });
    } */

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
    this._onlineJobsService.getPreferences(this.merit.Id).subscribe((res: any) => {
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
      this._onlineJobsService.removePreferences({ HfmisCode: HfmisCode, MeritId: this.merit.Id }).subscribe((res: any) => {
        if (res) {
          this.getPreferences();
          //this.preferences = res;
        }
      }, err => {
        console.log(err);
      });
    }
  }
  /*  public removePreferences(districtName) {
     let hfmisCode = this.dropDowns.districtsData.find(x => x.Name === districtName).Code;
     if (hfmisCode) {
       this._onlineJobsService.removePreferences({ MeritId: this.merit.Id, hfmisCode: hfmisCode }).subscribe((res: any) => {
         if (res) {
           this.getPreferences();
           //this.preferences = res;
         }
       }, err => {
         console.log(err);
       });
     }
   } */
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
      this._onlineJobsService.addPreferences({ MeritId: this.merit.Id, hfmisCode: this.hfmisCode, desigId: this.hf_Id }).subscribe((res: any) => {
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

  public saveAllPreferences() {
    this.addingAllPrefs = true;
    this._onlineJobsService.saveAllPreferences({ MeritId: this.merit.Id }).subscribe((res: any) => {
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
            background-color: rgb(254, 217, 102);
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
