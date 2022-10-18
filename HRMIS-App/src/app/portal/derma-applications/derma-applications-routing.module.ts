import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '../../_guards/auth.guard';
import { DermaAppFormComponent } from './derma-app-form/derma-app-form.component';
import { DermaAppListComponent } from './derma-app-list/derma-app-list.component';


const routes: Routes = [
  {
    path: '',
    component: DermaAppFormComponent,
    data: { title: 'Dermatologist Applications' },
    children: [
      { path: 'derma-app-form', component: DermaAppFormComponent },
    ]
  },
  { path: 'derma-list', component: DermaAppListComponent, canActivate: [AuthGuard] }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DermaApplicationsRoutingModule { }
