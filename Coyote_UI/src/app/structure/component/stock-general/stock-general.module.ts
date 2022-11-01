import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MinOnHandComponent } from './min-on-hand/min-on-hand.component';
import { Routes, RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { BsDatepickerModule, DatepickerModule } from 'ngx-bootstrap/datepicker';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import {ExternalStocktakeFileComponent} from './external-stocktake-file/external-stocktake-file.component'
import { NgSelectModule } from '@ng-select/ng-select';
const routes: Routes = [
  {
    path: '',
    component: MinOnHandComponent
  },
  {
    path: 'external-stocktake-file',
    component: ExternalStocktakeFileComponent
  },
];


@NgModule({
  declarations: [MinOnHandComponent, ExternalStocktakeFileComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    NgSelectModule,
    RouterModule.forChild(routes),    
    BsDatepickerModule.forRoot(),
    DatepickerModule.forRoot(),
    TooltipModule.forRoot(),
  ]
})
export class StockGeneralModule { }
