import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { Subject } from 'rxjs/Subject';
import { RootService } from '../../../_services/root.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { CookieService } from '../../../_services/cookie.service';
import { SwmoPromotionService } from '../swmo-promotion.service';


@Component({
  selector: 'app-swmo-promo-form',
  templateUrl: './swmo-promo-form.component.html',
})
export class SwmoPromoFormComponent implements OnInit {
  @ViewChild('photoRef', { static: false }) public photoRef: any;
  public swmoPromotion: any = {};
  public swmoPromotionDetail: any[] = [];
  public swmoPromotionDetailObj: any = {};
  public swmoPromotionDTO: any =
    {
      swmoPromotion: {},
      swmoPromotionDetail: [],
    };
  public Templist: any[] = [];
  public HFListPreference: any[] = [];
  private subscription: Subscription;
  public dropDowns: DropDownsHR = new DropDownsHR();
  public cnic: string = '';
  public cnicMask: string = "00000-0000000-0";
  public mobileMask: string = "0000-0000000";
  public photoSrc = '';
  public photoFile: any[] = [];
  public services: any[] = [];
  public merit: any = {};
  public service: any = {};
  public profile: any;
  public radnom: number = Math.random();
  public inputChange: Subject<any>;
  public showMeritForm: boolean = false;
  public loadingCNIC: boolean = false;
  public isUploading: boolean = false;
  public savingProfile: boolean = false;
  public existingProfile: boolean = false;
  public portalClosed: boolean = false;
  public searchEvent = new Subject<any>();
  public HFListAll: any[] = [];
  public FilteredHFListPref: any[] = [];


  constructor(private route: ActivatedRoute,
    private _cookieService: CookieService,
    private router: Router,
    private _rootService: RootService,
    private _swmopromotionService: SwmoPromotionService,
    private _authenticationService: AuthenticationService,
  ) { }

  ngOnInit() {
    // this.getMerit();
    this.getDesignations();
    this.getDistrict();
    this.getHFLists();
    this.GetHFListsPreference();
    this.getProfile();
    this.merit = { Designation_Id: 302 };
    this.dropDowns.selectedFiltersModel.designation = { Name: 'Charge Nurse', Id: 302 };
  }

  // public checkCandidate() {
  //   if (this.swmoPromotion.CNIC && this.swmoPromotion.CNIC[12] != ' ') {
  //     this._swmopromotionService.getMerit(this.swmoPromotion.CNIC).subscribe((res: any) => {

  //       console.log('merit: ', res);
  //       if (res.Id != null) {
  //         this.swmoPromotion.MeritId = res.Id;
  //         this.swmoPromotion.Name = res.Name;
  //         this.swmoPromotion.PhoneNumber = res.MobileNumber;
  //         this.swmoPromotion.CNIC = res.CNIC;
  //         this.getProfile();
  //       }
  //     }, err => { console.log(err); }
  //     );
  //   }
  // }

  public onSubmit() {
    this.savingProfile = true;
    if (this.swmoPromotion.PermanentSince) {
      this.swmoPromotion.PermanentSince = this.swmoPromotion.PermanentSince.toDateString();
    }
    this.swmoPromotionDTO.swmoPromotion = this.swmoPromotion;
    this.swmoPromotionDTO.swmoPromotionDetail = this.swmoPromotionDetail;
    console.log('dto: ', this.swmoPromotionDTO);
    this._swmopromotionService.saveSwmoPromotion(this.swmoPromotionDTO).subscribe((res: any) => {
      if (res) {
        alert('Record Saved Successfully');
        this.resetForm();
      }
    }, err => {
      this.handleError(err);
    });

  }
  public resetForm() {
    this.swmoPromotion = {};
    this.dropDowns.selectedFiltersModel.designations = this.dropDowns.defultFiltersModel.designation;
    this.dropDowns.selectedFiltersModel.healthFacilities = this.dropDowns.defultFiltersModel.healthFacility;
    this.dropDowns.selectedFiltersModel.districts = this.dropDowns.defultFiltersModel.district;
    this.Templist = [];
    this.photoFile = null;
    this.photoSrc = null;
    window.location.reload();
  }
  public addService() {
    this.services.push(this.service);
    this.service = {};
  }

