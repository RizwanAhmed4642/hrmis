import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AccountRoutingModule } from './account-routing.module';
import { HttpClientModule } from '@angular/common/http';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { FormValidationComponent } from '../_directives/form-validation.component';
import { AppFooterModule } from '@coreui/angular';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { LoginService } from './login/login.service';
import { RegisterService } from './register/register.service';
import { LaddaModule } from 'angular2-ladda';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    AppFooterModule,
    AccountRoutingModule,
    ButtonsModule,
    LaddaModule,
    InputsModule
  ],
  providers: [LoginService, RegisterService],
  declarations: [
    LoginComponent,
    RegisterComponent,
    ChangePasswordComponent,
    FormValidationComponent
  ]
})
export class AccountModule { }
