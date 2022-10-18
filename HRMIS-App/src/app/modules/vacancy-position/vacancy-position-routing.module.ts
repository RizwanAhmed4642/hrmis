import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { VacancyPositionComponent } from './vacancy-position.component';
import { AddEditComponent } from './add-edit/add-edit.component';
import { ReportManagerComponent } from './report-manager/report-manager.component';
import { VpHoldingsComponent } from './vp-holdings/vp-holdings.component';
import { VacancyStatusComponent } from './vacancy-status/vacancy-status.component';
import { VPQuotaComponent } from './quota/quota.component';

const routes: Routes = [
  {
    path: '', component: VacancyPositionComponent
  },
  {
    path: 'new', component: AddEditComponent, data: { title: 'New Vacancy' }
  },
  {
    path: 'check', component: VpHoldingsComponent, data: { title: 'Search Vacancy' }
  },
  {
    path: 'sts', component: VacancyStatusComponent, data: { title: 'Vacancy Status' }
  },
  {
    path: 'quota', component: VPQuotaComponent, data: { title: 'Vacancy Status' }
  },
  {
    path: ':hfcode/new', component: AddEditComponent, data: { title: 'New Vacancy' }
  },
  {
    path: 'detail', component: VacancyPositionComponent, data: { title: 'Vacancy Detail' }
  },
  {
    path: 'report-manager', component: ReportManagerComponent, data: { title: 'Report Manager' }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class VacancyPositionRoutingModule { }
