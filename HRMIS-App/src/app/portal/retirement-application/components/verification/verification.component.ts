import { Component, OnInit, Input, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import { ActivatedRoute, Router } from '@angular/router';
import { DropDownsHR } from '../../../../_helpers/dropdowns.class';
import { RootService } from '../../../../_services/root.service';
import { NotificationService } from '../../../../_services/notification.service';
import { RetirementApplicationService } from '../../retirement-application.service';
import { Subject } from 'rxjs/Subject';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { AuthenticationService } from '../../../../_services/authentication.service';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-profile-verification',
  templateUrl: './verification.component.html',
  styles: []
})
export class VerificationComponent implements OnInit, OnDestroy {
  private subscription: Subscription;
  @Input() public profile: any = {};
  @ViewChild('photoRef', { static: false }) public photoRef: any;
  public hf: any;
  public applications: any[] = [];
  public orders: any[] = [];
  public applicationAttachments: any[] = [];
  public applicationAttachmentsUpload: any[] = [];
  public application: any = {};
  public vpMaster: any;
  public dropDowns: DropDownsHR = new DropDownsHR();
  public transferReason: number = null;
  public disabilityReason: string = '';
  public showMessage: string = '';
  public cnic: string = "";
  public cnicMutual: string = "";
  public imagePath: string = '';
  public id: number = 0;
  public step: number = 1; public photoSrc = '';
  public photoSrces: any[] = [];
  public photoFile: any[] = [];
  public toOfficerId: number = 75;
  public cnicMask: string = "00000-0000000-0";
  public verifying: boolean = false;
  public noVacancyExist: boolean = false;
  public uploadingFile: boolean = false;
  public uploadingFileError: boolean = false;
  public savingApplication: boolean | number = false;
  public totalFiles: number = 0;
  public totalFilesUploaded: number = 0;
  public activeApplicationAttachmentId: number = 0;
  public applicationId: number = 0;
  public applicationtracking: number = 0;
  public totalDocsRequired: number = 0;
  public vacancyLoaded: boolean = false;
  public checkingVacancy: boolean = false;
  public otherProfileExist: boolean = false;
  public loadingMutualCNIC: boolean = false;
  public imageWindowOpened: boolean = false;
  public mutualProfile: any = {};
  public applicationDocuments: any[] = [];
  public barcodeImgSrc: string = '';
  public transferReasons: any[] = [
    { Name: 'General', Id: 1 },
    { Name: 'Disability', Id: 2 },
    /*   { Name: 'Spouse Death', Id: 3 }, */
    /*   { Name: 'Medical', Id: 4 }, */
    { Name: 'Mutual', Id: 7 },
    { Name: 'WedLock', Id: 5 }
    /*  { Name: 'Compassionate', Id: 6 },*/
  ];
  public inputChange: Subject<any>;
  private cnicSubscription: Subscription;

