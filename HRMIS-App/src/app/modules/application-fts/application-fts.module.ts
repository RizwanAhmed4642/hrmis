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


import { ApplicationFtsRoutingModule } from './application-fts-routing.module';
import { ApplicationFtsComponent } from './application-fts.component';
import { AddEditComponent } from './add-edit/add-edit.component';
import { ListComponent } from './list/list.component';
import { ViewComponent } from './view/view.component';
import { DialogsModule } from '@progress/kendo-angular-dialog';
import { ApplicationFtsService } from './application-fts.service';
import { ScrollViewModule } from '@progress/kendo-angular-scrollview';
import { InboxesComponent } from './inboxes/inboxes.component';
import { FileTrackingSystemService } from '../../file-tracking-system/file-tracking-system.service';
import { DocumentsComponent } from './documents/documents.component';
import { OnlineTransferComponent } from './list/online-transfer-applications/online-transfer.component';
import { ExcelExportModule } from '@progress/kendo-angular-excel-export';
import { DurationMomentPipe } from '../../_directives/moment-duration.pipe';
import { DDSFileComponent } from './add-edit/file/file.component';
import { TabsModule } from 'ngx-bootstrap';

@NgModule({
  declarations: [ApplicationFtsComponent, DDSFileComponent, OnlineTransferComponent, AddEditComponent, ListComponent, ViewComponent, InboxesComponent, DocumentsComponent],
  imports: [
    CommonModule,
    ApplicationFtsRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    InputsModule,
    GridModule,
    ButtonsModule,
    DropDownsModule,
    DateInputsModule,
    LaddaModule,
    DialogsModule,
    TooltipModule,
    ExcelExportModule,
    SharedModule,
    ScrollViewModule,
    TabsModule
  ],
  providers: [ApplicationFtsService, FileTrackingSystemService]
})
export class ApplicationFtsModule { }
