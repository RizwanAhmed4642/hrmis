import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { HelplineRoutingModule } from './helpline-routing.module';
import { UserComponent } from './user/user.component';
import { HelplineComponent } from './helpline.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { GridModule } from '@progress/kendo-angular-grid';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { LaddaModule } from 'angular2-ladda';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { ButtonsModule } from '@progress/kendo-angular-buttons';

@NgModule({
  declarations: [UserComponent, HelplineComponent],
  imports: [
    CommonModule,
    HelplineRoutingModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    InputsModule,
    GridModule,
    DateInputsModule,
    DropDownsModule,
    ButtonsModule,
    LaddaModule,
    LayoutModule,
  ]
})
export class HelplineModule { }
