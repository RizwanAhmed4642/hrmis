import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SwmoPromoFormComponent } from './swmo-promo-form/swmo-promo-form.component';


const routes: Routes = [
  {
    path: '',
    redirectTo: 'f',
    pathMatch: 'full',
  },
  {
    path: '',
    component: SwmoPromoFormComponent,
    data: { title: 'Swmo Promotion' },
    children: [
      { path: 'f', component: SwmoPromoFormComponent },
     
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SwmoPromotionRoutingModule { }
