import { NgModule } from '@angular/core';
import {
  Routes,
  RouterModule
} from '@angular/router';
import { PublicPortalMOComponent } from './public-portal-mo.component';
import { ApmoPreferencesComponent } from './apmo-preferences/apmo-preferences.component';
import { LoginComponent } from './login/login.component';
import { AuthPublicGuard } from '../../_guards/auth-public.guard';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full',
  },
  {
    path: '',
    component: PublicPortalMOComponent,
    data: { title: 'Medical Officer Applications' },
    children: [
      {
        path: 'login', component: LoginComponent
      }, {
        path: 'apply/:cnic', canActivate: [AuthPublicGuard], component: ApmoPreferencesComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PublicPortalMORoutingModule { }
