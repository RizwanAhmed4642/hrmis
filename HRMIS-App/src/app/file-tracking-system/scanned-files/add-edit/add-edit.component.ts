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
import { ApplicationProfileViewModel } from '../../../modules/application-fts/application-fts';

@Component({
  selector: 'app-add-edit',
  templateUrl: './add-edit.component.html',
  styles: []
})
export class AddEditComponent implements OnInit, OnDestroy {
  @ViewChild('photoRef', { static: false }) public photoRef: any;
  public dropDowns: DropDownsHR = new DropDownsHR();
  public kGrid: KGridHelper = new KGridHelper();
  public maxDate = new Date(2000, 1, 1);
  public isDDS: boolean = false;
  public addingDDs: boolean = false;
  public loadingDDsFile: boolean = true;
  public isUploading: boolean = true;
  public addingFile: boolean = false;
  public hasError: boolean = false;
  public uploading: boolean = false;
  public isNew: boolean = true;
  public conflicts: any = { name: false, number: false, cnic: false };
  public sectionOfficers: any[] = [];
  public dds: any = {};
  public ddsDetail: DDsDetail = new DDsDetail();
  public ddsDetails: DDsDetail[] = [];
  public fileUpdated: any = {};
  public fileTypeItems: any[] = [
    { text: 'Select File Type', value: null },
    { text: 'ACR', value: 1 },
  ]
  public fileError: string = '';
  public documentTypeItems: any[] = ['ACR', 'Certificate in lieu of ACR', 'Gap Period Order', 'Leave Order'];
  public cnicMask: string = "00000-0000000-0";
  public nicMask: string = "000-00-000000";
  public batchMask: string = "0000";
  public barcodeImgSrc: string = '';
  public photoSrc: string = '';
  public photoFile: any[] = [];
  private subscription: Subscription;
  public successDialogOpened: boolean = false;
  public fileAttachments: any[] = [];
  constructor(private sanitizer: DomSanitizer,
    private route: ActivatedRoute,
    private router: Router,
    public _notificationService: NotificationService, private _rootService: RootService, private _fileTrackingSystemService: FileTrackingSystemService, private _authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.ddsDetail.Remarks = 'ACR';
    this.fetchParams();
    this.getDesignations();
    this.getPandSOfficers('section');
  }
  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {

        if (params.hasOwnProperty('id') && +params['id']) {
          let id = +params['id'];
          this.fileUpdated.Id = id;
          this.getFile();
        } else {
          this.dds = new DDS_Files();
          this.isNew = true;
          if (params.hasOwnProperty('filetype')) {
            let filetype = +params['filetype'];
            if (filetype == 1) {
              this.dds.F_FileType_Id = filetype;
            } else {
              this.dds.F_FileType_Id = 0;
            }
          }
          else {
            this.fileUpdated.F_FileType_Id = 0;
          }
          this.loadingDDsFile = false;
        }

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
  public searchProfile = () => {
    if (!this.dds.F_CNIC) {
      return;
    }
    this._fileTrackingSystemService.searchProfile(this.dds.F_CNIC).subscribe((data: any) => {
      if (data == 'Invalid') {
      }
      else if (data && data != 'Invalid') {
        this.mapProfileToDDS(data);
      }
    });
  }
  public submitDDs() {
    console.log(this.dds);
  }
  public getFile() {

    this.loadingDDsFile = true;
    this._rootService.getDDSFileById(this.fileUpdated.Id).subscribe((dds: any) => {
      if (dds.Id) {
        dds.F_DateOfBirth = dds.F_DateOfBirth ? new Date(dds.F_DateOfBirth) : dds.F_DateOfBirth;
        dds.F_DateOfJoining = dds.F_DateOfJoining ? new Date(dds.F_DateOfJoining) : dds.F_DateOfJoining;
        dds.F_BatchNo = dds.F_BatchNo ? dds.F_BatchNo.toString() : dds.F_BatchNo;
        dds.Status = dds.Status ? dds.Status.toString() : dds.Status;
        dds.Status = dds.Status == 7 ? null : dds.Status;
        this.dds = dds;
        this.getDDSDetails();
        this.getFileAttachments();
        this.photoSrc = 'https://hrmis.pshealthpunjab.gov.pk/Uploads/Files/ACRFiles/' + this.dds.Id + '.jpeg';
        if (this.dds.Subject && this.dds.F_Name !== this.dds.Subject) {
          this.conflicts.name = true;
        }
        if (this.dds.DiaryNo && this.dds.F_FileNumber !== this.dds.DiaryNo) {
          this.conflicts.number = true;
        }
        if (this.dds.FileType && this.dds.F_CNIC !== this.dds.FileType) {
          this.conflicts.cnic = true;
        }
        this._rootService.getDDsBarcode(this.dds.Id).subscribe((res: any) => {
          if (res) {
            this.barcodeImgSrc = res.barCode;
          }
        }, err => {
          this.handleError(err);
        })
        this.isNew = false;
        this.loadingDDsFile = false;
        //this.mapDDsFileToFileUpdated(res);
      }
    }, err => {
      this.handleError(err);
    })
  }
  private getDDSDetails() {
    if (this.dds.F_DateOfBirth && this.dds.F_DateOfJoining) {
      this._rootService.getDDSDetails(this.dds.Id, this.kGrid.skip, this.kGrid.pageSize).subscribe((response: any) => {
        if (response) {
          this.kGrid.data = response.List;
          this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.data.length };
          this.kGrid.loading = false;
        }
      }, err => {
        this.handleError(err);
      });
    }
  }
  public saveDDSDetail() {
    this.ddsDetail.DDS_Id = this.dds.Id;
    let clone = Object.assign({}, this.ddsDetail) as any;
    clone.FromPeriod = clone.FromPeriod.toDateString();
    clone.ToPeriod = clone.ToPeriod.toDateString();
    if (clone.DCPDate) {
      clone.DCPDate = clone.ToPeriod.toDateString();
    }
    this._rootService.saveDDSDetail(clone).subscribe((res: any) => {
      if (res && res.Id) {
        this.ddsDetail = new DDsDetail();
        this._notificationService.notify('success', 'File Detail Saved.');
        this.getDDSDetails();
      }
    }, err => {
      this.handleError(err);
    });
  }
  public removeDDSDetail(detail_Id: number) {
    if (confirm('Are you sure?')) {
      this._rootService.removeDDSDetail(detail_Id).subscribe((res: any) => {
        if (res) {
          this._notificationService.notify('warning', 'File Detail Removed.');
          this.getDDSDetails();
        }
      }, err => {
        this.handleError(err);
      });
    }
  }
  public removeConflict(type: number, value: string) {
    if (type == 1) {
      this.dds.F_Name = value;
      this.dds.Subject = value;
      this.conflicts.name = false;
    }
    if (type == 2) {
      this.dds.F_FileNumber = value;
      this.dds.DiaryNo = value;
      this.conflicts.number = false;
    }
    if (type == 3) {
      this.dds.F_CNIC = value;
      this.dds.FileType = value;
      this.conflicts.cnic = false;
    }
  }
  public submitFile() {
    this.addingFile = true;
    this.hasError = false;
    if (this.dds.F_DateOfBirth) {
      this.dds.F_DateOfBirth = this.dds.F_DateOfBirth.toDateString();
    }
    if (this.dds.F_DateOfJoining) {
      this.dds.F_DateOfJoining = this.dds.F_DateOfJoining.toDateString();
    }
    this._fileTrackingSystemService.addFileACR(this.dds).subscribe((res: any) => {
      if (res && res.barCode && res.ddsFile.Id) {
        this.dds = res.ddsFile as DDS_Files;
        this.barcodeImgSrc = res.barCode;
        if (this.photoFile.length > 0) {
          this._fileTrackingSystemService.uploadFileAttachment(this.photoFile, this.dds.Id).subscribe((res: any) => {
            if (res) {
              this._notificationService.notify('success', 'File Added Successfully');
              this.openSuccessDialog();
            }
          }, err => {
            this.handleError(err);
          });
        } else {
          this._notificationService.notify('success', 'File Added Successfully');
          this.openSuccessDialog();
        }
      } else {
        this.hasError = true;
      }
      this.addingFile = false;
    }, err => {
      this.handleError(err);
    })
  }
  private getPandSOfficers = (type: string) => {
    this.sectionOfficers = [];
    this._rootService.getPandSOfficers(type).subscribe((res: any) => {
      this.sectionOfficers = res;
    },
      err => { this.handleError(err); }
    );
  }
  public barcodeSrc() {
    return this.sanitizer.bypassSecurityTrustUrl(this.barcodeImgSrc);
  }

