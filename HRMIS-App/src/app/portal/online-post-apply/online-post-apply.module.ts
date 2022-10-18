import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

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

import { AdvertisementComponent } from './advertisement/advertisement.component';
import { AccountComponent } from './account/account.component';
import { ProfileComponent } from './profile/profile.component';
import { OfferLetterComponent } from './offer-letter/offer-letter.component';
import { GrievanceComponent } from './grievance/grievance.component';
import { OnlinePostApplyRoutingModule } from './online-post-apply-routing.module';
import { OnlinePostApplyComponent } from './online-post-apply.component';

@NgModule({
  declarations: [OnlinePostApplyComponent, AccountComponent, ProfileComponent, OfferLetterComponent, AdvertisementComponent, GrievanceComponent],
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
    OnlinePostApplyRoutingModule
  ],
  providers: []
})
export class OnlinePostApplyModule { }
