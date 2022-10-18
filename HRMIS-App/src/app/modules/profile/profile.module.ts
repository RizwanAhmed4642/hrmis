import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TabsModule } from 'ngx-bootstrap/tabs';

import { ProfileRoutingModule } from './profile-routing.module';
import { ListComponent } from './list/list.component';
import { ViewComponent } from './view/view.component';
import { AddEditComponent } from './add-edit/add-edit.component';
import { AddEditChequeComponent } from './cheque-book/add-edit-cheque.component';
import { ProfileService } from './profile.service';

import { InputsModule } from '@progress/kendo-angular-inputs';
import { GridModule } from '@progress/kendo-angular-grid';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { ExcelExportModule } from '@progress/kendo-angular-excel-export';
import { SharedModule } from '../../shared.module';
import { BasicInfoComponent } from './view/components/basic-info/basic-info.component';
import { DepartmentalInfoComponent } from './view/components/departmental-info/departmental-info.component';
import { FileComponent } from './view/components/file/file.component';
import { OrdersComponent } from './view/components/orders/orders.component';
import { ApplicationsComponent } from './view/components/applications/applications.component';
import { LogsComponent } from './view/components/logs/logs.component';
import { ServiceRecordComponent } from './view/components/service-record/service-record.component';
import { LeaveRecordComponent } from './view/components/leave-record/leave-record.component';
import { LaddaModule } from 'angular2-ladda';
import { MyAccountComponent } from './my-account/my-account.component';
import { DialogModule } from '@progress/kendo-angular-dialog';
import { InquiryComponent } from './view/components/inquiry/inquiry.component';
import { RemarksComponent } from './view/components/remarks/remarks.component';
import { AddNewComponent } from './add-new/add-new.component';
import { ProfileEntryComponent } from './add-edit/profile-entry/profile-entry.component';
import { ReviewListComponent } from './review-list/review-list.component';
import { DuplicationComponent } from './view/components/duplication/duplication.component';
import { SenorityComponent } from './senority/senority.component';
import { ProfileInformationComponent } from './view/components/profile-information/profile-information.component';
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
    DialogModule,
    ProfileRoutingModule,
    LaddaModule,
    ExcelExportModule,
    TabsModule
  ],
  declarations: [ListComponent, ProfileInformationComponent, ReviewListComponent, DuplicationComponent,
    FileComponent,
    ProfileEntryComponent, LeaveRecordComponent, AddNewComponent, RemarksComponent, ViewComponent, AddEditComponent, AddEditChequeComponent, BasicInfoComponent, DepartmentalInfoComponent, OrdersComponent, ApplicationsComponent, LogsComponent, ServiceRecordComponent, InquiryComponent, SenorityComponent],
  providers: [ProfileService]
})
export class ProfileModule { }
