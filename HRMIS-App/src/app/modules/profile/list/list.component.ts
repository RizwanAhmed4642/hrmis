import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';

import { SortDescriptor, orderBy } from '@progress/kendo-data-query';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { AuthenticationService } from '../../../_services/authentication.service';
import { RootService } from '../../../_services/root.service';
import { from } from 'rxjs/observable/from';
import { delay } from 'rxjs/operators/delay';
import { switchMap } from 'rxjs/operators/switchMap';
import { map } from 'rxjs/operators/map';
import { ProfileService } from '../profile.service';
import { ProfileCompactView } from '../profile.class';
import { Subscription } from 'rxjs/Subscription';
import { ActivatedRoute } from '@angular/router';
import { LocalService } from '../../../_services/local.service';
import { Subject } from 'rxjs/Subject';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { NotificationService } from '../../../_services/notification.service';
import { isFirstDayOfWeek } from 'ngx-bootstrap';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styles: []
})
export class ListComponent implements OnInit, OnDestroy {
  @ViewChild("cadresList", { static: true }) cadresList;
  @ViewChild("designationsList", { static: true }) designationsList;
  @ViewChild("statusList", { static: true }) statusList;
  @ViewChild("employementModesList", { static: true }) employementModesList;
  @ViewChild("qualificationsList", { static: true }) qualificationsList;
  @ViewChild("specializationList", { static: true }) specializationList;

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
  public dropDowns: DropDownsHR = new DropDownsHR();
  public hfmisCode: string = '000000';
  public searchTerm: string = '';
  public selectedCadres: any[] = [];
  public selectedDesignations: any[] = [];
  public selectedStatuses: any[] = [];
  public selectedEmployementModes: any[] = [];
  //dropdowns : need two same arrays for search/filter purpose
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
  //selected object for drop downs
  public selectedFiltersModel: any = {
    division: { Name: 'Select Division', Code: '0' },
    district: { Name: 'Select District', Code: '0' },
    tehsil: { Name: 'Select Tehsil', Code: '0' }
  };
  public fields: string[] = [];
  public isPool: boolean = false;
  public isInactive: boolean = false;
  public retirementInOneYear: boolean = false;
  public retirementAlerted: boolean = false;
  public inputChange: Subject<any>;
  private searchSubscription: Subscription;




  public searchedCNIC: string = "";
  public IsSearched : boolean =false;
  public IsList : boolean=false;

  public  obj: any = {};

  constructor(private _localService: LocalService, private route: ActivatedRoute, private _rootService: RootService,
    private _profileService: ProfileService,
    private _notificationService: NotificationService,
    private _authenticationService: AuthenticationService
  ) { }

  ngOnInit() {
    this.currentUser = this._authenticationService.getUser();
    this.currentOfficer = this._authenticationService.getCurrentOfficer();
    this.hfmisCode = this.currentUser.HfmisCode;
    this.initializeProps();
    this.subscribeToFilter();
  }

