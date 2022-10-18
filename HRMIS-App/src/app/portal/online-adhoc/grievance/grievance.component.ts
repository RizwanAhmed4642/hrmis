import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RootService } from '../../../_services/root.service';
import { CookieService } from '../../../_services/cookie.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { Subscription } from 'rxjs/Subscription';
import { LocalService } from '../../../_services/local.service';
import { OnlineAdhocService } from '../online-adhoc.service';

@Component({
  selector: 'grievance',
  templateUrl: './grievance.component.html'
})
export class GrievanceComponent implements OnInit, OnDestroy {

  @ViewChild('photoRef', { static: false }) public photoRef: any;

  public cnic: string = '';
  public photoSrc = '';
  public applicant: any = {};
  public application: any = {};
  public grievances: any = [];
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
  public applicationId: number = 0;
  public isUploading: boolean = false;
  public offerLetterDownloaded: boolean = false;
  public acceptanceLetterDownloaded: boolean = false;
  public acceptanceUploaded: boolean = false;
  public addingExperience: boolean = false;
  public loading: boolean = false;
  public disableGrievance: boolean = false;
  public uploadingFile: boolean = false;
  public uploadingFileError: boolean = false;

  public downloadingOfferLetter: boolean = false;
  public downloadingAcceptanceLetter: boolean = false;
  public uploadingAcceptanceLetter: boolean = false;

  public currentDate: Date;
  public endDate: any = '12/16/2021 23:59:59';

  public photoFile: any[] = [];
  public houseJobFile: any[] = [];
  public applicationAttachments: any[] = [];
  private subscription: Subscription;
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
    this.fetchParams();
    this.currentDate = new Date();
    this.dateCompare(this.currentDate, this.endDate);
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

  // dateCompare("6/11/2020", "7/8/2019")
  // dateCompare("01/01/2021", "01/01/2021")


  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('appId')) {
          let param = params['appId'];
          if (+param) {
            this.applicationId = +params['appId'] as number;
          }
        } else {
          this.getApplicant();
        }
        this.getApplicant();
      }
    );
  }
  private getApplicant() {
    this._onlineAdhocService.getApplicant(this.cnic).subscribe((res3: any) => {
      if (res3) {
        this.applicant = res3;
        this.getApplicationGrievances();
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
        /*  var reader = new FileReader();
         reader.onload = ((event: any) => {
           this.photoSrc = event.target.result;
         }).bind(this);
         reader.readAsDataURL(event.target.files[0]); */
      }
    }
  }

  public getApplicationGrievances() {
    this.loading = true;
    this._onlineAdhocService.getApplicationGrievances(this.applicant.Id).subscribe((res: any) => {
      if (res) {
        this.grievances = res;
        console.log('grv: ', res);
        this.loading = false;
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
  public selectFile(event, document: any): void {
    let inputValue = event.target;
    let applicationAttachment: any = {};
    applicationAttachment.JobDocument_Id = document.Id;
    applicationAttachment.DocumentName = document.Name;
    applicationAttachment.files = inputValue.files;
    this.applicationAttachments.push(applicationAttachment);
    this.documents.find(x => x.Id == document.Id).attached = true;
  }
  public removeGrievance(Id) {
    this._onlineAdhocService.removeGrievance(Id).subscribe((res: any) => {
      if (res) {
        this.getApplicationGrievances();
      }
    }, err => {
      console.log(err);
    });
  }
  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
