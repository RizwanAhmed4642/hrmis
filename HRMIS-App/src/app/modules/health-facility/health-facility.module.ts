import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { HealthFacilityRoutingModule } from './health-facility-routing.module';
import { HealthFacilityService } from './health-facility.service';
import { ListComponent } from './list/list.component';
import { AddEditComponent } from './add-edit/add-edit.component';
import { ViewComponent } from './view/view.component';

import { InputsModule } from '@progress/kendo-angular-inputs';
import { GridModule } from '@progress/kendo-angular-grid';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { TooltipModule } from '@progress/kendo-angular-tooltip';
import { MultiSelectModule } from 'primeng/multiselect';
import { ExcelExportModule } from '@progress/kendo-angular-excel-export';

import { TabsModule } from 'ngx-bootstrap/tabs';

import { SharedModule } from '../../shared.module';
import { BasicInfoComponent } from './view/components/basic-info/basic-info.component';
import { HFServicesComponent } from './view/components/hf-services/hf-services.component';
import { HFWardsComponent } from './view/components/hf-wards/hf-wards.component';
import { HFWardsNewComponent } from './view/components/hf-wards-new/hf-wards-new.component';
import { StaffStatementComponent } from './view/components/staff-statement/staff-statement.component';
import { AdvanceTableComponent } from './list/advance-table/advance-table';
import { VacancyComponent } from './view/components/vacancy/vacancy.component';
import { DialogsModule } from '@progress/kendo-angular-dialog';
import { LaddaModule } from 'angular2-ladda';
import { HealthFacilityAuthGuard } from './_auth.guard';
import { HealthFacilityAddAuthGuard } from './add-edit/add.guard';
import { HealthFacilityEditAuthGuard } from './add-edit/edit.guard';
import { RouterModule } from '@angular/router';
import { CovidFacilitiesComponent } from './covid-facilities/covid-facilities.component';
import { CovidStaffComponent } from './covid-staff/covid-staff.component';
import { PpscPostingsComponent } from './view/components/ppsc-postings/ppsc-postings.component';


@NgModule({
  imports: [
    HealthFacilityRoutingModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    InputsModule,
    GridModule,
    DropDownsModule,
    DateInputsModule,
    TooltipModule,
    SharedModule, RouterModule,
    ButtonsModule,
    LaddaModule,
    DialogsModule,
    MultiSelectModule,
    ExcelExportModule,
    TabsModule
  ],
  declarations:
    [
      ListComponent,
      AddEditComponent,
      ViewComponent,
      BasicInfoComponent,
      HFServicesComponent,
      HFWardsComponent,
      HFWardsNewComponent,
      StaffStatementComponent,
      AdvanceTableComponent,
      VacancyComponent,
      CovidFacilitiesComponent,
      PpscPostingsComponent,
      CovidStaffComponent
    ],
  providers: [HealthFacilityService, HealthFacilityAuthGuard, HealthFacilityAddAuthGuard, HealthFacilityEditAuthGuard
  ]
})
export class HealthFacilityModule { }
