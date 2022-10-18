import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RootService } from '../../../_services/root.service';
import { CookieService } from '../../../_services/cookie.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { OnlinePostingService } from '../online-porting.service';

@Component({
  selector: 'app-offer-letter',
  templateUrl: './offer-letter.component.html',
  styleUrls: ['./offer-letter.component.scss']
})
export class OfferLetterComponent implements OnInit {
  @ViewChild('photoRef', {static: false}) public photoRef: any;

  public cnic: string = '';
  public photoSrc = '';
  public merit: any = {};
  public meritActiveDesignation: any = {};

  public isUploading: boolean = false;
  public offerLetterDownloaded: boolean = false;
  public acceptanceLetterDownloaded: boolean = false;
  public acceptanceUploaded: boolean = false;

  public downloadingOfferLetter: boolean = false;
  public downloadingAcceptanceLetter: boolean = false;
  public uploadingAcceptanceLetter: boolean = false;

  public photoFile: any[] = [];

  constructor(private route: ActivatedRoute, 
    private _rootService: RootService,
    private _cookieService: CookieService,
    private router: Router,
    private _authenticationService: AuthenticationService, private _onlinePostingService: OnlinePostingService) { }

  ngOnInit() {
    this.cnic = this._cookieService.getCookie('cnicussrpublic');
    this.fetchMeritInfo();
  }
  public fetchMeritInfo() {
    this._onlinePostingService.getMeritProfile(this.cnic).subscribe((res: any) => {
      if (res) {
        this._onlinePostingService.getMeritActiveDesignation(res.MeritsActiveDesignationId).subscribe((data: any) => {
          this.meritActiveDesignation = data;
          if (data.IsActive == 'Y') {
            this.merit = res;
          } else {
            this.router.navigate(['/ppsc/review']);
          }
        }, err => {
          console.log(err);
        });
        //   this.photoSrc = 'https://hrmis.pshealthpunjab.gov.pk/Uploads/A/' + this.merit.CNIC + '_23.jpg?v=' + this.radnom;
      }
    }, err => {
      console.log(err);
    });
  }
  getOfferLetterLink() {
    this.downloadingOfferLetter = true;
    this._onlinePostingService.getDownloadLink('offer', this.cnic).subscribe((link) => {
      if (link) {
        this.downloadingOfferLetter = false;
        window.open('' + link, '_blank');
        this.offerLetterDownloaded = true;
      } else {
        this.offerLetterDownloaded = false;
      }
    });
  }
  getAcceptanceLetterLink() {
    this.downloadingAcceptanceLetter = true;
    this._onlinePostingService.getDownloadLink('acceptance', this.cnic).subscribe((link) => {
      if (link) {
        this.downloadingAcceptanceLetter = false;
        window.open('' + link, '_blank');
        this.acceptanceLetterDownloaded = true;
      } else {
        this.acceptanceLetterDownloaded = false;
      }
    });
  }
  public readUrl(event: any, filter: string) {
    if (event.target.files && event.target.files[0]) {
      if (filter == 'pic') {
        this.photoFile = [];
        let inputValue = event.target;
        this.photoFile = inputValue.files;
        this._onlinePostingService.uploadSignedAcceptance(this.photoFile, this.merit.Id).subscribe((x: any) => {
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
     this._mainService.removeAcceptance(this.merit.Id).subscribe((x) => {
 
       if (x == true) {
         this.merit.Status = "ProfileBuilt";
       }
     });
   } */
}
