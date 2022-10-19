import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { orderBy, SortDescriptor } from '@progress/kendo-data-query';
import { Subject, Subscription } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { AuthenticationService } from '../../../_services/authentication.service';
import { DailyWagerService } from '../../../_services/daily-wager.service';
import { RootService } from '../../../_services/root.service';
import { ProfileCompactView } from '../../profile/profile.class';

@Component({
  selector: 'app-dailywagerlist',
  templateUrl: './dailywagerlist.component.html',
  styleUrls: ['./dailywagerlist.component.scss']
})
export class DailywagerlistComponent implements OnInit {
  public hfmisCodeParam: string;

  public currentUser: any;
  public currentOfficer: any;
  private subscription: Subscription;
  //loading flag
  public loading = false;
  public searching = false;
  
  //grid data
  public gridView: GridDataResult;
  public profiles: ProfileCompactView[] = [];
  public profilesAll: any[] = [];
  public userViewType: number = 2;
  public viewTypes: number[] = [1, 2, 3];
  //sorting variable
  public multiple = false;
  public allowUnsort = true;
  public sort: SortDescriptor[] = [];
  //Pagination variables
  public pageSizes = [50, 100, 200, 500];
  public totalRecords = 0;
  public pageSize = 50;
  public skip = 0;
  //dropdowns variables
  public designationStr : "";
  public dropDowns: DropDownsHR = new DropDownsHR();
  public hfmisCode: string = '000000';
  public searchTerm: string = '';
  public selectedCadres: any[] = [];
  public selectedDesignations: any[] = [];
  public selectedStatuses: any[] = [];
  public selectedEmployementModes: any[] = [];
  public isShowUC = false;
  public isShowHF = false;

  //dropdowns : need two same arrays for search/filter purpose
  public uc: Array<{ text: string, value: string }>;
  public ucData: Array<{ text: string, value: string }>;
  public divisions: Array<{ text: string, value: string }>;
  public divisionsData: Array<{ text: string, value: string }>;
  public districts: Array<{ text: string, value: string }>;
  public districtsData: Array<{ text: string, value: string }>;
  public tehsils: Array<{ text: string, value: string }>;
  public tehsilsData: Array<{ text: string, value: string }>;
  public cadres: Array<{ text: string, value: string }>;
  public cadresData: Array<{ text: string, value: string }>;
  public designations: Array<{ text: string, value: string }>;
  public designationsData: Array<{ text: string, value: string }>;
  public hf : Array<{ text: string, value: string }>;
  public hfData : Array<{ text: string, value: string }>;
  public bank: Array<{ text: string, value: string }>;
  public bankData: Array<{ text: string, value: string }>;
  //selected object for drop downs
  public selectedFiltersModel: any = {
    division: { Name: 'Select Division', Code: '0' },
    district: { Name: 'Select District', Code: '0' },
    tehsil: { Name: 'Select Tehsil', Code: '0' }
  };

  public CategoryList : string[]=[
    'Dengue','Madadgaar','PMIS','Polio' ,'Other' 
   
   
   ]
  public DesignationList: string[]= [
    'Dengue',
    'Dengue/Regular',
    'Polio',
    'Madadgaar',
    'PMIS',
'Lady Sanitary Patrol',
'Electrician',
'Data Entry Operator',
'Lift Operator',
'Stretcher Bearer',
'Sanitary Patrol',
'Computer Operator / Data Entry Operator',
'Ward Servant',
'Lab Technician',
'Data Entry Operator (Health Council Budget)',
'Gatekeeper',
'Security Guard',
'Lab Attendant',
'Wheel Chair/Strature',
  ]
  public imageURL:any;
  public fields: string[] = [];
  public isPool: boolean = false;
  public isInactive: boolean = false;
  public retirementInOneYear: boolean = false;
  public retirementAlerted: boolean = false;
  public inputChange: Subject<any>;
  private searchSubscription: Subscription;
  public designationCode = null;

