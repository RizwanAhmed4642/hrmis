import { Component, OnInit } from '@angular/core';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { RootService } from '../../../_services/root.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { EmployeeService } from '../employee.service';

@Component({
  selector: 'app-issue-report',
  templateUrl: './issue-report.component.html',
  styles: []
})
export class IssueReportComponent implements OnInit {
  public dropDowns: DropDownsHR = new DropDownsHR();
  public selectedType: number = 0;
  public saving: boolean = false;
  public loading: boolean = false;
  public complaint: any = {};
  public list: any[] = [];
  public issueTypes: any[] = [
    { Id: 1, Name: 'My information is incorrect' },
    { Id: 2, Name: 'I have a complaint about candidate' },
    { Id: 3, Name: 'I have a complaint about online portal' }
  ];
  constructor(private _rootService: RootService,
    private _authenticationService: AuthenticationService,
    private _employeeService: EmployeeService) { }

  ngOnInit() {
    this.getcomplaints();
  }
  public getcomplaints() {
    this.loading = true;
    this._employeeService.getHrComplain().subscribe((res: any) => {
      if (res && res.List) {
        this.list = res.List
        this.loading = false;
        this.saving = false;
      }
    }, err => {
      console.log(err);
    });
  }
  submit() {
    this.saving = true;
    let t = this.issueTypes.find(x => x.Id == this.complaint.Type_Id);
    if (t) {
      this.complaint.Type = t.Name;
    }
    this._employeeService.saveHrComplain(this.complaint).subscribe((res: any) => {
      if (res) {
        this.complaint = {};
        this.getcomplaints();
      }
    }, err => {
      console.log(err);
    });
  }

  public dropdownValueChanged = (value, filter) => {
    if (!value) {
      return;
    }
    if (filter == 'type') {
      this.complaint.Type_Id = value.Id;

      this.complaint.Type = value.Name;
    }
  }

}




