import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { LocationStrategy, HashLocationStrategy } from '@angular/common';

import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { TabsModule } from 'ngx-bootstrap/tabs';

import { AppComponent } from './app.component';

// Import containers
import { DefaultLayoutComponent, A4LayoutComponent } from './containers';
import { QuillModule } from 'ngx-quill'

const APP_CONTAINERS = [
  DefaultLayoutComponent,
  A4LayoutComponent
];

import {
  AppAsideModule,
  AppBreadcrumbModule,
  AppHeaderModule,
  AppSidebarModule,
  AppFooterModule
} from '@coreui/angular';

// Import routing module
import { AppRoutingModule } from './app.routing';

// Import 3rd party components
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { P404Component } from './_directives/error/404.component';
import { NotificationComponent } from './_directives/notification.component';
import { HttpClientModule } from '@angular/common/http';
import { AfterValueChangedDirective } from './_directives/after-value-changed.directive';
import { P500Component } from './_directives/error/500.component';
import { SharedModule } from './shared.module';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { DialogsModule } from '@progress/kendo-angular-dialog';
import { ApplicationFtsLivePreviewComponent } from './_directives/application-fts-live-preview/application-fts-live-preview.component';
// prime ng
import { MultiSelectModule } from 'primeng/multiselect';
import { RippleModule } from '@progress/kendo-angular-ripple';
import { MyAccountComponent } from './modules/profile/my-account/my-account.component';
import { P401Component } from './_directives/error/401.component';
import { Config } from './_helpers/config.class';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { RootDialogComponent } from './_directives/root-dialog.component';
import { PrintItComponent } from './_directives/print-it.component';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { LaddaModule } from 'angular2-ladda';
import { MomentModule } from 'ngx-moment';
import { NgxImageZoomModule } from 'ngx-image-zoom';
import { AgGridModule } from 'ag-grid-angular';
import { DepartmentFTSReportComponent } from './_directives/department-fts-report/department-fts-report.component';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { GridModule } from '@progress/kendo-angular-grid';
import { ChartsModule } from '@progress/kendo-angular-charts';
import 'hammerjs';
import { ReportingComponent } from './portal/online-posting/reporting/reporting.component';

import { ReportingSMOComponent } from './portal/online-post-apply/reporting/reporting.component';
import { ReportingApplicationComponent } from './portal/online-application/reporting/reporting.component';
import { MapsComponent } from './modules/dashboard/maps/maps.component';
import { EncryptionServiceModule, CRYPT_CONFIG_PROVIDER } from 'angular-encryption-service';
import { AppCryptConfigProvider } from './_helpers/crypto.config';

import { AgmCoreModule } from '@agm/core';
import { PostingComponent } from './portal/online-posting/posting/posting.component';
import { ExcelExportModule } from '@progress/kendo-angular-excel-export';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { CovidDashboardComponent } from './modules/dashboard/covid-dashboard/covid-dashboard.component';
import { PromotionApplicationComponent } from './portal/public-portal-mo/promotion-application/promotion-application.component';
import { PromotionApplicationListStartComponent } from './portal/public-portal-mo/promotion-application/promotion-application-list-start/promotion-application-list-start.component';
import { PromotionApplicationListDetailComponent } from './portal/public-portal-mo/promotion-application/promotion-application-list-detail/promotion-application-list-detail.component';
import { DepartmentFTSPendancyComponent } from './_directives/department-fts-pendancy/department-fts-pendancy.component';
import { DetailsComponent } from './_directives/department-fts-pendancy/details/details.component';
import { AwaitingPostingComponent } from './portal/online-posting/awaiting-posting/awaiting-posting.component';
import { ConsultantPromoComponent } from './portal/online-posting/consultant-promo/consultant-promo.component';
import { DesignationMapComponent } from './modules/dashboard/designation-map/designation-map.component';
import { JobPostingComponent } from './portal/public-portal/posting/posting.component';
import { OrdersComponent } from './portal/public-portal/orders/orders.component';
import { DepartmentFTSPendancyTableComponent } from './_directives/department-fts-pendancy-table/department-fts-pendancy-table.component';
import { DetailzComponent } from './_directives/department-fts-pendancy-table/detailz/detailz.component';
import { AdhocPostingComponent } from './modules/adhoc/adhoc-posting/adhoc-posting.component';
import { AdhocApplicantWorkingPaperComponent } from './modules/adhoc/adhoc-applicant-working-paper/adhoc-applicant-working-paper.component';
import { AdhocApplicantWPSComponent } from './modules/adhoc/adhoc-applicant-wps/adhoc-applicant-wps.component';
import { AdhocApplicantWRPSComponent } from './modules/adhoc/adhoc-applicant-wrps/adhoc-applicant-wrps.component';
import { AdhocMeritComponent } from './modules/adhoc/adhoc-merit/adhoc-merit.component';
import { AdhocMeritPostingComponent } from './modules/adhoc/adhoc-merit-posting/adhoc-merit-posting.component';
import {DailywagesComponent} from './modules/DailyWagesProfile/dailywages/dailywages.component'
import {DailywagerlistComponent} from './modules/DailyWagesProfile/dailywagerlist/dailywagerlist.component'
import {DailywagerCountComponent} from './modules/DailyWagesProfile/dailywager-count/dailywager-count.component'
import { DailyWagesMapComponent } from './modules/DailyWagesProfile/daily-wages-map/daily-wages-map.component';
import { WagerProfileViewComponent } from './modules/DailyWagesProfile/wager-profile-view/wager-profile-view.component';
import { WagerCatListComponent } from './modules/DailyWagesProfile/wager-cat-list/wager-cat-list.component';
import { PUCDetailComponent } from './_directives/puc-dashboard/puc-detail/puc-detail.component';
import { PUCDashboardComponent } from './_directives/puc-dashboard/puc-dashboard.component';
import { VacancyChartsMapComponent } from './modules/dashboard/vacancy-charts-map/vacancy-charts-map.component';
import { FileComponent } from './modules/profile/view/components/file/file.component';


