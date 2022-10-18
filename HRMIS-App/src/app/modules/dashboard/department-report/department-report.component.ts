import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { AgGrid } from '../../../_helpers/ag-grid.class';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { ActivatedRoute, Router } from '@angular/router';
import { RootService } from '../../../_services/root.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { aggregateBy } from '@progress/kendo-data-query';
import { DashboardService } from '../dashboard.service';

@Component({
  selector: 'app-department-report',
  templateUrl: './department-report.component.html',
  styleUrls: ['./department-report.component.scss']
})
export class DepartmentReportComponent implements OnInit {
  @ViewChild('popup', { static: false }) calendarpopup;
  @Input() public currentUser: any;
  public loading: boolean = false;
  public agGridHelper: AgGrid = new AgGrid();
  public dropDowns: DropDownsHR = new DropDownsHR();
  public currentOfficer: any;
  public rangeDept = { start: null, end: null };
  public data: any[] = [];
  public aggregatesDept: any[] = [];
  public totalSumsDept: any;
  public program: string = '';
  constructor(private _rootService: RootService, public route: ActivatedRoute, public _authService: AuthenticationService, private _dashboardService: DashboardService, private router: Router) { }

  ngOnInit() {
    //this.router.navigate(['/department-report']);
    //this.getDepartmentReport();
  }
  public getDepartmentReport() {
    this.loading = true;
    this._dashboardService.getDashboardFcAppFwdCounts(this.rangeDept.start, this.rangeDept.end, this.program).subscribe((data: any) => {
      if (data) {
        this.data = data.dept;
        this.getDashboardPendency3();
      }

    }, err => {
      console.log(err);
    });
  }
  public getDashboardPendency3() {
    this.loading = true;
    this._dashboardService.getDashboardPendency3(this.rangeDept.start, this.rangeDept.end, this.program).subscribe((data: any) => {
      if (data) {
        let p = data.pendancy;
        this.data.forEach(report => {
          p.forEach(pend => {
            if (report.OfficerDesignation == pend.OfficerDesignation) {
              report.TodayUnderProcess = +pend.TodayUnderProcess ? pend.TodayUnderProcess : 0;
              report.TodayDispose = +pend.TodayDispose ? pend.TodayDispose : 0;
              report.UnderProcessGT7Days = +pend.UnderProcessGT7Days ? pend.UnderProcessGT7Days : 0;
              report.UnderProcessGT15Days = +pend.UnderProcessGT15Days ? pend.UnderProcessGT15Days : 0;
              report.UnderProcessGT30Days = +pend.UnderProcessGT30Days ? pend.UnderProcessGT30Days : 0;
              report.UnderProcessUntilToday = +pend.UnderProcessUntilToday ? pend.UnderProcessUntilToday : 0;
            }
          });
        });
        this.setTotalAggregatesDept();
      }

    }, err => {
      console.log(err);
    });
  }
  public setTotalAggregatesDept() {
    this.aggregatesDept = [
      { field: 'Total', aggregate: 'sum' },
      { field: 'NoProcessInitiated', aggregate: 'sum' },
      { field: 'UnderProcessGT7Days', aggregate: 'sum' },
      { field: 'UnderProcessGT15Days', aggregate: 'sum' },
      { field: 'UnderProcessGT30Days', aggregate: 'sum' },
      { field: 'Section_Pendency', aggregate: 'sum' },
      { field: 'Approved', aggregate: 'sum' },
      { field: 'Rejected', aggregate: 'sum' },
      { field: 'No_Further_Action', aggregate: 'sum' },
      { field: 'UnderProcess', aggregate: 'sum' },
      { field: 'FileReturned', aggregate: 'sum' },
      { field: 'AReturn', aggregate: 'sum' },
      { field: 'RReturn', aggregate: 'sum' },
      { field: 'Waiting_Documents', aggregate: 'sum' },
      { field: 'FileRequested', aggregate: 'sum' }
    ];
    this.totalSumsDept = aggregateBy(this.data, this.aggregatesDept);
    console.log('aggregate data: ', this.totalSumsDept);

    if (this.calendarpopup) {
      this.calendarpopup.toggle(false);
    }
    this.loading = false;
  }
  public dropdownValueChanged = (value, filter) => {
    if (filter == 'program') {
      this.program = value == 'Select Program' ? '' : value;
      this.getDepartmentReport();
    }
  }
  public cellCLickedFc(item, statusId) {
    //var link = 'http://localhost:4200/dashboard/list/' + 'null/' + statusId;
    var link = '/dashboard/list/null/' + statusId;
    localStorage.setItem('officKey', item.OfficerDesignation);
    window.open(link, '_blank');
    //this.router.navigate(['/dashboard/list/' + item.OfficerDesignation + '/null/' + statusId]);
  }
  colCLicked(field) {
    //this.router.navigate(['/fts/my-applications/' + field]);
    console.log(field);
  }
}
