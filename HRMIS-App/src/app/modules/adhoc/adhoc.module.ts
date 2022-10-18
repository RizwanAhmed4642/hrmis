import { NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { PDFExportModule } from "@progress/kendo-angular-pdf-export";

import { InputsModule } from '@progress/kendo-angular-inputs';
import { GridModule } from '@progress/kendo-angular-grid';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { DialogsModule } from '@progress/kendo-angular-dialog';
import { SharedModule } from '../../shared.module';
import { ChipsModule } from 'primeng/chips';
import { LaddaModule } from 'angular2-ladda';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { AdhocComponent } from './adhoc.component';
import { AdhocRoutingModule } from './adhoc-routing.module';
import { AdhocService } from './adhoc.service';
import { AdhocApplicationComponent } from './adhoc-application/adhoc-application.component';
import { AdhocOpenComponent } from './adhoc-open/adhoc-open.component';
import { AdhocApplicationMeritComponent } from './adhoc-application-merit/adhoc-application-merit.component';
import { AdhocDashboardComponent } from './adhoc-dashboard/adhoc-dashboard.component';
import { AdhocApplicantComponent } from './adhoc-applicant/adhoc-applicant.component';
import { AdhocMeritComponent } from './adhoc-merit/adhoc-merit.component';
import { AdhocGrievanceComponent } from './adhoc-grievance/adhoc-grievance.component';
import { AdhocApplicantGrievanceComponent } from './adhoc-applicant-grievance/adhoc-applicant-grievance.component';
import { AdhocApplicantScrutinyComponent } from './adhoc-applicant-scrutiny/adhoc-applicant-scrutiny.component';
import { AdhocApplicationScrutinyComponent } from './adhoc-application-scrutiny/adhoc-application-scrutiny.component';
import { AdhocJobDetailComponent } from './adhoc-job-detail/adhoc-job-detail.component';
import { IntlModule } from '@progress/kendo-angular-intl';
import { AdhocApplicantGrievanceScrutinyComponent } from './adhoc-applicant-grievance-scrutiny/adhoc-applicant-grievance-scrutiny.component';
import { AdhocInterviewsComponent } from './adhoc-interviews/adhoc-interviews.component';
import { AdhocApplicationVerificationComponent } from './adhoc-application-verification/adhoc-application-verification.component';
import { AdhocHfOpenedComponent } from './adhoc-hf-opened/adhoc-hf-opened.component';
import { AdhocPromotionAnticorruptionComponent } from './adhoc-promotion-anticorruption/adhoc-promotion-anticorruption.component';
import { AdhocPromotionDepartmentComponent } from './adhoc-promotion-department/adhoc-promotion-department.component';
import { AdhocPromotionWorkingpaperComponent } from './adhoc-promotion-workingpaper/adhoc-promotion-workingpaper.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    InputsModule,
    GridModule,
    DropDownsModule,
    ButtonsModule,
    DateInputsModule,
    SharedModule,
    AdhocRoutingModule,
    DialogsModule,
    LayoutModule,
    TabsModule,
    ChipsModule,
    LaddaModule,
    IntlModule,
    DateInputsModule,
    PDFExportModule
  ],
  declarations: [AdhocComponent, AdhocApplicationVerificationComponent, AdhocInterviewsComponent, AdhocApplicantGrievanceScrutinyComponent, AdhocJobDetailComponent, AdhocApplicationScrutinyComponent, AdhocApplicantScrutinyComponent, AdhocApplicantGrievanceComponent, AdhocGrievanceComponent, AdhocApplicantComponent, AdhocApplicationMeritComponent, AdhocApplicationComponent, AdhocOpenComponent, AdhocDashboardComponent, AdhocHfOpenedComponent,AdhocPromotionAnticorruptionComponent,AdhocPromotionDepartmentComponent,AdhocPromotionWorkingpaperComponent],
  providers: [DatePipe]
})
export class AdhocModule { }
