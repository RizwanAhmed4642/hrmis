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
import { CEOApplicationRoutingModule } from './ceo-application-routing.module';
import { CEOApplicationComponent } from './ceo-application.component';
import { ListComponent } from './list/list.component';
import { ViewComponent } from './view/view.component';
import { DialogsModule } from '@progress/kendo-angular-dialog';
import { CEOApplicationService } from './ceo-application.service';
import { AddEditComponent } from './add-edit/add-edit.component';
import { AdhocComponent } from './adhoc/adhoc.component';
import { AdhocNewComponent } from './adhoc-new/adhoc-new.component';

@NgModule({
  declarations: [AddEditComponent, AdhocNewComponent, AdhocComponent, CEOApplicationComponent, ListComponent, ViewComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    InputsModule,
    GridModule,
    DropDownsModule,
    DateInputsModule,
    TooltipModule,
    SharedModule,
    ButtonsModule,
    DialogsModule,
    LaddaModule,
    CEOApplicationRoutingModule
  ],
  providers: [CEOApplicationService]
})
export class CEOApplicationModule { }
