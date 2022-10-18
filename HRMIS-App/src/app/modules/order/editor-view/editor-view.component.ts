import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { RootService } from '../../../_services/root.service';
import { ActivatedRoute, Router } from '@angular/router';
import { OrderService } from '../order.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { Subscription } from 'rxjs/Subscription';
import { ESR } from '../TransferAndPosting/ESR.class';
import { LocalService } from '../../../_services/local.service';

@Component({
  selector: 'app-editor-view',
  templateUrl: './editor-view.component.html',
  styles: []
})
export class EditorViewComponent implements OnInit, OnDestroy {
  @ViewChild('photoRef', { static: false }) public photoRef: any;
  public currentUser: any = {};
  public loading: boolean = false;
  public loadingQR: boolean = false;
  public saving: boolean = false;
  public saved: boolean = false;
  public showEditor: boolean = false;
  public uploadingFile: boolean = false;
  public uploadingFileError: boolean = false;
  public esr: any;
  public leaveOrder: any;
  public orderId: number = 0;
  public orderType: number = 1;
  public orderRequestId: number = 0;
  public qr: string = '';
  public photoSrc = '';
  public photoFile: any[] = [];
  name = 'ng2-ckeditor';
  ckeConfig: any;
  mycontent: string;
  log: string = '';
  @ViewChild("myckeditor", { static: false }) ckeditor: any;
  private subscription: Subscription;

  constructor(private sanitized: DomSanitizer,
    private _localService: LocalService,
    private _rootService: RootService,
    private router: Router,
    private route: ActivatedRoute, private _orderService: OrderService, private _authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.insertEditorScript();
    this.currentUser = this._authenticationService.getUser();

    this.ckeConfig = {
      height: 1005
    };
    this.orderRequestId = this._localService.get('DORId');
  }
  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('id') && +params['id'] && params.hasOwnProperty('type') && +params['type']) {
          this.orderId = +params['id'];
          this.orderType = +params['type'];
          debugger;
          this.loadOrder();
        }
      }
    );
  }
  public uploadFile() {
    this.uploadingFile = true;
    this._orderService.uploadReqeustedSignedCopy(this.photoFile, this.esr.Id, this.orderRequestId).subscribe((x: any) => {
      if (!x.result) {
        this.uploadingFileError = true;
      }
      this.uploadingFile = false;
      this.router.navigate(['/order/phfmc-order-list']);
    }, err => {
      this.uploadingFileError = true;
      this.uploadingFile = false;
    });
  }
  public uploadBtn() {
    this.photoRef.nativeElement.click();
  }
  public readUrl(event: any) {
    if (event.target.files && event.target.files[0]) {
      this.photoFile = [];
      let inputValue = event.target;
      this.photoFile = inputValue.files;
      this.uploadFile();
      /* var reader = new FileReader();
      reader.onload = ((event: any) => {
        this.photoSrc = event.target.result;
      }).bind(this);
      reader.readAsDataURL(event.target.files[0]); */
    }
  }
  insertEditorScript() {
    let script = document.querySelector('script[src="https://cdn.ckeditor.com/4.5.11/full-all/ckeditor.js"]');
    if (!script) {
      var externalScript = document.createElement('script');
      externalScript.setAttribute('src', 'https://cdn.ckeditor.com/4.5.11/full-all/ckeditor.js');
      document.head.appendChild(externalScript);
    }
    this.fetchParams();
  }
  public loadOrder = () => {
    this.loading = true;
    this._orderService.getOrder(this.orderId, this.orderType).subscribe((data: any) => {
      if (data) {
        if (this.orderType == 2) {
          this.esr = new ESR();
          this.esr.TransferTypeID = 5;
          this.leaveOrder = data;
          debugger;
          if (this.leaveOrder.UserName == this.currentUser.UserName || this.currentUser.UserName == 'dpd' || this.currentUser.UserName == 'hr.Irfan') {
            this.showEditor = true;
          }
        } else {
          this.esr = data;
          if (this.esr.Created_By == this.currentUser.UserName || this.currentUser.UserName == 'dpd' || this.currentUser.UserName == 'hr.Irfan') {
            this.showEditor = true;
          }
        }
        this.loading = false;
      }
    },
      err => {
        this.handleError(err);
      });
  }
  public generateQRManual = () => {
    this.loadingQR = true;
    this._orderService.generateQRManual(this.orderId, this.orderType).subscribe((data: any) => {
      if (data) {
        console.log(data)
        this.qr = data.qrSrc;
        this.loadingQR = false;
      }
    },
      err => {
        this.handleError(err);
      });
  }
  private handleError(err: any) {
    this.loading = false;
    this.saving = false;
    this.saved = false;
    this.esr = null;
    this.leaveOrder = null;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }
  onChange($event: any): void {
    this.saving = true;
    this.saved = false;
    if (this.esr.TransferTypeID == 5) {
      if (this.leaveOrder.UserId == this.currentUser.Id || this.currentUser.Id == '16842b18-d22d-4e81-8f94-97b6b028cf63') {
        this._orderService.UpdateLeaveOrderHTML(this.leaveOrder).subscribe((x) => {
          this.saving = false;
          this.saved = true;
        });
      }
    } else {
      if (this.esr.Users_Id == this.currentUser.Id || this.currentUser.Id == '16842b18-d22d-4e81-8f94-97b6b028cf63') {
        this._orderService.UpdateOrderHTML(this.esr.Id, this.esr.OrderHTML).subscribe((x) => {
          this.saving = false;
          this.saved = true;
        });
      }
    }
  }
  /* onChange($event: any): void {
    this.saving = true;
    this.saved = false;
    if (this.esr.TransferTypeID == 5) {
      if (this.leaveOrder.UserId == this.currentUser.Id) {
        this._orderService.UpdateLeaveOrderHTML(this.leaveOrder).subscribe((x) => {
          this.saving = false;
          this.saved = true;
        });
      }
    } else {
      if (this.esr.Users_Id == this.currentUser.Id) {
        this._orderService.UpdateOrderHTML(this.esr.Id, this.esr.OrderHTML).subscribe((x) => {
          this.saving = false;
          this.saved = true;
        });
      }
    }
  } */
  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
