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
import { AddEditComponent } from './add-edit/add-edit.component';
import { HrPostingComponent } from './hr-posting.component';
import { ListComponent } from './list/list.component';
import { ViewComponent } from './view/view.component';
import { DialogsModule } from '@progress/kendo-angular-dialog';
import { HrPostingService } from './hr-posting.service';
import { HrPostingRoutingModule } from './hr-posting-routing.module';

@NgModule({
  declarations: [AddEditComponent, HrPostingComponent, ListComponent, ViewComponent],
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
    HrPostingRoutingModule
  ],
  providers: [HrPostingService]
})
export class HrPostingModule { }
