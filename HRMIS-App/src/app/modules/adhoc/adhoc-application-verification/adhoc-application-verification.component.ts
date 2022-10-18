import { Observable } from "rxjs/Observable";
import {
  Component,
  OnInit,
  Inject,
  ViewChild,
  NgZone,
  OnDestroy
} from "@angular/core";
import { Validators, FormBuilder, FormGroup } from "@angular/forms";
import { AuthenticationService } from "../../../_services/authentication.service";
import {
  GridDataResult,
  PageChangeEvent,
  GridComponent
} from "@progress/kendo-angular-grid";
import {
  State,
  process,
  SortDescriptor,
  orderBy
} from "@progress/kendo-data-query";
import { NotificationService } from "../../../_services/notification.service";

import { KGridHelper } from "../../../_helpers/k-grid.class";
import { take } from "rxjs/operators/take";
import { DropDownsHR } from "../../../_helpers/dropdowns.class";

import { from } from "rxjs/observable/from";
import { delay } from "rxjs/operators/delay";
import { switchMap } from "rxjs/operators/switchMap";
import { map } from "rxjs/operators/map";
import { Subscription } from "rxjs/Subscription";
import { cstmdummyActiveStatus } from "../../../_models/cstmdummydata";
import { RootService } from "../../../_services/root.service";
import { Subject } from "rxjs/Subject";
import { debounceTime } from "rxjs/operators/debounceTime";
import { AdhocService } from "../adhoc.service";
//import { ActivatedRoute } from '@angular/router';
//import { LocalService } from '../../../_services/local.service';

@Component({
  selector: "app-adhoc-application-verification",
  templateUrl: "./adhoc-application-verification.component.html",
  styleUrls: ['./adhoc-application-verification.component.scss']
})
export class AdhocApplicationVerificationComponent implements OnInit, OnDestroy {
  // @ViewChild("grid") public grid: GridComponent;
  @ViewChild('photoRef', { static: false }) public photoRef: any;
  public kGrid: KGridHelper = new KGridHelper();
  public inputChange: Subject<any>;
  public hfOpens: any[] = [];
  public vpProfileStatus: any[] = [];
  public hfsList: any[] = [];
  public jobHFs: any[] = [];
  public documents: any[] = [];
  public jobApplications: any[] = [];
  public user: any = {};
  public job: any = {};
  public jobHF: any = {};
  public jobDocument: any = {};
  public reason: any = {};
  public applicantLog: any = {};
  public selectedApplicant: any = {};
  public jobDocuments: any[] = [];
  public jobApplicants: any[] = [];
  public dateNow: string = '';
  public hfName: string = '';
  public loading: boolean = false;
  public saving: boolean = false;
  public showHF: boolean = true;
  public decision: string = '';
  private inputChangeSubscription: Subscription;
  public scrutinyComplete: boolean = false;
  public scrutinyDonwloaded: boolean = false;
  public scrutinyUpDonwloaded: boolean = false;
  public searchingHfs: boolean = false;
  public preferencesWindow: boolean = false;
  public searchEvent = new Subject<any>();
  public searchSubcription: Subscription;
  public dropDowns: DropDownsHR = new DropDownsHR();
  public filters: any = {};
  public degrees: any = { One: 0, Two: 0, Three: 0 };
  public jobVacancy: any = {};
  public ApplicantInfo: any = {};
  public hfs: any[] = [];
  public hfsOrigional: any[] = [];
  public rejectReasons: any[] = [
    { Id: 1, Reason: 'PMDC document not valid' },
    { Id: 2, Reason: 'Hafiz-e-Quran certificate not valid' },
    { Id: 3, Reason: 'Qualification incomplete' },
    { Id: 4, Reason: 'Experience incomplete' },
  ];
  public designations: any[] = [];
  public designationsOrigional: any[] = [];
  constructor(
    private _adhocService: AdhocService,
    public _notificationService: NotificationService,
    public _rootService: RootService,
    private _authenticationService: AuthenticationService,
    private ngZone: NgZone
  ) { }

