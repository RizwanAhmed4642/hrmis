import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { OrderComponent } from './order.component';
import { NewComponent } from './new/new.component';
import { EditorViewComponent } from './editor-view/editor-view.component';
import { UploadComponent } from './upload/upload.component';
import { SearchOrderComponent } from './search-order/search-order.component';
import { AdvanceSearchComponent } from './advance-search/advance-search.component';
import { CombineOrderComponent } from './combine-order/combine-order.component';
import { MutualTransferComponent } from './mutual-transfer/mutual-transfer.component';
import { PhfmcOrderListComponent } from './phfmc-order-list/phfmc-order-list.component';

const routes: Routes = [
  { path: 'new/:cnic/:id', component: NewComponent },
  { path: 'new/:cnic/:cnic2/:id', component: MutualTransferComponent },
  { path: 'new/:id', component: NewComponent },
  { path: 'new/:cnic/:id/:appid/:trackingid', component: NewComponent },
  { path: 'editor-view/:id/:type', component: EditorViewComponent },
  /*  { path: 'upload', component: UploadComponent }, */
  { path: 'combine/:id', component: CombineOrderComponent },
  { path: 'verification', component: SearchOrderComponent },
  { path: 'search', component: AdvanceSearchComponent },
  {
    path: ':otid/:currentPage', component: OrderComponent
  },
  { path: 'phfmc-order-list', component: PhfmcOrderListComponent },

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class OrderRoutingModule { }
