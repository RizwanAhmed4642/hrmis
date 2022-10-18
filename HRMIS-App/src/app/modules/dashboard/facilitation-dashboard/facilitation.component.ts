import { Component, OnInit, Input } from '@angular/core';
import { DashboardService } from '../dashboard.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { aggregateBy } from '@progress/kendo-data-query';

import { Router } from '@angular/router';
import { RootService } from '../../../_services/root.service';

@Component({
  selector: 'app-hrmis-facilitationdashboard',
  templateUrl: './facilitation.component.html',
  styleUrls: ['./facilitation.component.scss']
})
export class FacilitationComponent implements OnInit {
  @Input() public currentUser: any;
  public currentOfficer: any;
  public fdoReport: any[] = [];
  public totals: any[] = [];
  public aggregates: any[] = [];
  public totalSums: any;
  public userId: string = null;
  public loading: boolean = false;
  constructor(private _rootService: RootService, public _authService: AuthenticationService, private _dashboardService: DashboardService, private router: Router) { }

  ngOnInit() {
    this.currentOfficer = this._authService.getCurrentOfficer();
    this.getFDOReportTypeWiseDate();
  }
  public getFDOReportTypeWiseDate() {
    this.loading = true;
    this._dashboardService.getFDOReportTypeWiseDate(null, null, this.userId).subscribe((data: any) => {
      this.fdoReport = data.d;
      this.setTotalAggregates();

    }, err => {
      this.handleError(err);
    });
  }
  public onTabSelect(e) {
    this.fdoReport = [];
    this.userId = e;
    this.getFDOReportTypeWiseDate();
  }
  cellCLicked(item, field) {
    console.log(item);
    console.log(field);

  }
  colCLicked(field) {
    console.log(field);
  }
  public setTotalAggregates() {
    this.aggregates = [
      { field: 'Total', aggregate: 'sum' },
      { field: 'Today', aggregate: 'sum' },
      { field: 'InProcess', aggregate: 'sum' },
      { field: 'Disposed', aggregate: 'sum' },
    ];
    this.totalSums = aggregateBy(this.fdoReport, this.aggregates);
    this.loading = false;
  }
  public numberWithCommas(x) {
    if (x) {
      return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }
  }
  private handleError(err: any) {
    this.loading = false;
    if (err.status == 403) {
      this._authService.logout();
    }
  }
}
