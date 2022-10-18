import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DermaApplicationsRoutingModule } from './derma-applications-routing.module';
import { DermaAppFormComponent } from './derma-app-form/derma-app-form.component';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { GridModule } from '@progress/kendo-angular-grid';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { LaddaModule } from 'angular2-ladda';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { FormsModule } from '@angular/forms';
import { DermaAppListComponent } from './derma-app-list/derma-app-list.component';

@NgModule({
  declarations: [DermaAppFormComponent, DermaAppListComponent],
  imports: [
    CommonModule,
    DermaApplicationsRoutingModule,
    InputsModule,
    GridModule,
    DateInputsModule,
    DropDownsModule,
    ButtonsModule,
    LaddaModule,
    LayoutModule,
    FormsModule,
  ]
})
export class DermaApplicationsModule { }
