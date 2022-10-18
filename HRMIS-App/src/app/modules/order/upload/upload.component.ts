import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-order-upload',
  templateUrl: './upload.component.html',
  styles: []
})
export class UploadComponent implements OnInit {
  public loaded: boolean = false;
  public uploading: boolean = false;
  public order: any = {};
  public cnicMask: string = "00000-0000000-0";
  public mobileMask: string = "0000-0000000";
  constructor() { }

  ngOnInit() {
  }
  onSubmit() {

  }
}
