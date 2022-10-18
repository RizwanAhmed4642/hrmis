import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { RiBranchComponent } from './ri-branch.component';
import { ListComponent } from './list/list.component';
import { AddEditComponent } from './add-edit/add-edit.component';
import { AddEditSummaryComponent } from './add-edit-summary/add-edit-summary.component';
import { AddEditPUCComponent } from './add-edit-puc/add-edit-puc.component';

const routes: Routes = [
  {
    path: '', component: RiBranchComponent
  },
  {
    path: 'new', component: AddEditComponent
  },
  {
    path: 'new-summary', component: AddEditSummaryComponent, data: { title: 'New Summary' }
  },
  {
    path: 'new-document', component: AddEditPUCComponent, data: { title: 'New Document' }
  },
  {
    path: 'list/:otid/:currentPage', component: ListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RiBranchRoutingModule { }