  public mapProfileToApplicant = (profile) => {
    if (profile) {
      this.fileUpdated.Name = profile.EmployeeName;
      this.fileUpdated.DateOfBirth = profile.DateOfBirth ? new Date(profile.DateOfBirth) : new Date(2000, 1, 1);
      this.fileUpdated.BPS = profile.CurrentGradeBPS ? profile.CurrentGradeBPS : 0;
      this.fileUpdated.DateOfJoining = profile.DateOfFirstAppointment ? profile.DateOfFirstAppointment : 0;

      let designations = this.dropDowns.designations as any[];
      if (designations) {
        let designation = designations.find(x => x.Id == profile.Designation_Id);
        if (designation) {
          this.fileUpdated.Designation_Id = profile.Designation_Id ? profile.Designation_Id : this.fileUpdated.Designation_Id;
          this.dropDowns.selectedFiltersModel.designation = { Name: designation.Name, Id: designation.Name.Id };
        }
      }
    }
  }

  public mapProfileToDDS = (profile: ApplicationProfileViewModel) => {
    if (profile) {
      this.dds.F_Name = profile.EmployeeName ? profile.EmployeeName : '';
      this.dds.Subject = profile.EmployeeName ? profile.EmployeeName : '';
      this.dds.F_FileNumber = '';
      this.dds.DiaryNo = '';
      this.dds.F_DateOfBirth = profile.DateOfBirth ? new Date(profile.DateOfBirth) : new Date(2000, 1, 1);
      this.dds.F_Designation_Id = profile.Designation_Id;
      this.dds.F_CNIC = profile.CNIC ? profile.CNIC : '';
      this.dds.FileType = profile.CNIC ? profile.CNIC : '';
      this.dds.F_BPS = profile.CurrentGradeBPS ? profile.CurrentGradeBPS : 0;
      this.dds.F_DateOfJoining = profile.JoiningDate ? profile.JoiningDate : 0;
      this.dds.FileType_Id = 1;
      this.dds.F_FileType_Id = 1;
      console.log(this.dds);

    }
  }
  public mapDDsFileToFileUpdated = (ddsFile) => {
    this.fileUpdated.Name = ddsFile.Subject;
    this.fileUpdated.FileNumber = ddsFile.DiaryNo;
    this.fileUpdated.CNIC = ddsFile.FileType;
    this.fileUpdated.NIC = ddsFile.FileNIC;
    this.fileUpdated.Rack = ddsFile.F_Rack;
    this.fileUpdated.Shelf = ddsFile.F_Shelf;
    this.fileUpdated.DateOfBirth = ddsFile.DateOfBirth ? new Date(ddsFile.DateOfBirth) : new Date(2000, 1, 1);
    this.fileUpdated.FileType_Id = ddsFile.FileType_Id ? ddsFile.FileType_Id : 0;
    this.fileUpdated.Designation_Id = ddsFile.F_Designation_Id ? ddsFile.F_Designation_Id : 0;
    this.fileUpdated.DateOfJoining = ddsFile.DateOfJoining ? new Date(ddsFile.DateOfJoining) : new Date(2000, 1, 1);
    this.fileUpdated.BPS = ddsFile.F_BPS;
    this.fileUpdated.BatchNo = ddsFile.F_BatchNo.toString();
    this.fileUpdated.Section_Id = ddsFile.F_Section_Id;
    /*  let designations = this.dropDowns.designations as any[];
     if (designations) {
       let designation = designations.find(x => x.Id == ddsFile.Designation_Id);
       if (designation) {
         this.dropDowns.selectedFiltersModel.designation = { Name: designation.Name, Id: designation.Name.Id };
       }
     } */
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
  public uploadFile() {
    this.uploading = true;
    this.fileError = '';
    if (this.photoFile.length > 0) {
      this._fileTrackingSystemService.uploadFileAttachment(this.photoFile, this.dds.Id).subscribe((res: any) => {
        if (res) {
          this.photoFile = [];
          this._notificationService.notify('success', 'File Upload Successfull');
          this.getFileAttachments();
        }
        this.uploading = false;
      }, err => {
        this.uploading = false;
        this.fileError = err.error.Message;
        this.handleError(err);
      });
    }
  }
  public getFileAttachments() {
    this._fileTrackingSystemService.getFileAttachments(this.dds.Id).subscribe((res: any) => {
      if (res) {
        this.fileAttachments = res;
      }
    }, err => {
      this.handleError(err);
    })
  }
  public removeFileAttachments(id) {
    if (confirm('Are you sure?')) {
      this._fileTrackingSystemService.removeFileAttachments(id).subscribe((res: any) => {
        if (res) {
          this.getFileAttachments();
        }
      }, err => {
        this.handleError(err);
      });
    }
  }
  public openSuccessDialog() {
    this.successDialogOpened = true;
  }
  public closeSuccessDialog() {
    this.successDialogOpened = false;
  }
  public dialogAction(action: string) {
    if (action == 'yes') {
      this.dds = new DDS_Files();
      this.router.navigate(['/fts/scanned-files-new' + (this.dds.F_FileType_Id == 1 ? '/1' : '')]);
      this.closeSuccessDialog();
    }
    else if (action == 'no') {
      this.router.navigate(['/fts/scanned-files-new/' + (this.dds.F_FileType_Id == 1 ? '1' : '0') + '/' + this.dds.Id + '/edit']);
      this.closeSuccessDialog();
    }
    else {
      this.closeSuccessDialog();
    }
  }
  public sortChange(sort: SortDescriptor[]): void {
    if (sort[0].field == "asd") {
      return;
    }
    console.log(sort);

    this.kGrid.sort = sort;
    this.sortData();
  }
  private sortData() {
    this.kGrid.gridView = {
      data: orderBy(this.kGrid.data, this.kGrid.sort),
      total: this.kGrid.totalRecords
    };
  }
  public pageChange(event: PageChangeEvent): void {
    this.kGrid.skip = event.skip;
    this.getDDSDetails();
  }
  public changePagesize(value: any) {
    this.kGrid.pageSize = +value;
    this.kGrid.skip = 0;
    this.getDDSDetails();
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
