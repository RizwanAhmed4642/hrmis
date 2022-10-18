import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';


import { InputsModule } from '@progress/kendo-angular-inputs';
import { GridModule } from '@progress/kendo-angular-grid';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { TooltipModule } from '@progress/kendo-angular-tooltip';


import { SharedModule } from '../../shared.module';
import { LaddaModule } from 'angular2-ladda';
import { CitizenPortalRoutingModule } from './citizen-portal-routing.module';
import { AddEditComponent } from './add-edit/add-edit.component';
import { CitizenPortalComponent } from './citizen-portal.component';
import { ListComponent } from './list/list.component';
import { ViewComponent } from './view/view.component';
import { DialogsModule } from '@progress/kendo-angular-dialog';
import { CitizenPortalService } from './citizen-portal.service';

@NgModule({
  declarations: [AddEditComponent, CitizenPortalComponent, ListComponent, ViewComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    InputsModule,
    GridModule,
    DropDownsModule,
    DateInputsModule,
    TooltipModule,
    SharedModule,
    ButtonsModule,
    DialogsModule,
    LaddaModule,
    CitizenPortalRoutingModule
  ],
  providers: [CitizenPortalService]
})
export class CitizenPortalModule { }
