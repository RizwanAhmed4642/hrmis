import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '../../_guards/auth.guard';
import { PostingPlaceComponent } from './posting-place/posting-place.component';
import { EmployeeComponent } from './employee.component';
import { ProfileComponent } from './profile/profile.component';
import { LeaveRecordComponent } from './leave-record/leave-record.component';
import { ServiceRecordComponent } from './service-record/service-record.component';
import { InquiriesComponent } from './inquiries/inquiries.component';
import { SearchComponent } from './search/search.component';
import { DocumentsComponent } from './documents/documents.component';
import { ApplicationsComponent } from './applications/applications.component';
import { ApplyComponent } from './applications/apply/apply.component';
import { IssueReportComponent } from './issue-report/issue-report.component';
import { TransferTypesComponent } from './applications/apply/transfer-types/transfer-types.component';
import { OpenMeritComponent } from './applications/apply/open-merit/open-merit.component';
import { CompassionateComponent } from './applications/apply/compassionate/compassionate.component';
import { MutualComponent } from './applications/apply/mutual/mutual.component';

const routes: Routes = [
  { path: '', component: EmployeeComponent, canActivate: [AuthGuard] },
  { path: 'facility', component: PostingPlaceComponent, canActivate: [AuthGuard] },
  { path: 'profile', component: ProfileComponent, canActivate: [AuthGuard] },
  { path: 'docs', component: DocumentsComponent, canActivate: [AuthGuard] },
  { path: 'issue', component: IssueReportComponent, canActivate: [AuthGuard] },
  { path: 'application', component: ApplicationsComponent, canActivate: [AuthGuard] },
  {
    path: 'apply', component: ApplyComponent, canActivate: [AuthGuard],
    children: [
      { path: '', component: TransferTypesComponent, canActivate: [AuthGuard] },
      { path: 'open-merit/:id', component: OpenMeritComponent, canActivate: [AuthGuard] },
      { path: 'compassionate', component: CompassionateComponent, canActivate: [AuthGuard] },
      { path: 'mutual', component: MutualComponent, canActivate: [AuthGuard] },
    ]
  },
  { path: 'leave', component: LeaveRecordComponent, canActivate: [AuthGuard] },
  { path: 'service', component: ServiceRecordComponent, canActivate: [AuthGuard] },
  { path: 'inquiry', component: InquiriesComponent, canActivate: [AuthGuard] },
  { path: 'search', component: SearchComponent, canActivate: [AuthGuard] }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class EmployeeRoutingModule { }