  constructor(private sanitizer: DomSanitizer, private _rootService: RootService, private _retirementApplicationService: RetirementApplicationService,
    private _notificationService: NotificationService,
    private _authenticationService: AuthenticationService,
    private router: Router,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.fetchParams();
    /*  this._profileService.getProfileDetail(this.profile.CNIC, 3).subscribe((data: any) => {
       this.orders = [];
     },
       err => {
         console.log(err);
       });
  */
    this.application.ApplicationType_Id = 4;
    this.subscribeCNIC();
  }
  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('cnic') && params.hasOwnProperty('id')) {
          this.cnic = params['cnic'];
          this.id = +params['id'];
          this.fetchData(this.cnic, this.id);
        }
      }
    );
  }
  public subscribeCNIC() {
    this.inputChange = new Subject();
    this.cnicSubscription = this.inputChange.pipe(debounceTime(300)).subscribe((query) => {
      this.loadingMutualCNIC = true;
      if (!query) { this.loadingMutualCNIC = false; return; }
      let cnic: string = query as string;
      cnic = cnic.replace(' ', '');
      if (cnic.length != 13) { this.loadingMutualCNIC = false; return; }
      if (cnic == this.cnic) { this.loadingMutualCNIC = false; return; }
    });
  }
  private fetchData(cnic, id) {
    this._retirementApplicationService.getProfileApplicant(cnic, id).subscribe(
      (res: any) => {
        this.profile = res.profile;
        this.hf = res.hf;
        this.applications = res.applications;
        this.checkApplications();
        /*  if (this.profile && this.hf) {
           this.setRoutingOfficer(this.hf.CategoryCode, this.hf.Classification_Id, this.profile.CurrentGradeBPS);
         } */
      }
    );
  }
  public checkApplications = () => {
    let exist = false;
    this.applications.forEach((app) => {
      if (app.ApplicationType_Id == 4) {
        this.getApplication(app.Id, app.TrackingNumber);
        exist = true;
        return;
      }
    });
    if (!exist) {
      this.getApplicationDocuments();
    }
  }
  /* public getApplicationDocuments = (applicationTypeId: number) => {
    this.applicationDocuments = [];
    this._rootService.getApplicationDocuments(applicationTypeId).subscribe((res: any) => {
      this.applicationDocuments = res;
    },
      err => { this.handleError(err); }
    );
  } */
  public capitalize(val: string) {
    if (!val) return '';
    return val.toUpperCase();
  }
  onSubmit() {
    this.savingApplication = true;
    this.totalFiles = this.applicationAttachmentsUpload.length;
    if (this.application.Id) {
      this.uploadAttachment();
      return;
    }
    this.mapProfileToApplicant(this.profile);
    this.savingApplication = true;
    /* let html = document.getElementById('applicationPrint').innerHTML;
    if (html) {
      this.application.RawText = html;
    } */
    this._retirementApplicationService.submitApplication(this.application).subscribe((response: any) => {
      if (response.application) {
        this.application.Id = response.application.Id;
        if (response) {
          this.application.TrackingNumber = response.application.TrackingNumber;
          if (this.applicationAttachmentsUpload.length > 0) {
            this.totalFiles = this.applicationAttachmentsUpload.length;
            this.savingApplication = 20;
            this.uploadAttachment();
          }
          /*   let applicationLog: any = {};
            applicationLog.Application_Id = this.application.Id;
  
            // Received to First officer
            applicationLog.ToStatus_Id = 9;
            applicationLog.ToStatus = 'Initiated';
            applicationLog.IsReceived = false;
            this.savingApplication = 10;
            this._retirementApplicationService.createApplicationLog(applicationLog).subscribe((response2: any) => {
              if (response2.Id) {
                
                //this.router.navigate(['/online-application/profile/']);
              }
            },
              err => { this.handleError(err); }
            ); */
        }
      }

    },
      err => { this.handleError(err); }
    );
  }
  public uploadAttachment() {
    if (this.totalFilesUploaded < this.totalFiles) {
      this._retirementApplicationService.uploadApplicationAttachments([this.applicationAttachmentsUpload[this.totalFilesUploaded]], this.application.Id).subscribe((res) => {
        if (res) {
          this.totalFilesUploaded += 1;
          this.savingApplication = +this.savingApplication + 10;
          this.uploadAttachment();
        }
      }, err => {
        this.handleError(err);
      });
    } else if (this.totalFilesUploaded == this.totalFiles && this.totalDocsRequired != this.applicationAttachments.length) {
      this.fetchParams();
      this.savingApplication = false;
    } else if (this.totalFilesUploaded == this.totalFiles && this.totalDocsRequired == this.applicationAttachments.length) {
      this._notificationService.notify('success', 'Application Submitted Successfully, Tracking Number: ' + this.application.TrackingNumber);
      this.savingApplication = false;
      this.fetchParams();
    }
  }
  public finalizeApplication() {
    this.savingApplication = true;
    let applicationLog: any = {};
    applicationLog.Application_Id = this.application.Id;
    applicationLog.ToOfficer_Id = this.toOfficerId;
    applicationLog.FromOfficer_Id = 164;
    // Received to First officer
    applicationLog.ToStatus_Id = 11;
    applicationLog.ToStatus = 'Marked';
    applicationLog.IsReceived = false;
    this.savingApplication = 10;
    this._retirementApplicationService.createApplicationLog(applicationLog).subscribe((response2: any) => {
      if (response2.Id) {
        if (this.applicationAttachmentsUpload.length > 0) {
          this.totalFiles = this.applicationAttachmentsUpload.length;
          this.savingApplication = 20;
          this.uploadAttachment();
        }
        else {
          this._notificationService.notify('success', 'Application Submitted Successfully, Tracking Number: ' + this.application.TrackingNumber);
          this.savingApplication = false;
          this.fetchParams();
        }
        //this.router.navigate(['/online-application/profile/']);
      }
    },
      err => { this.handleError(err); }
    );
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
  private getApplication(id, tracking) {
    this._retirementApplicationService.getApplication(+id, +tracking).subscribe((data: any) => {
      if (data) {
        this.application = data.application;
        this.application.DateOfBirth = new Date(this.application.DateOfBirth);
        this.application.FromDate = new Date(this.application.FromDate);
        this.application.ToDate = new Date(this.application.ToDate);
        this._rootService.generateBars(this.application.TrackingNumber).subscribe((res: any) => { this.barcodeImgSrc = res.barCode; }, err => { this.handleError(err); });
       /*  this._rootService.generateBars(this.application.TrackingNumber).subscribe((res: any) => { this.barcodeImgSrc = res.barCode; }, err => { this.handleError(err); });
        */ this.applicationAttachments = data.applicationAttachments as any[];
        /*  this.applicationLog.Application_Id = this.application.Id;
         this.dropDowns.selectedFiltersModel.applicationStatus = { Id: this.application.Status_Id, Name: this.application.StatusName };
         this.dropDowns.selectedFiltersModel.leaveType = { Id: this.application.LeaveType_Id, LeaveType1: this.application.leaveType };
         this.dropDowns.selectedFiltersModel.officer = { Id: this.application.PandSOfficer_Id, DesignationName: this.application.PandSOfficerName };
         this.getApplicationStatus();
         this.getAll('0');
         this.getDivisions('0');
         this.getDistricts('0');
         this.getTehsils('0');
   
         this.loading = false;
         this.getApplicationData("logs");
         this.getApplicationData("oldlogs");
         this.getApplicationData("file");
         this.getApplicationData("filereqs");
         this.getApplicationData("parliamentarian"); */
        this.getApplicationDocuments();
        /*   if (this.application.ApplicationType_Id == 2 && (this.user.UserName == 'og1' || this.user.UserName == 'ds.admin' || this.user.UserName == 'so.toqeer' || this.user.UserName == 'ds.general' || this.user.UserName == 'ordercell')) {
            this.checkVacancy();
            this.getVpMaster();
          }
          this.closeWindow(); */
      }
    }, err => { this.handleError(err); }
    );
  }
  private getApplicationDocuments = () => {
    this.applicationDocuments = [];
    this._rootService.getApplicationDocuments(this.application.ApplicationType_Id).subscribe((res: any[]) => {
      if (res && res.length > 0) {
        this.totalDocsRequired = res.length;
        let tempIds = [];
        this.applicationAttachments.forEach(x => {
          let doc = res.find(y => y.Id == x.Document_Id);
          if (doc) {
            tempIds.push(doc.Id);
          }
        });
        res.forEach(doc => {
          let id = tempIds.find(z => z == doc.Id);
          if (!id) {
            this.applicationDocuments.push(doc);
          }
        });
      }
    },
      err => { this.handleError(err); }
    );
  }
  public openWindow(imagePath: string, index: number, attahcmentId: number) {
    this.activeApplicationAttachmentId = attahcmentId;
    this.imagePath = imagePath;
    this.imageWindowOpened = true;
  }
  public closeWindow() {
    this.imageWindowOpened = false;
  }
  public barcodeSrc() {
    return this.sanitizer.bypassSecurityTrustUrl(this.barcodeImgSrc);
  }
  public logout() {
    this._authenticationService.logoutApplicant();
  }
  public uploadBtn(filter: string) {
    if (filter == 'pic') {
      this.photoRef.nativeElement.click();
      /*   this.uploadingAcceptanceLetter = true; */
    }
  }
  private setRoutingOfficer = (catCode: string, classification_Id: number, scale: number) => {
    if (classification_Id == 1) {
      this.toOfficerId = 12;
    } else {
      if (catCode == '001') {
        if (scale == 17) {
          this.toOfficerId = 8;
        } else if (scale > 17) {
          this.toOfficerId = 9;
        }
      } else if (catCode == '002') {
        if (scale == 17) {
          this.toOfficerId = 10;
        } else if (scale > 17) {
          this.toOfficerId = 11;
        }
      }
    }
  }
  public selectFile(event, document: any): void {
    debugger;
    let inputValue = event.target;
    let applicationAttachment: any = {};
    applicationAttachment = this.applicationDocuments.find(x => x.Id == document.Id);
    applicationAttachment.Document_Id = document.Id;
    applicationAttachment.documentName = document.Name;
    applicationAttachment.files = inputValue.files;
    this.applicationAttachmentsUpload.push(applicationAttachment);
    this.applicationDocuments.find(x => x.Id == document.Id).attached = true;
  }
  public next() {
    this.step = 2;
    if (this.transferReason == 7) {
      this.mapProfileToApplicant(this.profile);
    }
  }
  public mapProfileToApplicant = (profile: any) => {
    if (profile) {
      this.application.Profile_Id = profile.Id ? profile.Id : null;
      this.application.CNIC = profile.CNIC ? profile.CNIC : null;
      this.application.EmployeeName = profile.EmployeeName ? profile.EmployeeName : '';
      this.application.FatherName = profile.FatherName ? profile.FatherName : '';
      this.application.DateOfBirth = profile.DateOfBirth ? new Date(profile.DateOfBirth).toDateString() : new Date(2000, 1, 1).toDateString();
      this.application.Gender = profile.Gender ? profile.Gender : '';
      this.application.MobileNo = profile.MobileNo ? profile.MobileNo : '';
      this.application.EMaiL = profile.EMaiL ? profile.EMaiL : '';
      this.application.FileNumber = '';
      this.application.JoiningGradeBPS = profile.JoiningGradeBPS ? profile.JoiningGradeBPS : 0;
      this.application.CurrentGradeBPS = profile.CurrentGradeBPS ? profile.CurrentGradeBPS : 0;
      this.application.Designation_Id = profile.WDesignation_Id ? profile.WDesignation_Id : this.application.Designation_Id;
      this.application.HealthFacility_Id = profile.HealthFacility_Id;
      this.application.HfmisCode = profile.HfmisCode;
      this.application.Department_Id = 25;
      this.application.ApplicationSource_Id = 3;
      this.application.ForwardingOfficer_Id = 164;
      this.application.ForwardingOfficerName = 'Online Applicant';
      this.application.DepartmentName = 'Primary & Secondary Healthcare Department';
      let empModes = this.dropDowns.employementModesData as any[];
      if (empModes) {
        let empMode = empModes.find(x => x.Id == profile.EmpMode_Id);
        if (empMode) {
          this.application.EmpMode_Id = empMode.Id;
          this.application.EmpModeName = empMode.Name;
        }
      }
      let empStatuses = this.dropDowns.statusData as any[];
      if (empStatuses) {
        let empStatuse = empStatuses.find(x => x.Id == profile.EmpStatus_Id);
        if (empStatuse) {
          this.application.EmpStatus_Id = empStatuse.Id;
          this.application.EmpStatusName = empStatuse.Name;
        }
      }

      if (this.application.ApplicationType_Id == 3) {
        this.application.CurrentScale = profile.CurrentGradeBPS ? profile.CurrentGradeBPS : 0;
        this.application.SeniorityNumber = profile.SeniorityNo ? profile.SeniorityNo : '';
      }
    }
  }

  public dashifyCNIC(cnic: string) {
    if (!cnic) return;
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
    this.vacancyLoaded = false;
    this.checkingVacancy = false; this.savingApplication = false;
  }
  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
    if (this.cnicSubscription) {
      this.cnicSubscription.unsubscribe();
    }
  }
  printApplication() {
    let html = document.getElementById('applicationPrint').innerHTML;
    var mywindow = window.open('', 'PRINT', 'height=600,width=900');
    if (mywindow) {
      mywindow.document.write(`<html><head>
        <style>
            body {
              font-family: -apple-system, BlinkMacSystemFont, 
            'Segoe UI', 'Roboto' , 'Oxygen' , 'Ubuntu' , 'Cantarell' , 'Fira Sans' , 'Droid Sans' , 'Helvetica Neue' ,
              sans-serif !important;
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
        .application-page {
    
          padding: 50px;
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
        .watermark-hisdu {
          text-align: center;
          position: absolute;
          left: 0;
          width: 100%;
          opacity: 0.25;
        }

        .watermark-hisdu img {
          display: inline-block;
        }

        table.header-pshealth,
        .applicant-information,
        .application-type-detail-preview,
        .attached-document,
        .remarks-preview,
        .info-application-preview,
        table.pshealth {
          border-color: transparent !important;
          width: 100%;
        }

        table.header-pshealth td {
          border-color: transparent !important;
        }

        table.header-pshealth td.gop-logo-a4-header {
          text-align: left;
        }

        table.header-pshealth td.gop-logo-a4-header img {
          display: inline-block;
          width: 134px;
        }

        table.header-pshealth td.pshealth-right-a4-td-header {
          text-align: right;
        }

        table.header-pshealth td.pshealth-right-a4-td-header .pshealth-right-a4-text-header {
          display: inline-block;
          text-align: center;
        }

        table.header-pshealth td.app-type-preview {
          text-align: left;
          width: 100%;
        }

        /* Applicant Information */

        table.applicant-information {
          border-color: transparent !important;
          width: 100%;
        }

        table.applicant-information td.applicant-info-heading,
        table.application-type-detail-preview td.application-type-detail-preview-heading,
        table.remarks-preview td.remarks-heading,
        table.info-application-preview td.info-application-preview-heading,
        table.attached-document td.attached-document-heading {
          text-align: center;
          border: 1px solid black;
        }

        table.applicant-information td.applicant-info-detail-1 {
          width: 20% !important;
        }

        table.applicant-information td.applicant-info-detail-2 {
          width: 40% !important;
        }

        table.applicant-information td.applicant-info-detail-3 {
          width: 10% !important;
        }

        table.applicant-information td.applicant-info-detail-4 {
          width: 30% !important;
        }
        table.applicant-information td.applicant-info-detail-5 {
          width: 15% !important;
        }
        
        table.applicant-information td.applicant-info-detail-6 {
          width: 30% !important;
        }
        
        table.applicant-information td.applicant-info-detail-7 {
          width: 20% !important;
        }
        
        table.applicant-information td.applicant-info-detail-8 {
          width: 35% !important;
        }
        table.info-application-preview td.info-application-preview-left {
          border-left: 1px solid black;
        }

        table.info-application-preview td.info-application-preview-right {
          text-align: center;
          margin: 5px 5px !important;
          border-right: 1px solid black;
          border-left: 1px solid black;
        }

        table.application-route-detail {
          border-color: transparent !important;
          width: 100% !important;
          text-align: center;
        }

        table.application-route-detail td.application-route-detail-header {
          width: 50% !important;
          border: 1px solid black;
        }
        @page 
        {
            size:  auto;   
            margin: 0mm;
        }
        .w-20 { width: 20% !important; }
        .w-30 { width: 30% !important; }
        .w-50 { width: 50% !important; }
        .w-70 { width: 70% !important; }
        .w-80 { width: 80% !important; }

        .mt-10 { margin-top: 10px !important; }
        .mt-30 { margin-top: 30px  !important; }
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
      //show upload signed copy input chooser

      /*     mywindow.document.close(); // necessary for IE >= 10
          mywindow.focus(); // necessary for IE >= 10
          mywindow.print();
          mywindow.close(); */
    }
  }

}
