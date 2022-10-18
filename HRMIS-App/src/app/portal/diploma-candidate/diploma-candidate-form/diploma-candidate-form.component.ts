import { Component, OnInit, ViewChild } from '@angular/core';
import { ReportingRoutingModule } from '../../../modules/reporting/reporting-routing.module';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { Subject } from 'rxjs/Subject';

import { debounceTime } from 'rxjs/operators/debounceTime';
import { RootService } from '../../../_services/root.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { CookieService } from '../../../_services/cookie.service';
import { DiplomaCandidateService } from '../diploma-candidate.service';
import { Alert } from 'selenium-webdriver';



@Component({
  selector: 'app-diploma-candidate-form',
  templateUrl: './diploma-candidate-form.component.html',
})
export class DiplomaCandidateFormComponent implements OnInit {
  @ViewChild('photoRef', { static: false }) public photoRef: any;
  public diplomaCandidate: any = {};
  public diplomaCandidateDetail: any[] = [];
  public diplomaCandidateDetailObj: any = {};
  public diplomaCandidateDTO: any =
    {
      meritCandidate: {},
      meritCandidateDetail: [],
    };
  public Templist: any[] = [];
  public HFListPreference: any[] = [];
  private subscription: Subscription;
  public dropDowns: DropDownsHR = new DropDownsHR();
  public cnic: string = '';
  public cnicMask: string = "00000-0000000-0";
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
  public portalClosed: boolean = true;

  constructor(private route: ActivatedRoute,
    private _cookieService: CookieService,
    private router: Router,
    private _rootService: RootService,
    private _diplomaCandidateService: DiplomaCandidateService,
    private _authenticationService: AuthenticationService,
  ) { }

  ngOnInit() {
    // this.getMerit();
    this.getDesignations();
    this.getDistrict();
    this.getHFLists();
    this.GetHFListsPreference();
    this.merit = { Designation_Id: 302 };
    this.dropDowns.selectedFiltersModel.designation = { Name: 'Charge Nurse', Id: 302 };
    
  }

  public checkCandidate() {

    if (this.diplomaCandidate.CNIC && this.diplomaCandidate.CNIC[12] != ' ') {
      this._diplomaCandidateService.getMerit(this.diplomaCandidate.CNIC).subscribe((res: any) => {

        console.log('merit: ', res);
        if (res.Id != null) {
          this.diplomaCandidate.MeritId = res.Id;
          this.diplomaCandidate.Name = res.Name;
          this.diplomaCandidate.PhoneNumber = res.MobileNumber;
          this.diplomaCandidate.CNIC = res.CNIC;
          this.getProfile();
        }
      }, err => { console.log(err); }
      );
    }
  }

