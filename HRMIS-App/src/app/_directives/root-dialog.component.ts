import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import { Notification } from '../_models/notification.class';
import { NotificationService } from '../_services/notification.service';
import { RootService } from '../_services/root.service';
import { Subject } from 'rxjs/Subject';
import { DropDownsHR } from '../_helpers/dropdowns.class';
import { AuthenticationService } from '../_services/authentication.service';
import { DialogService } from '../_services/dialog.service';
import { Router, ActivatedRoute } from '@angular/router';
import { LocalService } from '../_services/local.service';

@Component({
  selector: 'app-root-dialog',
  templateUrl: './root-dialog.component.html'
})
export class RootDialogComponent implements OnInit, OnDestroy {
  public value: any;
  public loaded: boolean = false;
  public loadedContent: boolean = true;
  public outRangeProfile: boolean = false;
  public controlPlaceHolders: boolean[] = [false];
  public thumbPlaceHolders: boolean[] = [false, false];
  public inputChange: Subject<any>;
  public dropDowns: DropDownsHR = new DropDownsHR();
  public orders: any[] = [];
  public orderTypes: any[] = [];
  public applicationTypes: any[] = [];
  public applicationTypesData: any[] = [];
  public totalOrders: number = 0;
  public orderType_Id: number = 0;
  public currentPage: number = 1;
  public pageSize: number = 20;
  public selectedTypeId: number = 0;
  public searchQuery: string = '';
  public selectedOrder: any;
  private subscription: Subscription;
  public applicatonDialogOpened: boolean = false;
  public orderDialogOpened: boolean = false;
  public viewOrderWindow: boolean = false;
  public searchingProfile: boolean = false;
  public searchStarted: boolean = false;
  public profileExist: boolean = null;
  public profileNotExist: boolean = false;
  public mutualTransfer: boolean = false;
  public cnicMask: string = "00000-0000000-0";
  public cnic: string = '';
  public cnic2: string = '';
  public profile: any;
  public profile2: any;
  public currentUser: any;
  public orderRequestId: number = 0;
  constructor(private router: Router,
    private _localService: LocalService,
    private _rootService: RootService,
    private _dialogService: DialogService,
    private _authenticationService: AuthenticationService) { }
  ngOnInit() {
    this.currentUser = this._authenticationService.getUser();
    this.subscription = this._dialogService.getDialog().subscribe(
      (x: any) => {
        if (x.type == 'application') {
          this.loadApplicationTypes();
          this.openApplicationWindow();
        }
        if (x.type == 'order') {
          this.loadOrderTypes();
          if (x.cnic) {
            this.cnic = x.cnic;
          }
          this.orderRequestId = x.reqeustId;
          this.openWindow();
        }
      });
  }
  public loadOrderTypes = () => {
    this._rootService.getOrderTypes().subscribe((data: any) => {
      if (data) {
        this.orderTypes = data;
      }
    });
  }
  public combineOrder = (link) => {
    this.openInNewTab(link);
    this.closeApplicationWindow();
  }
  public loadApplicationTypes = () => {
    this._rootService.getApplicationTypesActive().subscribe((data: any) => {
      if (data) {
        this.applicationTypes = data;
        this.applicationTypesData = this.applicationTypes;
      }
    });
  }
  public typeSearch(value) {
    if (value && value.length == 1) {
      this.applicationTypesData = this.applicationTypes.filter((s: any) => s.Name.toLowerCase().startsWith(value.toLowerCase()));
    } else {
      this.applicationTypesData = this.applicationTypes.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
  }
  private handleError(err: any) {
    this.loadedContent = true;
    this.searchingProfile = false;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }
  public closeWindow() {
    this.cnic = '';
    this.cnicValueChange();
    this.orderDialogOpened = false;
  }
  public openWindow() {
    if (this.cnic) {
      this.getProfileByCNIC();
    }
    this.orderDialogOpened = true;
  }
  public closeApplicationWindow() {
    this.applicatonDialogOpened = false;
  }
  public openApplicationWindow() {
    this.applicatonDialogOpened = true;
  }
  public openApplicationType(link: string) {
    this.closeApplicationWindow();
    //this.openInNewTab(link);
    this.router.navigate([link]);
  }
  public openOrderType(link: string) {
    this._localService.set('ORId', this.orderRequestId);
    this.closeWindow();
    this.openInNewTab(link);
    //this.router.navigate([link]);
  }
  public openInNewTab(link: string) {
    /* window.open('http://localhost:4200/' + link, '_blank'); */
    window.open(link, '_blank');
  }
  public cnicValueChange() {
    this.profileExist = null;
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
      if (this.mutualTransfer) {
        this._rootService.getProfileByCNIC(this.cnic2).subscribe((res2: any) => {
          this.searchStarted = true;
          if (res2 == false) {
            this.profileExist = true;
            this.outRangeProfile = true;
          }
          if (res2 == 404) {
            this.profile2 = null;
            this.profileExist = false;
          } else {
            this.profileExist = true;
            this.profile2 = res2;
          }
          this.searchingProfile = false;
          if (this.profile2) {
            this.router.navigate(['/order/new/' + this.cnic + '/' + this.cnic2 + '/1']);
            this.closeWindow();
          }
        }, err => {
          this.handleError(err);
        });
      } else {
        this.searchingProfile = false;
      }
    }, err => {
      this.handleError(err);
    });
  }
  ngOnDestroy() {
    this.subscription.unsubscribe();
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
