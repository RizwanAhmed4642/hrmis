import { Component, OnInit, OnDestroy, ViewEncapsulation, ViewChild } from '@angular/core';
import { Subject } from 'rxjs/Subject';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { RootService } from '../../_services/root.service';
import { DropDownsHR } from '../../_helpers/dropdowns.class';
import { OrderService } from './order.service';
import { AuthenticationService } from '../../_services/authentication.service';
import { Subscription } from 'rxjs/Subscription';
import { ActivatedRoute } from '@angular/router';
import { DomSanitizer } from '@angular/platform-browser';
import { KGridHelper } from '../../_helpers/k-grid.class';
import { PageChangeEvent } from '@progress/kendo-angular-grid';
@Component({
  selector: 'app-order',
  templateUrl: './order.component.html',
  styles: [],
  encapsulation: ViewEncapsulation.None,
})
export class OrderComponent implements OnInit, OnDestroy {
  public value: any;
  public loaded: boolean = false;
  public loadedContent: boolean = true;
  public outRangeProfile: boolean = false;
  public removingOrder: boolean = false;
  public controlPlaceHolders: boolean[] = [false];
  public thumbPlaceHolders: boolean[] = [false, false];
  public inputChange: Subject<any>;
  public dropDowns: DropDownsHR = new DropDownsHR();
  public orders: any[] = [];
  public orderTypes: any[] = [];
  public totalOrders: number = 0;
  public orderType_Id: number = 0;
  public currentPage: number = 1;
  public pageSize: number = 20;
  public selectedTypeId: number = 0;
  public searchQuery: string = '';
  public selectedOrder: any;
  private subscription: Subscription;
  private searchSubscription: Subscription;
  public cnicMask: string = "00000-0000000-0";
  public applicationTypeWindow: any = {
    dialogOpened: false,
    data: null,
    orderTypes: [],
  }
  public loadingMoreOrders: boolean = false;
  public showLoadingMoreOrdersButton: boolean = true;
  public viewOrderWindow: boolean = false;
  public searchingProfile: boolean = false;
  public searchStarted: boolean = false;
  public profileExist: boolean = false;
  public profileNotExist: boolean = false;
  public cnic: string = '';
  public profile: any;
  public currentUser: any;

  public kGrid: KGridHelper = new KGridHelper();


