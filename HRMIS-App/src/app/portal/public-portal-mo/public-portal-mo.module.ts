import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './login/login.component';
import { ApmoPreferencesComponent } from './apmo-preferences/apmo-preferences.component';

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
import { PublicPortalMORoutingModule } from './public-portal-mo-routing.module';
import { PublicPortalMOComponent } from './public-portal-mo.component';
@NgModule({
  declarations: [PublicPortalMOComponent, LoginComponent, ApmoPreferencesComponent],
  imports: [
    CommonModule,
    PublicPortalMORoutingModule,
    FormsModule,
    ReactiveFormsModule,
    DateInputsModule,
    GridModule,
    InputsModule,
    DropDownsModule,
    ButtonsModule,
    LaddaModule
  ]
})
export class PublicPortalMOModule { }
