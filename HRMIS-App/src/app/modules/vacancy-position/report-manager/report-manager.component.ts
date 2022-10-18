import { Component, OnInit } from '@angular/core';
import { RootService } from '../../../_services/root.service';
import { VacancyPositionService } from '../vacancy-position.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';

@Component({
  selector: 'app-report-manager',
  templateUrl: './report-manager.component.html',
  styles: [`

  `]
})
export class ReportManagerComponent implements OnInit {
  public currentUser: any = {};
  public currentOfficer: any;
  public Designations: any[] = [];
  public DDTH: any[] = [];
  public Vecancies: any[] = [];
  public SelectedDesignation: any;
  public data: any[] = [];
  public Division: any[] = [];
  public SelectedDivision: any[] = [];
  public District: any[] = [];
  public SelectedDistrict: any[] = [];
  public Tehsil: any[] = [];
  public SelectedTehsil: any[] = [];
  public HFType: any[] = [];
  public SelectedHFType: any[] = [];
  public HF: any[] = [];
  public HFAC: any[] = [];
  public SelectedHF: any[] = [];
  public scales: any[] = [];
  public selectedScales = [];
  public cadres: any[] = [];
  public selectedCadres = [];
  public postTypes: any[] = [];
  public selectedPostTypes = [];
  public selectedHFACs = [];

  public totals: any[] = [];

  public dropDowns: DropDownsHR = new DropDownsHR();
  public sanctionedIndex: number = null;
  public filledIndex: number = null;
  public vacantIndex: number = null;
  public approvalsIndex: number = null;
  public profilesIndex: number = null;
  public workingProfilesIndex: number = null;
  public geoLevel: number = null;
  public selectedGeoColumn: string = '';
  public dataHFMISCodes: string[] = [];

  public ShowFiltrer: boolean = true;
  public isLoading: boolean = false;
  public showHFFilter: boolean = false;
  public isDownloading: boolean = false;
  public isDownloadingPDF: boolean = false;
  public Group = [
    { label: 'Division', value: { Display: 'Division', value: 'DivisionName' } },
    { label: 'District', value: { Display: 'District', value: 'DistrictName' } },
    { label: 'Tehsil', value: { Display: 'Tehsil', value: 'TehsilName' } },
    { label: 'Designation', value: { Display: 'Designation', value: 'DsgName' } },
    { label: 'Post Type', value: { Display: 'Post Type', value: 'PostTypeName' } },
    { label: 'Scale', value: { Display: 'Scale', value: 'BPS' } },
    { label: 'Cadre', value: { Display: 'Cadre', value: 'CadreName' } },
    { label: 'Administrative Control', value: { Display: 'Administrative Control', value: 'HFACName' } },
    { label: 'Health Facility Type', value: { Display: 'Health Facility Type', value: 'HFTypeName' } },
    { label: 'Health Facility', value: { Display: 'Health Facility', value: 'HFName' } },
  ];
  public Vecancy: any[] = [];

  constructor(private _rootService: RootService, private _vpService: VacancyPositionService,
    private _authenticationService: AuthenticationService) {

  }

  ngOnInit() {
    this.currentUser = this._authenticationService.getUser();
    this.currentOfficer - this._authenticationService.getCurrentOfficer();
    this.loadDesignation();
    this.loadDivision();
    this.loadDistrict();
    this.loadTehsil();
    this.loadHFType();
    this.setVacancyDisplay();
    if (this._authenticationService.getUserHfmisCode().length == 19) {
      this.showHFFilter = true;
      this.loadHF();
    }
    this.loadScale();
    this.loadCadre();
    this.loadPostTypes();
    this.loadHFAC();
  }

