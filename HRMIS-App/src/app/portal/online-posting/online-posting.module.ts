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

import { OnlinePostingRoutingModule } from './online-posting-routing.module';
import { OnlinePostingComponent } from './online-posting.component';
import { AccountComponent } from './account/account.component';
import { ProfileComponent } from './profile/profile.component';
import { OnlinePostingService } from './online-porting.service';
import { OfferLetterComponent } from './offer-letter/offer-letter.component';
import { PreferencesComponent } from './preferences/preferences.component';
import { GrievanceComponent } from './grievance/grievance.component';
import { ReportingComponent } from './reporting/reporting.component';
import { ProfileEntryComponent } from './profile-entry/profile-entry.component';
import { ReviewComponent } from './review/review.component';
import { AwaitingPostingComponent } from './awaiting-posting/awaiting-posting.component';

@NgModule({
  declarations: [OnlinePostingComponent, ReviewComponent, ProfileEntryComponent, AccountComponent, ProfileComponent, OfferLetterComponent, PreferencesComponent, GrievanceComponent],
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
    OnlinePostingRoutingModule,
    DialogsModule,
  ],
  providers: []
})
export class OnlinePostingModule { }
