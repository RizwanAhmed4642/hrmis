import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SwmoPromotionRoutingModule } from './swmo-promotion-routing.module';
import { SwmoPromoFormComponent } from './swmo-promo-form/swmo-promo-form.component';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { GridModule, SharedModule } from '@progress/kendo-angular-grid';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { TooltipModule } from '@progress/kendo-angular-tooltip';
import { LaddaModule } from 'angular2-ladda';
import { DialogsModule } from '@progress/kendo-angular-dialog';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { LayoutModule } from '@progress/kendo-angular-layout';


@NgModule({
  declarations: [SwmoPromoFormComponent],
  imports: [
    CommonModule,
    SwmoPromotionRoutingModule,
    ReactiveFormsModule,
    InputsModule,
    GridModule,
    DateInputsModule,
    DropDownsModule,
    ButtonsModule,
    LaddaModule,
    LayoutModule,
    FormsModule
  ]
})
export class SwmoPromotionModule { }