  public ngOnInit(): void {
    this.user = this._authenticationService.getUser();
    let today = new Date();
    let dd: any = today.getDate(), mm: any = today.getMonth() + 1, yyyy = today.getFullYear();
    if (dd < 10) dd = '0' + dd; if (mm < 10) mm = '0' + mm;
    this.dateNow = dd + '/' + mm + '/' + yyyy;

    this.kGrid.multiple = true;
    this.kGrid.loading = false;
    this.kGrid.pageSize = 100050;
    this.kGrid.pageSizes = [50, 100, 200, 300, 500, 1000];
    this.filters.Status_Id = 1;
    this.subscribeInputChange();
    this.getAdhocVerificationData();
    this.getAdhocJobs();
  }
  public filterQualifications(length: number) {
    let temp = [];
    this.kGrid.data = [];
    this.kGrid.dataOrigional.forEach(element => {
      if (element.pmcQualification && element.pmcQualification.length == length) {
        temp.push(element);
        console.log(element);
      }
    });
    this.kGrid.data = temp;
    this.kGrid.totalRecords = this.kGrid.data.length;
    this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords };
  }
  private subscribeInputChange() {
    this.inputChange = new Subject();
    this.inputChangeSubscription = this.inputChange.pipe(debounceTime(300)).subscribe((query) => {
      this.filters.query = query;
      this.getAdhocVerificationData();
    });
  }
  public getAdhocVerificationData = () => {
    this.kGrid.loading = true;
    this.kGrid.data = [];
    let obj = {
      Skip: this.kGrid.skip,
      PageSize: this.kGrid.pageSize,
      applicantId: 0,
      hfmisCode: this.user.HfmisCode,
      Query: this.filters.query,
      Designation_Id: this.filters.Designation_Id,
      Status_Id: this.filters.Status_Id,
    };
    this._adhocService.getAdhocVerificationData(obj).subscribe((res: any) => {
      if (res) {
        this.kGrid.dataOrigional = res.List;
        this.kGrid.data = this.kGrid.dataOrigional;
        this.kGrid.totalRecords = res.Count;
        this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords };
        this.kGrid.loading = false;
        this.degrees.One = 0;
        this.degrees.Two = 0;
        this.degrees.Three = 0;

        if (this.filters.Status_Id == 3 || this.filters.Status_Id == 5) {
          this.kGrid.dataOrigional.forEach(element => {
            /* this._adhocService.verifyPMC(element.PMDCNumber).subscribe((res) => {
              element.pmcVerification = res;
            }, err => {
              console.log(err);
            }); */
            this._adhocService.getAdhocApplicantPMC(element.Id).subscribe((r: any) => {
              if (r) {
                element.pmcVerification = r.AdhocApplicantPMC;
                element.pmcQualification = r.Qualifications;
                if (element.pmcQualification && element.pmcQualification.length == 1) {
                  this.degrees.One++;
                } else if (element.pmcQualification && element.pmcQualification.length == 2) {
                  this.degrees.Two++;
                } else if (element.pmcQualification && element.pmcQualification.length == 3) {
                  this.degrees.Three++;
                }
              }
              else if (!r) {
                this._adhocService.verifyPMCGetQualifications(element.PMDCNumber).subscribe((res) => {
                  element.pmcQualification = res;
                  if (element.pmcQualification && element.pmcQualification.data) {
                    let obj: any = {
                      AdhocApplicantPMC: {
                        Applicant_Id: element.Id,
                        Name: element.pmcQualification.data.Name,
                        FatherName: element.pmcQualification.data.FatherName,
                        Gender: element.pmcQualification.data.Gender,
                        RegistrationDate: new Date(element.pmcQualification.data.RegistrationDate).toDateString(),
                        RegistrationNo: element.pmcQualification.data.RegistrationNo,
                        RegistrationType: element.pmcQualification.data.RegistrationType,
                        Status: element.pmcQualification.data.Status,
                        ValidUpto: new Date(element.pmcQualification.data.ValidUpto).toDateString()
                      },
                      Qualifications: element.pmcQualification.data.Qualifications
                    };
                    if (element.pmcQualification && element.pmcQualification.data.Qualifications.length == 1) {
                      this.degrees.One++;
                    } else if (element.pmcQualification && element.pmcQualification.data.Qualifications.length == 2) {
                      this.degrees.Two++;
                    } else if (element.pmcQualification && element.pmcQualification.data.Qualifications.length == 3) {
                      this.degrees.Three++;
                    }
                    this._adhocService.saveAdhocApplicantPMC(obj).subscribe((r) => {
                     
                    }, err => {
                      console.log(err);
                    });
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

      }
    },
      err => {
        console.log(err);
        this.loading = false;
      }
    );
  }

  public changeData = (statusId: number) => {
    this.filters.Status_Id = statusId;
    this.getAdhocVerificationData();
  }
  private getAdhocJobs = () => {
    this._adhocService.getAdhocJobs().subscribe((res: any) => {
      if (res) {
        this.dropDowns.adhocJobs = res;
        this.dropDowns.adhocJobsData = this.dropDowns.adhocJobs;
      }
    },
      err => {
        console.log(err);
        this.loading = false;
      }
    );
  }
  public verifyHifzPosition = (item: any, type: number, status: number) => {
    item.saving = true;
    this._adhocService.verifyHifzPosition(item.Id, type, status).subscribe((res: any) => {
      item.saving = true;
      this.getAdhocVerificationData();
    },
      err => {
        console.log(err);
        this.loading = false;
      }
    );
  }
  public sortChange(sort: SortDescriptor[]): void {
    if (sort[0].field == 'asd') { return; }
    this.kGrid.sort = sort;
    this.sortData();
  }
  private sortData() {
    this.kGrid.gridView = {
      data: orderBy(this.kGrid.data, this.kGrid.sort),
      total: this.kGrid.data.length
    };
  }
  public pageChange(event: PageChangeEvent): void {
    this.kGrid.skip = event.skip;
    this.getAdhocVerificationData();
  }
  public changePagesize(value: any) {
    this.kGrid.pageSize = +value;
    this.kGrid.skip = 0;
    this.getAdhocVerificationData();
  }


  public openPreferencesWindow(dataItem: any) {
    this.preferencesWindow = true;
    this.selectedApplicant = dataItem;

    //this.getAdhocApplicant(this.selectedApplicant.Id);
    this._adhocService.getApplicantDocuments(this.selectedApplicant.Id).subscribe((x: any) => {
      if (x) {
        this.selectedApplicant.ApplicantDocuments = x;
      }
    }, err => {
      console.log(err);
    });
    /* this._adhocService.getExperiences(this.selectedApplicant.Id).subscribe((x: any) => {
      if (x) {
        this.selectedApplicant.ApplicantExperiences = x;
      }
    }, err => {
      console.log(err);
    }); */
    this._adhocService.getExperiences(this.selectedApplicant.Id).subscribe((x: any) => {
      if (x) {
        this.selectedApplicant.ApplicantExperiences = x;
      }
    }, err => {
      console.log(err);
    });
    this._adhocService.getApplicationPref(this.selectedApplicant.Id).subscribe((x: any) => {
      if (x) {
        this.selectedApplicant.Preferences = x;
        console.log(this.selectedApplicant.Preferences);
      }
    }, err => {
      console.log(err);
    });
  }


  public openInNewTab(link: string) {
    window.open(link, '_blank');
  }
  public closePreferencesWindow() {
    this.preferencesWindow = false;
    this.selectedApplicant = {};
    this.decision = '';
  }
  public changeStatus(status: string) {
    this.decision = status;
  }
  public dropValueChanged(filter: string, value: any) {
    console.log(value);
    console.log(filter);
    if (filter == 'reason') {

      this.applicantLog.ReasonId = value.Id;
      this.applicantLog.Reason = value.Reason;
    }
    if (filter == 'reasonSpecific') {
      if (this.applicantLog.ReasonId == 3) {
        this.applicantLog.QualificationId = value.Id;
      }
      if (this.applicantLog.ReasonId == 4) {
        this.applicantLog.ExperienceId = value.Id;
      }
    }
  }
  public dropdownValueChanged = (value, filter) => {
    if (filter == 'designation') {
      this.filters.Designation_Id = value.Designation_Id;
    }
    if (filter == 'status') {
      this.filters.Status_Id = value.Id;
    }
    this.getAdhocVerificationData();
  }
  public changeApplicationStatus(statusId: string) {
    this.applicantLog.StatusId = statusId;
    this.applicantLog.Applicant_Id = this.selectedApplicant.Id;
    this._adhocService.changeApplicationStatus(this.applicantLog).subscribe((res) => {
      if (res) {
        this.closePreferencesWindow();
        this.getAdhocVerificationData();
      }
    }, err => {
      console.log(err);
    });

  }

  public dashifyCNIC(cnic: string) {
    if (cnic) {
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
  }

  public getAdhocApplicant(applicantId: number) {
    this._adhocService.getAdhocApplicant(this.selectedApplicant.Id).subscribe((res: any) => {
      if (res) {
        this.ApplicantInfo = res;
        console.log('ApplicantInfoAd: ', this.ApplicantInfo);
      }
    }, err => {
      console.log(err);
    });
  }

  ngOnDestroy() {
    this.inputChangeSubscription.unsubscribe();
  }
  public uploadBtn() {
    this.scrutinyUpDonwloaded = true;
    this.photoRef.nativeElement.click();
  }

  public numberWithCommas(x) {
    if (x) {
      return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }
  }
  printApplication() {
    this.scrutinyDonwloaded = true
    let html = document.getElementById('formPrint').innerHTML;
    var mywindow = window.open('', 'PRINT', 'height=600,width=900');
    if (mywindow) {
      mywindow.document.write(`<html><head>
     
    <style>
   
    body {
      padding: 30px 50px 0 50px;
      font-family: -apple-system,system-ui;
  }
  table.file-table, table.file-table th, table.file-table td {
    border: 1px solid black;
    border-collapse: collapse;
  }
  table.file-table td {
    padding: 0.5rem;
  }
  p {
    margin-top: 0;
    margin-bottom: 1rem !important;
  }.mt-2 {
  margin-top: 0.5rem !important;
  }.mb-0 {
  margin-bottom: 0 !important;
  }
  .ml-1 {
  margin-left: 0.25rem !important;
  }
  .mb-2 {
  margin-bottom: 0.5rem !important;
  }
  button.print {
  padding: 10px 40px;
  font-size: 21px;
  position: absolute;
  margin-left: 40%;
  background: #46a23f;
  background-image: linear-gradient(rgba(63, 162, 79, 0), rgba(63, 162, 79, 0.2));
  cursor: pointer;
  border: none;
  color: #ffffff;
  z-index: 9999;
  }
  @media print {
  button.print {
    display: none;
  }
  }
    </style>
    <title>Application</title>`);
      mywindow.document.write('</head><body >');
      mywindow.document.write('<button onclick="printFunc()" class="print">Print</button>');
      mywindow.document.write(html);
      mywindow.document.write(`
      <script>
  function printFunc() {
  window.print();
  }
  </script>
      `);
      mywindow.document.write('</body></html>');
      /* this.dropDowns.selectedFiltersModel.inboxOfficers = { Name: 'Select Office', Id: 0 };
      this.dropdownValueChanged(this.dropDowns.selectedFiltersModel.inboxOfficers, 'office'); */
      //this.getFileMovements();
      /*     mywindow.document.close(); // necessary for IE >= 10
          mywindow.focus(); // necessary for IE >= 10
          mywindow.print();
          mywindow.close(); */
    }
  }
}
