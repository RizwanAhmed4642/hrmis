import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-search-tracking',
  templateUrl: './search-tracking.component.html',
  styles: []
})
export class SearchTrackingComponent implements OnInit {
  public showTrackingDetail: boolean = false;
  public searchingTrack: boolean = false;
  public trackingNumber: number = 0;

  constructor() { }

  ngOnInit() {
  }
  public searchApplication(trackingNumber: any) {
    this.searchingTrack = true;
    this.showTrackingDetail = false;
    if (+trackingNumber) {
      this.trackingNumber = trackingNumber;
      this.showTrackingDetail = true;
    } else {
      this.showTrackingDetail = false;
    }
    this.searchingTrack = false;
  }
}
