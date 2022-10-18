import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { AgGrid } from '../../../_helpers/ag-grid.class';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { Router } from '@angular/router';
import { RootService } from '../../../_services/root.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { aggregateBy } from '@progress/kendo-data-query';
import { DashboardService } from '../dashboard.service';

@Component({
  selector: 'app-posting-report',
  templateUrl: './posting-report.component.html',
  styleUrls: ['./posting-report.component.scss']
})
export class PostingReportComponent implements OnInit {
  @ViewChild('popup', {static: false}) calendarpopup;
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
  constructor(private _rootService: RootService, public _authService: AuthenticationService, private _dashboardService: DashboardService, private router: Router) { }

  ngOnInit() {
    this.getPostingReport();
  }
  public getPostingReport() {
    this.loading = true;
    this._dashboardService.getPostingReport().subscribe((data: any) => {
      if (data) {
        this.data = data;
        this.setTotalAggregatesDept();
      }

    }, err => {
      console.log(err);
    });
  }
  public setTotalAggregatesDept() {
    this.aggregatesDept = [
      { field: 'Count', aggregate: 'sum' }
    ];
    this.totalSumsDept = aggregateBy(this.data, this.aggregatesDept);
    this.loading = false;
  }
  public dropdownValueChanged = (value, filter) => {
    if (filter == 'program') {
      this.program = value == 'Select Program' ? '' : value;
      this.getPostingReport();
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
