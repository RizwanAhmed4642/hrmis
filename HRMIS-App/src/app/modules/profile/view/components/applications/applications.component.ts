import { Component, OnInit, Input } from '@angular/core';
import { ProfileService } from '../../../profile.service';
import { ApplicationMaster } from '../../../../application-fts/application-fts';

@Component({
  selector: 'app-profile-applications',
  templateUrl: './applications.component.html',
  styles: []
})
export class ApplicationsComponent implements OnInit {
  @Input() public profile: any;
  public applications: any[] = [];

  constructor(private _profileService: ProfileService) { }

  ngOnInit() { 
    this._profileService.getProfileDetail(this.profile.CNIC, 2).subscribe((data: any) => {
    this.applications = data;
  },
    err => {
      console.log(err);
    });

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
