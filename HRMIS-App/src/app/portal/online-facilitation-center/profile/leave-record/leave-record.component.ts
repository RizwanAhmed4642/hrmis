import { Component, OnInit } from '@angular/core';
import { CookieService } from '../../../../_services/cookie.service';
import { OnlineFacilitationCenterService } from '../../online-facilitation-center.service';

@Component({
  selector: 'app-leave-record',
  templateUrl: './leave-record.component.html',
})
export class LeaveRecordComponent implements OnInit {

  public resFound: boolean = false;
  public leaveRecord: any [] = [];
  public cnic: string = '';

  constructor(
    private _onlineFacilitationService: OnlineFacilitationCenterService,
    private _cookieService: CookieService) { }

  ngOnInit() {
    this.fetchParams();
  }

  private fetchParams() {
    this.cnic = this._cookieService.getCookie('cnicussrpublic');
    if (this.cnic) {
      this.getLeaveRecord();
    }
  }

  public getLeaveRecord()
  {
    this._onlineFacilitationService.getProfileDetail(this.cnic, 5).subscribe((data: any) => {
      if(data)
      {
        this.resFound = true;
        this.leaveRecord = data;
        console.log('Leaves: ', this.leaveRecord);
      }
      else{
        this.leaveRecord = [];
        console.log('res not found..');
      }
      
    }, err => {
        this.leaveRecord = [];
        this.resFound = true;
      console.log(err);
    });
  }

}
