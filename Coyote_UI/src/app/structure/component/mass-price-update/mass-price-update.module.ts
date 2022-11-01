import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MassPriceUpdateComponent } from './mass-price-update/mass-price-update.component';
import { Routes, RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { NgSelectModule } from '@ng-select/ng-select';
const routes: Routes = [
  {
    path: '',
    component: MassPriceUpdateComponent
  },
];

@NgModule({
  declarations: [MassPriceUpdateComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    NgSelectModule,
    RouterModule.forChild(routes),
    TooltipModule.forRoot()
  ]
})
export class MassPriceUpdateModule { }
