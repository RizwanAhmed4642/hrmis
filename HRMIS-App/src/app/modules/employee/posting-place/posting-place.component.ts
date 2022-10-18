import { Component, OnInit } from '@angular/core';
import { RootService } from '../../../_services/root.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { EmployeeService } from '../employee.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-posting-place',
  templateUrl: './posting-place.component.html',
  styles: []
})
export class PostingPlaceComponent implements OnInit {
  public currentUser: any;
  public profile: any;
  public hfPhoto = new Image();
  public hfmisCode: string = '0';
  public selectedTab: string = 'General Information';
  public healthFacility: any;
  public hfPhotoLoaded: boolean = false;
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
  private fetchData(cnic) {
    this.profile = null;
    this._employeeService.getProfile(cnic).subscribe(
      res => {
        this.profile = res as any;
        this.healthFacility = null;
        this._employeeService.getHealthFacilitiyDashboard(this.profile.HfmisCode).subscribe(
          res => {
            if (res) {
              this.healthFacility = res;
              this.hfPhoto.src = 'https://hrmis.pshealthpunjab.gov.pk/Uploads/Files/HFPics/' + this.healthFacility.HFPhoto.ImagePath;
              this.hfPhoto.onload = () => {
                this.hfPhotoLoaded = true;
              }
            }
          },
          err => {
            console.log(err);
          }
        );
      }, err => {
        console.log(err);
      }
    );
  }
  public onTabSelect(e) {
    if (!e.heading) {
      return;
    }
    this.selectedTab = e.heading;
  }

}
