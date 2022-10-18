import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { Subject } from 'rxjs/Subject';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { Subscription } from 'rxjs';
import { RootService } from '../../../../_services/root.service';
import { EmployeeService } from '../../employee.service';
import { DropDownsHR } from '../../../../_helpers/dropdowns.class';

@Component({
  selector: 'app-e-profile-qualification',
  templateUrl: './qualification.component.html',
  styles: []
})
export class QualificationComponent implements OnInit, OnDestroy {
  @Input() public profile: any;
  public serviceHistories: any[] = [];
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
  public addNew: boolean = true;
  public loading: boolean = false;
  public hfValue: string = '';
  public qualificationTypeId: number = 0;
  public serviceTotalHistroy: any[] = [];
  public joiningDate: Date = new Date();
  public dropDowns: DropDownsHR = new DropDownsHR();
  public photoSrc = '';
  public photoSrces: any[] = [];
  public photoFile: any[] = [];
  public savingRemarks: boolean = false;
  public uploadingFile: boolean = false;
  public uploadingFileError: boolean = false;
  public currentUser: any = {};
  constructor(private sanitized: DomSanitizer, private _rootService: RootService, private _employeeService: EmployeeService) { }

  ngOnInit() {
    this.handleSearchEvents();
    this.getQualificationTypes();
    this.getHrQualification();
  }
  public getHrQualification() {
    this._employeeService.getHrQualification(this.profile.Id).subscribe((data: any) => {
      if (data) {
        this.serviceHistories = data;
      }
    },
      err => {
        console.log(err);
      });
  }
  public submitServiceHistory() {
    this.saving = true;
    this.serviceHistory.Profile_Id = this.profile.Id;
    if (this.serviceHistory.PassingYear) {
      this.serviceHistory.PassingYear = this.serviceHistory.PassingYear.toDateString();
    }
    this._employeeService.saveHrQualification(this.serviceHistory).subscribe((res: any) => {
      if (res.Id) {
        this.serviceHistory = {};
        this.hfValue = '';
        this.saving = false;
        this.getHrQualification();
      }
    }, err => {
      console.log(err);

    });
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
  private getQualificationTypes = () => {
    this.dropDowns.qualificationTypes = [];
    this._rootService.getQualificationType().subscribe((res: any) => {
      this.dropDowns.qualificationTypes = res;
    },
      err => {
        console.log(err);
      }
    );
  }
  private getDegrees = (qualificationId: number) => {
    this.dropDowns.degrees = [];
    this._rootService.getDegrees(qualificationId).subscribe((res: any) => {
      this.dropDowns.degrees = res;
    },
      err => {
        console.log(err);
      }
    );
  }
  public readUrlAndUpload(event: any, dataItem: any) {
    if (event.target.files && event.target.files[0]) {
      dataItem.photoFile = [];
      let inputValue = event.target;
      dataItem.photoFile = inputValue.files;
      dataItem.uploadingFile = true;
      this._employeeService.uploadServiceAttachement(dataItem.photoFile, dataItem.Id).subscribe((x: any) => {
        if (!x) {
          dataItem.uploadingFileError = true;
        }
        dataItem.uploadingFile = false;
        this.serviceHistory = {};
        this.hfValue = '';
        this.dropDowns.selectedFiltersModel.designation = this.dropDowns.defultFiltersModel.designation;
        this.dropDowns.selectedFiltersModel.employementMode = this.dropDowns.defultFiltersModel.employementMode;
        this.saving = false;
        this.getHrQualification();
      }, err => {
        console.log(err);
        dataItem.uploadingFileError = true;
        dataItem.uploadingFile = false;
      });
    }
  }
  public dropdownValueChanged = (value, filter) => {
    if (!value) {
      return;
    }
    if (filter == 'qualificationType') {
      this.qualificationTypeId = value.Id;
      this.serviceHistory.QualificationTypeId = this.qualificationTypeId;
      this.getDegrees(this.qualificationTypeId);
    }
    if (filter == 'degree') {
      this.serviceHistory.Required_Degree_Id = value.Id;
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
  public openInNewTab(link: string) {
    window.open(link, '_blank');
  }
  public removeServiceHistory(item: any) {
    if (confirm('Confirm Remove Qualification?')) {
      item.removing = true;
      this._employeeService.removeHrQualification(item.Id).subscribe((data: any) => {
        this.getHrQualification();
        item.removing = false;
      },
        err => {
          console.log(err);
        });
    }
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