  public getDesignations = () => {
    console.log('in desgin...');
    this.dropDowns.designations = [];
    this.dropDowns.designationsData = [];
    this._swmopromotionService.getDesignations().subscribe((res: any) => {
      if (res) {
        console.log('Desigss: ', res);
        this.dropDowns.designations = res;
        this.dropDowns.designationsData = this.dropDowns.designations.slice();
      }
    },
      err => { this.handleError(err); }
    );
  }
  public getProfile() {
    
    if (this.swmoPromotion.CNIC && this.swmoPromotion.CNIC[12] != ' ') {
      this._swmopromotionService.getProfile(this.swmoPromotion.CNIC).subscribe((res: any) => {
        if (res && res.Id) {
          this.profile = res;
          console.log('profile: ', res);
          if (this.profile != null) {
            debugger;
            
            this._swmopromotionService.getPreferedHFL(this.swmoPromotion.CNIC).subscribe((ress: any) => {
              if(res && res.Id){
                debugger;
                this.Templist = ress;
                this.swmoPromotion.Name = ress[0].Name;
                this.swmoPromotion.DistrictCode = ress[0].DistrictCode;
                this.swmoPromotion.PhoneNumber = ress[0].PhoneNumber;
                this.swmoPromotion.PermanentSince = new Date(ress[0].PermanentSince);
                this.Templist.forEach(x => {
                  this.swmoPromotionDetail.push({ PreferenceHfId: x.PreferenceHfId});
                });
              }
              
            });

            this.swmoPromotion.ProfileId = this.profile.Id;
            this.swmoPromotion.DesignationId = this.profile.Designation_Id;
            this.swmoPromotion.HfId = this.profile.HealthFacility_Id;
            this.dropDowns.selectedFiltersModel.designations = { Name: this.profile.Designation_Name, Id: this.profile.Designation_Id };
            this.dropDowns.selectedFiltersModel.healthFacilities = { Name: this.profile.HealthFacility + ', ' + this.profile.Tehsil + ', ' + this.profile.District, Id: this.profile.HealthFacility_Id };
            let district = this.dropDowns.districts.find(x => x.Name == this.profile.District);
            this.dropDowns.selectedFiltersModel.districts = { Name: district.Name, Code: district.Code };
            this.swmoPromotion.DistrictCode = district.Code;
            console.log('swmo: ', this.swmoPromotion);
          }
        }
      }, err => {
        this.handleError(err);
      });
    }
  }

  public getHFLists = () => {
    this.dropDowns.healthFacilities = [];
    this.dropDowns.healthFacilitiesData = [];
    this._swmopromotionService.getHFLists().subscribe((res: any) => {
      if (res) {
        console.log('current HFs: ', res);
        this.dropDowns.healthFacilities = res
        this.HFListAll = res;
        this.dropDowns.healthFacilitiesData = this.dropDowns.healthFacilities.slice();
      }
    },
      err => { this.handleError(err); }
    );
  }
  public GetHFListsPreference = () => {
    this._swmopromotionService.GetHFListsPreference().subscribe((res: any) => {
      if (res) {
        this.HFListPreference = res;
        this.FilteredHFListPref = res;
      }
    },
      err => { this.handleError(err); }
    );
  }
  public getDistrict = () => {
    this.dropDowns.districts = [];
    this.dropDowns.districtsData = [];
    this._swmopromotionService.getDistrict().subscribe((res: any) => {
      if (res) {
        console.log('HF', res);
        this.dropDowns.districts = res
        this.dropDowns.districtsData = this.dropDowns.districts.slice();
      }
    },
      err => { this.handleError(err); }
    );
  }
  public getMerit() {
    this._swmopromotionService.getMerit(this.swmoPromotion.CNIC).subscribe((res: any) => {
      if (res && res.Id) {
        this.swmoPromotion.Name = res.Name;
        this.swmoPromotion.PhoneNumber = res.MobileNumber;
        this.swmoPromotion.CNIC = res.CNIC;
      }
    }, err => {
      this.handleError(err);
    });

  }

  public addPreferenceHF(Id: number) {
    debugger;
    if (Id != 0) {
      let Hf = this.HFListPreference.find(x => x.Id === Id);
      let PreferenceHfId = Hf.Id;
      if(this.Templist.length > 0)
      {
        let check =  this.Templist.find(x => x.Id === PreferenceHfId);
        if(!check)
        {
          this.Templist.push(Hf);
          this.swmoPromotionDetail.push({ PreferenceHfId: PreferenceHfId });
        }
      }
      else{
        this.Templist.push(Hf);
        this.swmoPromotionDetail.push({ PreferenceHfId: PreferenceHfId });
      }
    }
  }

  public removePreferenceHF(index, Id: number) {

    this.swmoPromotionDetail = this.swmoPromotionDetail.filter(({ PreferenceHfId }) => PreferenceHfId !== Id);
    this.Templist.splice(index, 1);
  }
  public dropdownValueChanged = (value, filter) => {

    if (!value) {
      return;
    }
    if (filter == 'district') {
      this.swmoPromotion.DistrictCode = value.Code;
    }
    if (filter == 'HF') {
      this.swmoPromotion.HfId = value.Id;
    }
    if (filter == 'designation') {
      this.swmoPromotion.DesignationId = value.Id;
    }
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
  private handleError(err: any) {
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }

  public readUrl(event: any, filter: string) {
    if (event.target.files && event.target.files[0]) {
      if (filter == 'pic') {
        this.photoFile = [];
        let inputValue = event.target;
        this.photoFile = inputValue.files;
        var reader = new FileReader();
        reader.onload = ((event: any) => {
          this.photoSrc = event.target.result;
        }).bind(this);
        reader.readAsDataURL(event.target.files[0]);
      }
    }
  }

  public uploadBtn(filter: string) {
    if (filter == 'pic') {
      this.photoRef.nativeElement.click();
    }
  }

  handleFilter(value) {
    this.HFListAll = this.dropDowns.healthFacilitiesData.filter(
      (s) => s.FullName.toLowerCase().indexOf(value.toLowerCase()) !== -1
    );
  }
  
  handleFilterPref(value) {
    this.FilteredHFListPref = this.HFListPreference.filter(
      (s) => s.FullName.toLowerCase().indexOf(value.toLowerCase()) !== -1
    );
  }
}
