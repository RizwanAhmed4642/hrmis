import { Component, OnInit } from '@angular/core';
import { RootService } from '../../_services/root.service';

@Component({
  selector: 'app-application-fts',
  templateUrl: './application-fts.component.html',
  styles: []
})
export class ApplicationFtsComponent implements OnInit {
  public applicationTypeWindow: any = {
    dialogOpened: false,
    data: null,
    applicationTypes: []
  }
  
  constructor(private _rootService: RootService) { }

  ngOnInit() {
  }
  public closeWindow() {
    this.applicationTypeWindow.dialogOpened = false;
  }

  public openWindow(dataItem) {
    this.applicationTypeWindow.data = dataItem;
    this.applicationTypeWindow.dialogOpened = true;
     this._rootService.getApplicationTypes().subscribe(data => {
      this.applicationTypeWindow.applicationTypes = data;
      this.applicationTypeWindow.dialogOpened = true;
    });

  }
}
