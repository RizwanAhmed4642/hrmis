import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ListComponent } from './list/list.component';
import { AddEditComponent } from './add-edit/add-edit.component';
import { HrPostingComponent } from './hr-posting.component';

const routes: Routes = [
  {
    path: '', component: HrPostingComponent
  },
  {
    path: 'new', component: AddEditComponent
  },
  {
    path: 'list/:otid/:currentPage', component: ListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HrPostingRoutingModule { }
