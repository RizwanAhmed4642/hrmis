import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthPHFMCGuard } from '../../_guards/auth-phfmc.guard';
import { AccountComponent } from './account/account.component';
import { ProfileComponent } from './profile/profile.component';
import { OnlineJobsComponent } from './online-jobs.component';
import { AuthGuard } from '../../_guards/auth.guard';
import { PreferencesComponent } from './preferences/preferences.component';
import { OfferLetterComponent } from './offer-letter/offer-letter.component';
import { DefaultLayoutComponent } from '../../containers';
import { ProfileEntryComponent } from './profile-entry/profile-entry.component';
import { ReviewComponent } from './review/review.component';
import { JobsListComponent } from './jobs-list/jobs-list.component';
import { ExperienceComponent } from './experience/experience.component';
import { ApplyNowComponent } from './apply-now/apply-now.component';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'account',
    pathMatch: 'full',
  },
  {
    path: '',
    component: OnlineJobsComponent,
    data: { title: 'Prefrences Portal' },
    children: [
      { path: 'account', component: AccountComponent },
      { path: 'account/:jobId', component: AccountComponent },
      { path: 'profile', component: ProfileComponent, canActivate: [AuthPHFMCGuard] },
      { path: 'profile/:jobId', component: ProfileComponent, canActivate: [AuthPHFMCGuard] },
      { path: 'review', component: ReviewComponent, canActivate: [AuthPHFMCGuard] },
      { path: 'profile-entry', component: ProfileEntryComponent, canActivate: [AuthGuard] },
      { path: 'profile-entry/:id', component: ProfileEntryComponent, canActivate: [AuthGuard] },
      { path: 'document', component: OfferLetterComponent, canActivate: [AuthPHFMCGuard] },
      { path: 'document/:jobId', component: OfferLetterComponent, canActivate: [AuthPHFMCGuard] },
      { path: 'experience', component: ExperienceComponent, canActivate: [AuthPHFMCGuard] },
      { path: 'apply-now/:desigId', component: ApplyNowComponent, canActivate: [AuthPHFMCGuard] },
      { path: 'preferences', component: PreferencesComponent, canActivate: [AuthPHFMCGuard] },
      { path: 'preferences/:desigId', component: PreferencesComponent, canActivate: [AuthPHFMCGuard] },
      { path: 'apply', component: JobsListComponent, canActivate: [AuthPHFMCGuard] },
    ]
  }
];
/* , canActivate: [AuthPHFMCGuard] */
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class OnlineJobsRoutingModule { }
