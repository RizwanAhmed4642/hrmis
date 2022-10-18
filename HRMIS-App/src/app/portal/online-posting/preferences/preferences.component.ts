import { Component, OnInit } from '@angular/core';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { Subscription } from 'rxjs/Subscription';
import { ActivatedRoute, Router } from '@angular/router';
import { OnlinePostingService } from '../online-porting.service';
import { RootService } from '../../../_services/root.service';
import { CookieService } from '../../../_services/cookie.service';

@Component({
  selector: 'app-preferences',
  templateUrl: './preferences.component.html',
  styles: []
})
export class PreferencesComponent implements OnInit {
  public dropDowns: DropDownsHR = new DropDownsHR();
  private subscription: Subscription;
  public cnic: string = '';
  public merit: any = {};
  public pgForm: any = {};
  public meritActiveDesignation: any = {};
  public profile: any = null;
  public hfmisCode: string = null;
  public hf_Id: number = 0;
  public hfs: any[] = [];
  public hfsOrigional: any[] = [];
  public preferencesOrigional: any[] = [];
  public pgHealthFacilities: any[] = [];
  public pgHealthFacilitiesOrigional: any[] = [];
  public specializations: any[] = [];
  public specializationsOrigional: any[] = [];
  public preferences: any[] = [];
  public excludeDistrictsOrigional: string[] = ['Lahore', 'Islamabad'];
  public excludeDistricts: string[] = [];
  public meritPreferences: any[] = [];
  public addingPrefs: boolean = false;
  public isPG: boolean = false;
  public isPGSaved: boolean = true;
  public isPgSaving: boolean = false;
  public addingAllPrefs: boolean = false;
  public showOther: boolean = false;
  constructor(private _rootService: RootService,
    private _cookieService: CookieService, private router: Router,
    private _onlinePostingService: OnlinePostingService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.fetchParams();
    this.cnic = this._cookieService.getCookie('cnicussrpublic');
  }
  private fetchParams() {
    this.cnic = this._cookieService.getCookie('cnicussrpublic');
    if (this.cnic) {
      this.fetchMeritInfo();
    }
  }
  public fetchMeritInfo() {
    this._onlinePostingService.getMeritProfile(this.cnic).subscribe((res: any) => {
      if (res) {
        this._onlinePostingService.getMeritActiveDesignation(res.MeritsActiveDesignationId).subscribe((data: any) => {
          this.merit = res;
          this.meritActiveDesignation = data;
          if (data.IsActive == 'Y') {
            this.getPGHealthFacilities();
            this.getVacantPlaces();
            this.getSpecializations();
           // this.checkPg();
            this.getPreferences();
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
  onPgYesNo(isPG) {
    this.pgForm.isPG = isPG;
    if (!this.pgForm.isPG) {
      this.yesNo();
    }
  }
  setAdmissionFile(event: any) {
    this.pgForm.pgFile = event.target.files[0];
  }
  checkPg() {
    this._onlinePostingService.isPGInit(this.merit.Id).subscribe((x: any) => {
      if (x === 404) {
        this.isPG = false;
        this.isPGSaved = false;
      }
      else {
        this.isPG = x.isPG;
        this.pgForm = x;
        this.isPGSaved = true;
        if (this.pgForm.isPG) {
          this.getDistricts();
        } else {
          this.getVacantPlaces();
        }
        this.getPreferences();
      }
    });
  }
  yesNo() {
    let dateString = null;
    if (this.pgForm.pgFrom instanceof Date) {
      dateString = this.pgForm.pgFrom.toDateString();
    }
    this.isPgSaving = true;
    let fd = new FormData();
    if (this.pgForm.pgFile) { fd.append("file", this.pgForm.pgFile); }
    this._onlinePostingService.isPGT(this.merit.Id, this.pgForm.isPG, dateString,
      this.pgForm.HealthFacilities_Id, this.pgForm.Specialization_Id,
      this.pgForm.pgHFName, fd).subscribe((x: any) => {
        this.isPgSaving = false;
        this.isPGSaved = true;
        if (x) {
          this.isPG = x.isPG;
          this.pgForm = x;
          this.checkPg();
        }
      });
  }


  /* 
    public fetchMeritInfo() {
      this._onlinePostingService.getMeritProfile(this.cnic).subscribe((res: any) => {
        if (res) {
          this._onlinePostingService.getMeritActiveDesignation(res.MeritsActiveDesignationId).subscribe((data: any) => {
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

  public getDistricts() {
    this._rootService.getDistricts('0').subscribe((res: any) => {
      if (res) {
        this.dropDowns.districts = res;
        this.dropDowns.districtsData = this.dropDowns.districts;
        this.setDistricts();
      }
    }, err => {
      console.log(err);
    });
  }
  public getHFOpened() {
    this._onlinePostingService.getHFOpened(this.merit.Designation_Id).subscribe((res: any) => {
      if (res) {
        this.hfsOrigional = res;
        this.hfs = this.hfsOrigional;
      }
    }, err => {
      console.log(err);
    });
  }
  public getSpecializations() {
    this._rootService.getSpecializations().subscribe((res: any) => {
      if (res) {
        this.specializationsOrigional = res;
        this.specializations = this.specializationsOrigional;
      }
    }, err => {
      console.log(err);
    });
  }
  public getVacantPlaces() {
    this._rootService.getVacantPlaces(this.merit.Designation_Id, this.profile ? this.profile.Id ? 1 : 0 : 0).subscribe((res: any) => {
      if (res) {
        console.log('hfs: ',res);
        this.hfsOrigional = res;
        this.hfs = this.hfsOrigional;
        /*  this.dropDowns.districts = res;
         this.dropDowns.districtsData = this.dropDowns.districts; */
      }
    }, err => {
      console.log(err);
    });
  }
  public getPGHealthFacilities() {
    this._rootService.getHealthFacilitiesByTypeCode(['016', '043']).subscribe((res: any) => {
      if (res) {
        this.pgHealthFacilitiesOrigional = res;
        this.pgHealthFacilities = this.pgHealthFacilitiesOrigional;
      }
    }, err => {
      console.log(err);
    });
  }
  public getPreferences() {
    if (this.pgForm.isPG) {
      this._onlinePostingService.getPgPreferences(this.merit.Id).subscribe((res: any) => {
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
    } else {
      this._onlinePostingService.getPreferences(this.merit.Id).subscribe((res: any) => {
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
  }
  public setDistricts() {
    this.excludeDistricts = ['Lahore', 'Islamabad'];
    this.dropDowns.districts = this.dropDowns.districtsData.filter(
      function (e) {
        return this.indexOf(e.Name) < 0;
      },
      this.excludeDistricts
    );
    this.dropDowns.districtsData = this.dropDowns.districts;
  }
  public removePreferences(HfmisCode) {
    console.log({ HfmisCode: HfmisCode, MeritId: this.merit.Id });

    if (confirm('Are you sure?')) {
      this._onlinePostingService.removePreferences({ HfmisCode: HfmisCode, MeritId: this.merit.Id }).subscribe((res: any) => {
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
       this._onlinePostingService.removePreferences({ MeritId: this.merit.Id, hfmisCode: hfmisCode }).subscribe((res: any) => {
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
      this.hf_Id = 0;
      this.hfmisCode = value.Code;
    }
    if (filter == 'hf') {
      this.hf_Id = value.HF_Id;
      this.hfmisCode = value.HFMISCode;
    }
    if (filter == 'ghf') {
      this.pgForm.HealthFacilities_Id = value.Id;
    }
    if (filter == 'sp') {
      this.pgForm.Specialization_Id = value.Id;
    }
  }

  public handleFilter = (value, filter) => {
    if (filter == 'district') {
      this.dropDowns.districtsData = this.dropDowns.districts.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
    if (filter == 'hf') {
      this.hfs = this.hfsOrigional.filter((s: any) => s.FullName.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
    if (filter == 'ghf') {
      this.pgHealthFacilities = this.pgHealthFacilitiesOrigional.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
    if (filter == 'sp') {
      this.specializations = this.specializationsOrigional.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
  }
  public addPreference() {
    if (this.hfmisCode && this.hfmisCode != '0') {
      this.addingPrefs = true;
      this._onlinePostingService.addPreferences({ MeritId: this.merit.Id, hfmisCode: this.hfmisCode, hfId: this.hf_Id }).subscribe((res: any) => {
        if (res) {
          this.hf_Id = null;
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
    this._onlinePostingService.saveAllPreferences({ MeritId: this.merit.Id }).subscribe((res: any) => {
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
