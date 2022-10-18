import { Component, OnInit } from '@angular/core';
import { RootService } from '../../_services/root.service';

@Component({
  selector: 'app-attandance',
  templateUrl: './attandance.component.html',
  styles: []
})
export class AttandanceComponent implements OnInit {

  public attandanceWindow: any = {
    dialogOpened: false,
    data: null,
    //applicationTypes: [],
  }

  constructor() { }

  ngOnInit() {
  }
  public closeWindow() {
    this.attandanceWindow.dialogOpened = false;
  }
  public openWindow(dataItem) {
    this.attandanceWindow.data = dataItem;
    this.attandanceWindow.dialogOpened = true;
   /*   this._rootService.getApplicationTypes().subscribe(data => {
      this.applicationTypeWindow.applicationTypes = data;
      this.applicationTypeWindow.dialogOpened = true;
    }); */

  }

}
