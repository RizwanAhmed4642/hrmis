import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { RegisterComponent } from './register/register.component';
import { ListComponent } from './list/list.component';
import { ClaimsComponent } from './claims/claims.component';
import { AddClaimsComponent } from './claims/add-claims/add-claims.component';
import { RoleComponent } from './role/role.component';
import { OfficersComponent } from './officers/officers.component';
import { AddEditComponent } from './officers/add-edit/add-edit.component';
import { PermissionsComponent } from './permissions/permissions.component';

const routes: Routes = [

  {
    path: '', component: ListComponent
  },
  {
    path: 'roles', component: RoleComponent
  },
  {
    path: 'registration', component: RegisterComponent
  },
  {
    path: 'officer', component: OfficersComponent
  },
  {
    path: 'officer/new', component: AddEditComponent
  },
  {
    path: 'officer/:userId/:officerId/edit', component: AddEditComponent
  },
  {
    path: ':userId/permission', component: PermissionsComponent
  },
  {
    path: ':id/edit', component: RegisterComponent
  },
  {
    path: ':id/edit', component: RegisterComponent
  },
  {
    path: ':id/claims', component: ClaimsComponent
  },
  {
    path: ':id/add-claims', component: AddClaimsComponent
  },
  {
    path: ':modId/add-claims', component: AddClaimsComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UserRoutingModule { }
