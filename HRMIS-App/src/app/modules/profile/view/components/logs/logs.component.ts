import { Component, OnInit, Input } from '@angular/core';
import { ProfileService } from '../../../profile.service';

@Component({
  selector: 'app-profile-logs',
  templateUrl: './logs.component.html',
  styles: []
})
export class LogsComponent implements OnInit {
  @Input() public profile: any;
  public loading: boolean = false;
  public logs: any[] = [];
  constructor(private _profileService: ProfileService) { }

  ngOnInit() {
    this.getProfileLogs();
  }
  public getProfileLogs() {
    this.loading = true;
    this._profileService.getProfileLogs(this.profile.Id).subscribe((data: any) => {
      if (data) {
        this.logs = data;
        this.loading = false;
      }
    },
      err => {
        console.log(err);
      });
  }
}
