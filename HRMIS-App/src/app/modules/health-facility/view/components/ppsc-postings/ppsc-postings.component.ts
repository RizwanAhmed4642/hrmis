import { Component, OnInit, Input } from '@angular/core';
import { HealthFacilityService } from '../../../health-facility.service';
import { AuthenticationService } from '../../../../../_services/authentication.service';
import { NotificationService } from '../../../../../_services/notification.service';

@Component({
  selector: 'app-ppsc-postings',
  templateUrl: './ppsc-postings.component.html',
  styles: []
})
export class PpscPostingsComponent implements OnInit {
  @Input() public hfmisCode: string = '';
  public selectedDesignation: any;
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
  constructor(private _notificationService: NotificationService, private _authenticationService: AuthenticationService, private _healthFacilityService: HealthFacilityService) {
  }
  ngOnInit() {
    this.getPPSCDesignations();
  }
  private getPPSCDesignations(): void {
    this.loading = true;
    this._healthFacilityService.getPPSCDesignations(this.hfmisCode).subscribe((data: any) => {
      if (data) {
        this.employeeDesignations = data;
        console.log(this.employeeDesignations);
        
        this.loading = false;
      }
    }, err => {
      this.handleError(err);
    });
  }
  public onSelectDesignation(designation: any) {
    this.selectedDesignation = designation;
    this.id = this.selectedDesignation.Id;
    this.name = this.selectedDesignation.Name;
    this.employeeProfiles = [];
    this.getPPSCCandidates();
  }
  private getPPSCCandidates() {
    this.loadingEmployeeProfiles = true;
    window.scroll(0, 0);
    this._healthFacilityService.getPPSCCandidates(this.hfmisCode, this.id).subscribe((data: any) => {
      if (data) {
        this.employeeProfiles = data;
        console.log(this.employeeProfiles);
        
        this.loadingEmployeeProfiles = false;
      }
    }, err => {
      this.handleError(err);
    });
  }
  public openInNewTab(link: string) {
    window.open('/' + link, '_blank');
  }
  public closeWindow() {
    this.joiningDialogOpened = false;
  }
  public openWindow(dataItem) {
    this.employeeProfile = dataItem;
    this.joiningDialogOpened = true;
  }
  public dashifyCNIC(cnic: string) {
  if(cnic){
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
  private handleError(err: any) {
    this.loading = false;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }
}
