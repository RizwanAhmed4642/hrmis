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
import { OnlineAdmissionsService } from '../online-admissions.service';

@Component({
  selector: 'app-profile-entry',
  templateUrl: './profile-entry.component.html',
  styles: []
})
export class ProfileEntryComponent implements OnInit {
  @ViewChild('photoRef', { static: false }) public photoRef: any;
  private subscription: Subscription;
  public dropDowns: DropDownsHR = new DropDownsHR();
  public cnic: string = '';
  public cnicMask: string = "00000-0000000-0";
  public photoSrc = '';
  public photoFile: any[] = [];
  public services: any[] = [];
  public merit: any = {};
  public service: any = {};
  public profile: any;
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
    private _authenticationService: AuthenticationService, 
    private _onlineAdmissionsService: OnlineAdmissionsService
    ) { }

  ngOnInit() {
    this.merit = { Designation_Id: 302 };
    this.birthDateMax.setFullYear(this.birthDateMax.getFullYear() - 18);
    this.fetchParams();
    this.subscribeCNIC();
    this.getDomiciles();
    this.dropDowns.selectedFiltersModel.designation = { Name: 'Charge Nurse', Id: 302 };
  }
  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('id')) {
          this.merit.Id = +params['id'];
          this.fetchMeritInfo();
        } else {
          this.getDesignations();
        }
      }
    );
  }
  public fetchMeritInfo() {
    this._onlineAdmissionsService.getMeritById(this.merit.Id).subscribe((res: any) => {
      if (res) {
        this.merit = res;
        this.setValues();
        this.savingProfile = false;
      }
    }, err => {
      console.log(err);
    });
  }
  public setValues() {
    if (this.merit.DOB) {
      this.merit.DOB = new Date(this.merit.DOB);
    }
    this._rootService.getDomiciles().subscribe((res: any) => {
      if (res) {
        let domiciles = res;
        let domicile = domiciles.find(x => x.Id == this.merit.Domicile_Id);
        if (domicile) {
          this.dropDowns.selectedFiltersModel.domicile = { Id: this.merit.Domicile_Id, DistrictName: domicile.Name };
        }
      }
    },
      err => { this.handleError(err); }
    );

    if (this.merit.Religion_Id == 2) {
      this.dropDowns.selectedFiltersModel.religion = { Id: 2, Name: 'Non Muslim' };
    } else {
      this.dropDowns.selectedFiltersModel.religion = { Id: 1, Name: 'Muslim' };
    }
    this.getDesignations();
    this.photoSrc = 'https://hrmis.pshealthpunjab.gov.pk/Uploads/MeritPhotos/' + this.merit.CNIC + '_23.jpg?v=' + this.radnom;
    this.showMeritForm = true;
  }
  public onSubmit() {
    this.savingProfile = true;
    if (this.merit.DOB) {
      this.merit.DOB = this.merit.DOB.toDateString();
    }
    this.merit.MeritsActiveDesignationId = 58;
    this.merit.Designation_Id = 302;
    this.merit.Status = this.merit.Status ? this.merit.Status : 'New';
    /*  this.merit.Status = 1320;  */
    /*    this._onlineAdmissionsService.verifyMeritProfile(this.merit).subscribe((res: any) => {
        if (res && res.Id) {
          if (this.photoFile.length > 0) {
            this._onlineAdmissionsService.uploadMeritProfilePhoto(this.photoFile, res.CNIC).subscribe((res: any) => {
              if (res) {
                this.savingProfile = false;
                this.router.navigate(['/ppsc/letter']);
              }
            }, err => {
              this.handleError(err);
            });
          } else {
            this.savingProfile = false;
            this.router.navigate(['/ppsc/letter']);
          }
        }
      }, err => {
        this.handleError(err);
      }); */
    this._onlineAdmissionsService.saveProfile(this.merit).subscribe((res: any) => {
      if (res && res.Id) {
        window.location.reload();
       /*  this.merit.Id = res.Id;
        this.fetchMeritInfo(); */
      }
    }, err => {
      this.handleError(err);
    });

  }
  public addService() {
    this.services.push(this.service);
    this.service = {};
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
  private getDesignations = () => {
    this.dropDowns.designations = [];
    this.dropDowns.designationsData = [];
    this._rootService.getConsultantDesignations().subscribe((res: any) => {
      if (res && res.List) {
        this.dropDowns.designations = res.List;
        this.dropDowns.designationsData = this.dropDowns.designations.slice();

        if (this.merit && this.merit.Id) {
          let designation = this.dropDowns.designationsData.find(x => x.Id == this.merit.Designation_Id);
          if (designation) {
            this.dropDowns.selectedFiltersModel.designation = { Name: 'Charge Nurse', Id: 302 };
          }
        }
      }
    },
      err => { this.handleError(err); }
    );
  }
  public dropdownValueChanged = (value, filter) => {
    if (!value) {
      return;
    }
    if (filter == 'domicile') {
      this.merit.Domicile_Id = value.Id;
    }
    if (filter == 'religion') {
      this.merit.Religion_Id = value.Id;
    }
    if (filter == 'designation') {
      this.merit.Designation_Id = value.Id;
    }
  }

  public subscribeCNIC() {
    this.inputChange = new Subject();
    this.cnicSubscription = this.inputChange.pipe(debounceTime(400)).subscribe((query) => {

      if (this.merit.CNIC && this.merit.CNIC.length == 13) {
        let i = +this.merit.CNIC[this.merit.CNIC.length - 1];
        this.merit.Gender = (i % 2 == 0) ? 'Female' : 'Male';
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
