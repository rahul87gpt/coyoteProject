import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StoreKpiComponent } from './store-kpi/store-kpi.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { BsDatepickerModule, DatepickerModule } from 'ngx-bootstrap/datepicker';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { NgSelectModule } from '@ng-select/ng-select';
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';
import { PipesModule } from 'src/app/pipes/pipes.module';
import { CustomdatetimeformatPipe } from 'src/app/pipes/customdatetimeformat.pipe';
import { DateTimeFormatePipe } from 'src/app/pipes/date-time-formate.pipe';
import { TimepickerModule } from 'ngx-bootstrap/timepicker';
const routes: Routes = [
  {
    path: 'report/:code',
    component: StoreKpiComponent
  },
  // {
  //   path: 'sales-reports/:code',
  //   component: SalesReportsComponent
  // },
  // {
  //   path: 'store-kpi/:code',
  //   component: StoreKpiComponent
  // }
];


@NgModule({
  providers: [CustomdatetimeformatPipe, DateTimeFormatePipe],
  declarations: [StoreKpiComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule.forChild(routes),
    BsDatepickerModule.forRoot(),
    DatepickerModule.forRoot(),
    TooltipModule.forRoot(),
    NgSelectModule,
    NgxExtendedPdfViewerModule,
    PipesModule,
    TimepickerModule.forRoot()
  ]
})
export class StoreKpiModule { }
