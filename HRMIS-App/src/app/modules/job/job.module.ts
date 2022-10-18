import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { JobRoutingModule } from './job-routing.module';

import { InputsModule } from '@progress/kendo-angular-inputs';
import { GridModule } from '@progress/kendo-angular-grid';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { DialogsModule } from '@progress/kendo-angular-dialog';
import { JobComponent } from './job.component';
import { JobService } from './job.service';
import { SharedModule } from '../../shared.module';
import { ChipsModule } from 'primeng/chips';
import { LaddaModule } from 'angular2-ladda';
import { JobsOpenComponent } from './jobs-open/jobs-open.component';
import { DocumentsComponent } from './documents/documents.component';
import { JobApplicationComponent } from './job-application/job-application.component';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { TabsModule } from 'ngx-bootstrap/tabs';

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
    JobRoutingModule,
    DialogsModule,
    LayoutModule,
    TabsModule,
    ChipsModule,
    LaddaModule
  ],
  declarations: [JobComponent, DocumentsComponent, JobApplicationComponent, JobsOpenComponent],
  providers: [JobService]
})
export class JobModule { }
