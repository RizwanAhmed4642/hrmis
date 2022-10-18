import { AddLeave } from "./add-leave.class";
import { DropDownsHR } from "../../../_helpers/dropdowns.class";
import {
  Component,
  OnInit,
  Inject,
  ViewChild,
  NgZone,
  OnDestroy
} from "@angular/core";
import { Observable } from "rxjs/Observable";
import { Subscription } from "rxjs/Subscription";
import { NotificationService } from "../../../_services/notification.service";
import {
  GridDataResult,
  PageChangeEvent,
  GridComponent
} from "@progress/kendo-angular-grid";
import {
  State,
  process,
  SortDescriptor,
  orderBy
} from "@progress/kendo-data-query";
import { KGridHelper } from "../../../_helpers/k-grid.class";
import { RootService } from "../../../_services/root.service";
import { AttandanceService } from "../attandance.service";
import { Validators, FormBuilder, FormGroup } from "@angular/forms";
import { AuthenticationService } from "../../../_services/authentication.service";
import { take } from "rxjs/operators/take";
import { from } from "rxjs/observable/from";
import { delay } from "rxjs/operators/delay";
import { switchMap } from "rxjs/operators/switchMap";
import { map } from "rxjs/operators/map";
import { Subject } from 'rxjs/Subject';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { Profile } from '../../profile/profile.class';
import { ActivatedRoute, Router } from '@angular/router';
import { ProfileService } from '../../profile/profile.service';
import { cstmdummyHOD } from '../../../_models/cstmdummydata';


@Component({
  selector: 'app-add-leave',
  templateUrl: './add-leave.component.html',
  styleUrls: ['./add-leave.component.scss']
})
export class AddLeaveComponent implements OnInit {
  public loading = false;
  public addLeave: AddLeave;
  public editLeave: boolean = false;
  public dropDowns: DropDownsHR = new DropDownsHR();
  public currentUser: any;
  public hfmisCode: string;
  public dateNow: string = '';
  public searchSubcription: Subscription = null;
  public searchEvent = new Subject<any>();
  public addingLeave = false;
 // public ProfileID: string;
  public profile: Profile;
  private subscription: Subscription;
  public cnic: string = '0';
  public existingProfile: boolean = false;
  public selectedFiltersModel: any;
  public cnicMask: string = "00000-0000000-0";
 /*  public employee = {
    profileid:0,
    name: '',
    code: '',
    designation: '',
    department: '',
    cnic:''
  } */

  public totalLeaves = {
    ProfileID:0,
    EmployeeName: '',
    Designation_Name: '',
    HealthFacility: '',
    Cnic:'',
    TotalSL: 0,
    TotalSick: 0,
    TotalCasual: 0,
    TotalLev: 0,
    Bal:0
  }

  constructor(
    private router: Router,
    private _rootService: RootService,
    private _profileService: ProfileService,
    private _attandanceService: AttandanceService,
    private _authenticationService: AuthenticationService,
    private ngZone: NgZone,
    public _notificationService: NotificationService,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
  // debugger;
    this.addLeave = new AddLeave();
    this.loadDropdownValues();
    this.currentUser = this._authenticationService.getUser();
    this.selectedFiltersModel = this.dropDowns.selectedFiltersModel;
    this.hfmisCode = this.currentUser.HfmisCode;
    this.addLeave.LeaveTypeID = 1;
    
  }


  private initializeProps() {
    let today = new Date();
    let dd: any = today.getDate(), mm: any = today.getMonth() + 1, yyyy = today.getFullYear();
    if (dd < 10) dd = '0' + dd; if (mm < 10) mm = '0' + mm;
    this.dateNow = dd + '/' + mm + '/' + yyyy;
    /* setInterval(() => {
      this.updateLivePreview(true);
    }, 1000); */
    this.handleSearchEvents();
  }

  public handleSearchEvents() {
    this.searchSubcription = this.searchEvent.pipe(
      debounceTime(400)).subscribe((x: any) => {
       // console.log(x);
       // this.search(x.event, x.filter);
      });
  }
  
  public saveLeave() {
    debugger;
    this.addingLeave = true;
   // this.addLeave.ProfileID = this.totalLeaves.ProfileID;
    this.addLeave.LeaveStatusID = 1;
    this.addLeave.CBalance=this.addLeave.Bal;

if(this.addLeave.LeaveTo){
  this.addLeave.LeaveTo = this.addLeave.LeaveTo.toDateString();
}
if(this.addLeave.LeaveFrom){
  this.addLeave.LeaveFrom = this.addLeave.LeaveFrom.toDateString();
}

    this._attandanceService.addLeave(this.addLeave).subscribe((res: any) => {
        if (res) {
       // console.log(res);
          let addLeave = this.addLeave;
          this._notificationService.notify(
            "success",
            "Leave Saved!"
          );
       
          this.addingLeave = false;
          this.router.navigate(['/attandance/leaveList']);
        
        }
         else if(res == 'Invalid') {
          this._notificationService.notify(
            "danger",
            "Cannot save duplicate leave."
          );
          this.addingLeave = false;
        }
        this.addLeave = new AddLeave();
      },
      err => {
        console.log(err);
      }
    );
  }