  public searchedCNIC: string = "";
  public IsSearched : boolean =false;
  public IsList : boolean=false;
  public PersonImageURL : any;
  public  obj: any = {};
  public contractId = 0;  
  public districtCode = null;
  public divisionCode = null;
  public tehsilCode = null;
  public hfCode = null;
  public ucCode = null;
  
  constructor(
    private _authenticationService: AuthenticationService,
    private _rootService: RootService,
    public _DailyWagerService : DailyWagerService,
    private route: ActivatedRoute
    
  ) { }

  ngOnInit() {
    this.currentUser = this._authenticationService.getUser();
    this.currentOfficer = this._authenticationService.getCurrentOfficer();
    this.hfmisCode = this.currentUser.HfmisCode;
    this.initializeProps();
    this.subscribeToFilter();
  }

  onChangedDesignation(event){
    console.log(event.value);
    if(this.designationCode == 'Dengue' ||
     this.designationCode == 'Polio'){
      this.isShowUC = true;
      this.isShowHF = false;
    }
    else{
      this.isShowUC = false;
      this.isShowHF = true;

    }
  }
  
  public onSearch() {
    this.searching = true;
    this.skip = 0;
    this.loadProfiles();
  }

  public loadProfiles() {
    this.loading = true;
    if (this.isPool) {
      debugger
      this._DailyWagerService.GetDailyWagesInPool(this.skip, this.pageSize, this.hfmisCode, this.searchTerm, this.selectedCadres, this.selectedDesignations).subscribe(
        response => this.handleResponse(response),
        err => this.handleError(err)
      );
    
    } else {
      debugger
      this._DailyWagerService.getProfiles(this.skip, this.pageSize, this.hfmisCode, this.searchTerm, this.selectedCadres, this.selectedDesignations, this.selectedStatuses, this.retirementInOneYear, this.retirementAlerted, this.designationStr).subscribe(
        response => this.handleResponse(response),
        err => this.handleError(err)
      );
    }

  }
  private handleResponse(response: any) {
    debugger
    this.profiles = [];
    this.profiles = response.List.List;
    
    if(this.searchTerm != "" && this.searchTerm != null )
      {
        this.searchedCNIC = this.searchTerm;
        this.IsSearched = true;
      }
      else{
        this.IsList = true;
      }

      

      this.SaveLogs();
    this.totalRecords = response.List.Count;
    this.gridView = { data: this.profiles, total: this.totalRecords };
    this.loading = false;
    this.searching = false;

    /* this._profileService.getProfiles(0, this.totalRecords, this.hfmisCode, this.searchTerm, this.selectedCadres, this.selectedDesignations, this.retirementInOneYear).subscribe(
      (response: any) => {
        this.profilesAll = [];
        this.profilesAll = response.List;
        this.fields = Object.keys(this.profilesAll[0]);
      },
      err => this.handleError(err)
    ); */
  }
  private initializeProps() {
    this.loadDropdownValues();
    // this.getSetSavedView();
    this.fetchParams();
    this.multiDropFilter();
  }

