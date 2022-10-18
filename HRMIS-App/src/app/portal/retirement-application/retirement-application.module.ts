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
import { RetirementApplicationRoutingModule } from './retirement-application-routing.module';
import { RetirementApplicationComponent } from './retirement-application.component';
import { AccountComponent } from './account/account.component';
import { ProfileComponent } from './profile/profile.component';
import { BasicInfoComponent } from './components/basic-info/basic-info.component';
import { DepartmentalInfoComponent } from './components/departmental-info/departmental-info.component';
import { VerificationComponent } from './components/verification/verification.component';
import { ReportingApplicationComponent } from './reporting/reporting.component';
import { NgxImageZoomModule } from 'ngx-image-zoom';

@NgModule({
  declarations: [
    RetirementApplicationComponent,
    AccountComponent,
    ProfileComponent,
    BasicInfoComponent,
    DepartmentalInfoComponent,
    VerificationComponent,
    ReportingApplicationComponent
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
    RetirementApplicationRoutingModule,
    NgxImageZoomModule
  ],
  providers: []
})
export class RetirementApplicationModule { }
