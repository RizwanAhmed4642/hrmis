import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

// Import Containers
import { DefaultLayoutComponent } from './containers';

import { AuthGuard } from './_guards/auth.guard';
import { P404Component } from './_directives/error/404.component';
import { ApplicationFtsLivePreviewComponent } from './_directives/application-fts-live-preview/application-fts-live-preview.component';
import { OrderGenerationGuard } from './_guards/order-generation.guard';
import { MyAccountComponent } from './modules/profile/my-account/my-account.component';
import { UserModuleGuard } from './_guards/user-module.guard';
import { StatisticalOfficerGuard } from './_guards/statistical-officer.guard';
import { FacilitationCentreGuard } from './_guards/facilitation-centre.guard';
import { ReportingModuleGuard } from './_guards/reporting-module.guard';
import { PSOfficerGuard } from './_guards/ps-officer.guard';
import { RiBranchGuard } from './_guards/ri-branch.guard';
import { P401Component } from './_directives/error/401.component';
import { PrintItComponent } from './_directives/print-it.component';
import { DepartmentFTSReportComponent } from './_directives/department-fts-report/department-fts-report.component';
import { ReportingComponent } from './portal/online-posting/reporting/reporting.component';
import { ReportingSMOComponent } from './portal/online-post-apply/reporting/reporting.component';
import { ReportingApplicationComponent } from './portal/online-application/reporting/reporting.component';
import { CitizenPortalGuard } from './_guards/citizen-portal.guard';
import { MapsComponent } from './modules/dashboard/maps/maps.component';
import { PostingComponent } from './portal/online-posting/posting/posting.component';
import { CEOApplicationGuard } from './_guards/ceo-application.guard';
import { PHFMCHrGuard } from './_guards/phfmc-hr.guard';
import { AdminAuthGuard } from './_guards/adminAuth.guard';
import { CovidDashboardComponent } from './modules/dashboard/covid-dashboard/covid-dashboard.component';
import { PromotionApplicationComponent } from './portal/public-portal-mo/promotion-application/promotion-application.component';
import { PromotionApplicationListStartComponent } from './portal/public-portal-mo/promotion-application/promotion-application-list-start/promotion-application-list-start.component';
import { PromotionApplicationListDetailComponent } from './portal/public-portal-mo/promotion-application/promotion-application-list-detail/promotion-application-list-detail.component';
import { DepartmentFTSPendancyComponent } from './_directives/department-fts-pendancy/department-fts-pendancy.component';
import { AwaitingPostingComponent } from './portal/online-posting/awaiting-posting/awaiting-posting.component';
import { ConsultantPromoComponent } from './portal/online-posting/consultant-promo/consultant-promo.component';
import { DesignationMapComponent } from './modules/dashboard/designation-map/designation-map.component';
import { JobPostingComponent } from './portal/public-portal/posting/posting.component';
import { OrdersComponent } from './portal/public-portal/orders/orders.component';
import { DepartmentFTSPendancyTableComponent } from './_directives/department-fts-pendancy-table/department-fts-pendancy-table.component';
import { AdhocPostingComponent } from './modules/adhoc/adhoc-posting/adhoc-posting.component';
import { AdhocApplicantWorkingPaperComponent } from './modules/adhoc/adhoc-applicant-working-paper/adhoc-applicant-working-paper.component';
import { AdhocApplicantWPSComponent } from './modules/adhoc/adhoc-applicant-wps/adhoc-applicant-wps.component';
import { AdhocApplicantWRPSComponent } from './modules/adhoc/adhoc-applicant-wrps/adhoc-applicant-wrps.component';
import { AdhocMeritComponent } from './modules/adhoc/adhoc-merit/adhoc-merit.component';
import { AdhocAdminGuard } from './_guards/adhoc-admin.guard';
import { AdhocMeritPostingComponent } from './modules/adhoc/adhoc-merit-posting/adhoc-merit-posting.component';
import { PUCDashboardComponent } from './_directives/puc-dashboard/puc-dashboard.component';
import { VacancyChartsMapComponent } from './modules/dashboard/vacancy-charts-map/vacancy-charts-map.component';
import {DailywagesComponent} from './modules/DailyWagesProfile/dailywages/dailywages.component'
import { DailywagerlistComponent } from './modules/DailyWagesProfile/dailywagerlist/dailywagerlist.component';
import { DailywagerCountComponent } from './modules/DailyWagesProfile/dailywager-count/dailywager-count.component';
import { DailyWagesMapComponent } from './modules/DailyWagesProfile/daily-wages-map/daily-wages-map.component';
import { WagerProfileViewComponent } from './modules/DailyWagesProfile/wager-profile-view/wager-profile-view.component';
import { WagerCatListComponent } from './modules/DailyWagesProfile/wager-cat-list/wager-cat-list.component';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full',
  },
  {
    path: 'account',
    loadChildren: './account/account.module#AccountModule',
    data: { title: 'Login' }
  },
  {
    path: '',
    component: DefaultLayoutComponent,
    data: { title: 'Home' },
    children: [
      {
        path: 'dashboard',
        loadChildren: './modules/dashboard/dashboard.module#DashboardModule',
        canActivate: [AuthGuard],
        data: { title: 'Dashboard' }
      },
      {
        path: 'e',
        loadChildren: './modules/employee/employee.module#EmployeeModule',
        canActivate: [AuthGuard],
        data: { title: 'Dashboard' }
      },
      {
        path: 'database',
        loadChildren: './modules/database/database.module#DatabaseModule',
        canActivate: [AuthGuard, StatisticalOfficerGuard],
        data: { title: 'Database' }
      },
      
      {
        path: 'health-facility',
        loadChildren: './modules/health-facility/health-facility.module#HealthFacilityModule',
        canActivate: [AuthGuard],
        data: { title: 'Health Facility' }
      },
      {
        path: 'profile',
        loadChildren: './modules/profile/profile.module#ProfileModule',
        canActivate: [AuthGuard],
        data: { title: 'Profile' }
      },
      {
        path: 'vacancy-position',
        loadChildren: './modules/vacancy-position/vacancy-position.module#VacancyPositionModule',
        canActivate: [AuthGuard],
        data: { title: 'Vacancy Position' }
      },
      {
        path: 'application',
        loadChildren: './modules/application-fts/application-fts.module#ApplicationFtsModule',
        canActivate: [AuthGuard, FacilitationCentreGuard],
        data: { title: 'Application' }
      },
      {
        path: 'order',
        loadChildren: './modules/order/order.module#OrderModule',
        canActivate: [AuthGuard, OrderGenerationGuard],
        data: { title: 'Order / Notification' }
      },
      {
        path: 'attandance',
        loadChildren: './modules/attandance/attandance.module#AttandanceModule',
        canActivate: [AuthGuard],
        data: { title: 'Attandance' }
      },
      {
        path: 'reporting',
        loadChildren: './modules/reporting/reporting.module#ReportingModule',
        canActivate: [AuthGuard, ReportingModuleGuard],
        data: { title: 'Reporting' }
      },
      {
        path: 'jobs',
        loadChildren: './modules/job/job.module#JobModule',
        canActivate: [AuthGuard, PHFMCHrGuard],
        data: { title: 'Jobs' }
      },
      {
        path: 'adhoc-applications',
        loadChildren: './modules/adhoc/adhoc.module#AdhocModule',
        canActivate: [AuthGuard],
        data: { title: 'Adhoc Applications' }
      },
      {
        path: 'p',
        loadChildren: './modules/promotion/promotion.module#PromotionModule',
        canActivate: [AuthGuard],
        data: { title: 'Promotion Applications' }
      },
      {
        path: 'user',
        loadChildren: './modules/user/user.module#UserModule',
        canActivate: [AuthGuard, UserModuleGuard],
        data: { title: 'User' }
      },
      {
        path: 'helpline',
        loadChildren: './modules/helpline/helpline.module#HelplineModule',
        canActivate: [AuthGuard],
        data: { title: 'Helpline' }
      },
      {
        path: 'fts',
        loadChildren: './file-tracking-system/file-tracking-system.module#FileTrackingSystemModule',
        canActivate: [AuthGuard, PSOfficerGuard],
        data: { title: 'File Tracking System' }
      },
      {
        path: 'ri',
        loadChildren: './modules/ri-branch/ri-branch.module#RiBranchModule',
        canActivate: [AuthGuard, RiBranchGuard],
        data: { title: 'R & I Branch' }
      },
      {
        path: 'hr-posting',
        loadChildren: './modules/hr-posting/hr-posting.module#HrPostingModule',
        canActivate: [AuthGuard],
        data: { title: 'HR Cell' }
      },
      {
        path: 'ceo-application',
        loadChildren: './modules/ceo-application/ceo-application.module#CEOApplicationModule',
        canActivate: [AuthGuard, CEOApplicationGuard],
        data: { title: 'CEO Application' }
      },
      {
        path: 'cp',
        loadChildren: './modules/citizen-portal/citizen-portal.module#CitizenPortalModule',
        canActivate: [AuthGuard, CitizenPortalGuard],
        data: { title: 'Citizen Portal' }
      },
      {
        path: 'merits',
        data: { title: 'PPSC Candidates' }, canActivate: [AuthGuard], component: ReportingComponent
      },
      {
        path: 'promoted-candidates',
        data: { title: 'Promoted Candidates' }, canActivate: [AuthGuard], component: ConsultantPromoComponent
      },
      {
        path: 'smo-report',
        data: { title: 'Admin Posts' }, canActivate: [AuthGuard], component: ReportingSMOComponent
      },
      {
        path: 'dailywagerlist',
        data: { title: 'Daily Wager List' }, canActivate: [AuthGuard], component: DailywagerlistComponent
      },
      {
        path: 'daily-wages-count',
        data: { title: 'Daily Wages Count' }, canActivate: [AuthGuard], component: DailywagerCountComponent
      },
      {
        path: 'wagesmap',
        data: { title: 'Daily Wages Count' }, canActivate: [AuthGuard], component: DailyWagesMapComponent
      },
      {
        path: 'dailywagerprofile/:Id',
        data: { title: 'Daily Wager Profile' }, canActivate: [AuthGuard], component: DailywagesComponent
      },
      {
        path: 'wagerprofileview/:Id',
        data: { title: 'Daily Wager Profile View' }, canActivate: [AuthGuard], component: WagerProfileViewComponent
      },
      {
        path: 'wagercatlist',
        data: { title: 'Daily Wager Category View' }, canActivate: [AuthGuard], component: WagerCatListComponent
      },  
            
      { path: 'application-report', component: ReportingApplicationComponent, canActivate: [AuthGuard], data: { title: 'Online Transfer' } },
      { path: 'promotion-applications', component: PromotionApplicationListStartComponent, data: { title: 'Promotion Applications' } },
      { path: 'promotion-applications/:id', component: PromotionApplicationListDetailComponent, data: { title: 'Promotion Application' } },
      { path: 'my-account', component: MyAccountComponent, canActivate: [AuthGuard], data: { title: 'My Account' } }
    ]
  },
  // {
  //   path: 'employee-posting',
  //   loadChildren: './modules/employee-posting/employee-posting.module#EmployeePosting',
  //   canActivate: [AuthGuard],
  //   data: { title: 'Posting' }
  // },
  {
    path: 'covid19', component: CovidDashboardComponent, canActivate: [AuthGuard], data: { title: 'COVID-19' }
  }, {
    path: 'applicant-working-papers/:desigId/:batchId', component: AdhocApplicantWPSComponent
  }, {
    path: 'applicant-working-paper/:desigId/:batchId', component: AdhocApplicantWRPSComponent
  }, {
    path: 'applicant-wp/:id/:appId', component: AdhocApplicantWorkingPaperComponent
  }, {
    path: 'map', component: MapsComponent, canActivate: [AuthGuard], data: { title: 'Map' }
  }, {
    path: 'vacancy-charts', component: VacancyChartsMapComponent, canActivate: [AuthGuard], data: { title: 'Vacancy Charts' }
  }, {
    path: 'designation-map', component: DesignationMapComponent, canActivate: [AuthGuard], data: { title: 'Map' }
  }, {
    path: 'ppsc-posting', component: PostingComponent, canActivate: [AuthGuard, AdminAuthGuard], data: { title: 'PPSC Posting' }
  },
  {
    path: 'adhoc-posting/:desigId', canActivate: [AuthGuard], component: AdhocPostingComponent
  }, {
    path: 'adhoc-merit-wp', component: AdhocMeritComponent, canActivate: [AdhocAdminGuard]
  }, {
    path: 'adhoc-merit-wp-post', component: AdhocMeritPostingComponent, canActivate: [AdhocAdminGuard]
  },
  {
    path: 'administrative-scrutiny', canActivate: [AuthGuard], component: JobPostingComponent
  },
  {
    path: 'administrative-orders', canActivate: [AuthGuard], component: OrdersComponent
  },
  {
    path: 'awaiting-posting', component: AwaitingPostingComponent, canActivate: [AuthGuard, AdminAuthGuard], data: { title: 'Awaiting Posting' }
  },

  {
    path: 'fts-live',
    component: ApplicationFtsLivePreviewComponent,
    canActivate: [AuthGuard],
    data: { title: 'Live Application Preview' }
  },
  /* { path: 'print-it', component: PrintItComponent, canActivate: [AuthGuard], data: { title: 'Print' } }, */
  { path: '401', component: P401Component, canActivate: [AuthGuard], data: { title: 'Unauthorized' } },
  { path: 'puc-dashboard', component: PUCDashboardComponent, canActivate: [AuthGuard], data: { title: 'PUC Dashboard' } },
  { path: 'department-report', component: DepartmentFTSReportComponent, canActivate: [AuthGuard], data: { title: 'P&S Department' } },
  { path: 'department-pendancy', component: DepartmentFTSPendancyComponent, canActivate: [AuthGuard], data: { title: 'P&S Department' } },
  { path: 'department-pendency', component: DepartmentFTSPendancyComponent, canActivate: [AuthGuard], data: { title: 'P&S Department' } },
  { path: 'department-pendency-table', component: DepartmentFTSPendancyTableComponent, canActivate: [AuthGuard], data: { title: 'P&S Department' } },
  { path: 'promotion-form', component: PromotionApplicationComponent, data: { title: 'MO/WMO to Consultant' } },
  {
    path: 'main',
    loadChildren: './portal/public-portal/public-portal.module#PublicPortalModule',
    data: { title: 'Public Portal' }
  },
 /*  {
    path: 'preference',
    loadChildren: './portal/public-portal-mo/public-portal-mo.module#PublicPortalMOModule',
    data: { title: 'Medical Officer Applications' }
  }, */ {
    path: 'online-application',
    loadChildren: './portal/online-application/online-application.module#OnlineApplicationModule',
    data: { title: 'Online Application' }
  }, {
    path: 'retirement-application',
    loadChildren: './portal/retirement-application/retirement-application.module#RetirementApplicationModule',
    data: { title: 'Retirement Application' }
  },
  // Path to online-facilitation center
  {
    path: 'online-facilitation-center',
    // canActivate: [OnlineFacilitationAuthGuard],
    // canActivate: [AuthGuard],
    loadChildren: './portal/online-facilitation-center/online-facilitation-center.module#OnlineFacilitationCenterModule',
    data: { title: 'Online Facilitation Center' }
  },
  {
    path: 'ppsc',
    loadChildren: './portal/online-posting/online-posting.module#OnlinePostingModule',
    data: { title: 'Posting Preferences Portal' }
  },
  // {
  //   path: 'da',
  //   loadChildren: './portal/diploma-candidate/diploma-candidate.module#DiplomaCandidateModule',
  //   data: { title: 'Diploma Candidate' }
  // }, 
  // {
  //   path: 'swmo-promo',
  //   loadChildren: './portal/swmo-promotion/swmo-promotion.module#SwmoPromotionModule',
  //   data: { title: 'SWMO Promotion' }
  // },
  {
    path: 'derma-apply',
    loadChildren: './portal/derma-applications/derma-applications.module#DermaApplicationsModule',
    data: { title: 'Dermatologist Applications' }
  },
  /* {
    path: 'apmo-promotion',
    loadChildren: './portal/swmo-promotion/swmo-promotion.module#SwmoPromotionModule',
    data: { title: 'Diploma Candidate' }
  }, */
  // {
  //   path: 'mo-promotion',
  //   loadChildren: './portal/swmo-promotion/swmo-promotion.module#SwmoPromotionModule',
  //   data: { title: 'Preferences Portal' }
  // },
  {
    path: 'administrative',
    loadChildren: './portal/online-post-apply/online-post-apply.module#OnlinePostApplyModule',
    data: { title: 'Administrative Post' }
  }, {
    path: 'job',
    loadChildren: './portal/online-jobs/online-jobs.module#OnlineJobsModule',
    data: { title: 'Jobs' }
  },
  // {
  //   path: 'adhoc',
  //   loadChildren: './portal/online-adhoc/online-adhoc.module#OnlineAdhocModule',
  //   data: { title: 'Adhoc' }
  // } ,
  // , {
  //   path: 'pnas',
  //   loadChildren: './portal/online-admissions/online-admissions.module#OnlineAdmissionsModule',
  //   data: { title: 'Admissions' }
  // }, 
    {path: 'promotion-apply',
    loadChildren: './portal/online-promotion-apply/online-promotion-apply.module#OnlinePromotionApplyModule',
    data: { title: 'Promotion Application' }
  }, { path: '**', component: P404Component, data: { title: 'Not Found' } }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
