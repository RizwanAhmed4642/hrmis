import { Component, OnInit, Input } from '@angular/core';
import { DashboardService } from '../dashboard.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { aggregateBy } from '@progress/kendo-data-query';

import { Router } from '@angular/router';
import { RootService } from '../../../_services/root.service';
import { KGridHelper } from '../../../_helpers/k-grid.class';
import { PageChangeEvent } from '@progress/kendo-angular-grid';

@Component({
  selector: 'app-hrmis-puc-chart',
  templateUrl: './puc-chart.component.html',
  styleUrls: ['./puc-chart.component.scss']
})
export class PUCChartComponent implements OnInit {
  @Input() public currentUser: any;
  public currentOfficer: any;
  public origionalData: any[] = [];
  public totals: any[] = [];
  public aggregates: any[] = [];
  public totalSums: any;
  public userId: string = null;
  public loading: boolean = false;
  public kGrid: KGridHelper = new KGridHelper();
  public series: any[] = [{
    name: "India",
    data: [3.907, 7.943, 7.848, 9.284, 9.263, 9.801, 3.890, 8.238, 9.552, 6.855]
  }, {
    name: "Russian Federation",
    data: [4.743, 7.295, 7.175, 6.376, 8.153, 8.535, 5.247, -7.832, 4.3, 4.3]
  }, {
    name: "Germany",
    data: [0.010, -0.375, 1.161, 0.684, 3.7, 3.269, 1.083, -5.127, 3.690, 2.995]
  }, {
    name: "World",
    data: [1.988, 2.733, 3.994, 3.464, 4.001, 3.939, 1.333, -2.245, 4.339, 2.727]
  }];
  public categories: number[] = [2002, 2003, 2004, 2005, 2006, 2007, 2008, 2009, 2010, 2011];

  constructor(private _rootService: RootService, public _authService: AuthenticationService, private _dashboardService: DashboardService, private router: Router) { }
  ngOnInit() {
    this.currentOfficer = this._authService.getCurrentOfficer();
    this.currentUser = this._authService.getUser();
    this.kGrid.pageSize = 30;
    this.getApplicationChart();
  }
  public getApplicationChart() {
    this.loading = true;
    this._dashboardService.getApplicationChart(null, null, this.kGrid.skip, this.kGrid.pageSize).subscribe((data: any) => {
      this.origionalData = data.d;

      this.kGrid.totalRecords = data.total;
      this.kGrid.gridView = { data: data.apmo, total: data.total };
    }, err => {
      this.handleError(err);
    });
  }
  public changePagesize(value: any) {
    this.kGrid.pageSize = +value;
    this.kGrid.skip = 0;
    this.getApplicationChart();
  }
  public pageChange(event: PageChangeEvent): void {
    this.kGrid.skip = event.skip;
    this.getApplicationChart();
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
