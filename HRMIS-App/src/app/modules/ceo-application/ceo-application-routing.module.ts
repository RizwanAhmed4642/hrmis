import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CEOApplicationComponent } from './ceo-application.component';
import { ListComponent } from './list/list.component';
import { AddEditComponent } from './add-edit/add-edit.component';
import { AdhocComponent } from './adhoc/adhoc.component';
import { AdhocNewComponent } from './adhoc-new/adhoc-new.component';

const routes: Routes = [
  {
    path: '', component: CEOApplicationComponent
  }, {
    path: 'list/:statusId', component: CEOApplicationComponent
  },
  {
    path: 'new', component: AddEditComponent
  },
  {
    path: 'adhoc', component: AdhocComponent
  },
  {
    path: 'adhoc-new', component: AdhocNewComponent
  },
  {
    path: 'list/:otid/:currentPage', component: ListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CEOApplicationRoutingModule { }
