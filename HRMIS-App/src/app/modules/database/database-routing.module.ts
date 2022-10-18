import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DatabaseComponent } from './database.component';
import { AuthGuard } from '../../_guards/auth.guard';
import { DesignationsComponent } from './designations/designations.component';
import { ServicesComponent } from './services/services.component';
import { HFTypeComponent } from './hftype/hftype.component';
import { HFCategoriesComponent } from './hfcategories/hfcategories.component';
import { VpProfileStatusComponent } from './vp-profile-status/vp-profile-status.component';
import { FacilitiesOpenComponent } from './facilities-open/facilities-open.component';
import { MeritactivedesignationComponent } from './merit-active-designation/merit-active-designation.component';
import { CordsAddressComponent } from './cords-address/cords-address.component';


const routes: Routes = [
  {
    path: '',
    component: DatabaseComponent,
    canActivate: [AuthGuard]
  },
  { path: 'designations', component: DesignationsComponent, canActivate: [AuthGuard] },
  { path: 'vp-profile-status', component: VpProfileStatusComponent, canActivate: [AuthGuard] },
  { path: 'services', component: ServicesComponent, canActivate: [AuthGuard] },
  { path: 'hftype', component: HFTypeComponent, canActivate: [AuthGuard] },
  { path: 'hfopen', component: FacilitiesOpenComponent, canActivate: [AuthGuard] },
  { path: 'hfcategories', component: HFCategoriesComponent, canActivate: [AuthGuard] },
  { path: 'cordsaddress', component: CordsAddressComponent, canActivate: [AuthGuard] },
  { path: 'meritactivedesig', component: MeritactivedesignationComponent, canActivate: [AuthGuard] },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DatabaseRoutingModule { }
