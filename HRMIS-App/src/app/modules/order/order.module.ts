import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { InputsModule } from '@progress/kendo-angular-inputs';
import { GridModule } from '@progress/kendo-angular-grid';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ButtonsModule } from '@progress/kendo-angular-buttons';

import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { TooltipModule } from '@progress/kendo-angular-tooltip';
import { PDFExportModule } from "@progress/kendo-angular-pdf-export";
import { SharedModule } from '../../shared.module';
import { DialogsModule } from '@progress/kendo-angular-dialog';
import { OrderRoutingModule } from './order-routing.module';
import { OrderComponent } from './order.component';
import { OrderService } from './order.service';
import { ViewComponent } from './view/view.component';
import { NewComponent } from './new/new.component';
import { LaddaModule } from 'angular2-ladda';
import { QuillModule } from 'ngx-quill';
import { CKEditorModule } from 'ng2-ckeditor';
import { EditorViewComponent } from './editor-view/editor-view.component';
import { TnPService } from './transfer-n-posting.service';
import { TnpEditorComponent } from './tnp-editor/tnp-editor.component';
import { UploadComponent } from './upload/upload.component';
import { SearchOrderComponent } from './search-order/search-order.component';
import { AdvanceSearchComponent } from './advance-search/advance-search.component';
import { CombineOrderComponent } from './combine-order/combine-order.component';
import { MutualTransferComponent } from './mutual-transfer/mutual-transfer.component';
import { PhfmcOrderListComponent } from './phfmc-order-list/phfmc-order-list.component';

@NgModule({
  declarations: [OrderComponent, CombineOrderComponent, MutualTransferComponent, ViewComponent, UploadComponent, SearchOrderComponent, NewComponent, EditorViewComponent, TnpEditorComponent, AdvanceSearchComponent, PhfmcOrderListComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    InputsModule,
    GridModule,
    DropDownsModule,
    DateInputsModule,
    TooltipModule,
    SharedModule,
    ButtonsModule,
    DialogsModule,
    LaddaModule,
    OrderRoutingModule,
    QuillModule, 
    CKEditorModule,
    PDFExportModule
  ],
  providers: [OrderService, TnPService]
})
export class OrderModule { }
