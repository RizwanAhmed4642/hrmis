import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RootService } from '../../../_services/root.service';
import { CookieService } from '../../../_services/cookie.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { OnlineJobsService } from '../online-jobs.service';
import { Subscription } from 'rxjs/Subscription';
import { LocalService } from '../../../_services/local.service';

@Component({
  selector: 'experience',
  templateUrl: './experience.component.html',
  styleUrls: ['./experience.component.scss']
})
export class ExperienceComponent implements OnInit, OnDestroy {

  @ViewChild('photoRef', { static: false }) public photoRef: any;

  public cnic: string = '';
  public photoSrc = '';
  public applicant: any = {};
  public experience: any = {};
  public experiences: any = [];
  public meritActiveDesignation: any = {};
  public documents: any[] = [
    /* { Name: 'Matric' },
    { Name: 'Intermediate' },
    { Name: 'MBBS' },
    { Name: 'Diploma' }, */
  ];
  public fromDate: Date;
  public toDate: Date;
  public jobId: number = 0;
  public isUploading: boolean = false;
  public offerLetterDownloaded: boolean = false;
  public acceptanceLetterDownloaded: boolean = false;
  public acceptanceUploaded: boolean = false;
  public addingExperience: boolean = false;
  public loading: boolean = false;
  public uploadingFile: boolean = false;
  public uploadingFileError: boolean = false;

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
    private _localService: LocalService,
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
        console.log(this.applicant);

        this.getExperiences();
      }

    }, err2 => {
      console.log(err2);
    });
  }
  public removeExperience(item: any) {
    if (confirm('Are you sure?')) {
      item.removing = true;
      this._onlineJobsService.removeExperience(item.Id).subscribe((res3: any) => {
        item.removing = false;
        if (res3) {
          this.getExperiences();
        }
      }, err2 => {
        console.log(err2);
        item.removing = false;
      });
    }
  }
  public readUrl(event: any, filter: string) {
    if (event.target.files && event.target.files[0]) {
      if (filter == 'pic') {
        this.photoFile = [];
        let inputValue = event.target;
        this.photoFile = inputValue.files;
        /*  var reader = new FileReader();
         reader.onload = ((event: any) => {
           this.photoSrc = event.target.result;
         }).bind(this);
         reader.readAsDataURL(event.target.files[0]); */
      }
    }
  }

  public uploadFile(experienceId: number) {
    this.uploadingFile = true;
    this.addingExperience = true;
    this._onlineJobsService.uploadExperienceCertificate(this.photoFile, experienceId).subscribe((x: any) => {
      if (!x) {
        this.uploadingFileError = true;
      }
      this.uploadingFile = false;
      this.addingExperience = false;
      this.experience = {};
      this.fromDate = null;
      this.toDate = null;
      this.photoFile = [];
      this.getExperiences();
    }, err => {
      console.log(err);
      this.uploadingFileError = true;
      this.uploadingFile = false;
    });
  }
  public getExperiences() {
    this.loading = true;
    this._onlineJobsService.getExperiences(this.applicant.Id).subscribe((res: any) => {
      if (res) {
        console.log(res);

        this.experiences = res;
        this.loading = false;
      }
    }, err => {
      console.log(err);
    });
  }
  public addExperience(obj) {
    this.addingExperience = true;
    if (this.fromDate && this.fromDate instanceof Date) {
      obj.FromDate = this.fromDate.toDateString();
    }
    if (this.toDate && this.toDate instanceof Date) {
      obj.ToDate = this.toDate.toDateString();
    }
    obj.Applicant_Id = this.applicant.Id;
    this._onlineJobsService.saveExperience(obj).subscribe((res: any) => {
      if (res) {
        this.uploadFile(res.Id);
      }
    }, err => {
      console.log(err);
    });
  }
  public proceed() {
    let designationId = this._localService.get('desigaplid');
    this.router.navigate(['/job/apply-now/' + designationId]);
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
