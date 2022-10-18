import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthenticationService } from '../../../_services/authentication.service';
import { HealthFacilityService } from '../health-facility.service';
import { RootService } from '../../../_services/root.service';
import { Subscription } from 'rxjs/Subscription';
import { ActivatedRoute } from '@angular/router';
import { cstmdummyHFAC, cstmdummyModes, cstmdummyStatus } from '../../../_models/cstmdummydata';
import { HealthFacility } from '../health-facility.class';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { NotificationService } from '../../../_services/notification.service';

@Component({
  selector: 'app-add-edit',
  templateUrl: './add-edit.component.html',
  styleUrls: ['./add-edit.component.scss']
})
export class AddEditComponent implements OnInit, OnDestroy {
  public loading = true;
  public savingHF = false;
  public addingWard = false;
  public addingService = false;
  public wardDialogOpened: boolean = false;
  public uploadingPhoto: boolean = false;
  public wardsBedsInfo: boolean = false;

  public healthFacility: any;
  public userHfmisCode: string = '000000000';
  public landlineMask: string = "000-0000000";
  public hfmisCode: string = '0';
  public hfTypeCode: string = '0';
  public hfCategoryCode: string = '0';
  public totalArea: number = 0;
  public hfTypeCodes: any[] = [];
  public dropDowns: DropDownsHR = new DropDownsHR();
  public selectedFiltersModel: any;
  public newWard: any = {
    HF_Id: 0,
    Name: '',
    NoB: '',
    NoGB: '',
    NoSB: ''
  }
  public serviceId: number = 0;
  public files: any[] = [];
  public services: any[] = [];
  public modeChanged: boolean = false;
  public photoAttached: boolean = false;
  public hfPhotoLoaded: boolean = false;
  public hfPhoto = new Image();
  private subscription: Subscription;
  public hfMode: any = {};
  public hfUc: any = {};

