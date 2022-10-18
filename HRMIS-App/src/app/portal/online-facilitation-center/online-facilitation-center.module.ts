import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OnlineFacilitationCenterRoutingModule } from './online-facilitation-center-routing.module';
import { OnlineFacilitationCenterComponent } from './online-facilitation-center.component';
import { AccountComponent } from './account/account.component';
import { ProfileComponent } from './profile/profile.component';
import { LeaveRecordComponent } from './profile/leave-record/leave-record.component';
import { ServiceHistoryComponent } from './profile/service-history/service-history.component';
import { ApplicationComponent } from './application/application.component';
import { AddComponent } from './application/add/add.component';
import { DetailsComponent } from './application/details/details.component';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { MegaMenuModule } from 'primeng/megamenu';
import {MenubarModule} from 'primeng/menubar';
import { GridModule } from '@progress/kendo-angular-grid';
import { DialogModule } from '@progress/kendo-angular-dialog';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { FormsModule } from '@angular/forms';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { DatePickerModule } from '@progress/kendo-angular-dateinputs';
import {TabViewModule} from 'primeng/tabview';
import { HttpClientModule } from '@angular/common/http';
import { LaddaModule } from 'angular2-ladda';


@NgModule({
  declarations: [OnlineFacilitationCenterComponent, AccountComponent, ProfileComponent, LeaveRecordComponent, ServiceHistoryComponent, ApplicationComponent, AddComponent, DetailsComponent],
  imports: [
    CommonModule,
    GridModule,
    LayoutModule,
    ButtonModule,
    FormsModule,
    InputsModule,
    DatePickerModule,
    DropDownsModule,
    LaddaModule,
    MegaMenuModule,
    MenubarModule,
    DialogModule,
    TabViewModule,
    HttpClientModule,  
    OnlineFacilitationCenterRoutingModule
  ]
})
export class OnlineFacilitationCenterModule { }
