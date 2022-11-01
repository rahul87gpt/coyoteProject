import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SalesHistoryComponent } from './sales-history.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { NgSelectModule } from '@ng-select/ng-select';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { DatepickerModule, BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { SalesChartComponent } from './sales-chart/sales-chart.component';
import { DateTimeFormatePipe } from 'src/app/pipes/date-time-formate.pipe';
import { CustomdatetimeformatPipe } from 'src/app/pipes/customdatetimeformat.pipe';
import { MyCommonModule } from 'src/app/my-common/my-common.module';


const routes: Routes = [
  {
    path: '',
    component: SalesHistoryComponent
  },
  {
    path: 'chart/:code',
    component: SalesChartComponent
  }

];

@NgModule({
  providers:[CustomdatetimeformatPipe,DateTimeFormatePipe],
  declarations: [SalesHistoryComponent, SalesChartComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule.forChild(routes),    
    BsDatepickerModule.forRoot(),
    DatepickerModule.forRoot(),
    TooltipModule.forRoot(),
    NgSelectModule,
    MyCommonModule
  ]
})
export class SalesHistoryModule { }
