import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CitizenPortalComponent } from './citizen-portal.component';
import { ListComponent } from './list/list.component';
import { AddEditComponent } from './add-edit/add-edit.component';

const routes: Routes = [
  {
    path: '', component: CitizenPortalComponent
  }, {
    path: 'list/:statusId', component: CitizenPortalComponent
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
export class CitizenPortalRoutingModule { }
