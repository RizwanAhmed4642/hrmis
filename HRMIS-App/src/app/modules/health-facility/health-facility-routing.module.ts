import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ListComponent } from './list/list.component';
import { AddEditComponent } from './add-edit/add-edit.component';
import { ViewComponent } from './view/view.component';
import { AuthGuard } from '../../_guards/auth.guard';
import { HealthFacilityAuthGuard } from './_auth.guard';
import { HealthFacilityEditAuthGuard } from './add-edit/edit.guard';
import { HealthFacilityAddAuthGuard } from './add-edit/add.guard';
import { CovidFacilitiesComponent } from './covid-facilities/covid-facilities.component';
import { CovidStaffComponent } from './covid-staff/covid-staff.component';

const routes: Routes = [
  { path: '', redirectTo: 'list', pathMatch: 'full' },
  { path: 'list', component: ListComponent, canActivate: [AuthGuard, HealthFacilityAuthGuard] },
  { path: 'new-facility', component: CovidFacilitiesComponent, canActivate: [AuthGuard, HealthFacilityAuthGuard] },
  { path: 'facility-staff', component: CovidStaffComponent, canActivate: [AuthGuard, HealthFacilityAuthGuard] },
  { path: 'new', component: AddEditComponent, canActivate: [AuthGuard, HealthFacilityAddAuthGuard] },
  { path: ':hfcode/edit', component: AddEditComponent, canActivate: [AuthGuard, HealthFacilityEditAuthGuard] },
  { path: ':hfcode', component: ViewComponent, canActivate: [AuthGuard] }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HealthFacilityRoutingModule { }
