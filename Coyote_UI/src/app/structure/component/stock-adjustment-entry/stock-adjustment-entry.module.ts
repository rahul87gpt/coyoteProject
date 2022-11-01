import { NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { StockAdjustmentEntryComponent } from './stock-adjustment-entry.component';
import { StockAdjustmentEntryDeatailsComponent } from './stock-adjustment-entry-deatails/stock-adjustment-entry-deatails.component';
import { DatepickerModule, BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { PipesModule } from 'src/app/pipes/pipes.module';
import { CustomdatetimeformatPipe } from 'src/app/pipes/customdatetimeformat.pipe';
import { MyCommonModule } from 'src/app/my-common/my-common.module';

const routes: Routes = [
  {
    path: '',
    component: StockAdjustmentEntryComponent
  },
  {
    path: 'new',
    component: StockAdjustmentEntryDeatailsComponent
  },
  {
    path: 'update/:id',
    component: StockAdjustmentEntryDeatailsComponent
  }
];

@NgModule({
  providers:[DatePipe,CustomdatetimeformatPipe],
  declarations: [
    StockAdjustmentEntryComponent,
    StockAdjustmentEntryDeatailsComponent,
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    BsDatepickerModule.forRoot(),
    DatepickerModule.forRoot(),
    RouterModule.forChild(routes),
    TooltipModule.forRoot(),
    PipesModule,
    MyCommonModule
  ]
})

export class StockAdjustmentEntryModule { }