  constructor(private _rootService: RootService, private _hfService: HealthFacilityService,
    private _authenticationService: AuthenticationService, public _notificationService: NotificationService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.userHfmisCode = this._authenticationService.getUser().HfmisCode;
    this.selectedFiltersModel = this.dropDowns.selectedFiltersModel;
    this.fetchParams();
  }
  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('hfcode')) {
          this.hfmisCode = params['hfcode'];
          this.fetchData(this.hfmisCode);
        } else {
          this.healthFacility = new HealthFacility();
          this.loadDropdownValues();
          this.loading = false;
        }
      }
    );
  }
  private fetchData(hfmisCode) {
    this._hfService.getHealthFacilitiyDashboard(hfmisCode).subscribe(
      res => {
        this.healthFacility = res;
        this.getUCs(this.healthFacility.DistrictCode);
        this.gethfUc();
        this.loadDropdownValues();
        this.bindValues();
        this.hfPhoto.src = 'https://hrmis.pshealthpunjab.gov.pk/Uploads/Files/HFPics/' + this.healthFacility.ImagePath;
        this.hfPhoto.onload = () => {
          this.hfPhotoLoaded = true;
        }
        if (this.savingHF) {
          this._notificationService.notify('success', 'Health Facility Saved!');
          this.savingHF = false;
        }
        this.loading = false;
      },
      err => {
        this.handleError(err);
      }
    )
  }
  private getHFMode() {
    this._hfService.getHFMode(this.hfmisCode).subscribe((data: any) => {
      if (data) {
        this.hfMode = data;
        this.dropDowns.selectedFiltersModel.mode = { Id: this.hfMode.Mode_Id, Name: this.hfMode.ModeName }
      } else {
        this.hfMode = {};
      }
    }, err => {
      this.handleError(err);
    });
  }
  public onSubmit(value) {
    this.savingHF = true;
    if (this.healthFacility.EPICode >= 0) {
      this.hfUc.EPI_Code = this.healthFacility.EPICode;
    }
    this._hfService.saveHF(this.healthFacility).subscribe((response: any) => {
      if (response && response.Id != null) {
        if (this.hfMode.Mode_Id && this.hfMode.ModeName) {
          this.addHFMode();
        }
        if (this.hfUc.UC_Id || this.hfUc.EPI_Code) {
          this.saveHFUC();
        }
        this.fetchData(response.HfmisCode);
      }
    }, err => {
      this.savingHF = false;
      this.handleError(err);
    });
  }
  public getServices() {
    this._rootService.getServices().subscribe((res: any) => {
      if (res) {
        this.services = res;
        console.log(this.services);

      }
    }, err => {
      this.handleError(err);
    });
  }

  private gethfUc() {
    this._hfService.getHFUCInfo(this.hfmisCode).subscribe(
      res => {
        if (res) {
          this.hfUc = res;
          if (this.hfUc.Id) {
            this.healthFacility.EPICode = this.hfUc.EPI_Code;
            this.dropDowns.selectedFiltersModel.ucDrp = { UC: this.hfUc.UC, Id: this.hfUc.Id };
          }
        }
      },
      err => {
        this.handleError(err);
      }
    );
  }
  public addHFWard() {
    this.addingWard = true;
    this.newWard.HF_Id = this.healthFacility.Id;
    this._hfService.addHfWard(this.newWard).subscribe((res: any) => {
      if (res.Id) {
        let ward = this.newWard;
        this.healthFacility.HFWardsList.unshift(ward);
        this.newWard = {
          HF_Id: 0,
          Name: '',
          NoB: '',
          NoGB: '',
          NoSB: ''
        };
        this.addingWard = false;
      }
    },
      err => {
        this.handleError(err);
      });
  }
  public addHFService() {
    this.addingService = true;
    this._hfService.addHFService(this.serviceId, this.healthFacility.Id, this.healthFacility.HFMISCode).subscribe((res: any) => {
      if (res) {
        let service = res;
        this.serviceId = 0;
        this._notificationService.notify('success', 'Service Added');
      } else {
        this._notificationService.notify('success', 'Duplicate Service!');
      }
      this.addingService = false;
    },
      err => {
        this.handleError(err);
      });
  }

  public addHFMode() {
    this.hfMode.HF_Id = this.healthFacility.Id;
    this._hfService.addHfMode(this.hfMode).subscribe((res: any) => {
      if (res.Id) {
        this.hfMode = res;
      }
    },
      err => {
        this.handleError(err);
      });
  }
  public saveHFUC() {
    this.hfUc.HF_Id = this.healthFacility.Id;
    this._hfService.saveHFUC(this.hfUc).subscribe((res: any) => {
      if (res.Id) {
        this.hfUc = res;
      }
    },
      err => {
        this.handleError(err);
      });
  }
  private bindValues() {
    this.calcArea();
    this.dropDowns.selectedFiltersModel.hfType = { Code: this.healthFacility.HFTypeCode, Name: this.healthFacility.HFTypeName };
    this.hfTypeCode = this.healthFacility.HFTypeCode;
    this.hfCategoryCode = this.healthFacility.CategoryCode;
    this.dropDowns.selectedFiltersModel.tehsil = { Code: this.healthFacility.TehsilCode, Name: this.healthFacility.TehsilName };
    this.dropDowns.selectedFiltersModel.hfac = cstmdummyHFAC.find(x => x.Id == this.healthFacility.HFAC);
    this.dropDowns.selectedFiltersModel.status = cstmdummyStatus.find(x => x.Name == this.healthFacility.Status);
    this.showWards();
    this.getHFMode();
  }
  private loadDropdownValues = () => {
    this.getDivisions(this.userHfmisCode);
    this.getDistricts(this.userHfmisCode);
    this.getTehsils(this.userHfmisCode);
    this.getHFTypes();
    this.getServices();
  }
  private getDivisions = (code: string) => {
    this._rootService.getDivisions(code).subscribe((res: any) => {
      this.dropDowns.divisions = res;
      this.dropDowns.divisionsData = this.dropDowns.divisions.slice();
    },
      err => { this.handleError(err); }
    );
  }
  private getDistricts = (code: string) => {
    this._rootService.getDistricts(code).subscribe((res: any) => {
      this.dropDowns.districts = res;
      this.dropDowns.districtsData = this.dropDowns.districts.slice();
    },
      err => { this.handleError(err); }
    );
  }
  private getTehsils = (code: string) => {
    this.dropDowns.tehsils = [];
    this.dropDowns.tehsilsData = [];
    this._rootService.getTehsils(code).subscribe((res: any) => {
      this.dropDowns.tehsils = res;
      this.dropDowns.tehsilsData = this.dropDowns.tehsils.slice();
      if (this.dropDowns.tehsils.length == 1) {
        this.selectedFiltersModel.tehsil = this.dropDowns.tehsilsData[0];
        this.hfmisCode = this.selectedFiltersModel.tehsil.Code;
      }
    },
      err => { this.handleError(err); }
    );
  }
  private getUCs = (code: string) => {
    this._rootService.getHFUCs(code).subscribe((res: any) => {
      this.dropDowns.ucs = res;
      this.dropDowns.ucsData = this.dropDowns.ucs.slice();
      console.log(res);

    },
      err => { this.handleError(err); }
    );
  }
  private getHFTypes = () => {
    this._rootService.getHFTypes().subscribe((res: any) => {
      this.dropDowns.hfTypes = res;
      this.dropDowns.hfTypesData = this.dropDowns.hfTypes.slice();
    },
      err => { this.handleError(err); }
    );
  }
  private getHFCategory = (hfCatId: number) => {
    if (!hfCatId) return;
    this._rootService.getHFCategory(hfCatId).subscribe((res: any) => {
      this.healthFacility.HFCategoryName = res.Name;
      this.hfCategoryCode = res.Code;
      this.getNewHFMISCode();
    },
      err => { this.handleError(err); }
    );
  }
  private getNewHFMISCode = () => {
    if (this.hfmisCode.length == 9 && this.hfCategoryCode.length == 3 && this.hfTypeCode.length == 3) {
      this._hfService.getNewHFMISCode(this.hfmisCode.substring(0, 9) + this.hfCategoryCode + this.hfTypeCode).subscribe((x) => {
        this.healthFacility.HFMISCode = x;
      }, err => {
        this.handleError(err);
      });
    }
  }
  public dropdownValueChanged = (value, filter) => {
    if (!value) {
      return;
    }
    /*   if (filter == 'division') {
        this.hfmisCode = value.Code;
        this.resetDrops(filter);
        this.getDistricts(value.Code);
        this.getTehsils(value.Code);
      }
      if (filter == 'district') {
        this.hfmisCode = value.Code;
        this.resetDrops(filter);
        this.getTehsils(value.Code);
      } */
    if (filter == 'tehsil') {
      let tehsilCode: string = value.Code as string;
      if (tehsilCode) {
        let tempdivisions = this.dropDowns.divisionsData as any[];
        let tempdivision = tempdivisions.find(x => tehsilCode.substring(0, 3) == x.Code);
        this.healthFacility.DivisionName = tempdivision.Name;

        let districts = this.dropDowns.districtsData as any[];
        let district = districts.find(x => tehsilCode.substring(0, 6) == x.Code);
        this.healthFacility.DistrictName = district.Name;
      }
      this.hfmisCode = value.Code;
      this.getNewHFMISCode();
    }
    if (filter == 'hfType') {
      this.hfTypeCode = value.Code;
      this.healthFacility.HFTypeCode = this.hfTypeCode;
      this.showWards();
      this.dropDowns.hfTypes.forEach((element: any) => {
        if (element.Code == value.Code) {
          this.getHFCategory(element.HFCat_Id);
        }
      });
    }
    if (filter == 'uc') {
      console.log(value);
      this.hfUc.UC_Id = value.Id;
    }
    if (filter == 'mode') {
      this.hfMode.Mode_Id = value.Id;
      this.hfMode.ModeName = value.Name;
      this.modeChanged = true;
    }
    if (filter == 'hfac') {
      this.healthFacility.HFAC = value.Id;
    }
    if (filter == 'status') {
      this.healthFacility.Status = value.Name;
    }
  }
  private resetDrops = (filter: string) => {
    if (filter == 'division') {
      this.selectedFiltersModel.district = { Name: 'Select District', Code: '0' };
      this.selectedFiltersModel.tehsil = { Name: 'Select Tehsil', Code: '0' };
    }
    if (filter == 'district') {
      this.selectedFiltersModel.tehsil = { Name: 'Select Tehsil', Code: '0' };
    }
  }
  public handleFilter = (value, filter) => {
    if (filter == 'division') {
      this.dropDowns.divisionsData = this.dropDowns.divisions.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
    if (filter == 'district') {
      this.dropDowns.districtsData = this.dropDowns.districts.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
    if (filter == 'tehsil') {
      this.dropDowns.tehsilsData = this.dropDowns.tehsils.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
  }
  public closeWardWindow() {
    this.wardDialogOpened = false;
  }

  public openWardWindow() {
    this.wardDialogOpened = true;
  }
  public photoChoosen(event) {
    let inputValue = event.target;

    this.files = inputValue.files;
    this.photoAttached = true;
  }
  public uploadPhoto() {
    this.uploadingPhoto = true;
    this._hfService.uploadPhoto(this.files, this.healthFacility.Id).subscribe((res: any) => {
      if (res.result) {
        this.hfPhoto.src = 'https://hrmis.pshealthpunjab.gov.pk/Uploads/Files/HFPics/' + res.src;
        this.hfPhoto.onload = () => {
          this.hfPhotoLoaded = true;
        }
        this.uploadingPhoto = false;
        this._notificationService.notify('success', 'Health Facility Photo Uploaded');
      }
    }, err => {
      this.handleError(err);
    })
  }
  public calcArea() {
    this.totalArea = (this.healthFacility.CoveredArea ? this.healthFacility.CoveredArea : 0)
      + (this.healthFacility.UnCoveredArea ? this.healthFacility.UnCoveredArea : 0)
      + (this.healthFacility.ResidentialArea ? this.healthFacility.ResidentialArea : 0)
      + (this.healthFacility.NonResidentialArea ? this.healthFacility.NonResidentialArea : 0);
  }
  public showWards() {
    if (this.healthFacility.HFTypeCode === '011' || this.healthFacility.HFTypeCode === '012'
      || this.healthFacility.HFTypeCode === '013' || this.healthFacility.HFTypeCode === '014'
      || this.healthFacility.HFTypeCode === '023' || this.healthFacility.HFTypeCode === '024'
      || this.healthFacility.HFTypeCode === '025' || this.healthFacility.HFTypeCode === '026'
      || this.healthFacility.HFTypeCode === '027' || this.healthFacility.HFTypeCode === '028'
      || this.healthFacility.HFTypeCode === '021'
      || this.healthFacility.HFTypeCode === '029'
      || this.healthFacility.HFTypeCode === '033'
      || this.healthFacility.HFTypeCode === '036'
      || this.healthFacility.HFTypeCode === '037'
    ) {
      this.wardsBedsInfo = true;
    }
    else {
      this.wardsBedsInfo = false;
    }
  }
  private handleError(err: any) {
    this._notificationService.notify('danger', 'Error!');
    this.loading = false;
    this.addingService = false;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }
  ngOnDestroy() {
    this.subscription.unsubscribe();
  }
}
