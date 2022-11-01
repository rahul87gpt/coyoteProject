import { NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { SchedulerComponent } from './scheduler/scheduler.component';
import { Routes, RouterModule } from '@angular/router';
// import { NewStoreGroupComponent } from './new-store-group/new-store-group.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { CustomdatetimeformatPipe } from 'src/app/pipes/customdatetimeformat.pipe';
import { PipesModule } from 'src/app/pipes/pipes.module';
import { DateTimeFormatePipe } from 'src/app/pipes/date-time-formate.pipe';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { TimepickerModule } from 'ngx-bootstrap/timepicker';
import { NgSelectModule } from '@ng-select/ng-select';
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';
import { TimeformatePipe } from 'src/app/pipes/timeformate.pipe';

const routes: Routes = [
  {
    path: '',
    component: SchedulerComponent
  }

];


@NgModule({
  declarations: [SchedulerComponent],
  providers: [DatePipe, CustomdatetimeformatPipe, DateTimeFormatePipe, TimeformatePipe],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule.forChild(routes),
    TooltipModule.forRoot(),
    BsDatepickerModule.forRoot(),
    TimepickerModule.forRoot(),
    NgSelectModule,
    NgxExtendedPdfViewerModule,
    PipesModule

  ]
})
export class SchedulerModule { }
