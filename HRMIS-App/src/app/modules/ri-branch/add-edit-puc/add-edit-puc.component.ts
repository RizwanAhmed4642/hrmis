import { Component, OnInit } from '@angular/core';
import { ApplicationMaster, ApplicationProfileViewModel, ApplicationLog } from '../../application-fts/application-fts';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { RootService } from '../../../_services/root.service';
import { RiBranchService } from '../ri-branch.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { DomSanitizer } from '@angular/platform-browser';
import { NotificationService } from '../../../_services/notification.service';
import { KGridHelper } from '../../../_helpers/k-grid.class';
import { Subject } from 'rxjs/Subject';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { PageChangeEvent } from '@progress/kendo-angular-grid';

@Component({
  selector: 'app-add-edit-puc',
  templateUrl: './add-edit-puc.component.html',
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
export class AddEditPUCComponent implements OnInit {
  public loading: boolean = true;
  public saving: boolean = false;
  public application: any = {};
  public previewApplication: number = 2;
  public toStatusId: number = 10;
  public toStatusName: string = 'No Process Initiated';
  public dropDowns: DropDownsHR = new DropDownsHR();
  public currentUser: any;
  public cnicMask: string = "00000-0000000-0";
  public mobileMask: string = "0000-0000000";
  public officers: any[] = [];
  public officersData: any[] = [];
  public officersInitiated: any[] = [];
  public officersInitiatedData: any[] = [];
  public applicationTypes: any[] = [];
  public punjabOfficers: any[] = [];
  public punjabOfficersData: any[] = [];
  public applicationDocuments: any[] = [];
  public selectedFiltersModel: any = {};
  public sourceId: number = 10;
  public savingApplication = false;
  public isRecieved = false;
  public isPendancy = false;
  public loadingDocs = false;
  public uploading = false;
  public barcodeImgSrc: string = '';
  public fileError: string = '';
  public applicationAttachments: any[] = [];
  public photoFile: any[] = [];
  public kGrid: KGridHelper = new KGridHelper();
  public inputChange: Subject<any>;
  public searchQuery: string = '';
  public office_Id: number = 0;
  public typeId: number = 0;
  public statusId: number = 0;
  public recieveFiles: boolean = false;
  public fromDate: Date = new Date();
  public selectedApplications: number[] = [];
  public range = { start: null, end: null };
  constructor(private sanitizer: DomSanitizer, private _notificationService: NotificationService, private _rootService: RootService, private _riService: RiBranchService, private _authenticationService: AuthenticationService) { }
  private loadDropdownValues = () => {
    this.getPandSOfficers('fts');
    this.getPandSOfficersInitiated('admn');
  }
  ngOnInit() {
    this.currentUser = this._authenticationService.getUser();
    this.kGrid.firstLoad = true;
    this.kGrid.pageSizes = [50, 100, 200, 300, 500, 1000];
    this.inputChange = new Subject();
    this.inputChange.pipe(debounceTime(300)).subscribe((query) => {
      this.searchQuery = query;
      if (this.searchQuery.length <= 2 && this.searchQuery.length != 0) {
        return;
      }
      this.getApplications();
    });
    this.initializeProps();
    this.loadDropDownValues();
    this.getApplications();
    this.fetchData();
    this.loadDropdownValues();
    //this.dropDowns.selectedFiltersModel.applicationType = { Id: 1, Name: 'Application' };
  }
  private fetchData() {
    this._rootService.getApplicationTypes().subscribe((data: any) => {
      this.applicationTypes = data.filter(x => x.RNIBranchPortal == true);
    });
  }

  private initializeProps() {
    this.kGrid.loading = false;
    this.kGrid.pageSize = 50;
    this.kGrid.pageSizes = [50, 100];
  }
  private loadDropDownValues() {
    this.getApplicationTypes();
    this.getApplicationStatus();
    //this.getInboxOffices();
  }
  private getApplicationTypes() {
    this._rootService.getApplicationTypes().subscribe(
      (response: any) => {
        this.dropDowns.applicationTypes = response;
        this.dropDowns.applicationTypesData = this.dropDowns.applicationTypes;
      },
      err => this.handleError(err)
    );
  }
  private getApplicationStatus() {


    this._rootService.getApplicationStatus().subscribe(
      (response: any) => {
        this.dropDowns.applicationStatus = response;
        this.dropDowns.applicationStatusData = this.dropDowns.applicationStatus;
      },
      err => this.handleError(err)
    );
  }
  public getApplications() {
    this.kGrid.loading = true;
    this._riService.getBarcodedApplications({
      Skip: this.kGrid.skip,
      PageSize: this.kGrid.pageSize,
      officeId: this.office_Id,
      Source_Id: this.sourceId,
      Type_Id: this.typeId,
      Query: this.searchQuery,
      Status_Id: this.statusId,
      From: this.range.start ? this.range.start.toDateString() : null, To: this.range.end ? this.range.end.toDateString() : null
    }).subscribe(
      (response: any) => {
        this.kGrid.data = [];
        this.kGrid.data = response.List;
        this.kGrid.totalRecords = response.Count;
        this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords };
        this.kGrid.loading = false;
        this.kGrid.firstLoad = false;
        this.kGrid.data.forEach(data => {
          this._rootService.generateBars(data.TrackingNumber).subscribe((res: any) => { data.barcodeImgSrc = res.barCode; }, err => { this.handleError(err); });

        });
      },
      err => this.handleError(err)
    );
  }
  public getApplicationDocuments = (applicationTypeId: number) => {
    this.loadingDocs = true;
    this.applicationDocuments = [];
    this._rootService.getApplicationDocuments(applicationTypeId).subscribe((res: any) => {
      this.applicationDocuments = res;
      this.loadingDocs = false;
    },
      err => { this.handleError(err); }
    );
  }
  onSubmit() {
    this.savingApplication = true;
    this.applicationAttachments = [];
    this._riService.submitApplication(this.application).subscribe((response: any) => {
      if (response.application) {
        this.application.Id = response.application.Id;
        let applicationLog: ApplicationLog = new ApplicationLog();
        applicationLog.Application_Id = this.application.Id;

        applicationLog.ToOfficer_Id = this.application.ForwardingOfficer_Id;

        // Marked to First officer
        applicationLog.ToStatus_Id = this.toStatusId;
        applicationLog.ToStatus = this.toStatusName;
        applicationLog.IsReceived = this.isRecieved;
        this._riService.createApplicationLog(applicationLog).subscribe((response2: any) => {
          if (response2.Id) {
            this.uploading = true;
            this.fileError = '';
            if (this.photoFile.length > 0) {
              this._riService.uploadApplicationAttachments([{ files: this.photoFile, Document_Id: 153 }], this.application.Id).subscribe((res: any) => {
                if (res) {
                  this.photoFile = [];
                  this._notificationService.notify('success', 'File Upload Successfull');
                  this.savingApplication = false;
                }
                this.uploading = false;
              }, err => {
                this.uploading = false;
                this.fileError = err.error.Message;
                this.savingApplication = false;
                this.handleError(err);
              });
            } else {
              this._notificationService.notify('success', 'Document Added Successfully!');
              this.savingApplication = false;
            }
            this.getApplications();
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
  public readUrl(event: any) {
    if (event.target.files && event.target.files[0]) {
      this.photoFile = [];
      let inputValue = event.target;
      this.photoFile = inputValue.files;
    }
  }
  public uploadFile() {
    this.uploading = true;
    this.fileError = '';
    if (this.photoFile.length > 0) {
      this._riService.uploadApplicationAttachments([{ files: this.photoFile, Document_Id: 153 }], this.application.Id).subscribe((res: any) => {
        if (res) {
          this.photoFile = [];
          this._notificationService.notify('success', 'File Upload Successfull');
        }
        this.uploading = false;
      }, err => {
        this.uploading = false;
        this.fileError = err.error.Message;
        this.handleError(err);
      });
    }
  }
  public submitDocumentList() {

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
  public dropdownValueChanged = (value, filter) => {
    if (filter == 'office') {
      this.application.ForwardingOfficer_Id = value.Id;
      this.application.ForwardingOfficerName = value.DesignationName;
    }
    if (filter == 'punjab') {
      this.application.ToOfficer_Id = value.Id;
      this.application.ToOfficerName = value.DesignationName;
    }
    if (filter == 'responsibleOffice') {
      this.application.DispatchFrom = value.DesignationName;
    }
    if (filter == 'applicationType') {
      this.application.ApplicationType_Id = value.Id;
      this.getApplicationDocuments(this.application.ApplicationType_Id);
      this.getAppTypePendancy(this.application.ApplicationType_Id);
    } if (filter == 'types') {
      this.typeId = value.Id;
      this.getApplications();
    }
    if (filter == 'status') {
      this.statusId = value.Id;
      this.getApplications();
    }
  }
  private getAppTypePendancy = (typeId: number) => {
    this.dropDowns.appTypePendancy = [];
    this._rootService.getAppTypePendancy(typeId).subscribe((res: any) => {
      if (res) {
        this.isPendancy = res.IsPendancy;
      } else {
        this.isPendancy = false;
      }
      if (this.isPendancy) {
        this.toStatusId = 10;
        this.toStatusName = 'No Process Initiated';
        this.isRecieved = true;
      } else {
        this.toStatusId = 11;
        this.toStatusName = 'Marked';
        this.isRecieved = false;
      }
    },
      err => { this.handleError(err); }
    );
  }
  public mapProfileToApplicant = (profile: ApplicationProfileViewModel) => {
    if (profile) {
      this.application.EmployeeName = profile.EmployeeName ? profile.EmployeeName : '';
      this.application.FatherName = profile.FatherName ? profile.FatherName : '';
      this.application.DateOfBirth = profile.DateOfBirth ? new Date(profile.DateOfBirth) : new Date(2000, 1, 1);
      this.application.Gender = profile.Gender ? profile.Gender : 'Select Gender';
      this.application.MobileNo = profile.MobileNo ? profile.MobileNo : '';
      this.application.EMaiL = profile.EMaiL ? profile.EMaiL : '';

      this.application.Department_Id = profile.Department_Id ? profile.Department_Id : this.application.Department_Id;
      this.application.DepartmentName = profile.Department_Id == 28 ? 'Specialized Healthcare & Medical Education' : 'Primary & Secondary Healthcare Department';
      this.dropDowns.selectedFiltersModel.departmentDefault = profile.Department_Id == 28 ? { Name: 'Specialized Healthcare & Medical Education', Id: 28 } : profile.Department_Id == 25 ? { Name: 'Primary & Secondary Healthcare Department', Id: 25 } : this.dropDowns.selectedFiltersModel.department;

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
      this.application.DepartmentName = 'Primary & Secondary Healthcare Department';
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
  private getPandSOfficersInitiated = (type: string) => {
    this.officersInitiated = [];
    this._rootService.getPandSOfficers(type).subscribe((res: any) => {
      this.officersInitiated = res;
      this.officersInitiatedData = this.officersInitiated;
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
  public handleFilter = (value, filter) => {
    if (filter == 'markingOfficer') {
      this.officersData = this.officers.filter((s: any) => s.DesignationName.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
    if (filter == 'initiatedOfficer') {
      this.officersInitiatedData = this.officersInitiated.filter((s: any) => s.DesignationName.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
    if (filter == 'punjab') {
      this.punjabOfficersData = this.punjabOfficers.filter((s: any) => s.DesignationName.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
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




  public pageChange(event: PageChangeEvent): void {
    this.kGrid.skip = event.skip;
    this.getApplications();
  }
  public changePagesize(value: any) {
    this.kGrid.pageSize = +value;
    this.kGrid.skip = 0;
    this.getApplications();
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
  public clear() {
    let forwardingOfficer_Id = this.application.ForwardingOfficer_Id;
    let forwardingOfficerName = this.application.ForwardingOfficerName;
    let applicationType_Id = this.application.ApplicationType_Id;
    this.application = {};
    this.application.ApplicationType_Id = applicationType_Id;
    this.application.ForwardingOfficer_Id = forwardingOfficer_Id;
    this.application.ForwardingOfficerName = forwardingOfficerName;
  }
  public barcodeSrc() {
    return this.sanitizer.bypassSecurityTrustUrl(this.application.barcode);
  }
  public barcodesssssSrc(codeImage) {
    return this.sanitizer.bypassSecurityTrustUrl(codeImage);
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