@NgModule({
  imports: [
    BrowserModule,
    HttpClientModule,
    //FormsModule,
    AppRoutingModule,
    AppAsideModule,
    AppBreadcrumbModule.forRoot(),
    AppHeaderModule,
    AppSidebarModule,
    AppFooterModule,
    PerfectScrollbarModule,
    BsDropdownModule.forRoot(),
    TabsModule.forRoot(),
    EncryptionServiceModule.forRoot(),
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    DropDownsModule,
    LaddaModule,
    DialogsModule,
    GridModule,
    DateInputsModule,
    ButtonsModule,
    InputsModule,
    RippleModule,
    QuillModule,
    MomentModule,
    NgxImageZoomModule.forRoot(),
    ChartsModule,
    ExcelExportModule,
    AgmCoreModule.forRoot({
      apiKey: 'AIzaSyDhK39-ZBuptKm4OabOMehNNRYL7DXalgI'
      // apiKey: 'AIzaSyDhK39-ZBuptKm4OabOMehNNRYL7DXalgI'

      /* apiKey is required, unless you are a 
      premium customer, in which case you can 
      use clientId 
      */
    }),
    LayoutModule
  ],
  declarations: [
    AppComponent,
    P404Component,
    P401Component,
    NotificationComponent,
    MyAccountComponent,
    PrintItComponent,
    P500Component,
    JobPostingComponent,
    OrdersComponent,
    AdhocPostingComponent,
    DailywagesComponent,
    DailywagerlistComponent,
    DailyWagesMapComponent,
    WagerProfileViewComponent,
    WagerCatListComponent,
    DailywagerCountComponent,
    DepartmentFTSReportComponent,
    PUCDashboardComponent,
    PUCDetailComponent,
    ReportingApplicationComponent,
    PromotionApplicationListStartComponent,
    PromotionApplicationListDetailComponent,
    ...APP_CONTAINERS,
    AfterValueChangedDirective,
    RootDialogComponent,
    ReportingComponent,
    AdhocApplicantWorkingPaperComponent,
    ConsultantPromoComponent, AdhocMeritComponent,
    AdhocApplicantWPSComponent,
    AdhocMeritPostingComponent,
    AdhocApplicantWRPSComponent,
    ReportingSMOComponent,
    DepartmentFTSPendancyTableComponent,
    PostingComponent,
    AwaitingPostingComponent,
    MapsComponent,
    VacancyChartsMapComponent,
    DesignationMapComponent,
    DetailsComponent,
    DetailzComponent,
    CovidDashboardComponent,
    PromotionApplicationComponent,
    ApplicationFtsLivePreviewComponent,
    DepartmentFTSPendancyComponent
  ],
  providers: [
    { provide: CRYPT_CONFIG_PROVIDER, useValue: AppCryptConfigProvider }
    /*     {
          provide: LocationStrategy,
          useClass: HashLocationStrategy
        } */
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }