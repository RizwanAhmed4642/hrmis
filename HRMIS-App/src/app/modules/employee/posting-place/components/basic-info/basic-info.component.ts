import { Component, OnInit, Input } from '@angular/core';
import { AuthenticationService } from '../../../../../_services/authentication.service';
import { NotificationService } from '../../../../../_services/notification.service';
import { Router } from '@angular/router';
import { RootService } from '../../../../../_services/root.service';
import { EmployeeService } from '../../../employee.service';

@Component({
  selector: 'app-basic-info-place',
  templateUrl: './basic-info.component.html'
})
export class BasicInfoComponent implements OnInit {
  public hfmisCode: string = '';
  @Input() public healthFacility: any;
  @Input() public dashboardView: boolean;
  @Input() public hfPhoto: any;
  @Input() public heads: any[] = [];
  @Input() public hfPhotoLoaded: boolean = false;
  public currentUser: any;
  constructor(private _rootService: RootService,
    private _authenticationService: AuthenticationService,
    private _employeeService: EmployeeService) { }

  ngOnInit() {
    this.currentUser = this._authenticationService.getUser();
  }
 
}
