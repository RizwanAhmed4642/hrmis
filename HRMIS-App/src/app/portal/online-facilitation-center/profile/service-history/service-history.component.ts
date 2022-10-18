import { Component, OnInit } from '@angular/core';
import { CookieService } from '../../../../_services/cookie.service';
import { OnlineFacilitationCenterService } from '../../online-facilitation-center.service';

@Component({
  selector: 'app-service-history',
  templateUrl: './service-history.component.html',
})
export class ServiceHistoryComponent implements OnInit {

  public serviceHistories: any [] = [];
  public orders: any [] = [];
  public serviceHistory: any = {};
  public profile: any = {};
  public cnic: string = '';

  constructor(
    private _onlineFacilitationService: OnlineFacilitationCenterService,
    private _cookieService: CookieService
  ) { }

  ngOnInit() {
    this.fetchParams();
  }

  private fetchParams() {
    this.cnic = this._cookieService.getCookie('cnicussrpublic');
    if (this.cnic) {
      this.getProfile();
    }
  }

  public getServiceHistory() {
    this._onlineFacilitationService.getServiceHistory(this.profile.Id).subscribe((data: any) => {
      if (data) {
        this.serviceHistories = data;
        console.log('serviceHistories: ', this.serviceHistories);
      }
    },
      err => {
        console.log(err);
      });
  }

  public getProfile() {
    this._onlineFacilitationService.getProfileByCNIC(this.cnic).subscribe((res: any) => {
      if (res) 
      {
        this.profile = res;
        console.log('profile: ', this.profile);
        this.getServiceHistory();
      }
    }, err => {
      console.log(err);
    });
  }

}
