import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { EmployeeRoutingModule } from './employee-routing.module';
import { EmployeeComponent } from './employee.component';
import { PostingPlaceComponent } from './posting-place/posting-place.component';
import { ProfileComponent } from './profile/profile.component';
import { DocumentsComponent } from './documents/documents.component';
import { LeaveRecordComponent } from './leave-record/leave-record.component';
import { ServiceRecordComponent } from './service-record/service-record.component';
import { InquiriesComponent } from './inquiries/inquiries.component';
import { SearchComponent } from './search/search.component';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';


import { InputsModule } from '@progress/kendo-angular-inputs';
import { GridModule } from '@progress/kendo-angular-grid';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { TooltipModule } from '@progress/kendo-angular-tooltip';
import { TabsModule } from 'ngx-bootstrap/tabs';


import { SharedModule } from '../../shared.module';
import { LaddaModule } from 'angular2-ladda';
import { DialogsModule } from '@progress/kendo-angular-dialog';
import { ScrollViewModule } from '@progress/kendo-angular-scrollview';
import { EmployeeService } from './employee.service';
import { StaffStatementComponent } from './posting-place/components/staff-statement/staff-statement.component';
import { BasicInfoComponent } from './posting-place/components/basic-info/basic-info.component';
import { ApplicationsComponent } from './applications/applications.component';
import { ApplyComponent } from './applications/apply/apply.component';
import { EmployeeLeaveRecordComponent } from './profile/leave-record/leave-record.component';
import { EmployeeServiceRecordComponent } from './profile/service-record/service-record.component';
import { IssueReportComponent } from './issue-report/issue-report.component';
import { QualificationComponent } from './profile/qualification/qualification.component';
import { TransferTypesComponent } from './applications/apply/transfer-types/transfer-types.component';
import { OpenMeritComponent } from './applications/apply/open-merit/open-merit.component';
import { CompassionateComponent } from './applications/apply/compassionate/compassionate.component';
import { MutualComponent } from './applications/apply/mutual/mutual.component';

@NgModule({
  declarations: [EmployeeComponent, EmployeeLeaveRecordComponent, EmployeeServiceRecordComponent, ApplyComponent, PostingPlaceComponent, StaffStatementComponent, BasicInfoComponent, ProfileComponent, DocumentsComponent,
    IssueReportComponent, LeaveRecordComponent, MutualComponent, CompassionateComponent, OpenMeritComponent, TransferTypesComponent, ServiceRecordComponent, QualificationComponent, InquiriesComponent, SearchComponent, ApplicationsComponent],
  imports: [
    CommonModule,
    EmployeeRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    InputsModule,
    GridModule,
    SharedModule,
    ButtonsModule,
    DropDownsModule,
    DateInputsModule,
    LaddaModule,
    DialogsModule,
    TooltipModule,
    ScrollViewModule,
    TabsModule
  ],
  providers: [EmployeeService]
})
export class EmployeeModule { }
