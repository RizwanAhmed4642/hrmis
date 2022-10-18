import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthAdhocGuard } from '../../_guards/auth-adhoc.guard';
import { AccountComponent } from './account/account.component';
import { AuthGuard } from '../../_guards/auth.guard';
import { DefaultLayoutComponent } from '../../containers';
import { OnlinePromotionApplyComponent } from './online-promotion-apply.component';
import { ProfileComponent } from './profile/profile.component';
import { SeniorityListComponent } from './seniority-list/seniority-list.component';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'account',
    pathMatch: 'full',
  },
  { path: 'account', component: AccountComponent },
  {
    path: '',
    component: OnlinePromotionApplyComponent,
    data: { title: 'Seniority List' },
    children: [
      { path: 'profile/:id', component: ProfileComponent },
      { path: 'seniority-list', component: SeniorityListComponent }
    ]
  }
];
/* , canActivate: [AuthAdhocGuard] */
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class OnlinePromotionApplyRoutingModule { }
