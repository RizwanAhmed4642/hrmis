import { Component, OnInit, ViewChild } from '@angular/core';
import { ReportingRoutingModule } from '../../../modules/reporting/reporting-routing.module';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { Subject } from 'rxjs/Subject';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { RootService } from '../../../_services/root.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { CookieService } from '../../../_services/cookie.service';
import * as moment from 'moment';
import { LocalService } from '../../../_services/local.service';
import { OnlineAdhocService } from '../online-adhoc.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styles: []
})
export class ProfileComponent implements OnInit {
  @ViewChild('photoRef', { static: false }) public photoRef: any;
  @ViewChild('photoRefCnic', { static: false }) public photoRefCnic: any;
  @ViewChild('photoRefDomicile', { static: false }) public photoRefDomicile: any;
  @ViewChild('photoRefPmdc', { static: false }) public photoRefPmdc: any;
  @ViewChild('photoRefHifz', { static: false }) public photoRefHifz: any;
  private subscription: Subscription;
  public dropDowns: DropDownsHR = new DropDownsHR();
  public cnic: string = '';
  public designationId: number = 0;
  public cnicMask: string = "00000-0000000-0";
  public photoSrc = '';
  public photoSrcCnic = '';
  public photoSrcDomicile = '';
  public photoSrcPmdc = '';
  public photoSrcHifz = '';
  public photoFile: any[] = [];
  public pmdcFile: any[] = [];
  public cnicFile: any[] = [];
  public domicileFile: any[] = [];
  public hifzFile: any[] = [];
  public services: any[] = [];
  public applicant: any = {};
  public job: any = {};
  public service: any = {};
  public application: any = {};
  public fileUpload: any = { photo: false, pmdc: false, domicile: false, cnic: false, hifz: false };
  public meritActiveDesignation: any = {};
  public profile: any;
  public radnom: number = Math.random();
  public inputChange: Subject<any>;
  private cnicSubscription: Subscription;
  public urdu: any = {
    info: `اپنے کوائف کا درست اندراج کیجئے اور مطلوبہ کاغذات اپلوڈ کرنے کے بعد سیو پروسیڈ کا بٹن دبائیں۔`,
    infoeng: 'Enter your data correctly and upload the required documents then press the Save & Proceed button'
  };
  public ageError: boolean = false;
  public basicInfoSaved: boolean = false;
  public allDocsSaved: boolean = false;
  public showMeritForm: boolean = false;
  public loadingCNIC: boolean = false;
  public isUploading: boolean = false;
  public savingProfile: boolean = false;
  public proceeding: boolean = false;
  public existingProfile: boolean = false;
  public birthDateMax: Date = new Date();
  public birthDateMin: Date = new Date();
  public mobileMask: string = "0000-0000000";
  constructor(private route: ActivatedRoute, private _rootService: RootService,
    private _cookieService: CookieService,
    private router: Router,
    private _localService: LocalService,
    private _authenticationService: AuthenticationService,
    private _onlineAdhocService: OnlineAdhocService
  ) { }

