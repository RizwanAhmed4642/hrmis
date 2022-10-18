import { Component, OnInit, Input } from '@angular/core';
import { HealthFacilityService } from '../../../health-facility.service';
import { AuthenticationService } from '../../../../../_services/authentication.service';
import { NotificationService } from '../../../../../_services/notification.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-basic-info',
  templateUrl: './basic-info.component.html'
})
export class BasicInfoComponent implements OnInit {
  public hfmisCode: string = '';
  @Input() public healthFacility: any;
  @Input() public dashboardView: boolean;
  @Input() public hfPhoto: any;
  @Input() public hfUCInfo: any;
  public heads: any[] = [];
  @Input() public hfPhotoLoaded: boolean = false;
  public currentUser: any;
  constructor(private router: Router, private _hfService: HealthFacilityService, private _authService: AuthenticationService, public _notificationService: NotificationService) { }

  ngOnInit() {
    this.currentUser = this._authService.getUser();
    this.getHFHOD();
  }
  getHFHOD() {
    this._hfService.getHFHOD(this.healthFacility.HFMISCode).subscribe((res: any) => {
      if (res) {
        this.heads = res.Heads;
      }
    }, err => {
      console.log(err);
    });
  }
  rmvHealthFacility() {
    this._hfService.rmvHealthFacility(this.healthFacility.Id, this._authService.getUserHfmisCode()).subscribe((res: boolean) => {
      if (res) {
        this._notificationService.notify('success', 'Health Facility Removed!');
        this.router.navigate(['/health-facility/list']);
      }
    }, err => {
      console.log(err);
    });
  }
}
