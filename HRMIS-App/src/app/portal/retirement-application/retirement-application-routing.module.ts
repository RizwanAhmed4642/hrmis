import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { RetirementApplicationComponent } from './retirement-application.component';
import { AccountComponent } from './account/account.component';
import { AuthPublicGuard } from '../../_guards/auth-public.guard';
import { AuthGuard } from '../../_guards/auth.guard';
import { ProfileComponent } from './profile/profile.component';
import { VerificationComponent } from './components/verification/verification.component';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'account',
    pathMatch: 'full',
  },
  {
    path: '',
    component: RetirementApplicationComponent,
    data: { title: 'Retirement Application' },
    children: [
      { path: 'account', component: AccountComponent },
      { path: 'profile', canActivate: [AuthPublicGuard], component: ProfileComponent },
      { path: 'verify/:cnic/:id', canActivate: [AuthPublicGuard], component: VerificationComponent }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RetirementApplicationRoutingModule { }
