import { Component, OnInit } from '@angular/core';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { Subscription } from 'rxjs/Subscription';
import { ActivatedRoute, Router } from '@angular/router';
import { RootService } from '../../../_services/root.service';
import { CookieService } from '../../../_services/cookie.service';
import { OnlinePostApplyService } from '../online-post-apply.service';
import { LocalService } from '../../../_services/local.service';

@Component({
  selector: 'app-advertisement',
  templateUrl: './advertisement.component.html',
  styles: []
})
export class AdvertisementComponent implements OnInit {
  public dropDowns: DropDownsHR = new DropDownsHR();
  private subscription: Subscription;
  public cnic: string = '';
  public profile: any = {};
  public promotionApply: any = {};
  public hfmisCode: string = null;
  public hf_Id: number = 0;
  public preferences: any[] = [];
  public excludeDistrictsOrigional: string[] = ['Lahore', 'Islamabad'];
  public excludeDistricts: string[] = [];
  public profilePreferences: any[] = [];
  public addingPrefs: boolean = false;
  public showOther: boolean = false;
  public submitted: boolean = false;
  constructor(private _rootService: RootService,
    private router: Router,
    private _cookieService: CookieService,
    private _localService: LocalService,
    private _onlinePostApplyService: OnlinePostApplyService, private route: ActivatedRoute) { }

  ngOnInit() {
    /* this.fetchParams();
    this.getDistricts();
    this.getDesignations(); */
  }
  public setDesignationId(id) {
    this._localService.set('desgId', id);
    console.log(id);
    console.log(this._localService.get('desgId'));
    if(this._localService.get('desgId')) {
      this.router.navigate(['/administrative/account']);
    }
  }
  private fetchParams() {
    this.cnic = this._cookieService.getCookie('cnicussrpublic');
    if (this.cnic) {
      this.fetchMeritInfo();
    }
  }
  public fetchMeritInfo() {
    this._onlinePostApplyService.getPromotionApplyData(this.cnic).subscribe((res: any) => {
      if (res && res.Profile && res.PromotionApply) {
        this.profile = res.Profile;
        this.promotionApply = res.PromotionApply;
      }
    }, err => {
      console.log(err);
    });
  }
  public getDistricts() {
    this._rootService.getDistricts('0').subscribe((res: any) => {
      if (res) {
        this.dropDowns.districts = res;
        this.dropDowns.districtsData = this.dropDowns.districts;
      }
    }, err => {
      console.log(err);
    });
  }
  public getDesignations() {
    this._rootService.getDesignationCadreWise(14).subscribe((res: any) => {
      if (res) {
        this.dropDowns.designations = res;
        this.dropDowns.designationsData = this.dropDowns.designations;
      }
    }, err => {
      console.log(err);
    });
  }
  public getPreferences() {
    this._onlinePostApplyService.getPreferences(this.profile.Id).subscribe((res: any) => {
      if (res) {
        this.preferences = res;
        this.addingPrefs = false;
        this.setDistricts();
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
  public removePreferences(districtName) {
    let hfmisCode = this.dropDowns.districtsData.find(x => x.Name === districtName).Code;
    if (hfmisCode) {
      this._onlinePostApplyService.removePreferences({ profileId: this.profile.Id, hfmisCode: hfmisCode }).subscribe((res: any) => {
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
    if (filter == 'designation') {
      this.promotionApply.ToDesignation_Id = value.Id;
    }
  }
  public addPreference() {
    if (this.hfmisCode && this.hfmisCode != '0') {
      this.addingPrefs = true;
      this._onlinePostApplyService.addPreferences({ profileId: this.profile.Id, hfmisCode: this.hfmisCode }).subscribe((res: any) => {
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
  public submitApplication() {
    if (this.promotionApply.ToDesignation_Id) {
      this.addingPrefs = true;
      this._onlinePostApplyService.submitSMOApplication(this.promotionApply).subscribe((res: any) => {
        if (res) {
          this.submitted = true;
        }
      }, err => {
        console.log(err);
      });
    }
  }
}
