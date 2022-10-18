import { Component, OnInit } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { RootService } from '../../../_services/root.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { EmployeeService } from '../employee.service';

@Component({
  selector: 'app-applications',
  templateUrl: './applications.component.html',
  styles: []
})
export class ApplicationsComponent implements OnInit {
  public recentApplications: any[] = [];
  public leaveOrders: any[] = [];
  public selectedApplication: any;
  public viewOrderWindow: boolean = false;
  public currentUser: any;
  public profile: any;

  constructor(private sanitized: DomSanitizer, private _rootService: RootService,
    private _authenticationService: AuthenticationService,
    private _employeeService: EmployeeService) { }

  ngOnInit() {
    this.currentUser = this._authenticationService.getUser();
    if (!this.currentUser) {
      this._authenticationService.logout();
    }
    this.fetchData(this.currentUser.cnic);
    this._employeeService.getProfileDetail(this.currentUser.cnic, 2).subscribe((data: any) => {
      this.recentApplications = data;
    },
      err => {
        console.log(err);
      });
  }
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
  public viewOrder(order) {
    this.selectedApplication = order;
    if (this.selectedApplication) {
      console.log(this.selectedApplication);

      this.openViewOrderWindow();
    }
  }
  public openViewOrderWindow() {
    this.viewOrderWindow = true;
  }
  public closeViewOrderWindow() {
    this.viewOrderWindow = false;
    this.selectedApplication = null;
  }
  transform(value) {
    return this.sanitized.bypassSecurityTrustHtml(value);
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
