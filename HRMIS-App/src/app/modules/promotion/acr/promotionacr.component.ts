import { Component, Input, OnInit } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { NotificationService } from '@progress/kendo-angular-notification';
import { DDsDetail } from '../../../file-tracking-system/file-search.class';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { KGridHelper } from '../../../_helpers/k-grid.class';
import { RootService } from '../../../_services/root.service';
import { ProfileService } from '../../profile/profile.service';
import { PromotionService } from '../promotion.service';

@Component({
  selector: 'app-promotionacr',
  templateUrl: './promotionacr.component.html',
  styleUrls: ['./promotionacr.component.scss']
})
export class PromotionacrComponent implements OnInit {
  public profile: any;
  CNIC : string = "3310023123933";
  public file: any;
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
  public DDSDeatils : any[] = [];

  constructor(private _rootService: RootService, private sanitizer: DomSanitizer, private _promotionService: PromotionService) { }

  ngOnInit() {
    
    this._promotionService.getProfileDetail(this.CNIC, 1).subscribe((data: any) => {
      this.file = data;
      debugger;
      if (this.file && this.file.Id) {
        this.file.DateOfBirth = this.file.DateOfBirth ? new Date(this.file.DateOfBirth) : null;
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
          this.DDSDeatils = response.List;
          this.kGrid.loading = false;
        }
    },
    err => 
    {
      this.handleError(err);
    });
  }

  public getFileAttachments() {
    this._promotionService.getFileAttachments(this.dds.Id).subscribe((res: any) => {
      if (res) {
        this.fileAttachments = res;
      }
    }, err => {
      this.handleError(err);
    })
  }
 

  private handleError(err: any) {
    this.addingDDs = false;
    this.addingFile = false;
  }

}
