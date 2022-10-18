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
  selector: 'app-add-edit-file',
  templateUrl: './add-edit-file.component.html',
  styles: []
})
export class AddEditFileComponent implements OnInit, OnDestroy {
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
  public file: any = { petitioners: [], respondents: [], judges: [], officers: [] };
  public barcodeImgSrc: string = '';
  public photoSrc: string = '';
  public photoFile: any[] = [];
  public fileAttachments: any[] = [];
  public petitioners: any[] = [];
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
    this.getPandSOfficers('fts');
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
        console.log(this.file);
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
    this._fileTrackingSystemService.addLawFile(this.file).subscribe((res: any) => {
      if (res && res.file && res.barCode) {
        this.file = res.file;
        this.barcodeImgSrc = res.barCode;
        this._notificationService.notify('success', 'File Added Successfully');
        this.openSuccessDialog();
      } else {
        this.hasError = true;
      }
      this.addingFile = false;
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

  }
  public search(value: string, filter: string) {
    if (filter == 'petitioner') {
      this.petitioners = [];
      if (value.length > 2) {
        this._fileTrackingSystemService.getPetitioners(value).subscribe((data) => {
          this.petitioners = data as any[];
        });
      }
    }
    if (filter == 'respondent') {
      this.respondents = [];
      if (value.length > 2) {
        this._fileTrackingSystemService.getRepresentatives(value).subscribe((data) => {
          this.respondents = data as any[];
        });
      }
    }
    if (filter == 'judge') {
      this.judges = [];
      if (value.length > 2) {
        this._fileTrackingSystemService.getJudges(value).subscribe((data) => {
          this.judges = data as any[];
        });
      }
    }
    if (filter == 'officer') {
      this.sectionOfficersData = [];
      if (value.length > 2) {
        this.sectionOfficersData = this.sectionOfficers.filter(x => x.DesignationName && x.DesignationName.toLowerCase().startsWith(value));
      }
    }
  }
  public searchClicked(name, filter) {
    if (filter == 'petitioner') {
      let item = this.petitioners.find(x => x.Name === name);
      if (item) {
        this.file.petitioners.push(item);
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
  }

  public removeSelected(index, filter) {
    if (filter == 'petitioner') {
      this.file.petitioners.splice(index, 1);
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
}
