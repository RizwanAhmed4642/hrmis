import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { InputsModule } from '@progress/kendo-angular-inputs';
import { GridModule, SharedModule } from '@progress/kendo-angular-grid';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { TooltipModule } from '@progress/kendo-angular-tooltip';
import { LaddaModule } from 'angular2-ladda';
import { DialogsModule } from '@progress/kendo-angular-dialog';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { OnlineApplicationRoutingModule } from './online-application-routing.module';
import { OnlineApplicationComponent } from './online-application.component';
import { AccountComponent } from './account/account.component';
import { ProfileComponent } from './profile/profile.component';
import { BasicInfoComponent } from './components/basic-info/basic-info.component';
import { DepartmentalInfoComponent } from './components/departmental-info/departmental-info.component';
import { VerificationComponent } from './components/verification/verification.component';

@NgModule({
  declarations: [
    OnlineApplicationComponent, 
    AccountComponent, 
    ProfileComponent,
    BasicInfoComponent,
    DepartmentalInfoComponent,
    VerificationComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    InputsModule,
    GridModule,
    DateInputsModule,
    DropDownsModule,
    ButtonsModule,
    DialogsModule,
    TabsModule,
    LaddaModule,
    OnlineApplicationRoutingModule
  ],
  providers: []
})
export class OnlineApplicationModule { }
