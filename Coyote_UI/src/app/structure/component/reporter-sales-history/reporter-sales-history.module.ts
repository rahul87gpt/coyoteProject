import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SalesChartComponent } from './sales-chart/sales-chart.component';
import { ReporterSalesHistoryComponent } from './reporter-sales-history.component';
import { RouterModule, Routes } from '@angular/router';
import { CustomdatetimeformatPipe } from 'src/app/pipes/customdatetimeformat.pipe';
import { DateTimeFormatePipe } from 'src/app/pipes/date-time-formate.pipe';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BsDatepickerModule, DatepickerModule } from 'ngx-bootstrap/datepicker';
import { NgSelectModule } from '@ng-select/ng-select';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';
import { PipesModule } from 'src/app/pipes/pipes.module';
import { TimepickerModule } from 'ngx-bootstrap/timepicker';

const routes: Routes = [
  {
    path: '',
    component: ReporterSalesHistoryComponent
  },
  {
    path: 'sales-chart/:code',
    component: SalesChartComponent
  }
];
@NgModule({
  providers: [CustomdatetimeformatPipe, DateTimeFormatePipe],
  declarations: [SalesChartComponent, ReporterSalesHistoryComponent],
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
export class ReporterSalesHistoryModule { }
