import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { DatabaseRoutingModule } from './database-routing.module';

import { InputsModule } from '@progress/kendo-angular-inputs';
import { GridModule } from '@progress/kendo-angular-grid';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { DialogsModule } from '@progress/kendo-angular-dialog';
import { DatabaseComponent } from './database.component';
import { DatabaseService } from './database.service';
import { DesignationsComponent } from './designations/designations.component';
import { SharedModule } from '../../shared.module';
import { ChipsModule } from 'primeng/chips';
import { ServicesComponent } from './services/services.component';
import { HFTypeComponent } from './hftype/hftype.component';
import { LaddaModule } from 'angular2-ladda';
import { HFCategoriesComponent } from './hfcategories/hfcategories.component';
import { VpProfileStatusComponent } from './vp-profile-status/vp-profile-status.component';
import { FacilitiesOpenComponent } from './facilities-open/facilities-open.component';
import { PostingOrderComponent } from './posting-order/posting-order.component';
import { MeritactivedesignationComponent } from './merit-active-designation/merit-active-designation.component';
import { CordsAddressComponent } from './cords-address/cords-address.component';


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
    DatabaseRoutingModule,
    DialogsModule,
    ChipsModule,
    LaddaModule
  ],
  declarations: [DatabaseComponent, PostingOrderComponent, FacilitiesOpenComponent, DesignationsComponent, VpProfileStatusComponent, ServicesComponent, HFTypeComponent, HFCategoriesComponent, MeritactivedesignationComponent, CordsAddressComponent],
  providers: [DatabaseService]
})
export class DatabaseModule { }
