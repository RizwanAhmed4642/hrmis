import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from '../../../_services/authentication.service';
import { RootService } from '../../../_services/root.service';
import { DashboardService } from '../dashboard.service';
import { PandSOfficerView } from '../../user/user-claims.class';
import { aggregateBy } from '@progress/kendo-data-query';
import { Router } from '@angular/router';

@Component({
  selector: 'app-hrmis-crr-dashboard',
  templateUrl: './crr-dashboard.component.html'
})
export class CrrDashboardComponent implements OnInit {
  public currentUser: any;
  public currentOfficer: any;
  public aggregates: any[] = [];
  public totals: any = {};
  public crrSectionReport: any[] = [];
  public loading: boolean = false;
  constructor(private _rootService: RootService,
    public _authService: AuthenticationService,
    private _dashboardService: DashboardService,
    private router: Router) { }

  ngOnInit() {
    this._rootService.getCurrentOfficer().subscribe((res: PandSOfficerView) => {
      if (res) {
        this.currentOfficer = res;
      }
    });
    this.currentUser = this._authService.getUser();
    this.getCRRReport();
  }
  public getCRRReport() {
    this.loading = true;
    this._dashboardService.getCRRReport().subscribe((data: any) => {
      if (data && data.crr) {
        this.crrSectionReport = data.crr;
        if (this.crrSectionReport.length > 0) {
          // this.setTotalAggregates();
        }
      }
      this.loading = false;
    }, err => {
      console.log(err);
    });
  }
  public setTotalAggregates() {
    this.aggregates = [
      { field: 'Total', aggregate: 'sum' },
      { field: 'No_Process_Initiated', aggregate: 'sum' },
      { field: 'Under_Process', aggregate: 'sum' },
      { field: 'Waiting_Documents', aggregate: 'sum' },
      { field: 'Pending_CRR', aggregate: 'sum' },
      { field: 'Approved', aggregate: 'sum' },
      { field: 'Rejected', aggregate: 'sum' },
      { field: 'Section_Pendency', aggregate: 'sum' },
    ];
    this.totals = aggregateBy(this.crrSectionReport, this.aggregates);
    this.loading = false;
  }
}