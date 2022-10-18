import { Component, OnInit, Input } from '@angular/core';
import { RootService } from '../../../../../_services/root.service';

import { ProfileFields } from '../../../profile-fields.class'

@Component({
  selector: 'app-profile-profile-information',
  templateUrl: './profile-information.component.html',
  styles: [`
  .zoom {
    transition: transform .2s;
    border: 1px solid black;
    /* Animation */
  }
  
  .zoom:hover {
    transform: scale(1.08);
    box-shadow: 0px 0px 5px black !important;
    /* (150% zoom)*/
  }`]
})
export class ProfileInformationComponent implements OnInit {
  @Input() public profile: any;
  public profileFields: ProfileFields = new ProfileFields();
  public radnom: number = Math.random();
  public newImg = new Image;
  constructor() { }

  ngOnInit() {
    console.log(this.profile.ContractStartDate);
    console.log(this.profile.ContractStartDate ? new Date(this.profile.ContractStartDate).toDateString() : '');

    let newImg = new Image;
    newImg.onload = (() => {
      this.profile.psrc = newImg.src;
    }).bind(this)
    newImg.src = 'https://hrmis.pshealthpunjab.gov.pk/Uploads/ProfilePhotos/' + this.profile.CNIC + '_23.jpg';
  }
  public onMouse(direction: number, prop: any) {
    /*  console.log(prop);
     this.profileFields[prop] = direction == 1 ? true : false;
     console.log(this.profileFields[prop]); */
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
