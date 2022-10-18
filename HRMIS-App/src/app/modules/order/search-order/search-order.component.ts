import { Component, OnInit } from '@angular/core';
import { ESR } from '../TransferAndPosting/ESR.class';
import { OrderService } from '../order.service';
import { DialogService } from '../../../_services/dialog.service';
import { DomSanitizer } from '@angular/platform-browser';
import { NotificationService } from '../../../_services/notification.service';

@Component({
  selector: 'app-search-order',
  templateUrl: './search-order.component.html',
  styles: []
})
export class SearchOrderComponent implements OnInit {
  public esrSinged: any = {};
  public showTrackingDetail: boolean = false;
  public searchingTrack: boolean = false;
  public searchNumber: number = 0;
  public esrNo = '';
  public elrNo = '';
  public currentUser: any;
  public loading: boolean = false;
  public saving: boolean = false;
  public saved: boolean = false;
  public esr: any;
  public leaveOrder: any;
  public orderId: number = 0;
  public orderType: number = 1;
  public selectedOrder: any;
  public viewOrderWindow: boolean = false;
  constructor(private sanitized: DomSanitizer,
    private _orderService: OrderService,
    private _notificationService: NotificationService,
    private _dialogSerivce: DialogService) { }

  ngOnInit() {
  }
  public searchApplication(number: string, type: number) {
    if (number.length < 4) return;
    this.searchingTrack = true;
    this.showTrackingDetail = false;
    this.searchNumber = +number;
    this.orderType = type;
    if (this.orderType == 2) {
      this.searchNumber -= 1003;
    }
    this.loadOrder();
    this.showTrackingDetail = true;
  }
  public loadOrder = () => {
    this.esr = null;
    this.leaveOrder = null;
    this.loading = true;

    this._orderService.getOrder(this.searchNumber, this.orderType).subscribe((data: any) => {

      if (data && data.Profile_Id) {
        if (this.orderType == 2) {
          this.esr = new ESR();
          this.esr.TransferTypeID = 5;
          this.leaveOrder = data;
          this.esrSinged.ELR_Id = this.leaveOrder.Id;
          console.log(data);
        } else {
          this.esr = data;
          this.esrSinged.ESR_Id = this.esr.Id;
          console.log(data);
        }
        this.loading = false;
      }
    },
      err => {
        console.log(err);
      });
  }

  public uploadOrder() {
    this.saving = true;
    this._orderService.uploadOrder(this.esrSinged).subscribe((res: any) => {
      if (res) {
        this._notificationService.notify('success', 'Upload Succesfull');
        this.esrSinged = {};
        this.esr = null;
        this.leaveOrder = null;
        this.esrNo = '';
        this.elrNo = '';
      } else {
        this._notificationService.notify('danger', 'Error Occured');
      }
      this.saving = false;
    }, err => {
      console.log(err);
      this.saving = false;
    });
  }

  public viewOrder(order) {
    console.log(order);
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
  transform(value) {
    return this.sanitized.bypassSecurityTrustHtml(value);
  }
}
