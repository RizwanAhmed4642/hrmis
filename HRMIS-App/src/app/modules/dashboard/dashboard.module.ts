import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { DashboardComponent } from './dashboard.component';
import { DashboardRoutingModule } from './dashboard-routing.module';
import { SharedModule } from '../../shared.module';
import { DashboardService } from './dashboard.service';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { CommonModule } from '@angular/common';
import { HrmisDashboardComponent } from './hrmis-dashboard/hrmis-dashboard.component';
import { GridModule } from '@progress/kendo-angular-grid';
import { DialogsModule } from '@progress/kendo-angular-dialog';
import { LaddaModule } from 'angular2-ladda';
import { FtsSectionDashboardComponent } from './fts-section-dashboard/fts-section.component';
import { FacilitationComponent } from './facilitation-dashboard/facilitation.component';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { AgGridModule } from 'ag-grid-angular';
import { TooltipModule } from 'primeng/tooltip';
import { CrrDashboardComponent } from './crr-dashboard/crr-dashboard.component';
import { DepartmentReportComponent } from './department-report/department-report.component';
import { ListViewComponent } from './department-report/list-view/list-view.component';
import { RiBranchComponent } from './ri-branch-dashboard/ri-branch.component';
import { ChartsModule } from '@progress/kendo-angular-charts';
import { CitizenPortalDComponent } from './citizen-portal-dashboard/citizen-portal-dashboard.component';
import { PUCChartComponent } from './puc-chart/puc-chart.component';
import { PostingReportComponent } from './posting-report/posting-report.component';
import { LawWingComponent } from './law-wing-dashboard/law-wing.component';
import { AdhocScrutinyDashboardComponent } from './adhoc-scrutiny-dashboard/adhoc-scrutiny-dashboard.component';
import { OfficerDiaryComponent } from './officer-diary-dashboard/officer-diary-dashboard.component';
@NgModule({
  imports: [
    CommonModule,
    DashboardRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    GridModule,
    InputsModule,
    ButtonsModule,
    DateInputsModule,
    LaddaModule,
    DropDownsModule,
    SharedModule,
    DialogsModule,
    TabsModule,
    AgGridModule.withComponents([]),
    TooltipModule,
    ChartsModule
  ],
  providers: [],
  declarations: [DashboardComponent, OfficerDiaryComponent, LawWingComponent, PostingReportComponent, CitizenPortalDComponent, PUCChartComponent, HrmisDashboardComponent, FtsSectionDashboardComponent, RiBranchComponent, FacilitationComponent, CrrDashboardComponent, DepartmentReportComponent, ListViewComponent, AdhocScrutinyDashboardComponent]
})
export class DashboardModule { }
