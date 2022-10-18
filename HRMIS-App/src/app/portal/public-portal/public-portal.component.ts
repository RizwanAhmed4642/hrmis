import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-public-portal',
  templateUrl: './public-portal.component.html',
  styleUrls: ['./public-portal.component.scss']
})
export class PublicPortalComponent implements OnInit {
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
