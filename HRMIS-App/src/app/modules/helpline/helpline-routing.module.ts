import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '../../_guards/auth.guard';
import { UserComponent } from './user/user.component';


const routes: Routes = [

  { path: '', redirectTo: 'user', pathMatch: 'full' },
  { path: 'user', component: UserComponent, canActivate: [AuthGuard] },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HelplineRoutingModule { }
