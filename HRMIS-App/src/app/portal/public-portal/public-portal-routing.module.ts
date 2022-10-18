import { NgModule } from '@angular/core';
import {
  Routes,
  RouterModule
} from '@angular/router';
import { PublicPortalComponent } from './public-portal.component';
import { ApmoPreferencesComponent } from './apmo-preferences/apmo-preferences.component';
import { LoginComponent } from './login/login.component';
import { AuthPublicGuard } from '../../_guards/auth-public.guard';
import { JobPostingComponent } from './posting/posting.component';
import { AuthGuard } from '../../_guards/auth.guard';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full',
  },
  {
    path: '',
    component: PublicPortalComponent,
    data: { title: 'Main' },
    children: [
      {
        path: 'login', component: LoginComponent
      }, {
        path: 'consultant/:cnic', canActivate: [AuthPublicGuard], component: ApmoPreferencesComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PublicPortalRoutingModule { }
