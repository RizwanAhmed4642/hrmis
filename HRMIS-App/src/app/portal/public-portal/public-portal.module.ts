import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PublicPortalComponent } from './public-portal.component';
import { LoginComponent } from './login/login.component';
import { ApmoPreferencesComponent } from './apmo-preferences/apmo-preferences.component';
import { ReportingComponent } from './reporting/reporting.component';

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
import { PublicPortalRoutingModule } from './public-portal-routing.module';
import { PublicPortalService } from './public-portal.service';
import { JobPostingComponent } from './posting/posting.component';

@NgModule({
  declarations: [PublicPortalComponent, ReportingComponent,  LoginComponent, ApmoPreferencesComponent],
  imports: [
    CommonModule,
    PublicPortalRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    DateInputsModule,
    InputsModule,
    DropDownsModule,
    ButtonsModule,
    LaddaModule
  ],
  providers: []
})
export class PublicPortalModule { }
