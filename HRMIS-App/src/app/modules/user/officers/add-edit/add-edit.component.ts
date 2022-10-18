import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs/Subject';
import { Subscription } from 'rxjs/Subscription';
import { ActivatedRoute } from '@angular/router';
import { SortDescriptor, orderBy } from '@progress/kendo-data-query';
import { User } from '../../../../_models/user.class';
import { KGridHelper } from '../../../../_helpers/k-grid.class';
import { DropDownsHR } from '../../../../_helpers/dropdowns.class';
import { NotificationService } from '../../../../_services/notification.service';
import { RootService } from '../../../../_services/root.service';
import { AuthenticationService } from '../../../../_services/authentication.service';
import { PandSOfficerView, PandSOfficerFilters } from '../../user-claims.class';
import { UserService } from '../../user.service';

@Component({
  selector: 'app-officers-add-edit',
  templateUrl: './add-edit.component.html',
  styles: []
})
export class AddEditComponent implements OnInit, OnDestroy {
  public currentUser: User;
  public currentOfficer: any;
  public officer: any = {};
  public officerFingerPrints: any[] = [];
  public cadres: any[] = [];
  public designations: any[] = [];
  public usersAll: any[] = [];
  public users: any[] = [];
  public userId: string = '';
  public officerId: number = 0;
  public selectedOfficer_Id: number = 0;
  public selectedCadre_Id: number = 0;
  public selectedDesignation_Id: number = 0;
  public kGrid: KGridHelper = new KGridHelper();
  public dropDowns: DropDownsHR = new DropDownsHR();
  public pSOfficerFilters: PandSOfficerFilters;
  public inputChange: Subject<any>;
  public searchQuery: string = '';
  public cnicMask: string = "00000-0000000-0";
  public mobileMask: string = "0000-0000000";
  public savingOfficer: boolean = false;
  public addingOfficer: boolean = false;
  public addingDesignation: boolean = false;
  public addingCadre: boolean = false;
  public relatedData: any = { concernedOfficers: [], concernedDesignations: [], concernedCadres: [], fps: [] };
  public subscription: Subscription;
  constructor(private route: ActivatedRoute,
    public _notificationService: NotificationService,
    private _rootService: RootService,
    private _authenticationService: AuthenticationService,
    private _userSerivce: UserService) { }

