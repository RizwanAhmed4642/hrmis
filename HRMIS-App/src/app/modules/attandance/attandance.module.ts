import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { GridModule } from '@progress/kendo-angular-grid';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { TooltipModule } from '@progress/kendo-angular-tooltip';
import { SharedModule } from '../../shared.module';
import { LaddaModule } from 'angular2-ladda';

import { DialogsModule } from '@progress/kendo-angular-dialog';
import { ScrollViewModule } from '@progress/kendo-angular-scrollview';

import { AttandanceRoutingModule } from './attandance-routing.module';
import { AttandanceService } from './attandance.service';
import { AttandanceComponent } from './attandance.component';
import { AttandanceLogComponent } from './attandance-log/attandance-log.component';
import { LeaveListComponent } from './leave-list/leave-list.component';
import { AddLeaveComponent } from './add-leave/add-leave.component';
import { ProfileService } from '../profile/profile.service';
import { RptLeaveComponent } from './rpt-leave/rpt-leave.component';
import { MonthlylogComponent } from './monthlylog/monthlylog.component';
import { RptAttandanceComponent } from './rpt-attandance/rpt-attandance.component';
import { DailyAttendanceComponent } from './daily-attendance/daily-attendance.component';
import { EmpListComponent } from './Employees/emp-list/emp-list.component';
import { AddEditComponent } from './Employees/add-edit/add-edit.component';
import { EmpViewComponent } from './Employees/emp-view/emp-view.component';


@NgModule({
  declarations: [AttandanceComponent, AttandanceLogComponent,LeaveListComponent, AddLeaveComponent, RptLeaveComponent, MonthlylogComponent, RptAttandanceComponent, DailyAttendanceComponent, EmpListComponent, AddEditComponent, EmpViewComponent],
  imports: [
    CommonModule,
    AttandanceRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    InputsModule,
    GridModule,
    ButtonsModule,
    DropDownsModule,
    DateInputsModule,
    LaddaModule,
    DialogsModule,
    TooltipModule,
    SharedModule,
    ScrollViewModule
  ],
  providers: [AttandanceService,ProfileService]
})

export class AttandanceModule { }
