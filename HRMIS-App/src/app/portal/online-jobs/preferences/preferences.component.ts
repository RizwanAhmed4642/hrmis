import { Component, OnInit, OnDestroy } from '@angular/core';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { Subscription } from 'rxjs/Subscription';
import { ActivatedRoute, Router } from '@angular/router';
import { OnlineJobsService } from '../online-jobs.service';
import { RootService } from '../../../_services/root.service';
import { CookieService } from '../../../_services/cookie.service';

@Component({
  selector: 'app-preferences',
  templateUrl: './preferences.component.html',
  styleUrls: ['./preferences.component.scss']
})
export class PreferencesComponent implements OnInit, OnDestroy {

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
  public hfsOrigional: any[] = [];
  public designations: any[] = [];
  public designationsOrigional: any[] = [];
  public preferencesOrigional: any[] = [];
  public preferences: any[] = [];
  public excludeDistrictsOrigional: string[] = ['Lahore', 'Islamabad'];
  public excludeDistricts: string[] = [];
  public meritPreferences: any[] = [];
  public loading: boolean = false;
  public addingPrefs: boolean = false;
  public addingAllPrefs: boolean = false;
  public showOther: boolean = false;
  constructor(private _rootService: RootService,
    private _cookieService: CookieService, private router: Router,
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
        this.getPHFMCVacants();
      }

    }, err2 => {
      this.loading = true;
      console.log(err2);
    });
  }

  public apply(item) {
    console.log(item);
    
    item.loading = true;
    this.application.Applicant_Id = this.applicant.Id;
    this.application.Designation_Id = this.desigId;
    this.application.HF_Id = item.Id;
    this._onlineJobsService.saveApplication(this.application).subscribe((res3: any) => {
      if (res3 && res3.Id) {
        item.applied = true;
      } else if (res3 && res3 == "limit") {
        item.applied = "limit";
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
  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
