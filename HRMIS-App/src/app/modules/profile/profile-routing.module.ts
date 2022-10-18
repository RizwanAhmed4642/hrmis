import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ListComponent } from './list/list.component';
import { AuthGuard } from '../../_guards/auth.guard';
import { AddEditComponent } from './add-edit/add-edit.component';
import { ViewComponent } from './view/view.component';
import { MyAccountComponent } from './my-account/my-account.component';
import { AddEditChequeComponent } from './cheque-book/add-edit-cheque.component';
import { AddNewComponent } from './add-new/add-new.component';
import { ProfileEntryComponent } from './add-edit/profile-entry/profile-entry.component';
import { ReviewListComponent } from './review-list/review-list.component';
import { SenorityComponent } from './senority/senority.component';

const routes: Routes = [
  { path: '', redirectTo: 'list', pathMatch: 'full' },
  { path: 'new', component: AddEditComponent, canActivate: [AuthGuard] },
  { path: 'new-ppsc', component: ProfileEntryComponent, canActivate: [AuthGuard] },
  { path: 'add', component: AddNewComponent, canActivate: [AuthGuard] },
  /*  { path: 'new-cheque', component: AddEditChequeComponent, canActivate: [AuthGuard] }, */
  { path: 'review', component: ReviewListComponent, canActivate: [AuthGuard] },
  { path: 'list', component: ListComponent, canActivate: [AuthGuard] },
  { path: 'list/:page', component: ListComponent, canActivate: [AuthGuard] },
  { path: 'senority', component: SenorityComponent, canActivate: [AuthGuard] },
  { path: ':cnic/edit', component: AddEditComponent, canActivate: [AuthGuard] },
  { path: ':cnic/:userId/edit', component: AddEditComponent, canActivate: [AuthGuard] },
  { path: ':cnic', component: ViewComponent, canActivate: [AuthGuard] },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ProfileRoutingModule { }
