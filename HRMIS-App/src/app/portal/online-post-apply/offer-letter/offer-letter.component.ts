import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RootService } from '../../../_services/root.service';
import { CookieService } from '../../../_services/cookie.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { OnlinePostApplyService } from '../online-post-apply.service';

@Component({
  selector: 'app-offer-letter',
  templateUrl: './offer-letter.component.html',
  styleUrls: ['./offer-letter.component.scss']
})
export class OfferLetterComponent implements OnInit {
  @ViewChild('photoRef', {static: false}) public photoRef: any;

  public cnic: string = '';
  public photoSrc = '';
  public profile: any = {};
  public promotionApply: any = {};

  public isUploading: boolean = false;
  public offerLetterDownloaded: boolean = false;
  public acceptanceLetterDownloaded: boolean = false;
  public acceptanceUploaded: boolean = false;

  public downloadingOfferLetter: boolean = false;
  public downloadingAcceptanceLetter: boolean = false;
  public uploadingAcceptanceLetter: boolean = false;

  public photoFile: any[] = [];

  constructor(private route: ActivatedRoute, private _rootService: RootService,
    private _cookieService: CookieService,
    private router: Router,
    private _authenticationService: AuthenticationService, private _onlinePostApplyService: OnlinePostApplyService) { }

  ngOnInit() {
    this.cnic = this._cookieService.getCookie('cnicussrpublic');
    this.fetchMeritInfo();
  }
  public fetchMeritInfo() {
    this._onlinePostApplyService.getPromotionApplyData(this.cnic).subscribe((res: any) => {
      if (res && res.Profile && res.PromotionApply) {
        this.profile = res.Profile;
        this.promotionApply = res.PromotionApply;
      }
    }, err => {
      console.log(err);
    });
  }
  getAcceptanceLetterLink() {
    this.downloadingAcceptanceLetter = true;
    window.open('http://localhost:8913/api/JobApp/DownloadAcceptanceLetterSMO/' + this.profile.Id, '_blank');
  }
  public readUrl(event: any, filter: string) {
    if (event.target.files && event.target.files[0]) {
      if (filter == 'pic') {
        this.photoFile = [];
        let inputValue = event.target;
        this.photoFile = inputValue.files;
        this._onlinePostApplyService.uploadSignedAcceptance(this.photoFile, this.promotionApply.Id).subscribe((x: any) => {
          if (x.result) {
            var reader = new FileReader();
            reader.onload = ((event: any) => {
              this.photoSrc = event.target.result;
            }).bind(this);
            reader.readAsDataURL(event.target.files[0]);
            this.acceptanceUploaded = true;
          } else {
            this.acceptanceUploaded = false;
          }
          this.uploadingAcceptanceLetter = false;
        }, err => {
          console.log(err);
          this.acceptanceUploaded = false;
        });
      }
    }
  }

  public uploadBtn(filter: string) {
    if (filter == 'pic') {
      this.photoRef.nativeElement.click();
      /*   this.uploadingAcceptanceLetter = true; */
    }
  }
  /*  removeAcceptance() {
     this._mainService.removeAcceptance(this.profile.Id).subscribe((x) => {
 
       if (x == true) {
         this.profile.Status = "ProfileBuilt";
       }
     });
   } */
}
