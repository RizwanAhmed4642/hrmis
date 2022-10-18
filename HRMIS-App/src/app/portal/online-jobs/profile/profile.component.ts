import { Component, OnInit, ViewChild } from '@angular/core';
import { ReportingRoutingModule } from '../../../modules/reporting/reporting-routing.module';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { Subject } from 'rxjs/Subject';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { RootService } from '../../../_services/root.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { OnlineJobsService } from '../online-jobs.service';
import { CookieService } from '../../../_services/cookie.service';
import * as moment from 'moment';
import { LocalService } from '../../../_services/local.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styles: []
})
export class ProfileComponent implements OnInit {
  @ViewChild('photoRef', { static: false }) public photoRef: any;
  private subscription: Subscription;
  public dropDowns: DropDownsHR = new DropDownsHR();
  public cnic: string = '';
  public cnicMask: string = "00000-0000000-0";
  public photoSrc = '';
  public photoFile: any[] = [];
  public services: any[] = [];
  public applicant: any = {};
  public job: any = {};
  public service: any = {};
  public meritActiveDesignation: any = {};
  public profile: any;
  public radnom: number = Math.random();
  public inputChange: Subject<any>;
  private cnicSubscription: Subscription;

  public ageError: boolean = false;
  public showMeritForm: boolean = false;
  public loadingCNIC: boolean = false;
  public isUploading: boolean = false;
  public savingProfile: boolean = false;
  public existingProfile: boolean = false;
  public birthDateMax: Date = new Date();
  public mobileMask: string = "0000-0000000";
  constructor(private route: ActivatedRoute, private _rootService: RootService,
    private _cookieService: CookieService,
    private router: Router,
    private _localService: LocalService,
    private _authenticationService: AuthenticationService, private _onlineJobsService: OnlineJobsService) { }

  ngOnInit() {
    this.birthDateMax.setFullYear(this.birthDateMax.getFullYear() - 18);
    this.fetchParams();
    this.subscribeCNIC();
    this.getDomiciles();
    this.getApplicant();
    this.getJob();
  }
  private getApplicant() {
    this._onlineJobsService.getApplicant(this.cnic).subscribe((res: any) => {
      if (res) {
        res.DOB = new Date(res.DOB);
        this.applicant = res;
        this.dropDowns.selectedFiltersModel.domicile = { DistrictName: this.applicant.DomicileName, Id: this.applicant.Domcile_Id };
      }
    }, err => {
      console.log(err);
    });
  }
  private getJob() {
    let designationId = this._localService.get('desigaplid');
    this._onlineJobsService.getJobByDesignation(designationId).subscribe((res: any) => {
      if (res) {
        this.job = res;
      }
    }, err2 => {
      console.log(err2);
    });
  }
  private fetchParams() {
    this.cnic = this._cookieService.getCookie('cnicussrphfmc');
    if (this.cnic) {
      this.applicant.CNIC = this.cnic;
      /*  this.fetchMeritInfo();
       this._rootService.getProfileByCNIC(this.cnic).subscribe((x) => {
         if (x) {
           this.profile = x;
           console.log(this.profile);
         }
       }, err => {
         console.log(err);
       }); */
    }
  }
  public checkD(): void {
    var years = moment().diff(this.applicant.DOB, 'years');
    console.log(years);

  }
  calculate_age(dob) {
    var diff_ms = Date.now() - dob.getTime();
    var age_dt = new Date(diff_ms);
    var age = Math.abs(age_dt.getUTCFullYear() - 1970);
    if (age > this.job.AgeLimit) {
      this.ageError = true;
    } else {
      this.ageError = false;
    }
  }
  public onSubmit() {
    this.savingProfile = true;
    if (this.applicant.DOB) {
      this.applicant.DOB = this.applicant.DOB.toDateString();
    }
    /* this.applicant.MeritsActiveDesignationId = 36;
    this.applicant.Status = 'New'; */
    /*  this.applicant.MeritsActiveDesignationId = 1320; */
    this._onlineJobsService.saveApplicant(this.applicant).subscribe((res: any) => {
      if (res && res.Id) {
        if (this.photoFile.length > 0) {
          this._onlineJobsService.uploadApplicantPhoto(this.photoFile, res.CNIC).subscribe((res2: any) => {
            this.savingProfile = false;
            if (res2) {
              this.router.navigate(['/job/document']);
            }
          }, err => {
            this.handleError(err);
          });
        } else {
          this.savingProfile = false;
          this.router.navigate(['/job/document']);
        }
      }
    }, err => {
      this.handleError(err);
    });
    /*  this._onlineJobsService.saveProfile(this.merit).subscribe((res: any) => {
       if (res && res.Id) {
         this.merit = {};
         this.savingProfile = false;
       }
     }, err => {
       this.handleError(err);
     }); */

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
      /*  this.loadingCNIC = true;
       this.existingProfile = null;
       if (!query) { this.loadingCNIC = false; return; }
       let cnic: string = query as string;
       cnic = cnic.replace(' ', '');
       if (cnic.length != 13) { this.loadingCNIC = false; return; }
       if (cnic == this.oldCNIC) { this.loadingCNIC = false; return; } */
      /*   this._profileService.getProfile(cnic).subscribe(
          res => {
            console.log(res);
            if (res) {
              this.existingProfile = res as Profile;
              console.log(this.existingProfile.CNIC);
            }
            this.loadingCNIC = false;
          },
          err => {
            this.handleError(err);
          }
        ); */
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
    }
  }

  public uploadBtn(filter: string) {
    if (filter == 'pic') {
      this.photoRef.nativeElement.click();
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
