import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import { RootService } from '../../../_services/root.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { ActivatedRoute } from '@angular/router';
import { EmployeeService } from '../employee.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styles: []
})
export class ProfileComponent implements OnInit {
  private subscription: Subscription;
  public cnic: string = '0';
  public selectedField: any = {};
  public profile: any;
  public currentUser: any;
  public editDialogOpened: boolean = false;
  constructor(private _rootService: RootService,
    private _authenticationService: AuthenticationService,
    private _employeeService: EmployeeService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.currentUser = this._authenticationService.getUser();
    if (!this.currentUser) {
      this._authenticationService.logout();
    }
    this.fetchData(this.currentUser.cnic);
  }
  /* private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('cnic')) {
          this.cnic = params['cnic'];
        }
      }
    );
  } */
  private fetchData(cnic) {
    this.profile = null;
    this._employeeService.getProfile(cnic).subscribe(
      res => {
        this.profile = res as any;
      }, err => {
        console.log(err);
      }
    );
  }
  public openEditDialog(fieldName, fieldLabel) {
   /*  this.selectedField.FieldName = fieldName;
    this.selectedField.FieldLabel = fieldLabel;
    
    this.editDialogOpened = true; */
  }
  public closeEditDialog() {
    this.editDialogOpened = false;
    this.selectedField = {};
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
}
