import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthAdhocGuard } from '../../_guards/auth-adhoc.guard';
import { AccountComponent } from './account/account.component';
import { ProfileComponent } from './profile/profile.component';
import { AuthGuard } from '../../_guards/auth.guard';
import { PreferencesComponent } from './preferences/preferences.component';
import { OfferLetterComponent } from './offer-letter/offer-letter.component';
import { DefaultLayoutComponent } from '../../containers';
import { ProfileEntryComponent } from './profile-entry/profile-entry.component';
import { ReviewComponent } from './review/review.component';
import { JobsListComponent } from './jobs-list/jobs-list.component';
import { ExperienceComponent } from './experience/experience.component';
import { ApplyNowComponent } from './apply-now/apply-now.component';
import { OnlineAdhocComponent } from './online-adhoc.component';
import { QualificationComponent } from './qualification/qualification.component';
import { HomeComponent } from './home/home.component';
import { GrievanceComponent } from './grievance/grievance.component';
import { InterviewsComponent } from './interviews/interviews.component';
import { MeritsComponent } from './merits/merits.component';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'account',
    pathMatch: 'full',
  },
  { path: 'account', component: AccountComponent },
  {
    path: '',
    component: OnlineAdhocComponent,
    data: { title: 'Prefrences Portal' },
    children: [
      { path: 'account/:jobId', component: AccountComponent },
      { path: 'profile', component: ProfileComponent, canActivate: [AuthAdhocGuard] },
      { path: 'home', component: HomeComponent, canActivate: [AuthAdhocGuard] },
      { path: 'interviews', component: InterviewsComponent, canActivate: [AuthAdhocGuard] },
      { path: 'merits', component: MeritsComponent, canActivate: [AuthAdhocGuard] },
      { path: 'profile/:jobId', component: ProfileComponent, canActivate: [AuthAdhocGuard] },
      { path: 'review', component: ReviewComponent, canActivate: [AuthAdhocGuard] },
      { path: 'profile-entry', component: ProfileEntryComponent, canActivate: [AuthGuard] },
      { path: 'profile-entry/:id', component: ProfileEntryComponent, canActivate: [AuthGuard] },
      { path: 'document', component: OfferLetterComponent, canActivate: [AuthAdhocGuard] },
      { path: 'qualification', component: QualificationComponent, canActivate: [AuthAdhocGuard] },
      { path: 'document/:jobId', component: OfferLetterComponent, canActivate: [AuthAdhocGuard] },
      { path: 'experience', component: ExperienceComponent, canActivate: [AuthAdhocGuard] },
      { path: 'apply-now/:desigId', component: ApplyNowComponent, canActivate: [AuthAdhocGuard] },
      { path: 'preferences', component: PreferencesComponent, canActivate: [AuthAdhocGuard] },
      { path: 'preferences/:desigId', component: PreferencesComponent, canActivate: [AuthAdhocGuard] },
      { path: 'apply', component: JobsListComponent, canActivate: [AuthAdhocGuard] },
      { path: 'grievance', component: GrievanceComponent, canActivate: [AuthAdhocGuard] },
      { path: 'grievance/:appId', component: GrievanceComponent, canActivate: [AuthAdhocGuard] },
    ]
  }
];
/* , canActivate: [AuthAdhocGuard] */
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class OnlineAdhocRoutingModule { }
