import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ApplicationFtsComponent } from './application-fts.component';
import { AddEditComponent } from './add-edit/add-edit.component';
import { AuthGuard } from '../../_guards/auth.guard';
import { ViewComponent } from './view/view.component';
import { InboxesComponent } from './inboxes/inboxes.component';
import { DocumentsComponent } from './documents/documents.component';
import { OnlineTransferComponent } from './list/online-transfer-applications/online-transfer.component';
import { AdhocMeritComponent } from '../adhoc/adhoc-merit/adhoc-merit.component';

const routes: Routes = [
  {
    path: '', component: ApplicationFtsComponent
  }, {
    path: 'list/:office/:type/:statusId', component: ApplicationFtsComponent
  }, {
    path: 'transfer-applications', component: OnlineTransferComponent
  }, {
    path: 'transfer-applications/:hfId/:dsgId', component: OnlineTransferComponent
  }  ,{
    path: 'document', component: DocumentsComponent
  } ,
  { path: 'view/:id/:tracking', component: ViewComponent, canActivate: [AuthGuard] }
  ,
  {
    path: 'new/:id', component: AddEditComponent
  }
  ,
  {
    path: 'fc-inbox', component: InboxesComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ApplicationFtsRoutingModule { }
