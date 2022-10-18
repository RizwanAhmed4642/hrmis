import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AdhocComponent } from './adhoc.component';
import { AuthGuard } from '../../_guards/auth.guard';
import { AdhocApplicationComponent } from './adhoc-application/adhoc-application.component';
import { AdhocOpenComponent } from './adhoc-open/adhoc-open.component';
import { AdhocApplicationMeritComponent } from './adhoc-application-merit/adhoc-application-merit.component';
import { AdhocDashboardComponent } from './adhoc-dashboard/adhoc-dashboard.component';
import { AdhocApplicantComponent } from './adhoc-applicant/adhoc-applicant.component';
import { AdhocMeritComponent } from './adhoc-merit/adhoc-merit.component';
import { AdhocGrievanceComponent } from './adhoc-grievance/adhoc-grievance.component';
import { AdhocApplicantGrievanceComponent } from './adhoc-applicant-grievance/adhoc-applicant-grievance.component';
import { AdhocApplicationScrutinyComponent } from './adhoc-application-scrutiny/adhoc-application-scrutiny.component';
import { AdhocApplicantScrutinyComponent } from './adhoc-applicant-scrutiny/adhoc-applicant-scrutiny.component';
import { AdhocJobDetailComponent } from './adhoc-job-detail/adhoc-job-detail.component';
import { AdhocApplicantGrievanceScrutinyComponent } from './adhoc-applicant-grievance-scrutiny/adhoc-applicant-grievance-scrutiny.component';
import { AdhocApplicantWorkingPaperComponent } from './adhoc-applicant-working-paper/adhoc-applicant-working-paper.component';
import { AdhocInterviewsComponent } from './adhoc-interviews/adhoc-interviews.component';
import { AdhocApplicationVerificationComponent } from './adhoc-application-verification/adhoc-application-verification.component';
import { AdhocHfOpenedComponent } from './adhoc-hf-opened/adhoc-hf-opened.component';
import { AdhocPromotionAnticorruptionComponent } from './adhoc-promotion-anticorruption/adhoc-promotion-anticorruption.component';

const routes: Routes = [
  //  {
  //   path: '',
  //   component: AdhocComponent,
  //   canActivate: [AuthGuard]
  // }, 
  {
    path: '',
    redirectTo: 'new',
    pathMatch: 'full'
  }, {
    path: 'applications', component: AdhocApplicationComponent
  }, {
    path: 'scrutiny/:desigId/:statusId', component: AdhocApplicationScrutinyComponent
  }, {
    path: 'applicant/:id/:appId', component: AdhocApplicantComponent
  }, {
    path: 'applicant/:id', component: AdhocApplicantComponent
  }, {
    path: 'applicant-scrutiny/:id/:appId', component: AdhocApplicantScrutinyComponent
  }, {
    path: 'applicant-grievance/:id/:appId', component: AdhocApplicantGrievanceComponent
  },{
    path: 'applicant-grievance-scrutiny/:id/:appId', component: AdhocApplicantGrievanceScrutinyComponent
  }, {
    path: 'job-detail/:deisgnationId', component: AdhocJobDetailComponent
  }, {
    path: 'verification', component: AdhocApplicationVerificationComponent
  }, {
    path: 'interviews', component: AdhocInterviewsComponent
  }, {
    path: 'grievance', component: AdhocGrievanceComponent
  }, {
    path: 'applications-merit', component: AdhocApplicationMeritComponent
  },
  {
    path: 'promotionanticorruption', component: AdhocPromotionAnticorruptionComponent
  },
  { path: 'new', component: AdhocOpenComponent, canActivate: [AuthGuard] },
  { path: 'dashboard', component: AdhocDashboardComponent, canActivate: [AuthGuard] },
  { path: 'adhoc-hf-opened', component: AdhocHfOpenedComponent}
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdhocRoutingModule { }
