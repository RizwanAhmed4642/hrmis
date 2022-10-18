import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-public-portal',
  templateUrl: './public-portal-mo.component.html'
})
export class PublicPortalMOComponent implements OnInit {
  public user: any = {};
  public profile: any;
  public sigingin: boolean = false;
  public cnicMask: string = "00000-0000000-0";
  public generatingPassword: boolean = false;
  public passwordGenerated: boolean = false;
  constructor() { }

  ngOnInit() {
  }
  public login() {

  }
}
