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

import { AccountComponent } from './account/account.component';
import { ProfileComponent } from './profile/profile.component';
import { OfferLetterComponent } from './offer-letter/offer-letter.component';
import { PreferencesComponent } from './preferences/preferences.component';
import { JobsListComponent } from './jobs-list/jobs-list.component';
import { ProfileEntryComponent } from './profile-entry/profile-entry.component';
import { ReviewComponent } from './review/review.component';
import { OnlineJobsRoutingModule } from './online-jobs-routing.module';
import { OnlineJobsComponent } from './online-jobs.component';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { ExperienceComponent } from './experience/experience.component';
import { ApplyNowComponent } from './apply-now/apply-now.component';
@NgModule({
  declarations: [OnlineJobsComponent, ApplyNowComponent, ExperienceComponent, ReviewComponent, ProfileEntryComponent, AccountComponent, ProfileComponent, OfferLetterComponent, PreferencesComponent, JobsListComponent],
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
    OnlineJobsRoutingModule
  ],
  providers: []
})
export class OnlineJobsModule { }
