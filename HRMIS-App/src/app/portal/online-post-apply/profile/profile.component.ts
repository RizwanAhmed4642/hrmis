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
import { OnlinePostApplyService } from '../online-post-apply.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styles: []
})
export class ProfileComponent implements OnInit {
  @ViewChild('photoRef', {static: false}) public photoRef: any;
  private subscription: Subscription;
  public dropDowns: DropDownsHR = new DropDownsHR();
  public cnic: string = '';
  public cnicMask: string = "00000-0000000-0";
  public photoSrc = '';
  public photoFile: any[] = [];
  public profile: any = {};
  public promotionApply: any = {};
  public radnom: number = Math.random();
  public inputChange: Subject<any>;
  private cnicSubscription: Subscription;

  public showMeritForm: boolean = false;
  public loadingCNIC: boolean = false;
  public isUploading: boolean = false;
  public savingProfile: boolean = false;
  public existingProfile: boolean = false;
  public birthDateMax: Date = new Date();
  constructor(private route: ActivatedRoute, private _rootService: RootService,
    private _cookieService: CookieService,
    private router: Router,
    private _authenticationService: AuthenticationService, private _onlinePostApplyService: OnlinePostApplyService) { }

  ngOnInit() {
    this.birthDateMax.setFullYear(this.birthDateMax.getFullYear() - 18);
    this.fetchParams();
    this.subscribeCNIC();
    this.getDomiciles();
  }
  private fetchParams() {
    this.cnic = this._cookieService.getCookie('cnicussrpublic');
    if (this.cnic) {
      this.fetchMeritInfo();
    }
  }
  public fetchMeritInfo() {
    this._onlinePostApplyService.getPromotionApplyData(this.cnic).subscribe((res: any) => {
      if (res && res.Profile && res.PromotionApply) {
        this.profile = res.Profile;
        this.promotionApply = res.PromotionApply;
        this.setValues();
      }
    }, err => {
      console.log(err);
    });
  }
  public setValues() {
    if (this.profile.DateOfBirth) {
      this.profile.DateOfBirth = new Date(this.profile.DateOfBirth);
    }
    this._rootService.getDomiciles().subscribe((res: any) => {
      if (res) {
        let domiciles = res;
        let domicile = domiciles.find(x => x.Id == this.profile.Domicile_Id);
        if (domicile) {
          this.dropDowns.selectedFiltersModel.domicile = { Id: this.profile.Domicile_Id, DistrictName: domicile.Name };
        }
      }
    },
      err => { this.handleError(err); }
    );

    if (this.profile.Religion_Id == 2) {
      this.dropDowns.selectedFiltersModel.religion = { Id: 2, Name: 'Non Muslim' };
    } else {
      this.dropDowns.selectedFiltersModel.religion = { Id: 1, Name: 'Muslim' };
    }
    this.photoSrc = 'https://hrmis.pshealthpunjab.gov.pk/Uploads/ProfilePhotos/' + this.profile.CNIC + '_23.jpg?v=' + this.radnom;
    this.showMeritForm = true;
  }
  public onSubmit() {
    this.savingProfile = true;
    this._onlinePostApplyService.updateSMOSatus(this.promotionApply).subscribe((res: any) => {
      if (res && res.Status == 'ProfileReviewed') {
        this.router.navigate(['/smo/submit']);
      }
      this.savingProfile = false;
    }, err => {
      this.handleError(err);
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
      this.profile.Domicile_Id = value.Id;
    }
    if (filter == 'religion') {
      this.profile.Religion_Id = value.Id;
    }
  }

  public subscribeCNIC() {
    this.inputChange = new Subject();
    this.cnicSubscription = this.inputChange.pipe(debounceTime(400)).subscribe((query) => {
      if (this.profile.CNIC && this.profile.CNIC.length == 13) {
        let i = +this.profile.CNIC[this.profile.CNIC.length - 1];
        this.profile.Gender = (i % 2 == 0) ? 'Female' : 'Male';
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
  }
}