  constructor(private sanitized: DomSanitizer, private _rootService: RootService,
    private route: ActivatedRoute, private _orderService: OrderService, private _authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.currentUser = this._authenticationService.getUser();
    this.insertEditorScript();
    this.fetchParams();
    this.loadOrderTypes();
    this.loadOrders();
    this.subsribeSearch();
    this.kGrid.loading = false;
    this.kGrid.pageSize = 50;
    this.kGrid.pageSizes = [50, 100];
  }
  private subsribeSearch(){
    this.inputChange = new Subject();
    this.searchSubscription = this.inputChange.pipe(debounceTime(800)).subscribe((x) => {
      this.searchQuery = x.query;
      if (!x.query) {
        this.loadOrders();
        return;
      }
    });
  }
  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('otid')) {
          this.orderType_Id = +params['otid'];
        }
        if (params.hasOwnProperty('currentPage') && +params['currentPage']) {
          this.currentPage = +params['currentPage'];
        }
      }
    );
  }
  public loadOrders = () => {
    this.loadedContent = false;
    let user = this._authenticationService.getUser();
    let districtCode = user.HfmisCode.length < 6 ? user.HfmisCode : user.UserName;
    this._orderService.getOrders({ Query: this.searchQuery }, districtCode, this.dropDowns.selectedFiltersModel.orderType.Id, this.currentPage, this.pageSize)
      .subscribe((data: any) => {
        if (data) {
          this.orders = data.esrlist;
          this.totalOrders = data.totalRecords;
          this.kGrid.data = data.esrlist;
          this.kGrid.totalRecords = data.totalRecords;
          this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords };
        }
        this.loaded = true;
        this.loadedContent = true;
      },
        err => {
          this.loadedContent = true;
          this.handleError(err);
        });
  }
  public loadMoreOrders() {
    this.loadingMoreOrders = true;
    if ((this.currentPage * this.pageSize) < this.kGrid.totalRecords) {
      this.currentPage++;
    } else {
      this.showLoadingMoreOrdersButton = false;
    }
    let user = this._authenticationService.getUser();
    let districtCode = user.HfmisCode.length == 1 ? '0' : user.UserName;
    this._orderService.getOrders({ Query: this.searchQuery }, districtCode, this.dropDowns.selectedFiltersModel.orderType.Id, this.currentPage, this.pageSize)
      .subscribe((data: any) => {
        if (data) {
          console.log(this.orders);
          this.orders.push.apply(this.orders, data.esrlist as any[]);
          console.log(this.orders);
          this.totalOrders = data.totalRecords;
          this.kGrid.data.concat(data.esrlist);
          this.kGrid.totalRecords = data.totalRecords;
          this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords };
        }
        this.loadingMoreOrders = false;
      },
        err => {
          this.loadingMoreOrders = false;
          this.handleError(err);
        });
  }
  public loadOrderTypes = () => {
    this._rootService.getOrderTypes().subscribe((data: any) => {
      this.orderTypes = data;
      this.dropDowns.orderTypes = data;
      this.dropDowns.orderTypesData = this.dropDowns.orderTypes;
      if (this.orderType_Id && this.orderType_Id != 0) {
        let selectOrderType = data.find(x => x.Id == this.orderType_Id);
        this.dropDowns.selectedFiltersModel.orderType = { Name: selectOrderType.Name, Id: selectOrderType.Id };
      }
    });
  }
  public onSearch = () => {
    this.loadOrders();

  }
  public removeOrder(id: number, orderType: number) {
    if (confirm('Are you sure?')) {
      this.removingOrder = true;
      this._orderService.removeOrder(id, orderType).subscribe((x) => {
        this.removingOrder = false;
        if (x) {
          this.loadOrders();
          this.closeViewOrderWindow();
        }
      }, err => {
        this.handleError(err);
      });
    }
  }
  private handleError(err: any) {
    this.loadedContent = true;
    this.searchingProfile = false;
    this.removingOrder = true;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }
  public closeWindow() {
    this.applicationTypeWindow.dialogOpened = false;
  }
  public viewOrder(order) {
    this.selectedOrder = order;
    if (this.selectedOrder) {
      console.log(this.selectedOrder);

      this.openViewOrderWindow();
    }
  }
  insertEditorScript() {
      let script = document.querySelector('script[src="https://cdn.ckeditor.com/4.5.11/full-all/ckeditor.js"]');
      if (!script) {
          var externalScript = document.createElement('script');
          externalScript.setAttribute('src', 'https://cdn.ckeditor.com/4.5.11/full-all/ckeditor.js');
          document.head.appendChild(externalScript);
      }
  }
  public openWindow(dataItem) {
    this.applicationTypeWindow.data = dataItem;
    this.applicationTypeWindow.dialogOpened = true;
    this._rootService.getOrderTypes().subscribe(data => {
      this.applicationTypeWindow.orderTypes = data;
      this.applicationTypeWindow.dialogOpened = true;
    });

  }
  public openViewOrderWindow() {
    this.viewOrderWindow = true;
  }
  public closeViewOrderWindow() {
    this.viewOrderWindow = false;
    this.selectedOrder = null;
  }
  public cnicValueChange() {
    this.profileExist = false;
    this.profileNotExist = false;
    this.profile = null;
  }
  public getProfileByCNIC() {
    this.outRangeProfile = false;
    this.searchingProfile = true;
    this._rootService.getProfileByCNIC(this.cnic).subscribe((res: any) => {
      this.searchStarted = true;
      if (res == false) {
        this.profileExist = true;
        this.outRangeProfile = true;
      }
      if (res == 404) {
        this.profile = null;
        this.profileExist = false;
      } else {
        this.profileExist = true;
        this.profile = res;
      }
      this.searchingProfile = false;
    }, err => {
      this.handleError(err);
    });
  }
  public pageChange(event: PageChangeEvent): void {
    this.kGrid.skip = event.skip;
    this.loadOrders();
  }
  public changePagesize(value: any) {
    this.kGrid.pageSize = +value;
    this.kGrid.skip = 0;
    this.loadOrders();
  }
  public numberWithCommas(x) {
    if (x) {
      return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }
  }
  transform(value) {
    return this.sanitized.bypassSecurityTrustHtml(value);
  }
  ngOnDestroy() {
    if(this.subscription) this.subscription.unsubscribe();
    if(this.searchSubscription) this.searchSubscription.unsubscribe();
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

  public printOrder() {
    //let html = document.getElementById('orderPrint').innerHTML;
    var mywindow = window.open('', 'PRINT', 'height=600,width=900');
    if (mywindow) {
      mywindow.document.write(`<html><head>
   <style>

   body {
           
    font-family: sans-serif, Arial, Verdana, "Trebuchet MS" !important;
    line-height: 1.6 !important;
    font-size: 13px !important;
    color: black !important;
    background-color: #fff !important;
}
p, li{
    font-family: sans-serif, Arial, Verdana, "Trebuchet MS" !important;
    line-height: 1.6 !important;
    font-size: 13px !important;
    color: black !important;
    background-color: #fff !important;
}
.mt-2 {
  margin-top: 0.5rem !important;
}.mb-0 {
  margin-bottom: 0 !important;
}
.ml-1 {
  margin-left: 0.25rem !important;
}
.mb-2 {
  margin-bottom: 0.5rem !important;
}

button.print {
  padding: 10px 40px;
  font-size: 21px;
  position: absolute;
  margin-left: 40%;
  background: #46a23f;
  background-image: linear-gradient(rgba(63, 162, 79, 0), rgba(63, 162, 79, 0.2));
  cursor: pointer;
  border: none;
  color: #ffffff;
  z-index: 9999;
}


.w-20 { width: 20% !important; }
.w-30 { width: 30% !important; }
.w-50 { width: 50% !important; }
.w-70 { width: 70% !important; }
.w-80 { width: 80% !important; }

.mt-10 { margin-top: 10px !important; }
.mt-30 { margin-top: 30px  !important; }
@media print {
  button.print {
    display: none;
  }
}

   </style>
           <title>Print Order</title>`);
      mywindow.document.write('</head><body >');
      mywindow.document.write('<button onclick="printFunc()" class="print">Print</button>');
      mywindow.document.write(this.selectedOrder.OrderHTML);
      mywindow.document.write(`
             <script>
       function printFunc() {
         window.print();
       }
       </script>
   `);
      mywindow.document.write('</body></html>');
      //show upload signed copy input chooser

      /*     mywindow.document.close(); // necessary for IE >= 10
          mywindow.focus(); // necessary for IE >= 10
          mywindow.print();
          mywindow.close(); */
    }
  }
}
