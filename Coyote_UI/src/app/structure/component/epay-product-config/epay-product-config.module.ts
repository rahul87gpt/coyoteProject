import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EpayProductConfigComponent } from './epay-product-config/epay-product-config.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { BsDatepickerModule, DatepickerModule } from 'ngx-bootstrap/datepicker';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { PipesModule } from 'src/app/pipes/pipes.module';
import { CustomdatetimeformatPipe } from 'src/app/pipes/customdatetimeformat.pipe';
import { DateTimeFormatePipe } from 'src/app/pipes/date-time-formate.pipe';

const routes: Routes = [
  {
    path: '',
    component: EpayProductConfigComponent
  },
  
  ];

@NgModule({
  providers:[CustomdatetimeformatPipe,DateTimeFormatePipe],
  declarations: [EpayProductConfigComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule.forChild(routes),
    BsDatepickerModule.forRoot(),
    DatepickerModule.forRoot(),
    TooltipModule.forRoot(),
    PipesModule
  ]
})
export class EpayProductConfigModule { }
