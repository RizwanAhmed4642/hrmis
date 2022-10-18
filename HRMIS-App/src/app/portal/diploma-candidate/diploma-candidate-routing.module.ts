import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DiplomaCandidateFormComponent } from './diploma-candidate-form/diploma-candidate-form.component';


const routes: Routes = [
  {
    path: '',
    redirectTo: 'diploma-candidate',
    pathMatch: 'full',
  },
  {
    path: '',
    component: DiplomaCandidateFormComponent,
    data: { title: 'Diploma Candidate' },
    children: [
      { path: 'diploma-candidate', component: DiplomaCandidateFormComponent },
     
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DiplomaCandidateRoutingModule { }