  ngOnInit() {
    debugger;
    this.currentOfficer = this._authenticationService.getCurrentOfficer();
    this.fetchParams();
    this.getDesignations();
    this.getCadres();
    this.getPandSOfficers('all');
  }
  private fetchParams() {
    this.officer.Code = '99999999';
    this.officer.Program = 'Office Register';
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('officerId') && params.hasOwnProperty('userId')) {
          this.pSOfficerFilters = new PandSOfficerFilters();
          this.pSOfficerFilters.User_Id = params['userId'];
          this.pSOfficerFilters.OfficerId = +params['officerId'];
          if (this.pSOfficerFilters.User_Id || this.pSOfficerFilters.OfficerId != 0) {
            this.getOfficer();
          }
        } else {
          this.getUsers();
        }
      }
    );
  }
  public getOfficer() {
    debugger;
    this._rootService.getOfficerData(this.pSOfficerFilters.User_Id, this.pSOfficerFilters.OfficerId).subscribe((response: any) => {
      if (response && response.officer) {
        this.officer = response.officer;
        this.relatedData.concernedOfficers = response.concernedOfficers;
        this.relatedData.fps = response.fingerPrints;
        this.dropDowns.selectedFiltersModel.designation = { Id: this.officer.Designation_Id, Name: this.officer.HrDesignationName }
      } else {
        this.officer = new PandSOfficerView();
        this.officer.Code = '99999999';
      }

      this.getUsers();
      this.savingOfficer = false;
    }, err => {
      this.handleError(err);
    });
  }

  private getDesignations = () => {
    this._rootService.getDesignations().subscribe((res: any) => {
      if (res) {
        this.designations = res;
        this.dropDowns.designations = res;
        this.dropDowns.designationsData = this.dropDowns.designations.slice();
      }

    },
      err => { this.handleError(err); }
    );
  }
  private getPandSOfficers = (type: string) => {
    this.dropDowns.officers = [];
    this.dropDowns.officersData = [];
    this._rootService.getPandSOfficers(type).subscribe((res: any) => {
      if (res) {
        this.dropDowns.officers = res;
        this.dropDowns.officers.forEach(officer => {
          if (officer.DesignationName == null || !officer.DesignationName) {
            officer.DesignationName = '';
          }
        });
        this.dropDowns.officersData = this.dropDowns.officers;
      }
    },
      err => { this.handleError(err); }
    );
  }
  private getCadres = () => {
    this._rootService.getCadres().subscribe((res: any) => {
      if (res) {
        this.dropDowns.cadres = res;
        console.log(this.dropDowns.cadres);

      }
    },
      err => { this.handleError(err); }
    );
  }
  public dropdownValueChanged = (value, filter) => {
    if (!value) {
      return;
    }
    if (filter == 'designation') {
      this.officer.Designation_Id = value.Id;
    }
    if (filter == 'officer') {
      console.log(value.Id);
    } if (filter == 'user') {
      this.officer.User_Id = value.Id;
    }
  }
  public handleFilter = (value, filter) => {
    if (filter == 'user') {
      this.users = this.usersAll.filter((s: any) => s.UserName.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
    if (filter == 'officer') {
      console.log(value);

      this.dropDowns.officersData = this.dropDowns.officers.filter((s: any) => s.DesignationName.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
  }
  /* 
    public mailOfficer() {
      this._userSerivce.mailOfficer(this.officer).subscribe((res: any) => {
        if (res) {
          this.getOfficer();
          this._notificationService.notify('success', 'Officer Mailed');
        }
      }, err => {
        console.log(err);
  
      });
    } */
  public getUsers() {
    this._userSerivce.getUsers(0, 50000, '', 'Office Diary').subscribe(
      (response: any) => {
        if (response.List) {
          this.users = response.List;
          this.usersAll = this.users;
          debugger;
          this.users = this.users.filter(user => !(parseInt(user.UserName) == user.UserName));
          if (this.officer.User_Id) {
            this.users.forEach(user => {
              if (user.Id == this.officer.User_Id) {
                this.dropDowns.selectedFiltersModel.user = { Id: user.Id, UserName: user.UserName };
              }
            });
          }
        }
      },
      err => this.handleError(err)
    );
  }
  public onSubmit(value: any) {
    this.savingOfficer = true;
    this._userSerivce.saveOfficer(this.officer).subscribe((res: any) => {
      if (res) {
        //this.pSOfficerFilters.User_Id = res.User_Id;
        //this.getOfficer();
        this.savingOfficer = false;
        this._notificationService.notify('success', 'Office Added!');
      } else {
        this.savingOfficer = false;
        this._notificationService.notify('danger', 'Office not added. Add from database.');
      }
    }, err => {
      console.log(err);
    });
  }
  public addOfficerData(type: number, add: boolean) {
    if (this.pSOfficerFilters.add) {
      this._notificationService.notify('success', 'Conerned Officer Added!');
      this.addingOfficer = true;
    }
    this.pSOfficerFilters.tableType = type;
    this.pSOfficerFilters.add = add;
    this._userSerivce.saveOfficerData(this.pSOfficerFilters).subscribe((res: any) => {
      if (res) {
        if (this.pSOfficerFilters.add) {
          this._notificationService.notify('success', 'Conerned Officer Added!');
        } else {
          this._notificationService.notify('danger', 'Conerned Officer Removed!');
        }
        this.getOfficer();
        this.addingOfficer = false;
      }
    }, err => {
      this.handleError(err);
    });
  }
  public removeOfficer(id: number) {
    this.pSOfficerFilters.concernedId = id;
    this.addOfficerData(1, false);
  }
  public sortChange(sort: SortDescriptor[]): void {
    if (sort[0].field == 'asd') { return; }
    this.kGrid.sort = sort;
    this.sortData();
  }
  private sortData() {
    this.kGrid.gridView = {
      data: orderBy(this.kGrid.data, this.kGrid.sort),
      total: this.kGrid.totalRecords
    };
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
  ngOnDestroy() {
    this.subscription.unsubscribe();
  }
  private handleError(err: any) {
    this.kGrid.loading = false;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }
}
