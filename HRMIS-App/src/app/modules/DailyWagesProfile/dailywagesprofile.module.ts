import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TabsModule } from 'ngx-bootstrap/tabs';

import { ProfileRoutingModule } from './DailyWagesProfile-routing.module';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { GridModule } from '@progress/kendo-angular-grid';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { ExcelExportModule } from '@progress/kendo-angular-excel-export';
import { SharedModule } from '../../shared.module';
import { LaddaModule } from 'angular2-ladda';
import { DialogModule } from '@progress/kendo-angular-dialog';
// import { DailyWagesMapComponent } from './daily-wages-map/daily-wages-map.component';
// import { WagerProfileViewComponent } from './wager-profile-view/wager-profile-view.component';
// import { DailywagerCountComponent } from './dailywager-count/dailywager-count.component';
// import { DailywagerlistComponent } from './dailywagerlist/dailywagerlist.component';
// import { DailywagesComponent } from './dailywages/dailywages.component';

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
    DialogModule,
    ProfileRoutingModule,
    LaddaModule,
    ExcelExportModule,
    TabsModule
  ],
  declarations: [ ],
  exports:[],
  providers: []
})
export class ProfileModule { }
