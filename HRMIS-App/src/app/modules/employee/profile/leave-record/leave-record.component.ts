import { Component, OnInit, Input } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { DropDownsHR } from '../../../../_helpers/dropdowns.class';
import { NotificationService } from '../../../../_services/notification.service';
import { RootService } from '../../../../_services/root.service';
import { EmployeeService } from '../../employee.service';

@Component({
  selector: 'app-e-profile-leave-record',
  templateUrl: './leave-record.component.html',
  styles: []
})
export class EmployeeLeaveRecordComponent implements OnInit {
  @Input() public profile: any;
  public orders: any[] = [];
  public leaveOrders: any[] = [];
  public leaveRecord: any[] = [];
  public selectedOrder: any;
  public viewOrderWindow: boolean = false;
  public savingLeave: boolean = false;
  public loading: boolean = false;
  public leaveRecordTemp: any = {};
  public dropDowns: DropDownsHR = new DropDownsHR();
  public addNew: boolean = true;
  public currentUser: any = {};
  public photoSrc = '';
  public photoSrces: any[] = [];
  public photoFile: any[] = [];
  public savingRemarks: boolean = false;
  public uploadingFile: boolean = false;
  public uploadingFileError: boolean = false;
  constructor(private sanitized: DomSanitizer, private _rootService: RootService,
    private _notificationService: NotificationService,
    private _employeeService: EmployeeService) { }

  ngOnInit() {
    this.getLeaveRecords();
    this.getLeaveTypes();
  }
  public viewOrder(order) {
    this.selectedOrder = order;
    if (this.selectedOrder) {
      console.log(this.selectedOrder);

      this.openViewOrderWindow();
    }
  }

  public getLeaveRecords() {
    this.loading = true;
    this._employeeService.getLeaveRecord(this.profile.Id).subscribe((data: any) => {
      if (data) {
        this.leaveRecord = data;
        this.loading = false;
      }
    },
      err => {
        console.log(err);
      });
  }
  public leaveTypeChanged(value) {
    this.leaveRecordTemp.LeaveType_Id = value.Id;
  }
  public SaveProfileLeaves() {
    this.savingLeave = true;
    this.leaveRecordTemp.Profile_Id = this.profile.Id;
    this.leaveRecordTemp.FromDate = this.leaveRecordTemp.FromDate ? this.leaveRecordTemp.FromDate.toDateString() : this.leaveRecordTemp.FromDate;
    this.leaveRecordTemp.ToDate = this.leaveRecordTemp.ToDate ? this.leaveRecordTemp.ToDate.toDateString() : this.leaveRecordTemp.ToDate;
    this._employeeService.saveProfileLeave(this.leaveRecordTemp).subscribe((response: any) => {
      if (response) {
        this.getLeaveRecords();
        this._notificationService.notify('success', 'Leave Record Saved!');
        this.leaveRecordTemp = {};
        this.dropDowns.selectedFiltersModel.leaveType = this.dropDowns.defultFiltersModel.leaveType;
        this.dropDowns.selectedFiltersModel.applicationStatus = this.dropDowns.defultFiltersModel.applicationStatus;
        //this.getLeaves();
      }
      else {
        this._notificationService.notify('danger', 'Something went wrong!');
      }
      this.savingLeave = false;
    }, err => {
      this.savingLeave = false;
      console.log(err);

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
  } public removeLeaveRecord(item: any) {
    if (confirm('Confirm Remove Leave Record?')) {
      item.removing = true;
      this._employeeService.removeLeaveRecord(item.Id).subscribe((data: any) => {
        if (data) {
          this.getLeaveRecords();
        }
        item.removing = false;
      },
        err => {
          console.log(err);
        });
    }
  }
  public readUrlAndUpload(event: any, dataItem: any) {
    if (event.target.files && event.target.files[0]) {
      dataItem.photoFile = [];
      let inputValue = event.target;
      dataItem.photoFile = inputValue.files;
      dataItem.uploadingFile = true;
      this._employeeService.uploaLeaveAttachement(dataItem.photoFile, dataItem.Id).subscribe((x: any) => {
        if (!x) {
          dataItem.uploadingFileError = true;
        }
        dataItem.uploadingFile = false;
        this.getLeaveRecords();
        this._notificationService.notify('success', 'Leave Record Saved!');
        this.leaveRecordTemp = {};
        this.dropDowns.selectedFiltersModel.leaveType = this.dropDowns.defultFiltersModel.leaveType;
        this.dropDowns.selectedFiltersModel.applicationStatus = this.dropDowns.defultFiltersModel.applicationStatus;
        //this.getLeaves();
      }, err => {
        console.log(err);
        dataItem.uploadingFileError = true;
        dataItem.uploadingFile = false;
      });
    }
  }
  public openInNewTab(link: string) {
    window.open(link, '_blank');
  }
  private getLeaveTypes() {
    this.dropDowns.leaveTypes = [];
    this.dropDowns.leaveTypesData = [];
    this._rootService.getLeaveTypes().subscribe((res: any) => {
      this.dropDowns.leaveTypes = res;
      this.dropDowns.leaveTypesData = this.dropDowns.leaveTypes.slice();
    },
      err => {
        console.log(err);
      }
    );
  }
  public openViewOrderWindow() {
    this.viewOrderWindow = true;
  }
  public closeViewOrderWindow() {
    this.viewOrderWindow = false;
    this.selectedOrder = null;
  }
  public onChangeToDate(e) {
    if (this.leaveRecordTemp.ToDate != null && this.leaveRecordTemp.FromDate != null) {
      this.leaveRecordTemp.TotalDays = (this.leaveRecordTemp.ToDate.getTime() - this.leaveRecordTemp.FromDate.getTime()) / (1000 * 3600 * 24) + 1;
    }

  }
  transform(value) {
    return this.sanitized.bypassSecurityTrustHtml(value);
  }
  public dashifyCNIC(cnic: string) {
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
