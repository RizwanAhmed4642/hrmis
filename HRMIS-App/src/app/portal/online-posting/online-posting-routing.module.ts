import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthPublicGuard } from '../../_guards/auth-public.guard';
import { AccountComponent } from './account/account.component';
import { ProfileComponent } from './profile/profile.component';
import { OnlinePostingComponent } from './online-posting.component';
import { AuthGuard } from '../../_guards/auth.guard';
import { GrievanceComponent } from './grievance/grievance.component';
import { PreferencesComponent } from './preferences/preferences.component';
import { OfferLetterComponent } from './offer-letter/offer-letter.component';
import { ReportingComponent } from './reporting/reporting.component';
import { DefaultLayoutComponent } from '../../containers';
import { ProfileEntryComponent } from './profile-entry/profile-entry.component';
import { ReviewComponent } from './review/review.component';
import { ConsultantPromoComponent } from './consultant-promo/consultant-promo.component';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'account',
    pathMatch: 'full',
  },
  {
    path: '',
    component: OnlinePostingComponent,
    data: { title: 'PHFMC - Jobs' },
    children: [
      { path: 'account', component: AccountComponent },
      { path: 'profile', component: ProfileComponent, canActivate: [AuthPublicGuard] },
      { path: 'review', component: ReviewComponent, canActivate: [AuthPublicGuard] },
      { path: 'profile-entry', component: ProfileEntryComponent, canActivate: [AuthGuard] },
      { path: 'profile-entry/:id', component: ProfileEntryComponent, canActivate: [AuthGuard] },
      { path: 'letter', component: OfferLetterComponent, canActivate: [AuthPublicGuard] },
      { path: 'preferences', component: PreferencesComponent, canActivate: [AuthPublicGuard] },
      { path: 'grievance', component: GrievanceComponent, canActivate: [AuthPublicGuard] },
      // { path: 'promoted-candidates', component: ConsultantPromoComponent, canActivate: [AuthPublicGuard] },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class OnlinePostingRoutingModule { }
