import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '../../_guards/auth.guard';
import { PromotionacrComponent } from './acr/promotionacr.component';
import { ProfileComponent } from './profile/profile.component';

import { SeniorityListComponent } from './seniority/seniority-list.component';

const routes: Routes = [
  /* {
    path: '',
    component: AdhocComponent,
    canActivate: [AuthGuard]
  }, */
  {
    path: '',
    redirectTo: 'new',
    pathMatch: 'full'
  }, {
    path: 'seniority', component: SeniorityListComponent
  }, 
  {
    path: 'synopsis', component: SeniorityListComponent
  },
   {
    path: 'quantification', component: SeniorityListComponent
  }, 
  {
    path: 'working-paper', component: SeniorityListComponent
  }, 
  {
    path: 'inquiry', component: SeniorityListComponent
  },
  {
    path: 'profile/:cnic', component: ProfileComponent
  },
  {
    path: 'promotion-acr', component: PromotionacrComponent
  },

];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PrmotionRoutingModule { }