  public onSubmit() {
    this.savingProfile = true;
    if (this.diplomaCandidate.PermanentSince) {
      this.diplomaCandidate.PermanentSince = this.diplomaCandidate.PermanentSince.toDateString();
    }
    this.diplomaCandidateDTO.meritCandidate = this.diplomaCandidate;
    this.diplomaCandidateDTO.meritCandidateDetail = this.diplomaCandidateDetail;
    console.log('dto: ', this.diplomaCandidateDTO);
    this._diplomaCandidateService.saveDiplomaCandidate(this.diplomaCandidateDTO).subscribe((res: any) => {
      if (res.meritCandidate && res.meritCandidate.Id) {
        if (this.photoFile.length > 0) {
          this._diplomaCandidateService.uploadMeritProfilePhoto(this.photoFile, res.meritCandidate.CNIC).subscribe((response: any) => {
            if (response) {
              alert('Record Saving Successfully');
              this.resetForm();
            }
          }, err => {
            this.handleError(err);
          });
        }
      }
    }, err => {
      this.handleError(err);
    });

  }
  public resetForm() {
    this.diplomaCandidate = {};
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
  private getDomiciles = () => {
    this.dropDowns.domicile = [];
    this.dropDowns.domicileData = [];
    this._rootService.getDomiciles().subscribe((res: any) => {
      this.dropDowns.domicile = res;
      this.dropDowns.domicileData = this.dropDowns.domicile.slice();
    },
      err => { this.handleError(err); }
    );
  }
  public getDesignations = () => {
    console.log('in desgin...');
    this.dropDowns.designations = [];
    this.dropDowns.designationsData = [];
    this._diplomaCandidateService.getDesignations().subscribe((res: any) => {
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
    this._diplomaCandidateService.getProfile(this.diplomaCandidate.CNIC).subscribe((res: any) => {

      if (res && res.Id) {
        this.profile = res;
        if (this.profile != null) {
          this.diplomaCandidate.ProfileId = this.profile.Id;
          this.diplomaCandidate.DesignationId = this.profile.Designation_Id;
          this.diplomaCandidate.HfId = this.profile.HealthFacility_Id;
          this.dropDowns.selectedFiltersModel.designations = { Name: this.profile.Designation_Name, Id: this.profile.Designation_Id };
          this.dropDowns.selectedFiltersModel.healthFacilities = { Name: this.profile.HealthFacility + ', ' + this.profile.Tehsil + ', ' + this.profile.District, Id: this.profile.HealthFacility_Id };
          let district = this.dropDowns.districts.find(x => x.Name == this.profile.District);
          this.dropDowns.selectedFiltersModel.districts = { Name: district.Name, Code: district.Code };
          this.diplomaCandidate.DistrictCode = district.Code;
        }
      }
    }, err => {
      this.handleError(err);
    });
  }

  public getHFLists = () => {
    this.dropDowns.healthFacilities = [];
    this.dropDowns.healthFacilitiesData = [];
    this._diplomaCandidateService.getHFLists().subscribe((res: any) => {
      if (res) {
        console.log('HF', res);
        this.dropDowns.healthFacilities = res
        this.dropDowns.healthFacilitiesData = this.dropDowns.healthFacilities.slice();
      }
    },
      err => { this.handleError(err); }
    );
  }
  public GetHFListsPreference = () => {
    this._diplomaCandidateService.GetHFListsPreference().subscribe((res: any) => {
      if (res) {
        this.HFListPreference = res;
      }
    },
      err => { this.handleError(err); }
    );
  }
  public getDistrict = () => {
    this.dropDowns.districts = [];
    this.dropDowns.districtsData = [];
    this._diplomaCandidateService.getDistrict().subscribe((res: any) => {
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
    this._diplomaCandidateService.getMerit(this.diplomaCandidate.CNIC).subscribe((res: any) => {
      if (res && res.Id) {

        this.diplomaCandidate.Name = res.Name;
        this.diplomaCandidate.PhoneNumber = res.MobileNumber;
        this.diplomaCandidate.CNIC = res.CNIC;
      }
    }, err => {
      this.handleError(err);
    });

  }
  public addPreferenceHF(Id: number) {

    if (this.Templist.length == 3) {
      alert('you already give three preferecnce')
    }
    else {
      if (Id != 0) {
        let Hf = this.HFListPreference.find(x => x.Id === Id);
        let PreferenceHfId = Hf.Id;
        this.Templist.push(Hf);
        this.diplomaCandidateDetail.push({ PreferenceHfId: PreferenceHfId });
      }
    }

  }
  public removePreferenceHF(index, Id: number) {

    this.diplomaCandidateDetail = this.diplomaCandidateDetail.filter(({ PreferenceHfId }) => PreferenceHfId !== Id);
    this.Templist.splice(index, 1);
  }
  public dropdownValueChanged = (value, filter) => {

    if (!value) {
      return;
    }
    if (filter == 'district') {
      this.diplomaCandidate.DistrictCode = value.Code;
    }
    if (filter == 'HF') {
      this.diplomaCandidate.HfId = value.Id;
    }
    if (filter == 'designation') {
      this.diplomaCandidate.DesignationId = value.Id;
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
}
