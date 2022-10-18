import { Component, OnInit, Input } from '@angular/core';
import { AuthenticationService } from '../../../../../_services/authentication.service';
import { NotificationService } from '../../../../../_services/notification.service';
import { EmployeeService } from '../../../employee.service';

@Component({
  selector: 'app-staff-statement-place',
  templateUrl: './staff-statement.component.html',
  styles: []
})
export class StaffStatementComponent implements OnInit {
  @Input() public hfmisCode: string = '';
  public selectedStatus: any;
  public selectedDesignation: any;
  public selectedEmpMode: any;
  public loading: boolean = true;
  public loadingEmployeeProfiles: boolean = false;
  public joiningDialogOpened: boolean = true;
  public joiningDate: Date = new Date();
  @Input() public headOfDepartment: any;
  public type: number = 0;
  public id: number = 0;
  public name: string = '';
  public employeeStatuses: any[] = [];
  public employeeDesignationsAnnex: any[] = [];
  public employeeDesignations: any[] = [];
  public employeeDesignationsDisplay: any[] = [];
  public employeeEmpModes: any[] = [];
  public employeeProfiles: any[] = [];
  public employeeProfile: any[] = [];
  constructor(private _notificationService: NotificationService,
    private _employeeService: EmployeeService,
    
    private _authenticationService: AuthenticationService) {
  }

  ngOnInit() {
    this.getEmployeeStatuses();
    this.getEmployeeDesignations();
    this.getEmployeeEmpModes();
  }
  private getEmployeeStatuses(): void {
    this.loading = true;
    this._employeeService.getHFProfileStatuses(this.hfmisCode).subscribe((data: any) => {
      if (data) {
        this.employeeStatuses = data;
        if (this.selectedStatus && this.selectedStatus.Id == 30) {
          let tempstatus = this.employeeStatuses.find(x => x.Id == 2);
          if (tempstatus) {
            this.onSelectStatus(tempstatus);
          }
        }
        this.loading = false;
      }
    }, err => {
      this.handleError(err);
    });
  }
  private getEmployeeDesignations(): void {
    this.loading = true;
    this._employeeService.getHFProfileDesignations(this.hfmisCode).subscribe((data: any) => {
      if (data) {
        this.employeeDesignations = data;
        this.employeeDesignationsDisplay = this.employeeDesignations;
        this.designationsAnnex();
        if (this.selectedDesignation) {
          let tempDesignation = this.employeeDesignations.find(x => x.Id == 2);
          if (tempDesignation) {
            this.onSelectDesignation(tempDesignation);
          }
        }
        this.loading = false;
      }
    }, err => {
      this.handleError(err);
    });
  }
  private getEmployeeEmpModes(): void {
    this.loading = true;
    this._employeeService.getHFProfileEmpModes(this.hfmisCode).subscribe((data: any) => {
      if (data) {
        this.employeeEmpModes = data;
        if (this.selectedEmpMode) {
          let tempEmpMode = this.employeeEmpModes.find(x => x.Id == 2);
          if (tempEmpMode) {
            this.onSelectEmpMode(tempEmpMode);
          }
        }
        this.loading = false;
      }
    }, err => {
      this.handleError(err);
    });
  }
  private designationsAnnex() {
    let startAlphabet = '';
    this.employeeDesignations.forEach(desig => {
      if (desig.Name) {
        let desgStartAlphabet = desig.Name[0];
        if (desgStartAlphabet && desgStartAlphabet != startAlphabet) {
          startAlphabet = desig.Name[0];
          this.employeeDesignationsAnnex.push(desgStartAlphabet);
        }
      }
    });
  }
  public filterDesignationAlphabatically(alphabet: string) {
    return this.employeeDesignations.filter(x => x.Name && x.Name[0] == alphabet);
  }
  public searchDesignation(query: string) {
    return this.employeeDesignations.filter(x => x.Name.includes(query));
  }
  public onSelectStatus(status: any) {
    this.selectedStatus = status;
    this.selectedEmpMode = null;
    this.selectedDesignation = null;
    this.type = 1;
    this.id = this.selectedStatus.Id;
    this.name = this.selectedStatus.Name;
    this.employeeProfiles = [];
    this.getProfiles();
  }
  public onSelectDesignation(designation: any) {
    this.selectedDesignation = designation;
    this.selectedEmpMode = null;
    this.selectedStatus = null;
    this.type = 2;
    this.id = this.selectedDesignation.Id;
    this.name = this.selectedDesignation.Name;
    this.employeeProfiles = [];
    this.getProfiles();
  }
  public onSelectEmpMode(empMode: any) {
    this.selectedEmpMode = empMode;
    this.selectedDesignation = null;
    this.selectedStatus = null;
    this.type = 3;
    this.id = this.selectedEmpMode.Id;
    this.name = this.selectedEmpMode.Name;
    this.employeeProfiles = [];
    this.getProfiles();
  }
  private getProfiles() {
    this.loadingEmployeeProfiles = true;
    window.scroll(0, 0);
    this._employeeService.getHFProfiles(this.hfmisCode, this.type, this.id).subscribe((data: any) => {
      if (data) {
        this.employeeProfiles = data;
        this.loadingEmployeeProfiles = false;
      }
    }, err => {
      this.handleError(err);
    });
  }
  public openInNewTab(link: string) {
    window.open('https://hrmis.pshealthpunjab.gov.pk/' + link, '_blank');
  }
  public closeWindow() {
    this.joiningDialogOpened = false;
  }
  public openWindow(dataItem) {
    this.employeeProfile = dataItem;
    this.joiningDialogOpened = true;
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
  private handleError(err: any) {
    this.loading = false;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }
}
