import { Component, OnInit, ViewChild } from '@angular/core';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { RootService } from '../../../_services/root.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { DomSanitizer } from '@angular/platform-browser';
import { NotificationService } from '../../../_services/notification.service';
import { CitizenPortalService } from '../citizen-portal.service';
@Component({
  selector: 'app-add-edit',
  templateUrl: './add-edit.component.html',
  styles: [`
  .print-barcode {
    display: hidden !important;
  }
  @media print {
    .row, .container, #note, .container-floating, .print-not {
      display: none !important;
    }
    .print-barcode {
      display: visible !important;
    }
  }
  `]
})
export class AddEditComponent implements OnInit {
  public loading: boolean = true;
  public saving: boolean = false;
  public application: any = {};
  public previewApplication: number = 2;
  public dropDowns: DropDownsHR = new DropDownsHR();
  public currentUser: any;
  public cnicMask: string = "00000-0000000-0";
  public mobileMask: string = "0000-0000000";
  public officers: any[] = [];
  public officersData: any[] = [];
  public types: any[] = [{ Id: 15, Name: 'Court Case' }, { Id: 16, Name: 'Summary' }, { Id: 17, Name: 'Note' }];
  public selectedFiltersModel: any = {};
  public savingApplication = false;
  public barcodeImgSrc: string = '';
  public applicationAttachments: any[] = [];
  @ViewChild('photoRef', {static: false}) public photoRef: any;
  public photoFile: any[] = [];
  public photoSrc = '';
  public photoSrces: any[] = [];
  public applicationDocuments: any[] = [];
  public uploadingFile: boolean = false;
  public uploadingFileError: boolean = false;
  public loadingDocs: boolean = false;
  constructor(private sanitizer: DomSanitizer, private _notificationService: NotificationService,
    private _rootService: RootService, private _riService: CitizenPortalService, private _authenticationService: AuthenticationService) { }
  private loadDropdownValues = () => {
    this.getDivisions('0');
    this.getDistricts('0');
    this.getTehsils('0');
    this.getPandSOfficers('fts');
    this.getApplicationDocuments(14);
    //this.getApplicationPriorities();
  }
  ngOnInit() {
    this.currentUser = this._authenticationService.getUser();
    this.loadDropdownValues();
  }
  private getApplicationDocuments = (applicationTypeId: number) => {
    this.loadingDocs = true;
    this.applicationDocuments = [];
    this._rootService.getApplicationDocuments(applicationTypeId).subscribe((res: any) => {
      this.applicationDocuments = res;
      this.loadingDocs = false;
    },
      err => { this.handleError(err); }
    );
  }
  public selectFile(event, document: any): void {
    let inputValue = event.target;
    let applicationAttachment: any = {};
    applicationAttachment.Document_Id = document.Id;
    applicationAttachment.documentName = document.Name;
    applicationAttachment.files = inputValue.files;
    this.applicationAttachments.push(applicationAttachment);
    this.applicationDocuments.find(x => x.Id == document.Id).attached = true;
  }
  onSubmit() {
    this.savingApplication = true;
    this._riService.submitApplication(this.application).subscribe((response: any) => {
      if (response.application) {
        this.application.Id = response.application.Id;
        let applicationLog: any = {};
        applicationLog.Application_Id = this.application.Id;

        applicationLog.ToOfficer_Id = this.application.ForwardingOfficer_Id;

        // Marked to First officer
        applicationLog.ToStatus_Id = 11;
        applicationLog.ToStatus = 'Marked';
        applicationLog.IsReceived = false;
        this._riService.createApplicationLog(applicationLog).subscribe((response: any) => {
          if (response.Id) {
            if (this.applicationAttachments.length > 0) {
              this.uploadFile(this.application.Id);
            } else {
              this._notificationService.notify('success', 'Tracking Number : ' + this.application.TrackingNumber + ' marked to ' + this.application.ForwardingOfficerName);
              this.savingApplication = false;
            }
          }
        },
          err => { this.handleError(err); }
        );
        if (response.barCode) {
          this.application.TrackingNumber = response.application.TrackingNumber;
          let barcode = response.barCode;
          this.application.barcode = barcode;
          //this._notificationService.notify('success', 'Application Saved!');
          //Proocess Further
          //window.scroll(0, 0);
        }
      }
    },
      err => { this.handleError(err); }
    );
  }
  printTest() {
    //printJS('barcodeFileBars', 'html');
  }
  public searchProfile = () => {
    if (!this.application.CNIC) {
      this.mapProfileToApplicant(null);
      return;
    }
    this._riService.searchProfile(this.application.CNIC).subscribe((data: any) => {
      if (data == 'Invalid') {
        this.mapProfileToApplicant(null);
      }
      if (data) {
        this.mapProfileToApplicant(data);
      }
    });
  }
  /*  private getApplicationPriorities = () => {
     this._rootService.getApplicationPriorities().subscribe((res: any) => {
       this.dropDowns.applicationPriorities = res;
     },
       err => { this.handleError(err); }
     );
   } */
  public setApplicationPriority() {
    this.application.Priority_Id = 1;
    this.dropDowns.selectedFiltersModel.applicationPriority = { Id: 1, Name: 'Normal' }
    let date7 = new Date();
    date7.setDate(date7.getDate() + 7);
    this.application.LastDate = date7;
  }
  public dropdownValueChanged = (value, filter) => {
    if (filter == 'office') {
      this.application.ForwardingOfficer_Id = value.Id;
      this.application.ForwardingOfficerName = value.DesignationName;
    }
    if (filter == 'applicationType') {
      this.application.ApplicationType_Id = value.Id;
      if (this.application.ApplicationType_Id == 16) {
        this.setApplicationPriority();
      } else {
        this.application.LastDate = null;
      }
    }
    if (filter == 'priority') {
      this.application.Priority_Id = value.Id;
      let date = new Date();
      date.setDate(date.getDate() + value.Days);
      this.application.LastDate = date;
    }
  }
  public handleFilter = (value, filter) => {
    if (filter == 'markingOfficer') {
      this.officersData = this.officers.filter((s: any) => s.DesignationName.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }

  }
  public mapProfileToApplicant = (profile: any) => {
    if (profile) {
      this.application.EmployeeName = profile.EmployeeName ? profile.EmployeeName : '';
      this.application.FatherName = profile.FatherName ? profile.FatherName : '';
      this.application.DateOfBirth = profile.DateOfBirth ? new Date(profile.DateOfBirth) : new Date(2000, 1, 1);
      this.application.Gender = profile.Gender ? profile.Gender : 'Select Gender';
      this.application.MobileNo = profile.MobileNo ? profile.MobileNo : '';
      this.application.EMaiL = profile.EMaiL ? profile.EMaiL : '';

      this.application.Department_Id = profile.Department_Id ? profile.Department_Id : this.application.Department_Id;
      this.application.DepartmentName = profile.Department_Id == 28 ? 'Specialized Healthcare & Medical Education' : 'Planning & Development Board';
      this.dropDowns.selectedFiltersModel.departmentDefault = profile.Department_Id == 28 ? { Name: 'Specialized Healthcare & Medical Education', Id: 28 } : profile.Department_Id == 25 ? { Name: 'Planning & Development Board', Id: 25 } : this.dropDowns.selectedFiltersModel.department;

      let designations = this.dropDowns.designations as any[];
      let designation = designations.find(x => x.Id == profile.Designation_Id);
      if (designation) {
        this.application.Designation_Id = profile.Designation_Id ? profile.Designation_Id : this.application.Designation_Id;
        this.application.designationName = designation.Name;
        this.dropDowns.selectedFiltersModel.designation = { Name: designation.Name, Id: designation.Name.Id };
      }
      if (profile.HealthFacility_Id && profile.HfmisCode.length == 19) {
        this.getDivisions('0');
        this.getDistricts('0');
        this.getTehsils('0');
        this.getHealthFacilities(profile.HfmisCode.substring(0, 9), profile.HfmisCode);
      }
      if (this.application.ApplicationType_Id == 3) {
        this.application.CurrentScale = profile.CurrentGradeBPS ? profile.CurrentGradeBPS : 0;
        this.application.SeniorityNumber = profile.SeniorityNo ? profile.SeniorityNo : '';
      }
    } else {
      this.application.EmployeeName = '';
      this.application.FatherName = '';
      this.application.DateOfBirth = new Date(2000, 1, 1);
      this.application.Gender = 'Select Gender';
      this.application.MobileNo = '';
      this.application.EMaiL = '';
      this.application.Department_Id = 25;
      this.application.DepartmentName = 'Planning & Development Board';
      this.application.Designation_Id = 0;
      this.application.designationName = '';
      this.dropDowns.selectedFiltersModel.designation = { Name: 'Select Designation', Id: 0 };
      this.dropDowns.selectedFiltersModel.healthFacility = { Name: 'Select Health Facility', Id: 0 };
      this.application.fromHealthFacility = '';
      this.application.HfmisCode = '';
      this.application.HealthFacility_Id = 0;
      this.application.fromHealthFacility = '';
    }
  }
  private getPandSOfficers = (type: string) => {
    this.officers = [];
    this._rootService.getPandSOfficers(type).subscribe((res: any) => {
      this.officers = res;
      this.officersData = this.officers;
    },
      err => { this.handleError(err); }
    );
  }
  private getDivisions = (code: string) => {
    this.dropDowns.divisions = [];
    this.dropDowns.divisionsData = [];
    this._rootService.getDivisions(code).subscribe((res: any) => {

      this.dropDowns.divisions = res;
      this.dropDowns.divisionsData = this.dropDowns.divisions.slice();
    },
      err => { this.handleError(err); }
    );
  }

  private getDistricts = (code: string) => {
    this.dropDowns.districts = [];
    this.dropDowns.districtsData = [];
    this._rootService.getDistricts(code).subscribe((res: any) => {
      this.dropDowns.districts = res;
      this.dropDowns.districtsData = this.dropDowns.districts.slice();
    },
      err => { this.handleError(err); }
    );
  }
  private getTehsils = (code: string) => {
    this.dropDowns.tehsils = [];
    this.dropDowns.tehsilsData = [];
    this._rootService.getTehsils(code).subscribe((res: any) => {
      this.dropDowns.tehsils = res;
      this.dropDowns.tehsilsData = this.dropDowns.tehsils.slice();
    },
      err => { this.handleError(err); }
    );
  }
  private getHealthFacilities = (hfmisCode: string, profileHfmisCode?: string) => {
    this._rootService.getHealthFacilities(hfmisCode).subscribe((res: any) => {
      this.dropDowns.healthFacilities = res;
      this.dropDowns.healthFacilitiesData = this.dropDowns.healthFacilities.slice();
      if (profileHfmisCode) { this.setProfileDefaultValues(profileHfmisCode); }
    },
      err => { this.handleError(err); }
    );
  }
  public setProfileDefaultValues = (profileHfmisCode) => {
    if (profileHfmisCode) {
      let divisions = this.dropDowns.divisions as any;
      let division = divisions.find(x => x.Code == profileHfmisCode.substring(0, 3));
      if (division) {
        this.dropDowns.selectedFiltersModel.division = { Code: division.Code, Name: division.Name };
      }

      let districts = this.dropDowns.districts as any;
      let district = districts.find(x => x.Code == profileHfmisCode.substring(0, 6));
      if (district) {
        this.dropDowns.selectedFiltersModel.district = { Code: district.Code, Name: district.Name };
      }

      let tehsils = this.dropDowns.tehsils as any;
      let tehsil = tehsils.find(x => x.Code == profileHfmisCode.substring(0, 9));
      if (tehsil) {
        this.dropDowns.selectedFiltersModel.tehsil = { Code: tehsil.Code, Name: tehsil.Name };
      }
      let healthFacilities = this.dropDowns.healthFacilities as any;
      let healthFacility = healthFacilities.find(x => x.HfmisCode == profileHfmisCode);
      if (healthFacility) {
        this.dropDowns.selectedFiltersModel.healthFacility = { Id: healthFacility.Id, HfmisCode: healthFacility.HfmisCode, Name: healthFacility.Name };
        this.application.HfmisCode = healthFacility.HfmisCode;
        this.application.HealthFacility_Id = healthFacility.Id;
        this.application.fromHealthFacility = healthFacility.Name;
      }
    }
  }

  public readUrl(event: any, filter: string) {
    if (event.target.files && event.target.files[0]) {
      if (filter == 'pic') {
        this.photoFile = [];
        let inputValue = event.target;
        this.photoFile = inputValue.files;
        console.log(this.photoFile);
        if (this.photoFile.length > 0) {
          if (this.photoFile[0].name.toLowerCase().endsWith('.pdf')) {
            this.photoSrc = '../../../../assets/img/icons/pdf-icon.png'
          }
        }
        this.application.attached = true;
        let applicationAttachment: any = {};
        applicationAttachment.Document_Id = 2;
        applicationAttachment.files = inputValue.files;
        this.applicationAttachments.push(applicationAttachment);
        var reader = new FileReader();
        reader.onload = ((event: any) => {
          this.photoSrc = event.target.result;
          console.log(this.photoSrc);
        }).bind(this);
        reader.readAsDataURL(event.target.files[0]);
      }
    }
  }
  public uploadBtn(filter: string) {
    if (filter == 'pic') {
      this.photoRef.nativeElement.click();
      /*   this.uploadingAcceptanceLetter = true; */
    }
  }
  public uploadFile(application_Id: number) {
    this.uploadingFile = true;
    this._riService.uploadApplicationAttachments(this.applicationAttachments, application_Id).subscribe((x: any) => {
      if (x) {
        this._notificationService.notify('success', 'Upload Completed Successfully!');
        this.photoFile = [];
        this.applicationAttachments = [];
      } else {
        this.uploadingFileError = true;
      }
      this.savingApplication = false;
      this.uploadingFile = false;
    }, err => {
      this.uploadingFileError = true;
      this.uploadingFile = false;
    });
  }
  public clear() {
    let forwardingOfficer_Id = this.application.ForwardingOfficer_Id;
    let forwardingOfficerName = this.application.ForwardingOfficerName;
    this.application = {};
    this.application.ForwardingOfficer_Id = forwardingOfficer_Id;
    this.application.ForwardingOfficerName = forwardingOfficerName;
  }

  public barcodeSrc() {
    return this.sanitizer.bypassSecurityTrustUrl(this.application.barcode);
  }
  printBarcodeInstant() {
    window.print();
  }
  printBarcode() {
    let html = document.getElementById('barcodeFileBars').innerHTML;
    var mywindow = window.open('', 'PRINT', 'height=600,width=900');
    if (mywindow) {
      mywindow.document.write(`<html><head>
      <style>
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
      <title>File</title>`);
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

      /*  mywindow.document.close(); // necessary for IE >= 10
      mywindow.focus(); // necessary for IE >= 10
        mywindow.print();
        mywindow.close(); */
    }
  }
  printApplication() {
    let html = document.getElementById('applicationPrint').innerHTML;
    var mywindow = window.open('', 'PRINT', 'height=600,width=900');
    if (mywindow) {
      mywindow.document.write(`<html><head>
        <style>
            body {
              font-family: -apple-system,system-ui;
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
      this.application = {};
      /*     mywindow.document.close(); // necessary for IE >= 10
          mywindow.focus(); // necessary for IE >= 10
          mywindow.print();
          mywindow.close(); */
    }
  }
  private handleError(err: any) {
    this.savingApplication = false;
    this.loading = false;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }
}
