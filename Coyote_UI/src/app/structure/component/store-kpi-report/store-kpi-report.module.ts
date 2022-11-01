import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StoreKpiReportComponent } from './store-kpi-report/store-kpi-report.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { BsDatepickerModule, DatepickerModule } from 'ngx-bootstrap/datepicker';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { NgSelectModule } from '@ng-select/ng-select';
const routes: Routes = [
  {
    path: '',
    component: StoreKpiReportComponent
  },

];


@NgModule({
  declarations: [StoreKpiReportComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule.forChild(routes),    
    BsDatepickerModule.forRoot(),
    DatepickerModule.forRoot(),
    TooltipModule.forRoot(),
    NgSelectModule
  ]
})
export class StoreKpiReportModule { }
