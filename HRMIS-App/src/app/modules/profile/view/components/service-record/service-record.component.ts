import { Component, OnInit, Input, OnDestroy, ViewChild } from '@angular/core';
import { ProfileService } from '../../../profile.service';
import { DomSanitizer } from '@angular/platform-browser';
import { RootService } from '../../../../../_services/root.service';
import { Subject } from 'rxjs/Subject';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { Subscription } from 'rxjs';
import { DropDownsHR } from '../../../../../_helpers/dropdowns.class';
import { NotificationService } from '../../../../../_services/notification.service';
import { AuthenticationService } from '../../../../../_services/authentication.service';

@Component({
  selector: 'app-profile-service-record',
  templateUrl: './service-record.component.html',
  styles: []
})
export class ServiceRecordComponent implements OnInit, OnDestroy {
  @Input() public profile: any;
  public serviceHistories: any[] = [];
  public serviceTotalHistroy: any[] = [];
  public orders: any[] = [];
  public leaveOrders: any[] = [];
  public selectedOrder: any;
  public viewOrderWindow: boolean = false;
  public searchingHfs: boolean = false;
  public saving: boolean = false;
  public hfsList: any[] = [];
  public searchEvent = new Subject<any>();
  public searchSubcription: Subscription;
  public serviceHistory: any = {};
  public addNew: boolean = false;
  public joiningDate: Date = new Date();
  public loading: boolean = false;
  public hfValue: string = '';
  public dropDowns: DropDownsHR = new DropDownsHR();
  public photoSrc = '';
  public photoSrces: any[] = [];
  public photoFile: any[] = [];
  public savingRemarks: boolean = false;
  public uploadingFile: boolean = false;
  public uploadingFileError: boolean = false;
  public currentUser: any = {};
  constructor(private sanitized: DomSanitizer,
    private _notificationService: NotificationService,
    private _authenticationService: AuthenticationService,
    private _rootService: RootService, private _profileService: ProfileService) { }

