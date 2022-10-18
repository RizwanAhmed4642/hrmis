import { Component, OnInit, Input } from '@angular/core';
import { DashboardService } from '../dashboard.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { aggregateBy } from '@progress/kendo-data-query';

import { Router } from '@angular/router';
import { RootService } from '../../../_services/root.service';

@Component({
  selector: 'app-hrmis-covid-dashboarddashboard',
  templateUrl: './covid-dashboard.component.html',
  styleUrls: ['./covid-dashboard.component.scss']
})
export class CovidDashboardComponent implements OnInit {
  @Input() public currentUser: any;
  public covids: any[] = [];
  public currentOfficer: any;
  public riBranchReport: any[] = [];
  public totals: any[] = [];
  public aggregates: any[] = [];
  public totalSums: any;
  public userId: string = null;
  public loading: boolean = false;
  public showTable: boolean = false;
  constructor(private _rootService: RootService, public _authService: AuthenticationService, private _dashboardService: DashboardService, private router: Router) { }

  ngOnInit() {
    this.currentOfficer = this._authService.getCurrentOfficer();
    this.fetchCovidData();
    setInterval(() => {
      if (!this.loading) {
        this.fetchCovidData();
      }
    }, (50000));
  }
  public mouseover(covid: any) {
    covid.mouseover = true;
  }
  public mouseout(covid: any) {
    covid.mouseover = false;
  }
  public fetchCovidData() {
    this.loading = true;
    this._dashboardService.fetchCovidData().subscribe(r => this.getCovidDBData());
  }
  public getCovidDBData() {
    this._dashboardService.getCovidDBData().subscribe((data: any) => {
      this.covids = data;
      // this.setTotalAggregates();
      this.loading = false;

    }, err => {
      this.handleError(err);
    });
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
    this.totalSums = aggregateBy(this.riBranchReport, this.aggregates);
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
