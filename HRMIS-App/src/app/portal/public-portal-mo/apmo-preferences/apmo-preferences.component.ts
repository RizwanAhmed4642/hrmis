import { Component, OnInit } from '@angular/core';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { PublicPortalMOService } from '../public-portal-mo.service';
import { Subscription } from 'rxjs/Subscription';
import { ActivatedRoute } from '@angular/router';
import { RootService } from '../../../_services/root.service';

@Component({
  selector: 'app-apmo-preferences',
  templateUrl: './apmo-preferences.component.html',
  styles: []
})
export class ApmoPreferencesComponent implements OnInit {
  public dropDowns: DropDownsHR = new DropDownsHR();
  private subscription: Subscription;
  public cnic: string = '';
  public other: string = '';
  public hf_Id: number = 0;
  public preferences: any[] = [];
  public services: any[] = [];
  public merit: any = {};
  public service: any = {};
  public profile: any = {};
  public addingPrefs: boolean = false;
  public showOther: boolean = false;
  public showProfile: boolean = true;
  public saving: boolean = false;
  public profileSaved: boolean = false;
  public submiting: boolean = false;
  public accepted: boolean = false;
  public cnicMask: string = "00000-0000000-0";
  public birthDateMax: Date = new Date();
  public profileObject: any = {};
  public domiciles: any[] = [];
  public designations: any[] = [];
  public statuses: any[] = [{ Id: 2, Name: 'Active' },
  { Id: 15, Name: 'On Deputation' },
  { Id: 17, Name: 'Leave' }];
  public healthFacilities: any[] = [];
  public healthFacilitiesAll: any[] = [];
  constructor(private _rootService: RootService, private _publicPortalService: PublicPortalMOService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.fetchParams();
  }
  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('cnic')) {
          this.cnic = params['cnic'];
          this.getProfile();
          this.getDomiciles();

        }
      }
    );
  }
  public getProfile() {
    /*  this._publicPortalService.getProfileByCNIC(this.cnic).subscribe((x) => {
       if (x) {
         this.profile = x;
         this.showProfile = true;
         this.getFacilities();
         this.getServiceTemps();
         this.getPreferences();
         this.getApplicationMO();
       }
     }, err => {
       console.log(err);
     }); */

    this._publicPortalService.getPromotedCandidate(this.cnic).subscribe((res: any) => {
      if (res) {
        if(res.profile){
          this.profile = res.profile;
          this.profile.DateOfBirth = new Date(res.profile.DateOfBirth);
          this.profile.DateOfRegularization = new Date(res.profile.DateOfRegularization);
        }else {
          this.profile = {};
        }
        this.getVacancy(res.candidate.DesignationId);
        this.getDesignations();
        this.getHealthFacilities();
        this.showProfile = true;
        /*if (res.Designation_Id && res.EmpMode_Id && res.PresentPostingDate) {
             if (res.Designation_Id != 802) {
              this.notEligible = true;
            }
            if (res.EmpMode_Id != 13) {
              this.notEligible = true;
            }
            let serviceDate: Date = res.PresentPostingDate as Date;
            if (this.datediff(serviceDate, new Date()) >= 1095) {
              this.notEligible = true;
            } 
            if (!this.notEligible) {
              this.getPassword();
              return;
            }
        }*/
        this.getPreferences();
        this.getApplicationMO();
        return;
        //this.notEligible = true;
      }
      //this.notEligible = true;
    }, err => {
      console.log(err);
    });

  }

  public getApplicationMO() {
    this._publicPortalService.getApplicationMO(this.profile.Id).subscribe((x) => {
      if (x) {
        this.accepted = true;
      }
    }, err => {
      console.log(err);
    });
  }
  public getServiceTemps() {
    this.services = [];
    this._publicPortalService.getServiceTemps(this.profile.Id).subscribe((res: any) => {
      if (res) {
        this.services = res;
      }
    }, err => {
      console.log(err);
    });

  }
  public submitApplicationMO() {
    this.submiting = true;
    this._publicPortalService.submitApplicationMO(this.profile.Id, this.profile.MobileNumber).subscribe((res: any) => {
      if (res) {
        this.submiting = false;
        this.accepted = true;
      }
    }, err => {
      console.log(err);
    });
  }
  public removeServiceTemp(Id: number) {
    if (confirm('Are you sure?')) {
      this._publicPortalService.removeServiceTemp(Id).subscribe((res: any) => {
        if (res) {
          this.getServiceTemps();
        }
      }, err => {
        console.log(err);
      });
    }
  }




  public saveServiceTemp() {
    this.saving = true;
    this.service.Profile_Id = this.profile.Id;
    this.service.CNIC = this.profile.CNIC;
    if (this.service.ToDate) {
      this.service.ToDate = this.service.ToDate.toDateString();
    }
    if (this.service.FromDate) {
      this.service.FromDate = this.service.FromDate.toDateString();
    }
    this._publicPortalService.saveServiceTemp(this.service).subscribe((data) => {
      if (data) {
        this.service = {};
        this.getServiceTemps();
        this.saving = false;
      }
    });
  }
  public getFacilities() {
    this._publicPortalService.getHFList(this.profile.Designation_Id).subscribe((res: any) => {
      console.log(res);
      if (res) {
        this.dropDowns.healthFacilities = res;
        this.dropDowns.healthFacilitiesData = this.dropDowns.healthFacilities;
        console.log(this.dropDowns.healthFacilitiesData);
        //this.dropDowns.healthFacilities.unshift({ Id: 232323, FullName: 'Other' });
      }
    }, err => {
      console.log(err);
    });
  }

  public getDomiciles() {
    this._rootService.getDomiciles().subscribe((res: any) => {
      console.log(res);
      if (res) {
        this.domiciles = res;
        console.log(this.domiciles);
      }
    }, err => {
      console.log(err);
    });
  }

  public getDesignations() {
    this._rootService.getDesignations().subscribe((res: any) => {
      console.log(res);
      if (res) {
        this.designations = [
          { Id: 802, Name: 'Medical Officer' },
          { Id: 1320, Name: 'Women Medical Officer' },
          { Id: 2525, Name: 'OPS' }
        ];
        //this.designations = res;
        console.log(this.designations);

      }
    }, err => {
      console.log(err);
    });
  }

  public getHealthFacilities() {
    this._rootService.getHealthFacilitiesAll().subscribe((res: any) => {
      console.log(res);
      if (res) {
        this.healthFacilitiesAll = res;
        this.healthFacilities = this.healthFacilitiesAll;
        this.healthFacilitiesAll.forEach(hf => {
          if (hf.Id == this.profile.HealthFacility_Id) {
            this.hf_Id = this.profile.HealthFacility_Id;
            this.profile.HF_Id = this.profile.HealthFacility_Id;
            this.profile.designationId = this.profile.Designation_Id;
            this.dropDowns.selectedFiltersModel.healthFacilityPref = { FullName: this.profile.HealthFacility + ', ' + this.profile.Tehsil + ', ' + this.profile.District, Id: this.profile.HealthFacility_Id };
          }
        });
        console.log(this.healthFacilitiesAll);
      }
    }, err => {
      console.log(err);
    });
  }

  public saveConsultantProfile() {
    this.saving = true;
    if (this.profile.DateOfBirth != undefined) {
      this.profile.DateOfBirth = new Date(this.profile.DateOfBirth).toDateString();
    }
    if (this.profile.DateOfRegularization != undefined) {
      this.profile.DateOfRegularization = new Date(this.profile.DateOfRegularization).toDateString();
    }
    this._publicPortalService.saveConsultantProfile(this.profile).subscribe((data) => {
      if (data) {
        this.saving = false;
        this.profileSaved = true;
      }
    });
  }

  public getVacancy(d) {
    this._publicPortalService.getVacancy(d).subscribe((res: any) => {
      console.log(res);
      if (res) {
        this.dropDowns.healthFacilities = res;
        this.dropDowns.healthFacilitiesData = this.dropDowns.healthFacilities;
        console.log(this.dropDowns.healthFacilitiesData);
        //this.dropDowns.healthFacilities.unshift({ Id: 232323, FullName: 'Other' });
      }
    }, err => {
      console.log(err);
    });
  }
  public getPreferences() {
    this._publicPortalService.getPreferences(this.cnic).subscribe((res: any) => {
      if (res) {
        this.preferences = res;
      }
    }, err => {
      console.log(err);
    });
  }
  public removePreferences(id: number) {
    this._publicPortalService.removePreferences(id).subscribe((res: any) => {
      if (res) {
        this.getPreferences();
        //this.preferences = res;
      }
    }, err => {
      console.log(err);
    });
  }
  public handleFilter = (value, filter) => {
    if (filter == 'healthFacility') {
      this.dropDowns.healthFacilitiesData = this.dropDowns.healthFacilities.filter((s: any) => s.HFName.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
    if (filter == 'hf') {
      this.healthFacilities = this.healthFacilitiesAll.filter((s: any) => s.FullName.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }

  }
  public dropdownValueChanged = (value, filter) => {
    this.showOther = false;
    this.other = '';
    if (filter == 'healthFacility') {
      this.hf_Id = value.HF_Id;
      if (this.hf_Id == 232323) {
        this.showOther = true;
      }
    }
    if (filter == 'currentHealthFacility') {
      this.profile.HF_Id = value.Id;
      this.hf_Id = value.Id;
    }
    if (filter == 'designation') {
      this.profile.designationId = value.Id;
      if (value.Id == 2525) {
        this._publicPortalService.addPreferences({ CNIC: this.cnic, HF_Id: this.hf_Id, Other: this.other }).subscribe((res: any) => {
          if (res) {
            this.showOther = false;
            this.other = '';
            this.hf_Id = 0;
            this.dropDowns.selectedFiltersModel.healthFacilityPref = this.dropDowns.defultFiltersModel.healthFacilityPref;
            this.getPreferences();
          }
        }, err => {
          console.log(err);
        });
      }
    }
    if (filter == 'domicile') {
      this.profile.Domicile_Id = value.Id;
    }
    if (filter == 'status') {
      this.profile.Status_Id = value.Id;
    }
  }
  public addPreference() {
    this._publicPortalService.addPreferences({ CNIC: this.cnic, HF_Id: this.hf_Id, Other: this.other }).subscribe((res: any) => {
      if (res) {
        this.showOther = false;
        this.other = '';
        this.hf_Id = 0;
        this.dropDowns.selectedFiltersModel.healthFacilityPref = this.dropDowns.defultFiltersModel.healthFacilityPref;
        this.getPreferences();
      }
    }, err => {
      console.log(err);
    });
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
}
