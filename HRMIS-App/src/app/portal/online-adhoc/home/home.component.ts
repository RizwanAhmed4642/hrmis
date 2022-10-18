import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RootService } from '../../../_services/root.service';
import { CookieService } from '../../../_services/cookie.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { Subscription } from 'rxjs/Subscription';
import { LocalService } from '../../../_services/local.service';
import { OnlineAdhocService } from '../online-adhoc.service';

@Component({
  selector: 'home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit, OnDestroy {

  @ViewChild('photoRef', { static: false }) public photoRef: any;

  public cnic: string = '';
  public photoSrc = '';
  public applicant: any = {};
  public application: any = {};
  public applications: any = [];
  public interviewApplications: any = [];
  public adhocGreivanceUploads: any = [];
  public adhocGreivances: any = [];
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
  public fileUpload: any = { photo: false, pmdc: false, domicile: false, cnic: false, hifz: false };
  public downloadingOfferLetter: boolean = false;
  public downloadingAcceptanceLetter: boolean = false;
  public uploadingAcceptanceLetter: boolean = false;
  public rejectedApplication: boolean = false;
  public rejectedGrievance: boolean = false;
  public showhafiz: boolean = false;
  public showPosition: boolean = false;
  public showMeritList: boolean = false;
  public urdu: any = {
    info: `آپ درخواست جمع کروانے کی آخری تاریخ سے پہلے اپنی درخواست یامہیا کردہ معلومات میں تبدیلی کرسکتے ہیں۔آخری تاریخ کے بعد تبدیلی کرنے کی رسائی ختم کر دی جائےگی۔`,
    info2: `آخری تاریخ کے گزرجانے کے بعد ضلعی جانچ پڑتال کی کمیٹی آپ کی درخواست کا جائزہ لے گی۔ اگر کسی بناپر آپ کی درخواست مسترد ہو گئی ہے تو آپ کو آن لائن بذریعہ اِن باکس اور ایس ایم ایس مطلع کیا جائے گا تاکہ آپ مقررہ تین ایام کے اندر اپنی شکایت درج کر سکیں۔ مقررہ تین یوم کے بعد شکایت درج کرنے کی رسائی ختم کر دی جائے گی۔ -`,
    info3: `آپ سے درخواست ہے کہ اصل کوائف کے ساتھ انٹرویو کے لیے حاضر ہوں۔ اگر کسی ضلع میں آپ اصل کوائف فراہم کرنے میں ناکام رہتے ہیں، تو اس ضلع میں آپ کا میرٹ صفر ہو جائے گا۔اگر آپ کا موبائل نمبر پورٹ ہے تو آپ ایس ایم ایس وصول نہیں کر سکیں گے۔`
  };
  public photoFile: any[] = [];
  public houseJobFile: any[] = [];
  public applicationAttachments: any[] = [];
  private subscription: Subscription;
  public endDate: any = '12/23/2021 23:59:59';
  public currentDate: Date;
  public disableGrievance: boolean = false;
  constructor(private route: ActivatedRoute,
    private _rootService: RootService,
    private _cookieService: CookieService,
    private router: Router,
    private _localService: LocalService,
    private _authenticationService: AuthenticationService,
    private _onlineAdhocService: OnlineAdhocService
  ) { }

  ngOnInit() {
    this.cnic = this._cookieService.getCookie('cnicussradhoc');
    this.getApplicant();
    this.currentDate = new Date();
    this.dateCompare(this.currentDate, this.endDate);
  }
  public checkShowHafiz() {
    let arr: any[] = [
      '3610338106935',
      '3520285521138',
      '3130479648789',
      '3130311627631',
      '3660218721475',
      '3310256720204',
      '3640131670840',
      '3310427512187'
    ];
    let d = arr.find(x => x == this.applicant.CNIC);
    if (d) {
      this.showhafiz = true;
    }
  }
  public checkShowPosition() {
    let arr: any[] = [
      '3210202649501',
      '3230362679970',
      '3120374701805',
      '3640128840438',
      '3130378190680',
      '3210217747346',
      '3460404194299',
      '3820171356312',
      '3720370053737'
    ];
    let d = arr.find(x => x == this.applicant.CNIC);
    if (d) {
      this.showPosition = true;
    }
  }
  public dateCompare(d1, d2) {
    const date1 = new Date(d1);
    const date2 = new Date(d2);

    if (date1 > date2) {
      this.disableGrievance = true;
      console.log(`${d1} is greater than ${d2}`, this.disableGrievance)
    } else if (date1 < date2) {
      console.log(`${d2} is greater than ${d1}`, this.disableGrievance)
    } else {
      console.log(date1, date2, `Both dates are equal`)
    }
  }
  private getApplicant() {
    this._onlineAdhocService.getApplicant(this.cnic).subscribe((res3: any) => {
      if (res3) {
        this.applicant = res3;
        this.checkShowHafiz();
        this.checkShowPosition();
        this.getAdhocApplications();
        this.getAdhocInterviewApplications();
        this.getAdhocGrievancesByApplicant();
      }

    }, err2 => {
      console.log(err2);
    });
  }
  public readUrl(event: any, filter: string) {
    if (event.target.files && event.target.files[0]) {
      if (filter == 'pic') {
        this.photoFile = [];
        let inputValue = event.target;
        this.photoFile = inputValue.files;
      }
    }
  }
  public readScrutinyUrl(event: any, item: any) {
    if (event.target.files && event.target.files[0]) {
      item.photoFile = [];
      let inputValue = event.target;
      item.photoFile = inputValue.files;
    }
  }
  public readPositionUrl(event: any, item: any) {
    if (event.target.files && event.target.files[0]) {
      item.photoFile = [];
      let inputValue = event.target;
      item.photoFile = inputValue.files;
    }
  }


  public uploadApplicantQualification(item: any) {
    item.saving = true;
    item.Qualification.Old_Id = item.Qualification.Old_Id ? item.Qualification.Old_Id : item.Qualification.Id;
    item.Qualification.PassingYear = item.Qualification.PassingYear ? item.Qualification.PassingYear.toDateString() : null;
    this._onlineAdhocService.saveApplicantQualification(item.Qualification).subscribe((res: any) => {
      if (res.Id) {
        if (item.photoFile.length > 0) {
          this._onlineAdhocService.uploadApplicantQualification(item.photoFile, res.Id).subscribe((x: any) => {
            if (!x) {
              this.uploadingFileError = true;
            }
            item.photoFile = [];
            this.getAdhocScrutiny();
          }, err => {
            console.log(err);
            this.uploadingFileError = true;
            this.uploadingFile = false;
          });
        } else {
          item.photoFile = [];
          this.getAdhocScrutiny();
        }
      }
    }, err => {
      console.log(err);
    });
  }
  public getApplicantQualificationById(dataItem: any, id: number) {
    this._onlineAdhocService.getApplicantQualificationById(id).subscribe((res2: any) => {
      if (res2) {
        dataItem.Qualification = res2;
        if (dataItem.Qualification) { dataItem.Qualification.PassingYear = new Date(dataItem.Qualification.PassingYear) }
        dataItem.saving = false;
      }
    }, err => {
      console.log(err);
      dataItem.saving = false;
    });
  }

  public reUploadApplicantDomicile(item) {
    item.saving = true;
    this.fileUpload.photo = true;
    this._onlineAdhocService.reUploadApplicantDomicile(item.photoFile, this.applicant.CNIC).subscribe((res: any) => {
      if (res) {
        item.saving = false;
        item.photoFile = [];
        this.getAdhocScrutiny();
      }
    }, err => {
      item.saving = false;
    });
  }
  public reUploadApplicantPMDC(item) {
    item.saving = true;
    this._onlineAdhocService.reUploadApplicantPMDC(item.photoFile, this.applicant.CNIC).subscribe((res: any) => {
      if (!res) {
        this.uploadingFileError = true;
      }
      item.photoFile = [];
      item.saving = false;
      this.getAdhocScrutiny();
    }, err => {
      console.log(err);
      item.saving = false;
    });
  }
  public reUploadApplicantHifz(item) {
    item.saving = true;
    this._onlineAdhocService.reUploadApplicantHifz(item.photoFile, this.applicant.CNIC).subscribe((res: any) => {
      if (res) {
        item.saving = false;
      }
      item.photoFile = [];
    }, err => {
      console.log(err);
      item.saving = false;
    });
  }

  public uploadApplicantHifz(item) {
    item.saving = true;
    this._onlineAdhocService.uploadApplicantHifz(item.photoFile, this.applicant.CNIC).subscribe((res: any) => {
      if (res) {
        item.saving = false;
      }
      item.photoFile = [];
      this.getApplicant();
    }, err => {
      console.log(err);
      item.saving = false;
    });
  }
  public uploadPositionDoc(item) {
    item.saving = true;
    this._onlineAdhocService.uploadPositionDoc(item.photoFile, this.applicant.CNIC).subscribe((res: any) => {
      if (res) {
        item.saving = false;
      }
      item.photoFile = [];
      this.getApplicant();
    }, err => {
      console.log(err);
      item.saving = false;
    });
  }

  public getAdhocGrievancesByApplicant() {
    this._onlineAdhocService.getAdhocGrievancesByApplicant(this.applicant.Id).subscribe((res2: any) => {
      if (res2) {
        this.adhocGreivances = res2;
        this.adhocGreivances.forEach(g => {
          if (g.StatusId == 3) {
            this.rejectedGrievance = true;
          }
        });

      }
    }, err => {
      console.log(err);
    });
  }

  public getAdhocScrutiny() {
    this._onlineAdhocService.getAdhocGrievance(this.application.Id).subscribe((res2: any) => {
      if (res2) {
        this.application.Grievance = res2;
        if (this.application.Grievance) {
          this.rejectedGrievance = this.application.Grievance.StatusId == 3 ? true : false;
        }
      }
    }, err => {
      console.log(err);
    });
    this._onlineAdhocService.getAdhocGrievanceUploads(this.applicant.Id).subscribe((grv) => {
      if (grv) {
        this.adhocGreivanceUploads = grv;
      }
      this._onlineAdhocService.getApplicantQualification(this.applicant.Id).subscribe((qualif: any) => {
        if (qualif) {
          let qualifications = qualif;
          this._onlineAdhocService.getAdhocScrutiny(this.application.Id).subscribe((x2: any) => {
            if (x2) {
              this.application.AdhocScrutinies = x2;
              let temp: any[] = [];
              this.application.AdhocScrutinies.forEach(scrutiny => {
                if (scrutiny.IsRejected) {

                  this.adhocGreivanceUploads.forEach(adhocGreivanceUpload => {
                    if (!adhocGreivanceUpload.IsQualification) {
                      if (adhocGreivanceUpload.DocName == scrutiny.DocName) {
                        scrutiny.UploadPath = adhocGreivanceUpload.UploadPath;
                      }
                    } else if (adhocGreivanceUpload.IsQualification) {
                      if (adhocGreivanceUpload.QualificationId == scrutiny.Qualification_Id) {
                        scrutiny.UploadPath = adhocGreivanceUpload.UploadPath;
                      }
                    }
                  });

                  scrutiny.Qualification = qualifications.find(x => x.Id == scrutiny.Qualification_Id || x.Old_Id == scrutiny.Qualification_Id);
                  console.log(scrutiny.Qualification);

                  this.adhocGreivanceUploads.forEach(adhocGreivanceUpload => {
                    if (!adhocGreivanceUpload.IsQualification) {
                      if (adhocGreivanceUpload.DocName == scrutiny.DocName) {
                        scrutiny.UploadPath = adhocGreivanceUpload.UploadPath;
                      }
                    } else if (adhocGreivanceUpload.IsQualification && scrutiny.Qualification) {
                      if (adhocGreivanceUpload.QualificationId == scrutiny.Qualification.Id) {
                        scrutiny.UploadPath = adhocGreivanceUpload.UploadPath;
                      }
                    }
                  });

                  if (scrutiny.Qualification) { scrutiny.Qualification.PassingYear = new Date(scrutiny.Qualification.PassingYear) }
                  temp.push(scrutiny);
                }
              });
              this.application.AdhocScrutinies = [];
              this.application.AdhocScrutinies = temp;
            }
          }, err => {
            console.log(err);
          });
        }
      }, err => {
        console.log(err);
      });
    });
  }
  public getAdhocApplications() {
    this.loading = true;
    debugger;
    this._onlineAdhocService.getAdhocApplications(this.applicant.Id).subscribe((res: any) => {
      if (res) {
        this.applications = res;
        if (this.applications.length == 1) {
          if (this.applications[0].Designation_Id == 1320 || this.applications[0].Designation_Id == 802
            || this.applications[0].Designation_Id == 431 || this.applications[0].Designation_Id == 302
            || this.applications[0].Designation_Id == 2136 || this.applications[0].Designation_Id == 365
            || this.applications[0].Designation_Id == 368 || this.applications[0].Designation_Id == 369
            || this.applications[0].Designation_Id == 374 || this.applications[0].Designation_Id == 375
            || this.applications[0].Designation_Id == 382 || this.applications[0].Designation_Id == 383
            || this.applications[0].Designation_Id == 384 || this.applications[0].Designation_Id == 385
            || this.applications[0].Designation_Id == 387 || this.applications[0].Designation_Id == 390
            || this.applications[0].Designation_Id == 1594 || this.applications[0].Designation_Id == 1598
            || this.applications[0].Designation_Id == 362
          ) {
            this.showMeritList = true;
          } else {
            this.showMeritList = false;
          }
        } else {
          this.showMeritList = false;
        }
        this.applications.forEach(app => {
          if (app.Status_Id == 3) {
            this.rejectedApplication = true;
          }
        });
        this.loading = false;
      }
    }, err => {
      console.log(err);
    });
  }
  public getAdhocInterviewApplications() {
    this.loading = true;
    this._onlineAdhocService.getAdhocInterviewApplications(this.applicant.Id).subscribe((res: any) => {
      if (res) {
        this.interviewApplications = res;
        this.loading = false;
      }
    }, err => {
      console.log(err);
    });
  }
  public openInNewTab(link: string) {
    debugger;
    window.open(link, '_blank');
  }
  public selectApplication(item) {
    this.application = item;
    this.application.Grievance = {};
    this.application.Grievance.Application_Id = this.application.Id;
    this.getAdhocScrutiny();
  }
  public saveApplicationGrievance() {
    this._onlineAdhocService.saveApplicationGrievance(this.application.Grievance).subscribe((res) => {
      if (res) {
        this.application = {};
        this.router.navigate(['/adhoc/grievance']);
      }
    });
  }
  public getAdhocGrievanceUploads() {
    this._onlineAdhocService.getAdhocGrievanceUploads(this.applicant.Id).subscribe((res) => {
      if (res) {
        this.adhocGreivanceUploads = res;
      }
    });
  }
  getOfferLetterLink() {
    this.downloadingOfferLetter = true;
    this._onlineAdhocService.getDownloadLink('offer', this.cnic).subscribe((link) => {
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
    this._onlineAdhocService.getDownloadLink('acceptance', this.cnic).subscribe((link) => {
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
    this._onlineAdhocService.getJobDocumentsRequired(this.jobId).subscribe((res: any) => {
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
  public viewApplication(item: any) {
    this._cookieService.deleteAndSetCookie('cnicussradhocdesig', item.Designation_Id.toString());
    this._cookieService.deleteAndSetCookie('cnicussradhocapp', item.Id);
    this.router.navigate(['/adhoc/apply-now/11']);
  }
  public viewPreference(item: any) {
    this._cookieService.deleteAndSetCookie('cnicussradhocdesig', item.Designation_Id.toString());
    this._cookieService.deleteAndSetCookie('cnicussradhocapp', item.Id);
    this.router.navigate(['/adhoc/preferences/' + item.Designation_Id.toString()]);
  }
  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
