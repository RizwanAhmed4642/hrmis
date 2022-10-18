import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthPublicGuard } from '../../_guards/auth-public.guard';
import { AccountComponent } from './account/account.component';
import { ProfileComponent } from './profile/profile.component';
import { AuthGuard } from '../../_guards/auth.guard';
import { GrievanceComponent } from './grievance/grievance.component';
import { OfferLetterComponent } from './offer-letter/offer-letter.component';
import { DefaultLayoutComponent } from '../../containers';
import { OnlinePostApplyComponent } from './online-post-apply.component';
import { AdvertisementComponent } from './advertisement/advertisement.component';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'ad',
    pathMatch: 'full',
  },
  {
    path: '',
    component: OnlinePostApplyComponent,
    data: { title: 'Administrative Posts' },
    children: [
      { path: 'account', component: AccountComponent },
      { path: 'profile', canActivate: [AuthPublicGuard], component: ProfileComponent },
      /*   { path: 'letter', canActivate: [AuthPublicGuard], component: OfferLetterComponent }, */
      { path: 'ad', component: AdvertisementComponent },
      /* { path: 'grievance', canActivate: [AuthPublicGuard], component: GrievanceComponent }, */
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class OnlinePostApplyRoutingModule { }