  public SaveLogs()
  {

    
    this.obj.searchedCNIC = '';
    this.obj.searchedCNIC=this.searchedCNIC;
    this.obj.IsSearched=this.IsSearched;
    this.obj.IsList=this.IsList;
    debugger
    this._profileService.SaveLogs(this.obj).subscribe((res: any)=>{

      this.searchedCNIC=' ';
      this.IsSearched=false;
      this.IsList=false;

    });
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
  private subscribeToFilter() {
    this.inputChange = new Subject();
    this.searchSubscription = this.inputChange.pipe(debounceTime(300)).subscribe((query: boolean) => {
      this.loadProfiles();
    });
  }
  private initializeProps() {
    this.loadDropdownValues();
    this.getSetSavedView();
    this.fetchParams();
    this.multiDropFilter();
  }
  private loadDropdownValues = () => {
    this.getDivisions();
    this.getDistricts();
    this.getTehsils();

    this.getDesignations();
    this.getStatuses();
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
    },
      err => { this.handleError(err); }
    );
  }
  private getCadres = () => {
    this._rootService.getCadres().subscribe((res: any) => {
      console.log(res);

      this.cadres = res;
      this.cadresData = this.cadres.slice();
    },
      err => { this.handleError(err); }
    );
  }
  private getDesignations = () => {
    this._rootService.getDesignations().subscribe((res: any) => {
      this.designations = res;
      this.designationsData = this.designations.slice();
    },
      err => { this.handleError(err); }
    );
  }
  private getStatuses = () => {
    this._rootService.getProfileStatuses().subscribe((res: any) => {
      this.dropDowns.profileStatuses = res;
      this.dropDowns.profileStatusesData = this.dropDowns.profileStatuses;
    },
      err => { this.handleError(err); }
    );
  }
  private getEmploymentModes = () => {
    this._rootService.getEmploymentModes().subscribe((res: any) => {
      this.dropDowns.employementModes = res;
      this.dropDowns.employementModesData = this.dropDowns.employementModes;
    },
      err => { this.handleError(err); }
    );
  }
  private getSpecializations = () => {
    this._rootService.getSpecializations().subscribe((res: any) => {
      this.dropDowns.specialization = res;
      this.dropDowns.specializationData = this.dropDowns.specialization;
    },
      err => { this.handleError(err); }
    );
  }
  private getQualifications = () => {
    this._rootService.getQualifications().subscribe((res: any) => {
      this.dropDowns.qualifications = res;
      this.dropDowns.qualificationsData = this.dropDowns.qualifications;
    },
      err => { this.handleError(err); }
    );
  }
  public sendSMSToEmployee = (profile: any, index: number) => {
    if (confirm("Send SMS Alert ?")) {
      profile.sendingSMS = true;
      let smsObj: any = {
        SMSType_Id: 1,
        Profile_Id: profile.Id,
        CNIC: profile.CNIC,

      };
      this._profileService.sendSMSToEmployee(smsObj).subscribe((res: any) => {
        if (res && res.Id) {
          profile.sendingSMS = false;
          this.gridView.data.splice(index, 1);
          this.gridView.total = (this.gridView.total - 1);
          this._notificationService.notify('success', 'SMS Sent. Profile Shifted to Sent List.');
        }
        profile.sendingSMS = false;
      },
        err => { profile.sendingSMS = false; this.handleError(err); }
      );
    }
  }
  public loadProfiles() {

    this.loading = true;
    if (this.isPool) {
      debugger
      this._profileService.getProfilesInPool(this.skip, this.pageSize, this.hfmisCode, this.searchTerm, this.selectedCadres, this.selectedDesignations).subscribe(
        response => this.handleResponse(response),
        err => this.handleError(err)
      );
    } else if (this.isInactive) {
      debugger
      this._profileService.getProfilesInActive(this.skip, this.pageSize, this.hfmisCode, this.searchTerm, this.selectedCadres, this.selectedDesignations).subscribe(
        response => this.handleResponse(response),
        err => this.handleError(err)
      );
    } else {
      debugger

      


      this._profileService.getProfiles(this.skip, this.pageSize, this.hfmisCode, this.searchTerm, this.selectedCadres, this.selectedDesignations, this.selectedStatuses, this.retirementInOneYear, this.retirementAlerted).subscribe(
        response => this.handleResponse(response),
        
        err => this.handleError(err)
      );
    }

  }

  

  private handleResponse(response: any) {
    debugger
    this.profiles = [];
    this.profiles = response.List;
    
    if(this.searchTerm != "" && this.searchTerm != null )
      {
        this.searchedCNIC = this.searchTerm;
        this.IsSearched = true;
      }
      else{
        this.IsList = true;
      }

      

      this.SaveLogs();
    this.totalRecords = response.Count;
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
  private handleError(err: any) {
    this.loading = false;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }
  public openInNewTab(link: string) {
    window.open('/' + link, '_blank');
  }
  public dropdownValueChanged = (value, filter) => {
    if (!value || !value.Code) {
      return;
    }
    this.hfmisCode = value.Code;

    if (filter == 'division') {
      this.resetDrops(filter);
      this.getDistricts();
      this.getTehsils();
    }
    if (filter == 'district') {
      this.resetDrops(filter);
      this.getTehsils();
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
      this.divisionsData = this.divisions.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
    if (filter == 'district') {
      this.districtsData = this.districts.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
    if (filter == 'tehsil') {
      this.tehsilsData = this.tehsils.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
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
    if (this.designationsList) {
      this.designationsList.filterChange.asObservable().pipe(
        switchMap(value => from([this.designations]).pipe(
          delay(300),
          map((data) => data.filter(contains(value)))
        ))
      ).subscribe(x => {
        this.designationsData = x;
      });
    }
    this.statusList.filterChange.asObservable().pipe(
      switchMap(value => from([this.dropDowns.profileStatuses]).pipe(
        delay(300),
        map((data) => data.filter(contains(value)))
      ))
    ).subscribe(x => {
      this.dropDowns.profileStatusesData = x;
    });
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
  public onSearch() {
    this.searching = true;
    this.skip = 0;
    this.loadProfiles();
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
  public hfClicked(HFMISCode: string, type: number) {
    type === 1 ?
      window.open('/health-facility/' + HFMISCode + '/edit', '_blank') :
      window.open('/health-facility/' + HFMISCode, '_blank');
  }
  public changeView(value: number) {
    this._localService.set('profileViewType', value);
    this.getSetSavedView();
  }
  public getSetSavedView() {
    let savedView = this._localService.get('profileViewType');
    this.userViewType = +savedView ? +savedView : 1;
  }
  // helpers
  get showInitialPageSize() {
    return this.pageSizes
      .filter(num => num === Number(this.pageSize))
      .length === 0;
  }
  public dashifyCNIC(cnic: string) {
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
  public numberWithCommas(x) {
    if (x) {
      return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }
  }
  ngOnDestroy() {
    if (this.subscription) this.subscription.unsubscribe();
    if (this.searchSubscription) this.searchSubscription.unsubscribe();
  }
}
