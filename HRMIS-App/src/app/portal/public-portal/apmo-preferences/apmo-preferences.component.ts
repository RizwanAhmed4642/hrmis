import { Component, OnInit } from '@angular/core';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { PublicPortalService } from '../public-portal.service';
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
  public profile: any;
  public addingPrefs: boolean = false;
  public showOther: boolean = false;
  public showProfile: boolean = false;
  public saving: boolean = false;
  constructor(private _rootService: RootService, private _publicPortalService: PublicPortalService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.fetchParams();
  }
  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('cnic')) {
          this.cnic = params['cnic'];
          this.getProfile();
        }
      }
    );
  }
  public getProfile() {
    this._publicPortalService.getProfileByCNIC(this.cnic).subscribe((x) => {
      if (x) {
        this.profile = x;
        this.showProfile = true;
        this.getFacilities();
        this.getServiceTemps();
        this.getPreferences();
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
    /* this.profile.Designation_Id = this.profile.Designation_Id == 2404 ? 1320 : this.profile.Designation_Id; */
    this._publicPortalService.getHFList(this.profile.Designation_Id).subscribe((res: any) => {
   
      if (res) {
        this.dropDowns.healthFacilities = res;
        
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
  public dropdownValueChanged = (value, filter) => {
    this.showOther = false;
    this.other = '';
    if (filter == 'healthFacility') {
      this.hf_Id = value.HF_Id;
      if (this.hf_Id == 232323) {
        this.showOther = true;
      }
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
}