  public dropdownValueChanged = (value, filter) => {
  
    if (filter == 'hod') {
      this.addLeave.ForwardedByID = value.Id;
    }
  }

  public radioValueChanged = (value, filter) => {
    debugger;
      if (filter == '3') {
        this.addLeave.TotalDays = 0.5;
     
    }  
    
  }

  private loadDropdownValues = () => {
   // this.getLeaveTypes();
  };

  private handleError(err: any) {
    this._notificationService.notify("danger", "Error!");
    this.loading = false;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }

  public clearData() {
  //  debugger;
  }

  public profileSearch1(){
  // debugger;
    this._attandanceService.getTotalLeaves(this.addLeave.Cnic).subscribe(
      (lev: any) => {   
        if (lev) 
        {
                  this.addLeave.EmployeeName = lev.EmployeeName;
                  this.addLeave.Cnic = lev.Cnic;
                  this.addLeave.Designation_Name = lev.Designation_Name;
                  this.addLeave.HealthFacility = lev.HealthFacility;
                  this.addLeave.ProfileID = lev.ProfileID;

//debugger;
                  if(lev.TotalSL == 0) {
                    this.addLeave.TotalSL = lev.TotalSL;
                   }
                   else if(lev.TotalSL == 0.5) 
                   {
                     this.addLeave.TotalSL = 1;
                   }
                   else if(lev.TotalSL > 0.5) {
                    this.addLeave.TotalSL = lev.TotalSL / 0.5 ;
                   }
                   else
                   {
                    this.addLeave.TotalSL = lev.TotalSL;
                   }

                   this.addLeave.TotalSick = lev.TotalSick;
                   this.addLeave.TotalCasual = lev.TotalCasual;
                   this.addLeave.TotalLev = lev.TotalLev;
                   this.addLeave.Bal = lev.Bal;
 

        }
      }
          );
  }

  /* public profileSearch(){
   
   
            this._attandanceService.getTotalLeaves(this.addLeave.Cnic).subscribe(
              lev => {   
                if (lev) 
                {
                  this.addLeave = lev as AddLeave;
                  this.totalLeaves.ProfileID = this.addLeave.ProfileID;
                 
                  
                  this.totalLeaves.EmployeeName = this.addLeave.EmployeeName;
                  this.totalLeaves.Cnic = this.addLeave.Cnic;
                  this.totalLeaves.Designation_Name = this.addLeave.Designation_Name;
                  this.totalLeaves.HealthFacility = this.addLeave.HealthFacility;   

                  if(this.addLeave.TotalSL == 0) {
                    this.totalLeaves.TotalSL = this.addLeave.TotalSL;
                  }
                  else
                  {
                    this.addLeave.TotalSL = this.addLeave.TotalSL / 0.5 ;
                    this.totalLeaves.TotalSL = this.addLeave.TotalSL;
                  }

                //  this.totalLeaves.TotalSL = this.addLeave.TotalSL;
                  this.totalLeaves.TotalSick = this.addLeave.TotalSick;
                  this.totalLeaves.TotalCasual = this.addLeave.TotalCasual;
                  this.totalLeaves.TotalLev = this.addLeave.TotalLev;   
                  this.totalLeaves.Bal = this.addLeave.Bal;  
                 
                }
                else
                {
                  this.totalLeaves = {
                    ProfileID:0,
                    EmployeeName:'',
                    Designation_Name:'',
                    Cnic:'',
                    HealthFacility:'',
                    TotalSL: 0,
                    TotalSick: 0,
                    TotalCasual: 0,
                    TotalLev: 0,
                    Bal:0,
                
                  }
                }
              },
              err => {
                this.handleError(err);
              }
            ); 

          
          }  */

        
  public leaveInputsChanged(type: number) {
    debugger;
    if (type == 1) {
      if (this.addLeave.LeaveFrom && this.addLeave.LeaveTo) {
        this._rootService.calcDate(this.addLeave.LeaveFrom.toDateString(), this.addLeave.LeaveTo.toDateString(), 0).subscribe((x: number) => {
          this.addLeave.TotalDays = x;
        });
      }
    } 
    else if (type == 2) {
      this._rootService.calcDate(this.addLeave.LeaveFrom.toDateString(), 'noDate', this.addLeave.TotalDays).subscribe((x: any) => {
        this.addLeave.LeaveTo = new Date(x);
      });
    } 
    else if (type == 3) {
    
      if(this.addLeave.LeaveTypeID == 3){
      this.addLeave.TotalDays = 0.5;
      this.addLeave.LeaveTo = this.addLeave.LeaveFrom;
      }
     /*  else{
        this.addLeave.LeaveFrom = "";
      } */
    }
    else if (type == 4) {
      
     

      this.addLeave.LeaveFrom = "";
      this.addLeave.LeaveTo = "";
      this.addLeave.TotalDays = 0;
    }
  }

/*   private getLeaveTypes = () => {
    this._rootService.getLeaveTypes().subscribe((res: any) => {
      this.dropDowns.leaveTypes = res;
      this.dropDowns.leaveTypesData = this.dropDowns.leaveTypes.slice();
    },
      err => { this.handleError(err); }
    );
  } */

}
