import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { OnlineFacilitationAuthGuard } from '../../_guards/online-facilitation-auth.guard';
import { AccountComponent } from './account/account.component';
import { AddComponent } from './application/add/add.component';
import { ApplicationComponent } from './application/application.component';
import { DetailsComponent } from './application/details/details.component';
import { OnlineFacilitationCenterComponent } from './online-facilitation-center.component';
import { LeaveRecordComponent } from './profile/leave-record/leave-record.component';
import { ProfileComponent } from './profile/profile.component';
import { ServiceHistoryComponent } from './profile/service-history/service-history.component';


// const routes: Routes = [];

const routes: Routes = [
  {
    path: '',
    redirectTo: 'account',
    pathMatch: 'full',
  },
  { path: 'account', component: AccountComponent },

  {
    path: '',
    component: OnlineFacilitationCenterComponent,
    data: { title: 'Online Facilitation Center' },
    children: [
      // { path: 'account', component: AccountComponent },
      { path: 'application', component: ApplicationComponent },
      // { path: 'application/add', component: AddComponent },
      { path: 'application/add/:id', component: AddComponent },
      { path: 'application/details', component: DetailsComponent },
      { path: 'application/details/:id/:tracking', component: DetailsComponent },
      { path: 'profile', component: ProfileComponent },
      { path: 'leave-record', component: LeaveRecordComponent },
      { path: 'service-history', component: ServiceHistoryComponent },
      
      // { path: 'profile', canActivate: [AuthPublicGuard], component: ProfileComponent },
      // { path: 'verify/:cnic/:id', canActivate: [AuthPublicGuard], component: VerificationComponent }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class OnlineFacilitationCenterRoutingModule { }
