import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { UserRoutingModule } from './user-routing.module';
import { RegisterComponent } from './register/register.component';
import { ListComponent } from './list/list.component';
import { ClaimsComponent } from './claims/claims.component';
import { UserService } from './user.service';

import { InputsModule } from '@progress/kendo-angular-inputs';
import { GridModule } from '@progress/kendo-angular-grid';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { SharedModule } from '../../shared.module';
import { AddClaimsComponent } from './claims/add-claims/add-claims.component';
import { RoleComponent } from './role/role.component';
import { OfficersComponent } from './officers/officers.component';
import { AddEditComponent } from './officers/add-edit/add-edit.component';
import { LaddaModule } from 'angular2-ladda';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { PermissionsComponent } from './permissions/permissions.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    UserRoutingModule,
    InputsModule,
    GridModule,
    DropDownsModule,
    SharedModule,
    ButtonsModule,
    LaddaModule,
    TabsModule
  ],
  declarations: [RegisterComponent,PermissionsComponent, ListComponent, ClaimsComponent, AddClaimsComponent, RoleComponent, OfficersComponent, AddEditComponent],
  providers: [UserService]
})
export class UserModule { }