  CustomerImagePreview(files,Id:any) {
    debugger
     this.contractId = Id;
    if (files.length === 0)
      return;
    var mimeType = files[0].type;
    if (mimeType.match(/image\/*/) == null) {
      // this.message = "Only images are supported.";
      return;
    }
 
    var reader = new FileReader();
    // this.imagePath = files;
    reader.readAsDataURL(files[0]); 
    reader.onload = (_event) => {
      this.PersonImageURL = reader.result; 
      this.imageURL = this.PersonImageURL.split(',')[1];
      this.AddContractFile(Id,this.imageURL);
    }
  }

  AddContractFile(Id:any,imageFile:any){
    debugger
    this._DailyWagerService.AddContractFileById(Id,imageFile).subscribe((res: any) => {
      if (res.Status) {
        alert('File Added Successfully')
        this.loadProfiles();
          // this.dailyWagerProfileForm.reset();
      }
      else{
        alert(res.Message)
      }
    }, err => {
      console.log(err);
    })
  }



  private subscribeToFilter() {
    debugger
    this.inputChange = new Subject();
    this.searchSubscription = this.inputChange.pipe(debounceTime(300)).subscribe((query: boolean) => {
      this.loadProfiles();
    });
  }

  public sortChange(sort: SortDescriptor[]): void {
    if (sort[0].field == 'asd') { return; }
    this.sort = sort;
    this.sortData();
  }
  private sortData() {
    this.gridView = {
      data: orderBy(this.profiles, this.sort),
      total: this.totalRecords
    };
  }
  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadProfiles();
  }
  public changePagesize(value: any) {
    this.pageSize = +value;
    this.skip = 0;
    this.loadProfiles();
  }

  public numberWithCommas(x) {
    if (x) {
      return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }
  }
  private multiDropFilter = () => {
    /*  if (this.cadresList) {
       this.cadresList.filterChange.asObservable().pipe(
         switchMap(value => from([this.cadres]).pipe(
           delay(300),
           map((data) => data.filter(contains(value)))
         ))
       ).subscribe(x => {
         this.cadresData = x;
       });
     } */
    
   
    /*  
     this.employementModesList.filterChange.asObservable().pipe(
       switchMap(value => from([this.dropDowns.employementModes]).pipe(
         delay(300),
         map((data) => data.filter(contains(value)))
       ))
     ).subscribe(x => {
       this.dropDowns.employementModesData = x;
     });
     this.specializationList.filterChange.asObservable().pipe(
       switchMap(value => from([this.dropDowns.specialization]).pipe(
         delay(300),
         map((data) => data.filter(contains(value)))
       ))
     ).subscribe(x => {
       this.dropDowns.specializationData = x;
     });
     this.qualificationsList.filterChange.asObservable().pipe(
       switchMap(value => from([this.dropDowns.qualifications]).pipe(
         delay(300),
         map((data) => data.filter(contains(value)))
       ))
     ).subscribe(x => {
       this.dropDowns.qualificationsData = x;
     }); */
    const contains = value => (s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1;
  }
  public SaveLogs()
  {
  }
  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('page')) {
          let param = params['page'];
          if (+param) {
            this.skip = (+params['page'] - 1) as number;
            this.isPool = false;
          } else if (param == 'pool') {
            this.isPool = true;
            this.isInactive = false;
            this.retirementInOneYear = false;
            this.retirementAlerted = false;
          }
          else if (param == 'InActive') {
            this.isInactive = true;
            this.retirementInOneYear = false;
            this.retirementAlerted = false;
            this.isPool = false;
          }
          else if (param == 'retiree') {
            this.retirementInOneYear = true;
            this.retirementAlerted = false;
            this.isInactive = false;
            this.isPool = false;
          }
          else if (param == 'retireeSms') {
            this.retirementAlerted = true;
            this.retirementInOneYear = false;
            this.isInactive = false;
            this.isPool = false;
          }
          this.loadProfiles();
        } else {
          this.loadProfiles();
        }
      }
    );
  }

  private loadDropdownValues = () => {
    this.getDivisions();
    this.getDistricts();
     this.getTehsils();

    // this.getDesignations();
    // this.getStatuses();
    /*    this.getCadres();
      this.getEmploymentModes();
      this.getSpecializations();
      this.getQualifications(); */
  }

  private getDivisions = () => {
     
    this.divisions = [];
    this.divisionsData = [];
    this._rootService.getDivisions(this.hfmisCode).subscribe((res: any) => {
      this.divisions = res;
      this.divisionsData = this.divisions.slice();
      if (this.divisions.length == 1) {
        this.selectedFiltersModel.division = this.divisionsData[0];
      }
    },
      err => { this.handleError(err); }
    );
  }
  private getDistricts = () => {
    this.districts = [];
    this.districtsData = [];
     
    this._rootService.getDistricts(this.hfmisCode).subscribe((res: any) => {
      this.districts = res;
      this.districtsData = this.districts.slice();
      if (this.districts.length == 1) {
        this.selectedFiltersModel.district = this.districtsData[0];
      }
    },
      err => { this.handleError(err); }
    );
  }
  private getTehsils = () => {
    this.tehsils = [];
    this.tehsilsData = [];
    this._rootService.getTehsils(this.hfmisCode).subscribe((res: any) => {
      this.tehsils = res;
      this.tehsilsData = this.tehsils.slice();
      if (this.tehsils.length == 1) {
        this.selectedFiltersModel.tehsil = this.tehsilsData[0];
      }
    },
      err => { this.handleError(err); }
    );
  }
  private handleError(err: any) {
    this.loading = false;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }
  public dropdownValueChanged = (value, filter) => {
     
    if (!value || !value.Code) {
      return;
      
    }
     
    this.hfmisCodeParam = undefined;
    this.hfmisCode = value.Code;

    if (filter == 'division') {
       
      this.divisionCode = null;
      this.divisionCode = value.Code;
      this.selectedFiltersModel.district.Code = this.hfmisCode;
      this.resetDrops(filter);
      this.getDistricts();
      // this.getTehsils();
    }
    if (filter == 'district') {
      this.districtCode = null;
      this.districtCode = value.Code;
      this.selectedFiltersModel.tehsil.Code = this.hfmisCode;
      this.resetDrops(filter);
      this.getTehsils();
    }
    if (filter == 'tehsil') {
      this.tehsilCode = null;
      this.tehsilCode = value.Code;
      this.selectedFiltersModel.hf.Code = this.hfmisCode;
      this.resetDrops(filter);
      this.getHealthFacilities();
      this.getUCs();
    }
    if (filter == 'hf') {
     
      this.selectedFiltersModel.uc.Code = this.hfmisCode;
      this.resetDrops(filter);
      this.getUCs();
    }
    if (filter == 'uc') {
       
      
      this.selectedFiltersModel.uc.Code = this.hfmisCode;
      this.resetDrops(filter);
      this.getUCs();
    }
    // if (filter == 'bank') {
    //   this.selectedFiltersModel.uc.Code = this.hfmisCode;
    //   this.resetDrops(filter);
    //   this.getUCs();
    // }
    // if (filter == 'designation') {
     
    //   this.selectedFiltersModel.uc.Code = this.hfmisCode;
    //   this.resetDrops(filter);
    //   this.getUCs();
    // }
  }
  public handleFilter = (value, filter) => {
    if (filter == 'division') {
      this.divisionsData = this.divisions.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
    if (filter == 'district') {
      this.districtsData = this.districts.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
    if (filter == 'tehsil') {
      this.tehsilsData = this.tehsils.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
    if (filter == 'hf') {
      this.hfData = this.hf.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
    if (filter == 'uc') {
      this.ucData = this.uc.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
    if (filter == 'bank') {
      this.bankData = this.bank.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
  }
  private resetDrops = (filter: string) => {
    if (filter == 'division') {
      this.selectedFiltersModel.district = { Name: 'Select District', Code: this.currentUser.HfmisCode };
      this.selectedFiltersModel.tehsil = { Name: 'Select Tehsil', Code: this.currentUser.HfmisCode };
    }
    if (filter == 'district') {
      this.selectedFiltersModel.tehsil = { Name: 'Select Tehsil', Code: this.currentUser.HfmisCode };
    }
  }

  private getHealthFacilities = () => {
    this._rootService.getHealthFacilities(this.hfmisCode).subscribe((res: any) => {
       
      this.hf = res;
      this.hfData = this.hf.slice();
      // this.dropDowns.healthFacilities = res;
      // this.dropDowns.healthFacilitiesData = this.dropDowns.healthFacilities.slice();

    },
      err => { this.handleError(err); }
    );
    
  }
  private getUCs(){
    this.uc = [];
    this.ucData = [];
    this._rootService.GetHFUCsForDailyWages(this.hfmisCode).subscribe((res: any) => {
       
      this.uc = res;
      this.ucData = this.uc.slice();
      console.log(res);

    },
      err => { this.handleError(err); }
    );
  }

  get showInitialPageSize() {
    return this.pageSizes
      .filter(num => num === Number(this.pageSize))
      .length === 0;
  }
}


