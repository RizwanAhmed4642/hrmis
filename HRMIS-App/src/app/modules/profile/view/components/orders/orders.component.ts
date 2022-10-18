import { Component, OnInit, Input } from '@angular/core';
import { ProfileService } from '../../../profile.service';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-profile-orders',
  templateUrl: './orders.component.html',
  styles: []
})
export class OrdersComponent implements OnInit {
  @Input() public profile: any;
  public orders: any[] = [];
  public leaveOrders: any[] = [];
  public selectedOrder: any;
  public viewOrderWindow: boolean = false;

  constructor(private sanitized: DomSanitizer, private _profileService: ProfileService) { }

  ngOnInit() {
    this._profileService.getProfileDetail(this.profile.CNIC, 3).subscribe((data: any) => {
      this.orders = data;
      console.log(this.orders);
    },
      err => {
        console.log(err);
      });
    this._profileService.getProfileDetail(this.profile.CNIC, 4).subscribe((data: any) => {
      this.leaveOrders = data;
    },
      err => {
        console.log(err);
      });
  }
  public viewOrder(order) {
    this.selectedOrder = order;
    if (this.selectedOrder) {
      console.log(this.selectedOrder);

      this.openViewOrderWindow();
    }
  }
  public openViewOrderWindow() {
    this.viewOrderWindow = true;
  }
  public closeViewOrderWindow() {
    this.viewOrderWindow = false;
    this.selectedOrder = null;
  }
  transform(value) {
    return this.sanitized.bypassSecurityTrustHtml(value);
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