  public loadDesignation() {
    this._rootService.getPostNamesForVecancy()
      .subscribe((x: any) => {
        this.Designations = [];
        x.forEach(element => {
          this.Designations.push({ label: element.name, value: element.id });
        });
      });
  }
  public loadDivision() {
    this.Division = [];
    this._rootService.getDivisions(this._authenticationService.getUserHfmisCode())
      .subscribe((x: any[]) => {
        if (x) {
          this.dropDowns.divisions = x;
          if (x.length == 1) {
            this.SelectedDivision.push(x[0].Name);
          }
          x.forEach(element => {
            this.Division.push({ label: element.Name, value: element.Name });
          });
        }
      });
  }
  public loadDistrict() {
    this.District = [];
    this._rootService.getDistricts(this._authenticationService.getUserHfmisCode())
      .subscribe((x: any[]) => {
        if (x) {
          this.dropDowns.districts = x;
          if (x.length == 1) {
            this.SelectedDistrict.push(x[0].Name);
          }
          x.forEach(element => {
            this.District.push({ label: element.Name, value: element.Name });
          });
        }
      });
  }
  public loadTehsil() {
    this.Tehsil = [];
    this._rootService.getTehsils(this._authenticationService.getUserHfmisCode())
      .subscribe((x: any[]) => {
        if (x) {
          this.dropDowns.tehsils = x;
          if (x.length == 1) {
            this.SelectedTehsil.push(x[0].Name);
          }
          x.forEach(element => {
            this.Tehsil.push({ label: element.Name, value: element.Name });
          });
        }
      });
  }
  public loadHFType() {
    this.HFType = [];
    this._rootService.getHFTypes()
      .subscribe((x: any) => {
        if (x) {
          this.dropDowns.hfTypes = x;
          if (x.length == 1) {
            this.SelectedHFType.push(x[0].Name);
          }
          x.forEach(element => {
            this.HFType.push({ label: element.Name, value: element.Name });
          });
        }
      });
  }
  public loadHFAC() {
    this.HFAC = [];
    this._rootService.getHFAC()
      .subscribe((x: any) => {
        if (x) {
          this.dropDowns.hfacs = x;
          if (x.length == 1) {
            this.selectedHFACs.push(x[0].Name);
          }
          x.forEach(element => {
            this.HFAC.push({ label: element.Name, value: element.Name });
          });
        }
      });
  }
  public loadHF() {
    this.HF = [];
    this._rootService.getHealthFacilities(this._authenticationService.getUserHfmisCode())
      .subscribe((x: any) => {
        if (x) {
          this.dropDowns.healthFacilities = x;
          if (x.length == 1) {
            this.SelectedHF.push(x[0].Name);
          }
          x.forEach(element => {
            this.HF.push({ label: element.Name, value: element.Name });
          });
        }

      });
  }
  public loadScale() {
    this._rootService.getScales()
      .subscribe((x: any) => {
        this.scales = [];
        x.forEach(element => {
          this.scales.push({ label: element.Name, value: element.Name });
        });
      });
  }
  public loadCadre() {
    this._rootService.getPostCadres()
      .subscribe((x: any) => {
        this.cadres = [];
        x.forEach(element => {
          this.cadres.push({ label: element.name, value: element.name });
        });
      });
  }
  public loadPostTypes() {
    this._rootService.getPostTypes()
      .subscribe((x: any) => {
        this.postTypes = [];
        x.forEach(element => {
          this.postTypes.push({ label: element.Name, value: element.Name });
        });
      });
  }
  public clear() {
    this.DDTH = [];
    this.Vecancies = [];
    this.SelectedDesignation = [];
    this.SelectedDistrict = [];
    this.SelectedDivision = [];
    this.SelectedTehsil = [];
    this.SelectedHFType = [];
    this.SelectedHF = [];
    this.selectedScales = [];
    this.selectedCadres = [];
    this.selectedPostTypes = [];
    this.resetData();
  }

  public resetData() {
    this.data = [];
  }

