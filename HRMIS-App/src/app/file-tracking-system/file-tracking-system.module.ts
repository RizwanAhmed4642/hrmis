import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';


import { InputsModule } from '@progress/kendo-angular-inputs';
import { GridModule } from '@progress/kendo-angular-grid';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { TooltipModule } from '@progress/kendo-angular-tooltip';
import { SharedModule } from '../shared.module';
import { LaddaModule } from 'angular2-ladda';
import { DialogsModule } from '@progress/kendo-angular-dialog';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { ExcelExportModule } from '@progress/kendo-angular-excel-export';

import { FileTrackingSystemRoutingModule } from './file-tracking-system-routing.module';
import { FileTrackingSystemComponent } from './file-tracking-system.component';
import { InboxComponent } from './inbox/inbox.component';
import { MyApplicationsComponent } from './my-applications/my-applications.component';
import { SentComponent } from './sent/sent.component';
import { ImportantComponent } from './important/important.component';
import { ApplicationComponent } from './application/application.component';
import { FileTrackingSystemService } from './file-tracking-system.service';
import { ApplicationFtsService } from '../modules/application-fts/application-fts.service';
import { ScrollViewModule } from '@progress/kendo-angular-scrollview';
import { RecordRoomComponent } from './record-room/record-room.component';
import { LetterComponent } from './letter/letter.component';
import { FileRequisitionComponent } from './file-requisition/file-requisition.component';
import { NewComponent } from './file-requisition/new/new.component';
import { DispatchFilesComponent } from './file-requisition/dispatch-files/dispatch-files.component';
import { ScannedFilesComponent } from './scanned-files/scanned-files.component';
import { AddEditComponent } from './scanned-files/add-edit/add-edit.component';
import { DetailsComponent } from './application/details/details.component';
import { DiaryComponent } from './diary/diary.component';
import { NgxImageZoomModule } from 'ngx-image-zoom';
import { SearchTrackingComponent } from './search-tracking/search-tracking.component';
import { ScannedLawFilesComponent } from './scanned-law-files/scanned-law-files.component';
import { AddEditFileComponent } from './scanned-law-files/add-edit/add-edit-file.component';
import { CallsListComponent } from './scanned-law-files/calls-list/calls-list.component';
import { DurationMomentPipe } from '../_directives/moment-duration.pipe';
import { CauseListComponent } from './cause-list/cause-list.component';
import { AddEditCauseComponent } from './cause-list/add-edit/add-edit-cause.component';
import { InquiryFilesComponent } from './inquiry-files/inquiry-files.component';
import { EmployeeOnLeaveComponent } from './employee-on-leave/employee-on-leave.component';
import { ShowCauseLetterComponent } from './show-cause-letter/show-cause-letter.component';
import { SummariesComponent } from './summaries/summaries.component';
import { SummaryComponent } from './summary/summary.component';
import { ScannedDocumentsComponent } from './scanned-documents/scanned-documents.component';
@NgModule({
  declarations: [FileTrackingSystemComponent, ScannedLawFilesComponent, AddEditFileComponent, InboxComponent, MyApplicationsComponent,
    SentComponent, ImportantComponent, ApplicationComponent, RecordRoomComponent, LetterComponent, FileRequisitionComponent,
    NewComponent, DispatchFilesComponent, ScannedDocumentsComponent, SummaryComponent, SummariesComponent, ShowCauseLetterComponent, EmployeeOnLeaveComponent, AddEditCauseComponent, CauseListComponent, CallsListComponent, ScannedFilesComponent, AddEditComponent, DiaryComponent, SearchTrackingComponent, InquiryFilesComponent],
  imports: [
    CommonModule,
    FileTrackingSystemRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    InputsModule,
    GridModule,
    DropDownsModule,
    DateInputsModule,
    TooltipModule,
    SharedModule,
    ButtonsModule,
    LaddaModule,
    DialogsModule,
    TabsModule,
    ExcelExportModule,
    ScrollViewModule,
    NgxImageZoomModule

  ],
  providers: [FileTrackingSystemService, ApplicationFtsService]
})
export class FileTrackingSystemModule { }