  ngOnInit() {
    this.currentUser = this._authenticationService.getUser();
    this.handleSearchEvents();
    this.getDesignations();
    this.getEmploymentModes();
    this.getServiceHistory();
    //this.getServiceDetails();
    this._profileService.getProfileDetail(this.profile.CNIC, 3).subscribe((data: any) => {
      this.orders = data;
    },
      err => {
        console.log(err);
      });
    this._profileService.getProfileDetail(this.profile.CNIC, 4).subscribe((data: any) => {
      this.leaveOrders = data;
    },
      err => {
        console.log(err);
      });
  }
  public getServiceHistory() {
    this._profileService.getServiceHistory(this.profile.Id).subscribe((data: any) => {
      if (data) {
        this.serviceHistories = data;
        this.addNew = false;
      }
    },
      err => {
        console.log(err);
      });
  }
  public lockServiceHistory() {
    if (confirm('Lock Service History of ' + this.profile.EmployeeName + ' ?')) {
      this.profile.locking = true;
      this._profileService.lockServiceHistory(this.profile.Id).subscribe((res: any) => {
        if (res == true) {
          this._notificationService.notify('success', 'Service History Locked!');
          this.profile.Tenure = 'Completed';
          this.profile.locking = false;
        }
        if (res == false) {
          this._notificationService.notify('success', 'Service History Unlocked!');
          this.profile.Tenure = 'Incomplete';
          this.profile.locking = false;
        }
      },
        err => {
          console.log(err);
        });
    }
  }
  public removeServiceHistory(item: any) {
    if (confirm('Confirm Remove Service History?')) {
      item.removing = true;
      this._profileService.removeServiceHistory(item.Id).subscribe((data: any) => {
        this.getServiceHistory();
        item.removing = false;
      },
        err => {
          console.log(err);
        });
    }
  }
  private getDesignations = () => {
    this.dropDowns.designations = [];
    this.dropDowns.designationsData = [];
    this._rootService.getDesignations().subscribe((res: any) => {
      this.dropDowns.designations = res;
      this.dropDowns.designationsData = this.dropDowns.designations.slice();
    },
      err => {
        console.log(err);
      }
    );
  }
  private getEmploymentModes = () => {
    this.dropDowns.employementModes = [];
    this.dropDowns.employementModesData = [];
    this._rootService.getEmploymentModes().subscribe((res: any) => {
      this.dropDowns.employementModes = res;
      this.dropDowns.employementModesData = this.dropDowns.employementModes;
    },
      err => {
        console.log(err);
      }
    );
  }
  public dropdownValueChanged = (value, filter) => {
    if (!value) {
      return;
    }
    if (filter == 'designation') {
      this.serviceHistory.Designation_Id = value.Id;
    }
    if (filter == 'employementMode') {
      this.serviceHistory.EmpMode_Id = value.Id;
    }
  }
  public handleFilter = (value, filter) => {
    if (filter == 'designation') {
      this.dropDowns.designationsData = this.dropDowns.designations.filter((s: any) => s.Name.toLowerCase().startsWith(value.toLowerCase()));
    }
    if (filter == 'employementMode') {
      this.dropDowns.employementModesData = this.dropDowns.employementModes.filter((s: any) => s.Name.toLowerCase().startsWith(value.toLowerCase()));
    }
  }
  public submitServiceHistory(dataItem?: any) {
    console.log(dataItem);

    this.saving = true;
    if (dataItem) {
      this.serviceHistory = dataItem;
      this.serviceHistory.saving = true;
    }
    this.serviceHistory.Profile_Id = this.profile.Id;
    if (this.serviceHistory.From_Date) {
      this.serviceHistory.From_Date = new Date(this.serviceHistory.From_Date).toDateString();
    }
    if (this.serviceHistory.To_Date) {
      this.serviceHistory.To_Date = new Date(this.serviceHistory.To_Date).toDateString();
    }
    if (this.serviceHistory.OrderDate) {
      this.serviceHistory.OrderDate = new Date(this.serviceHistory.OrderDate).toDateString();
    }
    this._profileService.saveServiceHistory(this.serviceHistory).subscribe((res: any) => {
      if (res && res.Id) {
        if (this.photoFile.length > 0) {
          this.uploadFile(res.Id);
        } else {
          this.serviceHistory = {};
          this.hfValue = '';
          this.dropDowns.selectedFiltersModel.designation = this.dropDowns.defultFiltersModel.designation;
          this.dropDowns.selectedFiltersModel.employementMode = this.dropDowns.defultFiltersModel.employementMode;
          this.saving = false;
          this.serviceHistory.saving = false;
          this.serviceHistory.contEdit = false;
          this._notificationService.notify('success', 'Service Record Saved!');
          this.getServiceHistory();
        }
      }
    }, err => {
      console.log(err);

    });
  }
  public confirmJoining(dataItem: any) {
    dataItem.confirming = true;
    if (confirm("Confirm Joining on " + this.joiningDate + " ?")) {
      this._profileService.confirmJoining(dataItem.Profile_Id, this.joiningDate.toDateString()).subscribe((res: boolean) => {
        if (res) {
          this._notificationService.notify('success', 'Joining Confirmed!');
          dataItem.confirming = false;
          this.getServiceHistory();
        }
      }, err => {
        console.log(err);
      });
    } else {
      dataItem.confirming = false;
    }
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

  public uploadBtn(filter: string) {
    if (filter == 'pic') {
      /* this.photoRef.nativeElement.click(); */
    }
  }
  public uploadFile(serviceHistory_Id: number) {
    this.uploadingFile = true;
    this._profileService.uploadServiceAttachement(this.photoFile, serviceHistory_Id).subscribe((x: any) => {
      if (!x) {
        this.uploadingFileError = true;
      }
      this.uploadingFile = false;
      this.serviceHistory = {};
      this.hfValue = '';
      this.dropDowns.selectedFiltersModel.designation = this.dropDowns.defultFiltersModel.designation;
      this.dropDowns.selectedFiltersModel.employementMode = this.dropDowns.defultFiltersModel.employementMode;
      this.saving = false;
      this._notificationService.notify('success', 'Service Record Saved!');
      this.getServiceHistory();
    }, err => {
      console.log(err);
      this.uploadingFileError = true;
      this.uploadingFile = false;
    });
  }
  public readUrlAndUpload(event: any, dataItem: any) {
    if (event.target.files && event.target.files[0]) {
      dataItem.photoFile = [];
      let inputValue = event.target;
      dataItem.photoFile = inputValue.files;
      dataItem.uploadingFile = true;
      this._profileService.uploadServiceAttachement(dataItem.photoFile, dataItem.Id).subscribe((x: any) => {
        if (!x) {
          dataItem.uploadingFileError = true;
        }
        dataItem.uploadingFile = false;
        this.serviceHistory = {};
        this.hfValue = '';
        this.dropDowns.selectedFiltersModel.designation = this.dropDowns.defultFiltersModel.designation;
        this.dropDowns.selectedFiltersModel.employementMode = this.dropDowns.defultFiltersModel.employementMode;
        this.saving = false;
        this.getServiceHistory();
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
  private getServiceDetails() {
    if (this.profile.DateOfBirth && this.profile.DateOfFirstAppointment) {
      this._profileService.getServiceDetails(this.profile.Id).subscribe((response: any) => {
        if (response) {
          this.serviceTotalHistroy = response.List;
        }
      }, err => {
        console.log(err);
      });
    }
  }
  public viewOrder(order) {
    this.selectedOrder = order;
    if (this.selectedOrder) {
      console.log(this.selectedOrder);

      this.openViewOrderWindow();
    }
  }
  public openViewOrderWindow() {
    this.viewOrderWindow = true;
  }
  public closeViewOrderWindow() {
    this.viewOrderWindow = false;
    this.selectedOrder = null;
  }

  public search(value: string, filter: string) {
    if (filter == 'hfs') {
      this.hfsList = [];
      if (value.length > 2) {
        this.searchingHfs = true;
        this._rootService.searchHealthFacilitiesAll(value).subscribe((data) => {
          this.hfsList = data as any[];
          this.searchingHfs = false;
        });
      }
    }
  }
  public searchClicked(FullName, filter) {

    if (filter == 'hfs') {
      let item = this.hfsList.find(x => x.FullName === FullName);
      if (item) {
        this.serviceHistory.HF_Id = item.Id;
      }
    }
    if (filter == 'whfs') {
      if (!FullName) {
        this.profile.WorkingHealthFacility_Id = null;
        this.profile.WorkingHFMISCode = null;
      }
    }
  }
  transform(value) {
    return this.sanitized.bypassSecurityTrustHtml(value);
  }
  public handleSearchEvents() {
    this.searchSubcription = this.searchEvent.pipe(
      debounceTime(400)).subscribe((x: any) => {
        this.search(x.event, x.filter);
      });
  }
  ngOnDestroy() {
    this.searchSubcription.unsubscribe();
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