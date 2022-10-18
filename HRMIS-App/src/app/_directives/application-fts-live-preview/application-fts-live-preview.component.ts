import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import { LivePreviewService } from '../../_services/live-preview.service';
import { ApplicationMaster } from '../../modules/application-fts/application-fts';
import { DomSanitizer } from '@angular/platform-browser';
import { RootService } from '../../_services/root.service';
import { AuthenticationService } from '../../_services/authentication.service';

@Component({
  selector: 'app-application-fts-live-preview',
  templateUrl: './application-fts-live-preview.component.html'
})
export class ApplicationFtsLivePreviewComponent implements OnInit, OnDestroy {
  public htmlLive: any;
  public tokenNumber: any;
  private subscription: Subscription;
  public application: any;
  public barcode: any;
  public user: any;
  constructor(private sanitizer: DomSanitizer, private _authenticationService: AuthenticationService, private _rootService: RootService) { }
  ngOnInit() {
    this.user = this._authenticationService.getUser();
    /*  this._livePreviewService.update('');
   setInterval(() => {
      let html = this._livePreviewService.getLivePreview();
      if (html != this.htmlLive) {

      }
      this.htmlLive = html;
      this.tokenNumber = this._livePreviewService.getTokenNumber();
    }, 1000); */
   /*  this.subscribeToFirebase(); */
  }
  private getBarcode() {
    this._rootService.generateBars(1234).subscribe(x => x ? this.barcode = x.barCode : '');
  }
  /* private subscribeToFirebase() {
    let metaInfo: any = {
      uid: this.user.Id,
    };
    this._firebaseHisduService.getApplicationPreview(metaInfo).subscribe(data => {
      let arr = data.map(e => {
        return {
          id: e.payload.doc.id,
          ...e.payload.doc.data()
        };
      });
      console.log(arr[0]);
      
      this.application = arr[0];
      this.getBarcode();
      this.application.time = new Date();
    });
  } */
  public barcodeSrc() {
    return this.sanitizer.bypassSecurityTrustUrl(this.barcode);
  }
  public capitalize(val: string) {
    if (!val) return '';
    if (!val.toLowerCase().endsWith('application')) val += ' Application';
    return val.toUpperCase();
  }
  public dashifyCNIC(cnic: string) {
    if (!cnic) return '';
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
  ngOnDestroy() {
    this.subscription.unsubscribe();
  }
}
