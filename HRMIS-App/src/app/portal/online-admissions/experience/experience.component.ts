import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RootService } from '../../../_services/root.service';
import { CookieService } from '../../../_services/cookie.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { Subscription } from 'rxjs/Subscription';
import { LocalService } from '../../../_services/local.service';
import { OnlineAdmissionsService } from '../online-admissions.service';


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
  public houseJobxperience: any;
  public experiences: any = [];
  public meritActiveDesignation: any = {};
  public documents: any[] = [
    /* { Name: 'Matric' },
    { Name: 'Intermediate' },
    { Name: 'MBBS' },
    { Name: 'Diploma' }, */
  ];
  public expTypes: any[] = [
    {Id: 1, Name: 'Clinical'},
    {Id: 2, Name: 'Non-Clinical'}
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
  public houseJobFile: any[] = [];
  public applicationAttachments: any[] = [];
  private subscription: Subscription;
  public urdu: any = {
    info: `اپنے تجربے  کا درست اندراج کیجئے۔ اپنے تجربہ سرٹیفیکیٹ و مطلوبہ کاغذات اپلوڈ کریں۔ کسی بھی قسم کے غلط اندراج کی صورت میں آپ کی درخواست پر عمل درآمد روک دیا جائے گا۔`,
    infoeng: 'Please correctly enter your experience data and upload the required document. In case of any wrong entry your request will not be processed',
    infoeng2: 'House job will not be considered as experience, so the experience as house job must not be entered'
  };
  constructor(private route: ActivatedRoute,
    private _rootService: RootService,
    private _cookieService: CookieService,
    private router: Router,
    private _localService: LocalService,
    private _authenticationService: AuthenticationService,
    private _onlineAdmissionsService: OnlineAdmissionsService
  ) { }

  ngOnInit() {
    this.cnic = this._cookieService.getCookie('cnicussradhoc');
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
    this._onlineAdmissionsService.getApplicant(this.cnic).subscribe((res3: any) => {
      if (res3) {
        this.applicant = res3;
        this.getExperiences();
      }

    }, err2 => {
      console.log(err2);
    });
  }
  public removeExperience(item: any) {
    if (confirm('Are you sure?')) {
      item.removing = true;
      this._onlineAdmissionsService.removeExperience(item.Id).subscribe((res3: any) => {
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
    this._onlineAdmissionsService.uploadExperienceCertificate(this.photoFile, experienceId).subscribe((x: any) => {
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
    this._onlineAdmissionsService.getExperiences(this.applicant.Id).subscribe((res: any) => {
      if (res) {
        console.log(res);
        this.experiences = res;
        this.houseJobxperience = this.experiences.find(x => x.JobDocument_Id == 1);
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
    if (obj.TypeId == 1) {
      obj.Type = 'Clinical'
    }
    if (obj.TypeId == 2) {
      obj.Type = 'Non-Clinical'
    }
    this._onlineAdmissionsService.saveExperience(obj).subscribe((res: any) => {
      if (res && res.Id) {
        if (this.photoFile.length > 0) {
          this.uploadingFile = true;
          this.addingExperience = true;
          this._onlineAdmissionsService.uploadExperienceCertificate(this.photoFile, res.Id).subscribe((x: any) => {
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
        this.addingExperience = false;
        this.experience = {};
        this.fromDate = null;
        this.toDate = null;
        this.photoFile = [];
        this.getExperiences();
      }
    }, err => {
      console.log(err);
    });
  }
  public addHouseJobExperience(obj) {
    this.addingExperience = true;
    obj.JobTitle = 'House Job';
    obj.JobDocument_Id = 1;
    if (this.fromDate && this.fromDate instanceof Date) {
      obj.FromDate = this.fromDate.toDateString();
    }
    if (this.toDate && this.toDate instanceof Date) {
      obj.ToDate = this.toDate.toDateString();
    }
    obj.Applicant_Id = this.applicant.Id;
    this._onlineAdmissionsService.saveExperience(obj).subscribe((res: any) => {
      if (res && res.Id) {
        if (this.photoFile.length > 0) {
          this.uploadingFile = true;
          this.addingExperience = true;
          this._onlineAdmissionsService.uploadExperienceCertificate(this.photoFile, res.Id).subscribe((x: any) => {
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
        this.addingExperience = false;
        this.experience = {};
        this.fromDate = null;
        this.toDate = null;
        this.photoFile = [];
        this.getExperiences();
      }
    }, err => {
      console.log(err);
    });
  }
  public openInNewTab(link: string) {
    window.open(link, '_blank');
  }
  public proceed() {
    let designationId = +this._cookieService.getCookie('cnicussradhocdesig');
    this.router.navigate(['/adhoc/apply-now/' + designationId]);
  }
  getOfferLetterLink() {
    this.downloadingOfferLetter = true;
    this._onlineAdmissionsService.getDownloadLink('offer', this.cnic).subscribe((link) => {
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
    this._onlineAdmissionsService.getDownloadLink('acceptance', this.cnic).subscribe((link) => {
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
    this._onlineAdmissionsService.getJobDocumentsRequired(this.jobId).subscribe((res: any) => {
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
}
