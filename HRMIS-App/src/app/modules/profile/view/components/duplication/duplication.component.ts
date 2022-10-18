import { Component, OnInit, Input } from '@angular/core';
import { ProfileFields } from '../../../profile-fields.class';
import { Profile } from '../../../profile.class';
import { ProfileService } from '../../../profile.service';

@Component({
  selector: 'app-profile-duplication',
  templateUrl: './duplication.component.html',
  styleUrls: ['./duplication.component.scss']
})
export class DuplicationComponent implements OnInit {

  @Input() public duplicateprofile: any[] = [];
  public profileFields: ProfileFields = new ProfileFields();
  public radnom: number = Math.random();

  constructor(private _service: ProfileService) { }

  ngOnInit() {
  }
  onMouse(a, b) {

  }
  deleteProfile(profile, index: number): void {
    profile.loading = true;
    this._service.removeDuplication(profile.Id, profile.CNIC).subscribe((res: any) => {
      profile.loading = false;
      window.location.reload();
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
