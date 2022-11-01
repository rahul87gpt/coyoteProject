import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CostPriceZonesComponent } from './cost-price-zones/cost-price-zones.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { NgSelectModule } from '@ng-select/ng-select';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { MyCommonModule } from 'src/app/my-common/my-common.module';

const routes: Routes = [
  {
    path: '',
    component: CostPriceZonesComponent
  },
  {
    path: 'cost-price-zones-module/:code',
    component: CostPriceZonesComponent
  },
  
];


@NgModule({
  declarations: [CostPriceZonesComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule.forChild(routes),
    NgSelectModule,
    TooltipModule.forRoot(),
    MyCommonModule
  ]
})
export class CostPriceZonesModule { }
