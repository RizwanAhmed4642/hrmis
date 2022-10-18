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
import { ReportingRoutingModule } from './reporting-routing.module';
import { ReportingComponent } from './reporting.component';
import { ReportingService } from './reporting.service';

@NgModule({
  declarations: [ReportingComponent],
  imports: [
    CommonModule,
    ReportingRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    InputsModule,
    GridModule,
    DropDownsModule,
    DateInputsModule,
    TooltipModule,
    SharedModule,
    ButtonsModule
  ],
  providers: [ReportingService]
})
export class ReportingModule { }
