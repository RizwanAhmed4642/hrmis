import { NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';

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

import { AccountComponent } from './account/account.component';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { OnlinePromotionApplyComponent } from './online-promotion-apply.component';
import { OnlinePromotionApplyRoutingModule } from './online-promotion-apply-routing.module';
import { ProfileComponent } from './profile/profile.component';
import { SeniorityListComponent } from './seniority-list/seniority-list.component';
@NgModule({
  declarations: [OnlinePromotionApplyComponent, SeniorityListComponent, AccountComponent, ProfileComponent],
  imports: [
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
    OnlinePromotionApplyRoutingModule
  ],
  providers: [DatePipe]
})
export class OnlinePromotionApplyModule { }
