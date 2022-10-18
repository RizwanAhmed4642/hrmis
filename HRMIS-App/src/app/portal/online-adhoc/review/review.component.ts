import { Component, OnInit, ViewChild } from '@angular/core';
import { ReportingRoutingModule } from '../../../modules/reporting/reporting-routing.module';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { Subject } from 'rxjs/Subject';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { RootService } from '../../../_services/root.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { OnlineAdhocService } from '../online-adhoc.service';
import { CookieService } from '../../../_services/cookie.service';

@Component({
  selector: 'app-review',
  templateUrl: './review.component.html',
  styles: []
})
export class ReviewComponent implements OnInit {
  @ViewChild('photoRef', { static: false }) public photoRef: any;
  private subscription: Subscription;
  public dropDowns: DropDownsHR = new DropDownsHR();
  public cnic: string = '';
  public cnicMask: string = "00000-0000000-0";
  public photoSrc = '';
  public hfmisCode = '0';
  public photoFile: any[] = [];
  public selectedMeritPreferences: any[] = [];
  public services: any[] = [];
  public merit: any = {};
  public service: any = {};
  public profile: any;
  public radnom: number = Math.random();
  public inputChange: Subject<any>;
  private cnicSubscription: Subscription;
  public applicant: any = {};
  public applications: any[] = [];
  public showMeritForm: boolean = false;
  public downloadingOfferLetter: boolean = false;
  public loadingPreferences: boolean = false;
  public loadingCNIC: boolean = false;
  public isUploading: boolean = false;
  public savingProfile: boolean = false;
  public existingProfile: boolean = false;
  public birthDateMax: Date = new Date();
  constructor(private route: ActivatedRoute, private _rootService: RootService,
    private _cookieService: CookieService,
    private router: Router,
    private _authenticationService: AuthenticationService, 
    private _onlineAdhocService: OnlineAdhocService
    ) { }

  ngOnInit() {
    this.cnic = this._cookieService.getCookie('cnicussrphfmc');
    this.getApplicant();
  }
  private getApplicant() {
    this._onlineAdhocService.getApplicant(this.cnic).subscribe((res3: any) => {
      if (res3) {
        this.applicant = res3;
        this.photoSrc = 'https://hrmis.pshealthpunjab.gov.pk/Uploads/PHFMCApplicantsPhoto/' + this.applicant.CNIC + '_23.jpg?v=' + this.radnom;

        this.getJobApplications();
      }

    }, err2 => {
      console.log(err2);
    });
  }

  public getJobApplications() {
    this._onlineAdhocService.getJobApplications(this.applicant.Id, this.hfmisCode).subscribe((res: any) => {
      if (res) {
        this.applications = res;
        /*   this.getPreferences(); */
      }
    }, err => {
      console.log(err);
    });
  }
  public fetchMeritInfo() {
    this._onlineAdhocService.getMeritProfile(this.cnic).subscribe((res: any) => {
      if (res) {
        this.merit = res;
        this.photoSrc = 'https://hrmis.pshealthpunjab.gov.pk/Uploads/MeritPhotos/' + this.merit.CNIC + '_23.jpg?v=' + this.radnom;
        this.getPreferences();
      }
    }, err => {
      console.log(err);
    });
  }
  public getPreferences() {
    this.loadingPreferences = true;
    this._onlineAdhocService.getPreferences(this.merit.Id).subscribe((response: any) => {
      if (response) {
        this.selectedMeritPreferences = response;
        console.log(this.selectedMeritPreferences);
        this.loadingPreferences = false;
      }
    }, err => {
      console.log(err);
      this.loadingPreferences = false;
    });
  }

  getOfferLetterLink() {
    this.downloadingOfferLetter = true;
    this._onlineAdhocService.getDownloadLinkById(this.merit.Id).subscribe((link) => {
      this.downloadingOfferLetter = false;
      if (link) {
        window.open('' + link, '_blank');
      }
    });
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
  }
}
