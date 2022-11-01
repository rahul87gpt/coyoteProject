import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FinancialSummaryComponent } from './financial-summary.component';
import { SummaryComponent } from './summary/summary.component';
import { RouterModule, Routes } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BsDatepickerModule, DatepickerModule } from 'ngx-bootstrap/datepicker';
import { NgSelectModule } from '@ng-select/ng-select';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';
import { TimepickerModule } from 'ngx-bootstrap/timepicker';

const routes: Routes = [
  {
    path: '',
    component: FinancialSummaryComponent
  },
  {
    path: 'summary/:code',
    component: SummaryComponent
  },
]

@NgModule({
  declarations: [FinancialSummaryComponent, SummaryComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule.forChild(routes),
    BsDatepickerModule.forRoot(),
    DatepickerModule.forRoot(),
    TooltipModule.forRoot(),
    TimepickerModule.forRoot(),
    NgSelectModule,
    NgxExtendedPdfViewerModule

  ]
})
export class FinancialSummaryModule { }
