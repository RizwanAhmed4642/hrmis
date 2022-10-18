import { Component, OnInit, Input } from '@angular/core';
import { HealthFacilityService } from '../../../health-facility.service';
import { AuthenticationService } from '../../../../../_services/authentication.service';
import { NotificationService } from '../../../../../_services/notification.service';

@Component({
  selector: 'app-staff-statement',
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
  public employeeProfilesPJ: any[] = [];
  public employeeProfile: any[] = [];
  constructor(private _notificationService: NotificationService, private _authenticationService: AuthenticationService, private _healthFacilityService: HealthFacilityService) {
  }

  ngOnInit() {
    this.getEmployeeStatuses();
    this.getEmployeeDesignations();
    this.getEmployeeEmpModes();
  }
  private getEmployeeStatuses(): void {
    this.loading = true;
    this._healthFacilityService.getHFProfileStatuses(this.hfmisCode).subscribe((data: any) => {
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
    this._healthFacilityService.getHFProfileDesignations(this.hfmisCode).subscribe((data: any) => {
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
    this._healthFacilityService.getHFProfileEmpModes(this.hfmisCode).subscribe((data: any) => {
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
    this._healthFacilityService.getHFProfiles(this.hfmisCode, this.type, this.id).subscribe((data: any) => {
      if (data) {
        this.employeeProfiles = data;
        this.loadingEmployeeProfiles = false;
        if (this.selectedStatus.Id == 30) {
          let ids: number[] = [];
          this.employeeProfiles.forEach((x: any) => {
            ids.push(x.Id);
          });
          this._healthFacilityService.getProfilePJHistory(ids).subscribe((data2: any) => {
            if (data) {
              console.log(data2);
              this.employeeProfilesPJ = data2;
              this.employeeProfilesPJ.forEach((x: any) => {
                let prof = this.employeeProfiles.find(k => k.Id == x.Profile_Id);
                if (prof) {
                  prof.OrderDatePj = x.OrderDate;
                }
              });
            }
          }, err => {
            this.handleError(err);
          });
        }
      }
    }, err => {
      this.handleError(err);
    });
  }
  public openInNewTab(link: string) {
    window.open('/' + link, '_blank');
  }
  public confirmJoining(id: number) {
    this._healthFacilityService.confirmJoining(id, this.joiningDate.toDateString()).subscribe((res: boolean) => {
      if (res) {
        this._notificationService.notify('success', 'Joining Confirmed!');
        this.getEmployeeStatuses();
      }
    }, err => {
      this.handleError(err);
    })
  }
  public notJoined(id: number) {
    this._healthFacilityService.notJoined(id).subscribe((res: boolean) => {
      if (res) {
        this._notificationService.notify('success', 'Joining Confirmed!');
        this.getEmployeeStatuses();
      }
    }, err => {
      this.handleError(err);
    })
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