  public getData() {
    this.isLoading = true;
    this.setClickables();
    this.setGeoLevel();
    let filters = {
      Designations: this.SelectedDesignation, Divisions: this.SelectedDivision, Districts: this.SelectedDistrict, Tehsils: this.SelectedTehsil, HFTypes: this.SelectedHFType, HFs: this.SelectedHF, Scales: this.selectedScales, Cadres: this.selectedCadres, PostTypes: this.selectedPostTypes, HFACs: this.selectedHFACs
    };
    this._vpService.getVacancyData(this.DDTH, this.Vecancies, filters)
      .subscribe((x: any) => {
        if (x) {
          this.data = x;
          this.setTotal();
          //this.ShowFiltrer = false;
        }
        this.isLoading = false;
      }, err => {
        console.log(err);

        this.isLoading = false;
      });
  }
  public setTotal() {
    this.totals = [];
    this.Vecancies.forEach(vacancy => {
      this.totals.push(0);
    });
    for (let i = 0; i < this.data.length; i++) {
      const element = this.data[i];
      let lastIndex = 0;
      for (let j = (this.DDTH.length); j < element.length; j++) {
        const elem = element[j];
        this.totals[lastIndex++] += +elem;
      }
    }
  }
  public setClickables() {
    for (let index = 0; index < this.Vecancies.length; index++) {
      const element = this.Vecancies[index];
      if (element) {
        if (element.value == 'TotalSanctioned') {
          this.sanctionedIndex = (this.DDTH.length) + index;
        }
        if (element.value == 'TotalWorking') {
          this.filledIndex = (this.DDTH.length) + index;
        }
        if (element.value == 'Vacant') {
          this.vacantIndex = (this.DDTH.length) + index;
        }
        if (element.value == 'TotalApprovals') {
          this.approvalsIndex = (this.DDTH.length) + index;
        }
        if (element.value == 'Profiles') {
          this.profilesIndex = (this.DDTH.length) + index;
        }
        if (element.value == 'WorkingProfiles') {
          this.workingProfilesIndex = (this.DDTH.length) + index;
        }
      }
    }

  }
  public setGeoLevel() {
    this.geoLevel = this.SelectedHF.length > 0 ? 5
      : this.SelectedHFType.length > 0 ? 4
        :
        this.SelectedTehsil.length > 0 ? 3
          :
          this.SelectedDistrict.length > 0 ? 2
            :
            this.SelectedDivision.length > 0 ? 1
              : 0;
    this.dataHFMISCodes = [];
    if (this.geoLevel == 0) {
      this.dataHFMISCodes.push('0');
    }
    else if (this.geoLevel == 1) {
      this.SelectedDivision.forEach(selectedElement => {
        this.dropDowns.divisions.forEach(element => {
          if (selectedElement == element.Name) {
            this.dataHFMISCodes.push(element.Code);
          }
        });
      });
    }
    else if (this.geoLevel == 2) {
      this.SelectedDistrict.forEach(selectedElement => {
        this.dropDowns.districts.forEach(element => {
          if (selectedElement == element.Name) {
            this.dataHFMISCodes.push(element.Code);
          }
        });
      });
    }
    else if (this.geoLevel == 3) {
      this.SelectedTehsil.forEach(selectedElement => {
        this.dropDowns.tehsils.forEach(element => {
          if (selectedElement == element.Name) {
            this.dataHFMISCodes.push(element.Code);
          }
        });
      });
    }
    else if (this.geoLevel == 4) {
      this.SelectedHFType.forEach(selectedElement => {
        this.dropDowns.hfTypes.forEach(element => {
          if (selectedElement == element.Name) {
            this.dataHFMISCodes.push(element.Code);
          }
        });
      });
    }
    else if (this.geoLevel == 5) {
      this.SelectedHF.forEach(selectedElement => {
        this.dropDowns.healthFacilities.forEach(element => {
          if (selectedElement == element.Name) {
            this.dataHFMISCodes.push(element.Code);
          }
        });
      });
    }
    if (this.currentUser.RoleName == 'PHFMC Admin' || this.currentUser.RoleName == 'PHFMC') {
      if (this.SelectedDistrict.length == 0) {
        this.SelectedDistrict.push('Faisalabad');
        this.SelectedDistrict.push('Rahim Yar Khan');
        this.SelectedDistrict.push('T.T Singh');
        this.SelectedDistrict.push('Sahiwal');
        this.SelectedDistrict.push('Kasur');
        this.SelectedDistrict.push('Chakwal');
        this.SelectedDistrict.push('Lahore');
        this.SelectedDistrict.push('Mianwali');
        this.SelectedDistrict.push('Vehari');
        this.SelectedDistrict.push('Dera Ghazi Khan');
        this.SelectedDistrict.push('Pakpattan');
        this.SelectedDistrict.push('Hafizabad');
        this.SelectedDistrict.push('Lodhran');
        this.SelectedDistrict.push('Rajanpur');
      }
    }
  }
  public cellClicked(i: number, obj: any) {

    if (this.sanctionedIndex == i || this.filledIndex == i || this.vacantIndex == i) {
      let clickType: string = '';
      if (this.sanctionedIndex == i) {
        clickType = 's';
      }
      if (this.filledIndex == i) {
        clickType = 'f';
      }
      if (this.vacantIndex == i) {
        clickType = 'v';
      }
      this.DDTH.forEach(colum => {
        if (colum.Display == 'Division') {
          this.selectedGeoColumn = 'Division';
        }
        if (colum.Display == 'District' && this.selectedGeoColumn != 'Division') {
          this.selectedGeoColumn = 'District';
        }
      });
      var selectedGeoColumnValue = this.selectedGeoColumn
      var selectedGeoColumnEncodedValue = encodeURIComponent(selectedGeoColumnValue);


      var selectedGeoNameValue = obj[0];
      var selectedGeoNameEncodedValue = encodeURIComponent(selectedGeoNameValue);


      let d = this.SelectedDesignation ? '&d=' + this.SelectedDesignation : '';
      this.openInNewTab('vacancy-position/detail?g=' + selectedGeoColumnEncodedValue + '&l=' + selectedGeoNameEncodedValue + d + '&c=' + clickType);

      // this.openInNewTab('vacancy');
    }

    if (this.approvalsIndex == i) {
      this.openInNewTab('fts/my-applications/' + '0/2');
    }
    if (this.profilesIndex == i) {
      this.openInNewTab('profile/' + '0/2');
    }
    if (this.workingProfilesIndex == i) {
      this.openInNewTab('profile/' + '1/2');
    }
  }
  public setVacancyDisplay() {

    this.Vecancy.push({ label: 'Sanctioned', value: { Display: 'Sanctioned', value: 'TotalSanctioned' } });
    this.Vecancy.push({ label: 'Filled', value: { Display: 'Filled', value: 'TotalWorking' } });
    this.Vecancy.push({ label: 'Vacant', value: { Display: 'Vacant', value: 'Vacant' } });
    this.Vecancy.push({ label: 'Approvals', value: { Display: 'Approvals', value: 'TotalApprovals' } });
    this.Vecancy.push({ label: 'Profiles', value: { Display: 'Profiles', value: 'Profiles' } });
    this.Vecancy.push({ label: 'WorkingProfiles', value: { Display: 'WorkingProfiles', value: 'WorkingProfiles' } });
    this.Vecancy.push({ label: 'Adhoc', value: { Display: 'Adhoc', value: 'Adhoc' } });
    this.Vecancy.push({ label: 'Regular', value: { Display: 'Regular', value: 'Regular' } });
    this.Vecancy.push({ label: 'PHFMC', value: { Display: 'PHFMC', value: 'PHFMC' } });
    this.Vecancy.push({ label: 'Contract', value: { Display: 'Contract', value: 'Contract' } });
    this.Vecancy.push({ label: 'Adjusted For Pay Purpose', value: { Display: 'Adjusted For Pay Purpose', value: 'AdjustedForPayPurpose' } });
    this.Vecancy.push({ label: 'Daily Wages', value: { Display: 'Daily Wages', value: 'DailyWages' } });
    this.Vecancy.push({ label: 'PMHI', value: { Display: 'PMHI', value: 'PMHI' } });

    if (!this.currentUser.RoleName.toLowerCase().includes('phfmc')) {
      this.Vecancy.push({ label: 'Acting Charge', value: { Display: 'Acting Charge', value: 'ActingCharge' } });
      this.Vecancy.push({ label: 'Active Charge', value: { Display: 'Active Charge', value: 'ActiveCharge' } });
      this.Vecancy.push({ label: 'Adjusted', value: { Display: 'Adjusted', value: 'Adjusted' } });
      this.Vecancy.push({ label: 'Consultancy', value: { Display: 'Consultancy', value: 'Consultancy' } });
      this.Vecancy.push({ label: 'Contract', value: { Display: 'Contract', value: 'Contract' } });
      this.Vecancy.push({ label: 'Current Charge', value: { Display: 'Current Charge', value: 'CurrentCharge' } });
      this.Vecancy.push({ label: 'Deputation', value: { Display: 'Deputation', value: 'Deputation' } });
      this.Vecancy.push({ label: 'On Pay Scale', value: { Display: 'On Pay Scale', value: 'OnPayScale' } });
      this.Vecancy.push({ label: 'Project Contract', value: { Display: 'Project Contract', value: 'ProjectContract' } });
      this.Vecancy.push({ label: 'Reinstated', value: { Display: 'Reinstated', value: 'Reinstated' } });
    }
  }
  public download() {
    this.isDownloading = true;
    this.setGeoLevel();
    let filters = {
      Designations: this.SelectedDesignation, Divisions: this.SelectedDivision, Districts: this.SelectedDistrict, Tehsils: this.SelectedTehsil, HFTypes: this.SelectedHFType, HFs: this.SelectedHF, Scales: this.selectedScales, Cadres: this.selectedCadres, PostTypes: this.selectedPostTypes, HFACs: this.selectedHFACs
    };
    this._vpService.downloadVacancyData(this.DDTH, this.Vecancies, filters)
      .subscribe((x: any) => {
        this.isDownloading = false;
        var blob = new Blob([x.body], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        this.saveAs(blob, "VacancyData-" + this.getDateToday() + ".xls");
      }, err => {
        this.isDownloading = false;
        console.log(err);
      });
  }
  saveAs(blob, fileName) {
    var link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    link.download = fileName;
    link.click();
    window.URL.revokeObjectURL(link.href);
  }
  public openInNewTab(link: string) {
    window.open('/' + link, '_blank');
  }
  public switchStatus() {
    this.ShowFiltrer = !this.ShowFiltrer;
  }

  public getDateToday(): string {
    let today = new Date();
    return today.getDate() + '-' + (today.getMonth() + 1) + '-' + today.getFullYear() + " " + today.getHours() + "_" + today.getMinutes() + "_" + today.getSeconds();
  }
  print() {
    let html = document.getElementById('printOld').innerHTML;
    var mywindow = window.open('', 'PRINT', 'height=600,width=900');
    if (mywindow) {
      mywindow.document.write(`<html><head>
          <style>
              body {
                font-family: -apple-system, system-ui, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue' , Arial, sans-serif !important;
            }
       
          button.print {
            padding: 10px 40px;
            font-size: 21px;
            position: absolute;
            background: transparent;
            width: 100%;
            position: fixed;

            background-image: linear-gradient(rgba(63, 162, 79, 0), rgba(63, 162, 79, 0.2));
            cursor: pointer;
            border: none;
            color: #ffffff;
            z-index: 9999;
          }
          table {
            border-collapse: collapse;
            width: 100%;
          }
          
          table td, table th {
            border: 1px solid black;
            padding: 6px;
          }
          
          table tr {background-color: white;}
          
          table tr:hover {background-color: #ddd;}
          
          table th {
            padding-top: 12px;
            padding-bottom: 12px;
            text-align: left;
            font-weight: bold;
          }
          table { page-break-inside:auto; }
          td    { border:1px solid lightgray; }
          tr    { page-break-inside:auto; }
        .text-right{
          text-align: right;
        }
          @media print {
            button.print {
              display: none;
            }
          }
                </style>
                <title>Application</title>`);
      mywindow.document.write('</head><body >');
      mywindow.document.write(html);
      mywindow.document.write('</body></html>');
      mywindow.document.close(); // necessary for IE >= 10
      mywindow.focus(); // necessary for IE >= 10
      mywindow.print();
      mywindow.close();
    }
  }
}
