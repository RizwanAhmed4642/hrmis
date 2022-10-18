import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';


import { InputsModule } from '@progress/kendo-angular-inputs';
import { GridModule } from '@progress/kendo-angular-grid';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { TooltipModule } from '@progress/kendo-angular-tooltip';
import { MultiSelectModule } from 'primeng/multiselect';
import { PDFExportModule } from '@progress/kendo-angular-pdf-export';
import { SharedModule } from '../../shared.module';
import { DialogsModule } from '@progress/kendo-angular-dialog';
import { VacancyPositionRoutingModule } from './vacancy-position-routing.module';
import { VacancyPositionComponent } from './vacancy-position.component';
import { VacancyPositionService } from './vacancy-position.service';
import { AddEditComponent } from './add-edit/add-edit.component';
import { LaddaModule } from 'angular2-ladda';
import { ReportManagerComponent } from './report-manager/report-manager.component';
import { VpHoldingsComponent } from './vp-holdings/vp-holdings.component';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { VacancyStatusComponent } from './vacancy-status/vacancy-status.component';
import { VPQuotaComponent } from './quota/quota.component';

@NgModule({
  declarations: [VacancyPositionComponent, VPQuotaComponent, AddEditComponent, ReportManagerComponent, VpHoldingsComponent, VacancyStatusComponent],
  imports: [
    CommonModule,
    MultiSelectModule,
    VacancyPositionRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    InputsModule,
    GridModule,
    DropDownsModule,
    DateInputsModule,
    TooltipModule,
    LaddaModule,
    SharedModule,
    ButtonsModule,
    DialogsModule,
    PDFExportModule,
    TabsModule
  ],
  providers: [VacancyPositionService]

})
export class VacancyPositionModule { }
