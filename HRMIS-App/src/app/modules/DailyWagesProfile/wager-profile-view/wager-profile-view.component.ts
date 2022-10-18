import { Component, OnInit, Input } from '@angular/core';
import { RootService } from '../../../_services/root.service';

import { Subscription } from 'rxjs/Subscription';
import { DailyWagerService } from '../../../_services/daily-wager.service';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-wager-profile-view',
  templateUrl: './wager-profile-view.component.html',
  styleUrls: ['./wager-profile-view.component.scss']
})
export class WagerProfileViewComponent implements OnInit {
  //@Input() public profile: any;
  public Id : number
  //public profileFields: ProfileFields = new ProfileFields();
  public profileData: any
  private subscription: Subscription;
  constructor(public _DailyWagerService : DailyWagerService,private route: ActivatedRoute,) { }

  ngOnInit() {
    debugger
    this.fetchParams();
    this.profile();
  }

  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('Id')) {
          let param = params['Id'];
          if (+param) {
            this.Id = +params['Id'] as number;
          }
        }
      }
    );
  }

  public profile(){
    this._DailyWagerService.GetProfileById(this.Id).subscribe((res: any) => {
     console.log(res);
       this.profileData = res.Profile
      },
        err => {  }
      );
    
  }

  
  
}
