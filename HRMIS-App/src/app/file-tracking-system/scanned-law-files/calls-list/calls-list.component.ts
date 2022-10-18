import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { FilesUpdated, DDS_Files, DDsDetail } from '../../file-search.class';
import { AuthenticationService } from '../../../_services/authentication.service';
import { FileTrackingSystemService } from '../../file-tracking-system.service';
import { NotificationService } from '../../../_services/notification.service';
import { RootService } from '../../../_services/root.service';
import { DomSanitizer } from '@angular/platform-browser';
import { Subscription } from 'rxjs/Subscription';
import { ActivatedRoute, Router } from '@angular/router';
import { KGridHelper } from '../../../_helpers/k-grid.class';
import { SortDescriptor, orderBy } from '@progress/kendo-data-query';
import { PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs/Subject';
import { debounceTime } from 'rxjs/operators/debounceTime';

@Component({
  selector: 'app-calls-list',
  templateUrl: './calls-list.component.html',
  styles: []
})
export class CallsListComponent implements OnInit, OnDestroy {
  @ViewChild('photoRef', { static: false }) public photoRef: any;
  public dropDowns: DropDownsHR = new DropDownsHR();
  public kGrid: KGridHelper = new KGridHelper();
  public maxDate = new Date(2000, 1, 1);
  public addingDDs: boolean = false;
  public loadingDDsFile: boolean = true;
  public isUploading: boolean = true;
  public addingFile: boolean = false;
  public hasError: boolean = false;
  public isNew: boolean = true;
  public conflicts: any = { name: false, number: false, cnic: false };
  public sectionOfficers: any[] = [];
  public sectionOfficersData: any[] = [];
  public lawWingReport: any[] = [];
  public lawOfficers: string[] = ['Legal Consultant 1', 'Legal Consultant 2', 'Legal Consultant 3', 'Law Officer 1', 'Law Officer 2', 'Law Officer 3'];
  public file: any = { concernedOfficers: [] };
  public barcodeImgSrc: string = '';
  public photoSrc: string = '';
  public photoFile: any[] = [];
  public fileAttachments: any[] = [];
  public petitioners: any[] = [];
  public concernedOfficers: any[] = [];
  public respondents: any[] = [];
  public judges: any[] = [];
  public selectedPetitioners: any[] = [];
  public selectedRespondents: any[] = [];
  private subscription: Subscription;
  public successDialogOpened: boolean = false;
  public searchEvent = new Subject<any>();
  public searchSubcription: Subscription = null;

  constructor(private sanitizer: DomSanitizer,
    private route: ActivatedRoute,
    private router: Router,
    public _notificationService: NotificationService, private _rootService: RootService,
    private _fileTrackingSystemService: FileTrackingSystemService,
    private _authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.fetchParams();
    this.getDesignations();
    this.handleSearchEvents();
    this.getPandSOfficers('all');
    this.getCallsList();
  }
  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('id') && +params['id']) {
          let id = +params['id'];
          this.getFile(id);
        }
        this.loadingDDsFile = false;
      }
    );
  }
  private getDesignations = () => {
    this._rootService.getDesignations().subscribe((res: any) => {
      this.dropDowns.designations = res;
      this.dropDowns.designationsData = this.dropDowns.designations.slice();
    },
      err => { this.handleError(err); }
    );
  }
  public submitDDs() {

  }
  public getFile(id) {
    this._fileTrackingSystemService.getLawFile(id).subscribe((res: any) => {
      if (res) {
        this.file = res;
        this.getLawFilePetitioners();
        this.getLawFileRespondants();
        this.getLawFileOfficers();
        this.getLawFileAttachments();
      }
    }, err => {
      this.handleError(err);
    })
  }
  public getLawFileAttachments() {
    this._fileTrackingSystemService.getLawFileAttachments(this.file.ID).subscribe((res: any) => {
      if (res) {
        this.fileAttachments = res;
      }
    }, err => {
      this.handleError(err);
    })
  }
  public removeLawFileAttachments(id) {
    if (confirm('Are you sure?')) {
      this._fileTrackingSystemService.removeLawFileAttachments(id).subscribe((res: any) => {
        if (res) {
          this.getLawFileAttachments();
        }
      }, err => {
        this.handleError(err);
      });
    }
  }
  public removeLawFile(id) {
    if (confirm('Are you sure?')) {
      this._fileTrackingSystemService.removeLawFile(id).subscribe((res: any) => {
        if (res) {
          this.router.navigate(['/fts/lawwing-files']);
        }
      }, err => {
        this.handleError(err);
      });
    }
  }
  public submitFile() {
    this.addingFile = true;
    this.hasError = false;
    if (this.file.NextDate) {
      this.file.NextDate = this.file.NextDate.toDateString();
    }
    if (this.file.LastDate) {
      this.file.LastDate = this.file.LastDate.toDateString();
    }
    this._fileTrackingSystemService.submitCallsList(this.file).subscribe((res: any) => {
      if (res && res.file) {
        this.file = res.file;
        this._notificationService.notify('success', 'Added Successfully');
        this.getCallsList();
      } else {
        this.hasError = true;
      }
      this.addingFile = false;
    }, err => {
      this.handleError(err);
    });
  }
  public getCallsList() {
    this._fileTrackingSystemService.getCallsList().subscribe((res: any) => {
      if (res) {
        this.lawWingReport = res;
      }
    }, err => {
      this.handleError(err);
    });
  }
  public uploadFile() {
    debugger;
    if (this.photoFile.length > 0) {
      this._fileTrackingSystemService.uploadLawFile(this.photoFile, this.file.ID).subscribe((res: any) => {
        if (res) {
          this.photoFile = [];
          this._notificationService.notify('success', 'File Upload Successfull');
          this.getLawFileAttachments();
        }
      }, err => {
        this.handleError(err);
      });
    }
  }
  public handleSearchEvents() {
    this.searchSubcription = this.searchEvent.pipe(
      debounceTime(400)).subscribe((x: any) => {
        this.search(x.event, x.filter);
      });
  }
  public dropdownValueChanged = (value, filter) => {
    if (filter == 'office') {
      this.file.Section_Id = value.Id;
      this.file.SectionName = value.DesignationName;
    }
  }
  public search(value: string, filter: string) {
    if (filter == 'office') {
      this.sectionOfficersData = [];
      if (value.length > 2) {
        this.sectionOfficersData = this.sectionOfficers.filter(x => x.DesignationName && x.DesignationName.toLowerCase().startsWith(value));
      }
    }
    if (filter == 'concernedOfficers') {
      this.sectionOfficersData = [];
      if (value.length > 2) {
        this.sectionOfficersData = this.sectionOfficers.filter(x => x.DesignationName && x.DesignationName.toLowerCase().startsWith(value));
      }
    }
  }
  public searchClicked(name, filter) {
    if (filter == 'concernedOfficers') {
      let item = this.concernedOfficers.find(x => x.Name === name);
      if (item) {
        this.file.concernedOfficers.push(item);
      }
    }
    if (filter == 'respondent') {
      let item = this.respondents.find(x => x.Name === name);
      if (item) {
        this.file.respondents.push(item);
      }
    }
    if (filter == 'judge') {
      let item = this.judges.find(x => x.Name === name);
      if (item) {
        this.file.judges.push(item);
      }
    }
    if (filter == 'officer') {
      let item = this.sectionOfficers.find(x => x.DesignationName === name);
      if (item) {
        this.file.officers.push(item);
      }
    }
    if (filter == 'concernedOfficers') {
      let item = this.sectionOfficers.find(x => x.DesignationName === name);
      if (item) {
        this.file.concernedOfficers.push(item);
      }
    }
  }

  public removeSelected(index, filter) {
    if (filter == 'concernedOfficers') {
      this.file.concernedOfficers.splice(index, 1);
    }
    if (filter == 'respondent') {
      this.file.respondents.splice(index, 1);
    }
    if (filter == 'judge') {
      this.file.judges.splice(index, 1);
    }
    if (filter == 'officer') {
      this.file.officers.splice(index, 1);
    }
  }
  private getPandSOfficers = (type: string) => {
    this._rootService.getPandSOfficers(type).subscribe((res: any) => {
      this.sectionOfficers = res;
      this.sectionOfficersData = this.sectionOfficers;
    },
      err => { this.handleError(err); }
    );
  }
  private getLawFilePetitioners = () => {
    this._fileTrackingSystemService.getLawFilePetitioners(this.file.ID).subscribe((res: any) => {
      if (res) {
        this.file.petitioners = res;
      }
    },
      err => { this.handleError(err); }
    );
  }
  private getLawFileRespondants = () => {
    this._fileTrackingSystemService.getLawFileRespondants(this.file.ID).subscribe((res: any) => {
      if (res) {
        this.file.respondents = res;
      }
    },
      err => { this.handleError(err); }
    );
  }
  private getLawFileOfficers = () => {
    this._fileTrackingSystemService.getLawFileOfficers(this.file.ID).subscribe((res: any) => {
      if (res) {
        this.file.officers = res;
      }
    },
      err => { this.handleError(err); }
    );
  }
  public barcodeSrc() {
    return this.sanitizer.bypassSecurityTrustUrl(this.barcodeImgSrc);
  }

  public handleFilter = (value, filter) => {
    if (filter == 'office') {
      this.sectionOfficersData = this.sectionOfficers.filter((s: any) => s.DesignationName.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }

  }
  public uploadBtn() {
    this.photoRef.nativeElement.click();
  }
  public readUrl(event: any) {
    if (event.target.files && event.target.files[0]) {
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
  public openSuccessDialog() {
    this.successDialogOpened = true;
  }
  public closeSuccessDialog() {
    this.successDialogOpened = false;
  }
  public dialogAction() {
    this.file = {};
    //this.router.navigate(['/fts/lawwing-files']);
    this.closeSuccessDialog();
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

      /*     mywindow.document.close(); // necessary for IE >= 10
          mywindow.focus(); // necessary for IE >= 10
          mywindow.print();
          mywindow.close(); */
    }
  }
  private handleError(err: any) {
    this.addingDDs = false;
    this.addingFile = false;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }
  ngOnDestroy() {
    this.subscription.unsubscribe();
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
