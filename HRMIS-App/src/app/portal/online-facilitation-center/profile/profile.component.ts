import { Component, OnInit } from '@angular/core';
import { CookieService } from '../../../_services/cookie.service';
import { OnlineFacilitationCenterService } from '../online-facilitation-center.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
})
export class ProfileComponent implements OnInit {

  public cnic: string = '';
  public profile: any = {};

  public fileMaster: any = [];

  constructor(
    private _onlineFacilitationService: OnlineFacilitationCenterService,
    private _cookieService: CookieService) { }

  ngOnInit() {
    this.fetchParams();
  }

  private fetchParams() {
    this.cnic = this._cookieService.getCookie('cnicussrpublic');
    if (this.cnic) {
      this.getProfile();
    }
  }

  public getProfile() {
    this._onlineFacilitationService.getProfileByCNIC(this.cnic).subscribe((res: any) => {
      if (res) 
      {
        this.profile = res;
        console.log('profile: ', this.profile);
      }
    }, err => {
      console.log(err);
    });
  }

  public dashifyCNIC(cnic: string) {
    if (!cnic) 
    {
      return;
    }

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
