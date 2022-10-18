import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AttandanceComponent } from './attandance.component';
import { AuthGuard } from '../../_guards/auth.guard';
import { AttandanceLogComponent } from './attandance-log/attandance-log.component';
import { LeaveListComponent } from './leave-list/leave-list.component';
import { AddLeaveComponent } from './add-leave/add-leave.component';
import { RptLeaveComponent } from './rpt-leave/rpt-leave.component';
import { RptAttandanceComponent } from './rpt-attandance/rpt-attandance.component';
import { DailyAttendanceComponent } from './daily-attendance/daily-attendance.component';
import { EmpListComponent } from './Employees/emp-list/emp-list.component';
import { AddEditComponent } from './Employees/add-edit/add-edit.component';
import { EmpViewComponent } from './Employees/emp-view/emp-view.component';

const routes: Routes = [
  {
    path: '',
    component: AttandanceComponent,
    canActivate: [AuthGuard]
  },
  { path: 'attandanceLog', component: AttandanceLogComponent, canActivate: [AuthGuard] },
  { path: 'leaveList', component: LeaveListComponent, canActivate: [AuthGuard] },
  { path: 'leaveform', component: AddLeaveComponent, canActivate: [AuthGuard] },
  { path: 'rptleave', component: RptLeaveComponent, canActivate: [AuthGuard] },
  { path: 'dailyattendance', component: DailyAttendanceComponent, canActivate: [AuthGuard] },
  { path: 'rptattendance', component: RptAttandanceComponent, canActivate: [AuthGuard] },
  // { path: '', redirectTo: 'list', pathMatch: 'full' },
  { path: 'empList', component: EmpListComponent, canActivate: [AuthGuard] },
  { path: 'empList/:type', component: EmpListComponent, canActivate: [AuthGuard] },
  { path: 'new', component: AddEditComponent, canActivate: [AuthGuard] },
  { path: ':cnic/edit', component: AddEditComponent, canActivate: [AuthGuard] },
  { path: ':cnic/:userId/edit', component: AddEditComponent, canActivate: [AuthGuard] },
  { path: ':cnic', component: EmpViewComponent, canActivate: [AuthGuard] },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AttandanceRoutingModule { }
