import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { DDsDetail } from '../../../../../file-tracking-system/file-search.class';
import { DropDownsHR } from '../../../../../_helpers/dropdowns.class';
import { KGridHelper } from '../../../../../_helpers/k-grid.class';
import { NotificationService } from '../../../../../_services/notification.service';
import { RootService } from '../../../../../_services/root.service';
import { ProfileService } from '../../../profile.service';

@Component({
  selector: 'app-profile-file',
  templateUrl: './file.component.html',
  styles: []
})
export class FileComponent implements OnInit {
  @Input() public profile: any;
  public file: any;
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
  public successDialogOpened: boolean = false;
  public fileAttachments: any[] = [];
  constructor(private _rootService: RootService, private sanitizer: DomSanitizer, public _notificationService: NotificationService, private _profileService: ProfileService) { }

  ngOnInit() {

    this._profileService.getProfileDetail(this.profile.CNIC, 1).subscribe((data: any) => {
      this.file = data;
      if (this.file && this.file.Id) {
        this.getFile();
      }
    },
    err => {
      console.log(err);
    });

  }

  public getFile() {

    this.loadingDDsFile = true;
    this._rootService.getDDSFileById(this.file.Id).subscribe((dds: any) => {
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
    this._rootService.getDDSDetails(this.dds.Id, this.kGrid.skip, this.kGrid.pageSize).subscribe((response: any) => {
        if (response) {
          this.kGrid.data = response.List;
          this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.data.length };
          this.kGrid.loading = false;
        }
    },
    err => 
    {
      this.handleError(err);
    });
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
      this._profileService.uploadFileAttachment(this.photoFile, this.dds.Id).subscribe((res: any) => {
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
    this._profileService.getFileAttachments(this.dds.Id).subscribe((res: any) => {
      if (res) {
        this.fileAttachments = res;
      }
    }, err => {
      this.handleError(err);
    })
  }
  public removeFileAttachments(id) {
    if (confirm('Are you sure?')) {
      this._profileService.removeFileAttachments(id).subscribe((res: any) => {
        if (res) {
          this.getFileAttachments();
        }
      }, err => {
        this.handleError(err);
      });
    }
  }
  public barcodeSrc() {
    return this.sanitizer.bypassSecurityTrustUrl(this.barcodeImgSrc);
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
    this.addingDDs = false;
    this.addingFile = false;
  }
}
