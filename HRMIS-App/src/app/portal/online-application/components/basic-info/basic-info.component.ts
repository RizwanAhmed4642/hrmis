import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-profile-basic-info',
  templateUrl: './basic-info.component.html'
})
export class BasicInfoComponent implements OnInit {
  @Input() public profile: any;
  public radnom: number = Math.random();

  constructor() { }

  ngOnInit() {
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
