import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { InputsModule } from '@progress/kendo-angular-inputs';
import { GridModule, SharedModule } from '@progress/kendo-angular-grid';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { TooltipModule } from '@progress/kendo-angular-tooltip';
import { LaddaModule } from 'angular2-ladda';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { DiplomaCandidateRoutingModule } from './diploma-candidate-routing.module';
import { DiplomaCandidateFormComponent } from './diploma-candidate-form/diploma-candidate-form.component';
import { LayoutModule } from '@progress/kendo-angular-layout';


@NgModule({
  declarations: [DiplomaCandidateFormComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    InputsModule,
    GridModule,
    DateInputsModule,
    DropDownsModule,
    ButtonsModule,
    LaddaModule,
    LayoutModule,
    DiplomaCandidateRoutingModule
  ]
})
export class DiplomaCandidateModule { }
