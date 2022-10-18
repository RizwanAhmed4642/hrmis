import { Component, OnInit, OnDestroy } from '@angular/core';
import { RootService } from '../../../_services/root.service';
import { OrderService } from '../order.service';
import { Subject } from 'rxjs/Subject';
import { Subscription } from 'rxjs/Subscription';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-advance-search',
  templateUrl: './advance-search.component.html',
  styles: []
})
export class AdvanceSearchComponent implements OnInit, OnDestroy {
  public cnicMask: string = "00000-0000000-0";
  public obj: any = {};
  public selectedOrder: any;
  public orders: any[] = [];
  public serviceOrders: any[] = [];
  public leaveOrders: any[] = [];
  public serviceOrdersData: any[] = [];
  public leaveOrdersData: any[] = [];
  public results: any[] = [];
  public searching: boolean = false;
  public viewOrderWindow: boolean = false;
  public type: number = 1;
  public serviceOrderQuery: string = '';
  public leaveOrderQuery: string = '';
  constructor(private sanitized: DomSanitizer, private _rootService: RootService, private _orderService: OrderService) { }

  ngOnInit() {
  }
  public searchServiceOrder() {
    this.serviceOrdersData = [];
    if (!this.serviceOrderQuery) {
      this.serviceOrdersData = this.serviceOrders;
    }
    this.serviceOrdersData = this.serviceOrders.filter(x => x.OrderHTML.toLowerCase().includes(this.serviceOrderQuery.toLowerCase()));
  }
  public searchLeaveOrder() {
    this.leaveOrdersData = [];
    if (!this.leaveOrderQuery) {
      this.leaveOrdersData = this.leaveOrders;
    }
    this.leaveOrdersData = this.leaveOrders.filter(x => x.OrderHTML.toLowerCase().includes(this.leaveOrderQuery.toLowerCase()));
  }
  public onSearch() {
    if (this.searching) return;
    this.orders = [];
    this.serviceOrders = [];
    this.leaveOrders = [];
    this.type = 1;
    this.searching = true;
    let Id = 0;
    if (this.obj.CNIC && this.obj.CNIC.length > 3) {
      this._orderService.getProfileDetail(this.obj.CNIC, 3).subscribe((data: any) => {
        if (data) {
          this.serviceOrders = data;
          this.serviceOrdersData = this.serviceOrders;
        }
        this.searching = false;
      },
        err => {
          console.log(err);
        });
      this._orderService.getProfileDetail(this.obj.CNIC, 4).subscribe((data: any) => {
        if (data) {
          this.leaveOrders = data;
          this.leaveOrdersData = this.leaveOrders;
        }
        this.searching = false;
      },
        err => {
          console.log(err);
        });
    }
    if (this.obj.ESR || this.obj.ELR) {
      if (this.obj.ESR) {
        this.type = 1;
        let ESR = (this.obj.ESR as string).toLocaleLowerCase();
        ESR = ESR.replace('esr', '');
        ESR = ESR.replace('-', '');
        let ESRId: number = +ESR;
        if (ESRId && ESRId > 100) {
          Id = ESRId;
          this.searching = true;
          this._orderService.getOrder(Id, this.type).subscribe((res: any) => {
            if (res) {
              res.tt = 1;
              this.orders.push(res);
            }
            this.searching = false;
          }, err => {
            console.log(err);
          });
        }
      }
      if (this.obj.ELR) {
        this.type = 2;
        let ELR = (this.obj.ELR as string).toLocaleLowerCase();
        ELR = ELR.replace('elr', '');
        ELR = ELR.replace('-', '');
        let ELRId: number = +ELR;
        if (ELRId && ELRId > 100) {
          Id = ELRId;
          this.searching = true;
          this._orderService.getOrder(Id, this.type).subscribe((res: any) => {
            if (res) {
              res.tt = 2;
              this.orders.push(res);
            }
            this.searching = false;
          }, err => {
            console.log(err);
          });
        }
      }
    }
  }
  public openInNewTab(link) {
    window.open(link, '_blank');
  }
  public viewOrder(order) {
    this.selectedOrder = order;
    if (this.selectedOrder) {
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
    if (cnic && cnic.length == 13) {
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
    return '';
  }
  ngOnDestroy() {
  }
}
