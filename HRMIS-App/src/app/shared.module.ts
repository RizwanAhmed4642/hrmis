import { NgModule } from '@angular/core';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

import { InputsModule } from '@progress/kendo-angular-inputs';
import { GridModule } from '@progress/kendo-angular-grid';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ButtonsModule } from '@progress/kendo-angular-buttons';

import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { JwtInterceptor } from './_helpers/jwt.interceptor';
import { ErrorInterceptor } from './_helpers/error.interceptor';
import { DetailsComponent } from './file-tracking-system/application/details/details.component';
import { LaddaModule } from 'angular2-ladda';
import { DurationMomentPipe } from './_directives/moment-duration.pipe';


@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    InputsModule,
    GridModule,
    LaddaModule,
    DropDownsModule,
    ButtonsModule,
    ReactiveFormsModule
  ],
  declarations: [ DurationMomentPipe,DetailsComponent],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true }
  ],
  exports: [DurationMomentPipe,DetailsComponent]
})
export class SharedModule { }
