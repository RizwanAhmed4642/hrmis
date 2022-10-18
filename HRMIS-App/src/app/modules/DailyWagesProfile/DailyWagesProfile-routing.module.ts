import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '../../_guards/auth.guard';
import {DailywagesComponent} from './dailywages/dailywages.component'



const routes: Routes = [
  { path: '', redirectTo: 'list', pathMatch: 'full' },
  { path: 'new', component: DailywagesComponent, canActivate: [AuthGuard] },
  {
    path: '',
    component: DailywagesComponent,
    data: {
      title: 'DailyWages'
    }
  }

 
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ProfileRoutingModule { }


