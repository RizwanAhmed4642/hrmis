import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RootService } from '../../../_services/root.service';
import { CookieService } from '../../../_services/cookie.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { OnlineJobsService } from '../online-jobs.service';
import { Subscription } from 'rxjs/Subscription';

@Component({
  selector: 'app-offer-letter',
  templateUrl: './offer-letter.component.html',
  styleUrls: ['./offer-letter.component.scss']
})
export class OfferLetterComponent implements OnInit, OnDestroy {

  @ViewChild('photoRef', { static: false }) public photoRef: any;

  public cnic: string = '';
  public photoSrc = '';
  public applicant: any = {};
  public meritActiveDesignation: any = {};
  public documents: any[] = [
    /* { Name: 'Matric' },
    { Name: 'Intermediate' },
    { Name: 'MBBS' },
    { Name: 'Diploma' }, */
  ];
  public jobId: number = 0;
  public isUploading: boolean = false;
  public offerLetterDownloaded: boolean = false;
  public acceptanceLetterDownloaded: boolean = false;
  public acceptanceUploaded: boolean = false;

  public downloadingOfferLetter: boolean = false;
  public downloadingAcceptanceLetter: boolean = false;
  public uploadingAcceptanceLetter: boolean = false;

  public photoFile: any[] = [];
  public applicationAttachments: any[] = [];
  private subscription: Subscription;

  constructor(private route: ActivatedRoute,
    private _rootService: RootService,
    private _cookieService: CookieService,
    private router: Router,
    private _authenticationService: AuthenticationService, private _onlineJobsService: OnlineJobsService) { }

  ngOnInit() {
    this.cnic = this._cookieService.getCookie('cnicussrphfmc');
    this.fetchParams();
    this.getApplicant();
    this.getJobDocumentsRequired();
  }
  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('jobId')) {
          let param = params['jobId'];
          if (+param) {
            this.jobId = +params['jobId'] as number;
            this.getJobDocumentsRequired();
          }
        }
      }
    );
  }

  private getApplicant() {
    this._onlineJobsService.getApplicant(this.cnic).subscribe((res3: any) => {
      if (res3) {
        this.applicant = res3;
      }

    }, err2 => {
      console.log(err2);
    });
  }
  public uploadJobApplicantAttachments() {
    if (this.applicationAttachments.length > 0) {
      this.applicationAttachments.forEach(applicationAttachment => {
        let doc = this.documents.find(x => x.Id == applicationAttachment.JobDocument_Id);
        if (doc) {
          applicationAttachment.TotalMarks = doc.TotalMarks;
          applicationAttachment.ObtainedMarks = doc.ObtainedMarks;
        /*   applicationAttachment.Degree = doc.Degree;
          applicationAttachment.PassingYear = doc.PassingYear;
          applicationAttachment.Division = doc.Division;
          applicationAttachment.Grade = doc.Grade; */
        }
      });
      this._onlineJobsService.uploadJobApplicantAttachments(this.applicationAttachments, this.cnic).subscribe((res) => {
        if (res) {
          this.router.navigate(['/job/experience']);
        }
      }, err => {
        console.log(err);
      });
    } else {
    }
  }
  getOfferLetterLink() {
    this.downloadingOfferLetter = true;
    this._onlineJobsService.getDownloadLink('offer', this.cnic).subscribe((link) => {
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
    this._onlineJobsService.getDownloadLink('acceptance', this.cnic).subscribe((link) => {
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
        this._onlineJobsService.uploadSignedAcceptance(this.photoFile, this.applicant.Id).subscribe((x: any) => {
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
  public getJobDocumentsRequired() {
    this._onlineJobsService.getJobDocumentsRequired(this.jobId).subscribe((res: any) => {
      if (res) {
        this.documents = res;
      }
    }, err => {
      console.log(err);
    })
  }
  public selectFile(event, document: any): void {
    let inputValue = event.target;
    let applicationAttachment: any = {};
    applicationAttachment.JobDocument_Id = document.Id;
    applicationAttachment.DocumentName = document.Name;
    applicationAttachment.files = inputValue.files;
    this.applicationAttachments.push(applicationAttachment);
    this.documents.find(x => x.Id == document.Id).attached = true;
  }
  public uploadBtn(filter: string) {
    if (filter == 'pic') {
      this.photoRef.nativeElement.click();
      /*   this.uploadingAcceptanceLetter = true; */
    }
  }
  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
  /*  removeAcceptance() {
     this._mainService.removeAcceptance(this.applicant.Id).subscribe((x) => {
 
       if (x == true) {
         this.applicant.Status = "ProfileBuilt";
       }
     });
   } */
}
