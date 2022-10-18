import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { JobComponent } from './job.component';
import { AuthGuard } from '../../_guards/auth.guard';
import { JobsOpenComponent } from './jobs-open/jobs-open.component';
import { DocumentsComponent } from './documents/documents.component';
import { JobApplicationComponent } from './job-application/job-application.component';

const routes: Routes = [
  /* {
    path: '',
    component: JobComponent,
    canActivate: [AuthGuard]
  }, */
  {
    path: '',
    redirectTo: 'new',
    pathMatch: 'full'
  }, {
    path: 'document', component: DocumentsComponent
  }, {
    path: 'applications', component: JobApplicationComponent
  },
  { path: 'new', component: JobsOpenComponent, canActivate: [AuthGuard] }
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class JobRoutingModule { }