  ngOnInit() {
    this.birthDateMax.setFullYear(this.birthDateMax.getFullYear() - 18);
    this.birthDateMin.setFullYear(this.birthDateMin.getFullYear() - 60);
    this.fetchParams();
    this.subscribeCNIC();
    this.getDomiciles();
    this.getJob();
  }
  private getApplicant() {
    this._onlineAdhocService.getApplicant(this.cnic).subscribe((res: any) => {
      if (res) {
        res.DOB = new Date(res.DOB);
        res.PMDCRegDate = new Date(res.PMDCRegDate);
        res.PMDCValidUpto = new Date(res.PMDCValidUpto);
        this.applicant = res;
        this.getAdhocApplicationByDesig();
        this.dropDowns.selectedFiltersModel.domicile = { DistrictName: this.applicant.DomicileName, Id: this.applicant.Domcile_Id };
        this.basicInfoSaved = true;
        if (this.applicant.PMDCDoc) {
          this.photoSrcPmdc = '';
          this.fileUpload.pmdc = false;
          this.allDocsSaved = true;
        } else {
          this.allDocsSaved = false;
        }
        if (this.applicant.DomicileDoc) {
          this.photoSrcDomicile = '';
          this.fileUpload.domicile = false;
          this.allDocsSaved = true;
        } else {
          this.allDocsSaved = false;
        }
        if (this.applicant.CNICDoc) {
          this.photoSrcCnic = '';
          this.fileUpload.cnic = false;
          this.allDocsSaved = true;
        } else {
          this.allDocsSaved = false;
        }
        if (this.applicant.ProfilePic) {
          this.photoSrc = '';
          this.fileUpload.photo = false;
          this.allDocsSaved = true;
        } else {
          this.allDocsSaved = false;
        }
        if (this.applicant.HifzDocument) {
          this.photoSrcHifz = '';
          this.fileUpload.hifz = false;
          this.allDocsSaved = true;
        } else {
          this.allDocsSaved = false;
        }
      }
    }, err => {
      console.log(err);
    });
  }
  private getAdhocApplicationByDesig() {
    this._onlineAdhocService.getAdhocApplicationByDesig(this.applicant.Id, this.designationId).subscribe((res: any) => {
      if (res) {
        this._cookieService.deleteAndSetCookie('cnicussradhocapp', res.Id);
      }
    }, err => {
      console.log(err);
    });
  }
  private getJob() {
    let designationId = this._localService.get('desigaplid');
    this._onlineAdhocService.getJobByDesignation(designationId).subscribe((res: any) => {
      if (res) {
        this.job = res;
      }
    }, err2 => {
      console.log(err2);
    });
  }
  private fetchParams() {
    this.cnic = this._cookieService.getCookie('cnicussradhoc');
    this.designationId = +this._cookieService.getCookie('cnicussradhocdesig');
    this.getApplicant();
    if (this.cnic) {
      this.applicant.CNIC = this.cnic;
    }
  }
  public checkD(): void {
    var years = moment().diff(this.applicant.DOB, 'years');
    console.log(years);

  }
  calculate_age(dob) {
    var diff_ms = Date.now() - dob.getTime();
    var age_dt = new Date(diff_ms);
    this.applicant.Age = Math.abs(age_dt.getUTCFullYear() - 1970);
    /* if (this.applicant.Age > this.job.AgeLimit) {
      this.ageError = true;
    } else {
      this.ageError = false;
    } */
  }
  public getAge(dateString) {
    let d1 = Date.now(), d2 = new Date(dateString).getTime();
    let ageInMilliseconds: number = d1 - d2;
    this.applicant.Age = Math.floor(ageInMilliseconds / 1000 / 60 / 60 / 24 / 365); // convert to years
  }
  public uploadFile(filter: string) {
    if (filter == 'photo' && this.photoFile.length > 0) {
      this.fileUpload.photo = true;
      this._onlineAdhocService.uploadApplicantPhoto(this.photoFile, this.applicant.CNIC).subscribe((res: any) => {
        if (res) {
          this.getApplicant();
        }
      }, err => {
        this.handleError(err);
        this.fileUpload.photo = false;
      });
    }
    if (filter == 'pmdc' && this.pmdcFile.length > 0) {
      this.fileUpload.pmdc = true;
      this._onlineAdhocService.uploadApplicantPMDC(this.pmdcFile, this.applicant.CNIC).subscribe((res: any) => {
        if (res) {
          this.getApplicant();
        }
      }, err => {
        this.handleError(err);
        this.fileUpload.pmdc = false;
      });
    }
    if (filter == 'cnic' && this.cnicFile.length > 0) {
      this.fileUpload.cnic = true;
      this._onlineAdhocService.uploadApplicantCNIC(this.cnicFile, this.applicant.CNIC).subscribe((res: any) => {
        if (res) {
          this.getApplicant();
        }
      }, err => {
        this.handleError(err);
        this.fileUpload.cnic = false;
      });
    }
    if (filter == 'domicile' && this.domicileFile.length > 0) {
      this.fileUpload.domicile = true;
      this._onlineAdhocService.uploadApplicantDomicile(this.domicileFile, this.applicant.CNIC).subscribe((res: any) => {
        if (res) {
          this.getApplicant();
        }
      }, err => {
        this.handleError(err);
        this.fileUpload.domicile = false;
      });
    }
    if (filter == 'hifz' && this.hifzFile.length > 0) {
      this.fileUpload.hifz = true;
      this._onlineAdhocService.uploadApplicantHifz(this.hifzFile, this.applicant.CNIC).subscribe((res4: any) => {
        this.getApplicant();
      }, err => {
        this.handleError(err);
      });
    }
  }
  public onSubmit() {
    //this.router.navigate(['/adhoc/qualification']);
    this.savingProfile = true;
    if (this.applicant.DOB) {
      this.applicant.DOB = this.applicant.DOB.toDateString();
    }

    if (this.applicant.PMDCRegDate) {
      this.applicant.PMDCRegDate = this.applicant.PMDCRegDate.toDateString();
    }

    if (this.applicant.PMDCValidUpto) {
      this.applicant.PMDCValidUpto = this.applicant.PMDCValidUpto.toDateString();
    }
    this._onlineAdhocService.saveApplicant(this.applicant).subscribe((res: any) => {
      if (res && res.Id) {
        this.basicInfoSaved = true;
        let fileCheck: number = 0;
        /*  if (this.photoFile.length > 0) {
           this._onlineAdhocService.uploadApplicantPhoto(this.photoFile, res.CNIC).subscribe((res2: any) => {
             fileCheck += 1;
           }, err => {
             this.handleError(err);
           });
         } else {
           fileCheck += 1;
         }
         if (this.hifzFile.length > 0) {
           this._onlineAdhocService.uploadApplicantHifz(this.hifzFile, res.Id).subscribe((res4: any) => {
             fileCheck += 1;
           }, err => {
             this.handleError(err);
           });
         } else {
           fileCheck += 1;
         }
         if (this.pmdcFile.length > 0) {
           this._onlineAdhocService.uploadApplicantPMDC(this.pmdcFile, res.Id).subscribe((res3: any) => {
             fileCheck += 1;
           }, err => {
             this.handleError(err);
           });
         } else {
           fileCheck += 1;
         }
         if (this.domicileFile.length > 0) {
           this._onlineAdhocService.uploadApplicantDomicile(this.domicileFile, res.Id).subscribe((res3: any) => {
             fileCheck += 1;
           }, err => {
             this.handleError(err);
           });
         } else {
           fileCheck += 1;
         }
         if (this.cnicFile.length > 0) {
           this._onlineAdhocService.uploadApplicantCNIC(this.cnicFile, res.Id).subscribe((res3: any) => {
             fileCheck += 1;
           }, err => {
             this.handleError(err);
           });
         } else {
           fileCheck += 1;
         } */
        /*  while (fileCheck != 3) {
           if (fileCheck == 3) {
             this.savingProfile = false;
             this.router.navigate(['/adhoc/qualification']);
           }
         } */
        this.applicant.Id = res.Id;
        this.application.Applicant_Id = res.Id;
        this.application.Designation_Id = +this._cookieService.getCookie('cnicussradhocdesig');
        this._onlineAdhocService.saveAdhocApplication(this.application).subscribe((res3: any) => {
          if (res3 && res3.Id) {
            this._cookieService.deleteAndSetCookie('cnicussradhocapp', res3.Id);
            this.savingProfile = false;
            //this.router.navigate(['/adhoc/qualification']);
          }
        }, err2 => {
          this.savingProfile = false;
          console.log(err2);
        });

      }
    }, err => {
      this.handleError(err);
    });
  }
  public proceed() {
    this.proceeding = true;
    this.application.Applicant_Id = this.applicant.Id;
    this.application.Designation_Id = +this._cookieService.getCookie('cnicussradhocdesig');
    this._onlineAdhocService.saveAdhocApplication(this.application).subscribe((res3: any) => {
      if (res3 && res3.Id) {
        this._cookieService.deleteAndSetCookie('cnicussradhocapp', res3.Id);
        this.savingProfile = false;
        this.proceeding = false;
        this.router.navigate(['/adhoc/qualification']);
      }
    }, err2 => {
      this.savingProfile = false;
      console.log(err2);
    });
  }
  private getDomiciles = () => {
    this.dropDowns.domicile = [];
    this.dropDowns.domicileData = [];
    this._rootService.getDomiciles().subscribe((res: any) => {
      this.dropDowns.domicile = res;
      this.dropDowns.domicileData = this.dropDowns.domicile.slice();
    },
      err => { this.handleError(err); }
    );
  }
  public dropdownValueChanged = (value, filter) => {
    if (!value) {
      return;
    }
    if (filter == 'domicile') {
      this.applicant.Domicile_Id = value.Id;
    }
    if (filter == 'religion') {
      this.applicant.Religion_Id = value.Id;
    }
  }

  public subscribeCNIC() {
    this.inputChange = new Subject();
    this.cnicSubscription = this.inputChange.pipe(debounceTime(400)).subscribe((query) => {
      if (this.applicant.CNIC && this.applicant.CNIC.length == 13) {
        let i = +this.applicant.CNIC[this.applicant.CNIC.length - 1];
        this.applicant.Gender = (i % 2 == 0) ? 'Female' : 'Male';
      }
    });
  }
  public readUrl(event: any, filter: string) {
    if (event.target.files && event.target.files[0]) {
      if (filter == 'pic') {
        this.photoFile = [];
        let inputValue = event.target;
        this.photoFile = inputValue.files;
        var reader = new FileReader();
        reader.onload = ((event: any) => {
          this.photoSrc = event.target.result;
        }).bind(this);
        reader.readAsDataURL(event.target.files[0]);
      }
      if (filter == 'pmdc') {
        this.pmdcFile = [];
        let inputValue = event.target;
        this.pmdcFile = inputValue.files;
        var reader = new FileReader();
        reader.onload = ((event: any) => {
          this.photoSrcPmdc = event.target.result;
        }).bind(this);
        reader.readAsDataURL(event.target.files[0]);
      }

      if (filter == 'hifz') {
        this.hifzFile = [];
        let inputValue = event.target;
        this.hifzFile = inputValue.files;
        var reader = new FileReader();
        reader.onload = ((event: any) => {
          this.photoSrcHifz = event.target.result;
        }).bind(this);
        reader.readAsDataURL(event.target.files[0]);
      }

      if (filter == 'domicile') {
        this.domicileFile = [];
        let inputValue = event.target;
        this.domicileFile = inputValue.files;
        var reader = new FileReader();
        reader.onload = ((event: any) => {
          this.photoSrcDomicile = event.target.result;
        }).bind(this);
        reader.readAsDataURL(event.target.files[0]);
      }

      if (filter == 'cnic') {
        this.cnicFile = [];
        let inputValue = event.target;
        this.cnicFile = inputValue.files;
        var reader = new FileReader();
        reader.onload = ((event: any) => {
          this.photoSrcCnic = event.target.result;
        }).bind(this);
        reader.readAsDataURL(event.target.files[0]);
      }
    }
  }
  public uploadBtn(filter: string) {
    if (filter == 'pic') {
      this.photoRef.nativeElement.click();
    }
    if (filter == 'cnic') {
      this.photoRefCnic.nativeElement.click();
    }
    if (filter == 'domicile') {
      this.photoRefDomicile.nativeElement.click();
    }
    if (filter == 'pmdc') {
      this.photoRefPmdc.nativeElement.click();
    }
    if (filter == 'hifz') {
      this.photoRefHifz.nativeElement.click();
    }
  }
  public dashifyCNIC(cnic: string) {
    if (!cnic) {
      return;
    }
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
  private handleError(err: any) {
    if (err.status == 403) {
      this._authenticationService.logout();
    }
    this.savingProfile = false;
  }
}
