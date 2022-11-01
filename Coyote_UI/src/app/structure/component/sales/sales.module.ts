import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { DatepickerModule, BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { SalesComponent } from './sales.component';
import { SalesReportsComponent } from './sales-reports/sales-reports.component';
import { NgSelectModule, NgOption} from '@ng-select/ng-select';
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';
import { ReporterComponent } from './reporter/reporter.component';
import { PipesModule } from 'src/app/pipes/pipes.module';
import { DateTimeFormatePipe } from 'src/app/pipes/date-time-formate.pipe';
import { CustomdatetimeformatPipe } from 'src/app/pipes/customdatetimeformat.pipe';
import { TimepickerModule } from 'ngx-bootstrap/timepicker';
import { MyCommonModule } from 'src/app/my-common/my-common.module';
import { StimulsoftViewerModule } from 'stimulsoft-viewer-angular';


const routes: Routes = [
  {
    path: '',
    component: SalesComponent
  },
  {
    path: 'sales-reports/:code',
    component: SalesReportsComponent
  },
  {
    path: 'reporter/:code',
    component: ReporterComponent
  }
];

@NgModule({
  providers:[CustomdatetimeformatPipe,DateTimeFormatePipe],
  declarations: [
    SalesComponent,
    SalesReportsComponent,
    ReporterComponent,
  ],
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
    NgxExtendedPdfViewerModule,
    PipesModule,
    MyCommonModule,
    StimulsoftViewerModule
  ]
})

export class SalesModule { }
