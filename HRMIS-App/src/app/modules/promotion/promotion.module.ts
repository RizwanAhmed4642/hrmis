import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { PDFExportModule } from "@progress/kendo-angular-pdf-export";

import { InputsModule } from '@progress/kendo-angular-inputs';
import { GridModule } from '@progress/kendo-angular-grid';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { DialogsModule } from '@progress/kendo-angular-dialog';
import { SharedModule } from '../../shared.module';
import { ChipsModule } from 'primeng/chips';
import { LaddaModule } from 'angular2-ladda';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { PrmotionRoutingModule } from './promotion-routing.module';
import { IntlModule } from '@progress/kendo-angular-intl';
import { PromotionComponent } from './promotion.component';
import { SeniorityListComponent } from './seniority/seniority-list.component';
import { ProfileComponent } from './profile/profile.component';
import { PromotionacrComponent } from './acr/promotionacr.component';



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
    PrmotionRoutingModule,
    DialogsModule,
    LayoutModule,
    TabsModule,
    ChipsModule,
    LaddaModule,
    IntlModule,
    DateInputsModule,
    PDFExportModule
  ],
  declarations: [PromotionComponent, SeniorityListComponent, ProfileComponent, PromotionacrComponent]
})
export class PromotionModule { }
