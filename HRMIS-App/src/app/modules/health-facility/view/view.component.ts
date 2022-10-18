import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthenticationService } from '../../../_services/authentication.service';
import { HealthFacilityService } from '../health-facility.service';
import { RootService } from '../../../_services/root.service';
import { Subscription } from 'rxjs/Subscription';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-view',
  templateUrl: './view.component.html'
})
export class ViewComponent implements OnInit, OnDestroy {

  public customTempFlags: boolean[] = [true, true, true, true, true, true, true, true, true];
  public showMoreInfoFlag: boolean = false;
  public showMoreProfilesFlag: boolean = false;
  public dashboardView: boolean = false;
  public showMoreServicesFlag: boolean = false;
  public hfPhotoLoaded: boolean = false;
  public currentUser: any;
  public hfPhoto = new Image();
  private subscription: Subscription;
  public hfmisCode: string = '0';
  public selectedTab: string = '';
  public healthFacility: any;
  public userRight: any;
  public hfUCInfo: any = {};
  public employementModes: any[] = [];
  public wardsBedsInfo: boolean = false;
  constructor(private _rootService: RootService, private _hfService: HealthFacilityService,
    private _authenticationService: AuthenticationService,
    private router: Router, private route: ActivatedRoute) { }

  ngOnInit() {
    window.scroll(0, 0);
    this.currentUser = this._authenticationService.getUser();
    this.getUserRightById();
    this.fetchParams();
  }
  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('hfcode')) {
          this.hfmisCode = params['hfcode'];
          this.fetchData(this.hfmisCode);
         // this.getHFUCInfo();
          this.getEmploymentModes();
        }
      }
    );

  }
  private fetchData(hfmisCode) {
    this.healthFacility = null;
    this._hfService.getHealthFacilitiyDashboard(hfmisCode).subscribe(
      res => {
        if (res) {
          this.healthFacility = res;
          this.showWards();
          this.hfPhoto.src = 'https://hrmis.pshealthpunjab.gov.pk/Uploads/Files/HFPics/' + this.healthFacility.HFPhoto.ImagePath;
          this.hfPhoto.onload = () => {
            this.hfPhotoLoaded = true;
          }
          this.onTabSelect({ heading: 'General Information' });
          this.route.queryParams.subscribe(
            (params: any) => {
              if (params.hasOwnProperty('t')) {
                this.onTabSelect({ heading: params['t'] });
              }
            }
          );
        }
      },
      err => {
        this.handleError(err);
      }
    );
  }
  private getUserRightById() {
    this._rootService.getUserRightById({ User_Id: this.currentUser.Id }).subscribe(
      res => {
        if (res) {
          this.userRight = res;
        } else {
          this.userRight = null;
        }
      },
      err => {
        this.handleError(err);
      }
    );
  }
  private getHFUCInfo() {
    this._hfService.getHFUCInfo(this.hfmisCode).subscribe(
      res => {
        if (res) {
          this.hfUCInfo = res;
        }
      },
      err => {
        this.handleError(err);
      }
    );
  }
  private getEmploymentModes = () => {
    this._rootService.getEmploymentModes().subscribe((res: any) => {
      this.employementModes = res;
    },
      err => { this.handleError(err); }
    );
  }
  public onTabSelect(e) {
    if (!e.heading) {
      return;
    }
    console.log(e);

    this.selectedTab = e.heading;
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
  private handleError(err: any) {
    this.router.navigate(['/health-facility/list']);
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }
  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

}
