import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrderComponent } from './order/order.component';
import { AddOrderComponent } from './add-order/add-order.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { DatepickerModule, BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { NgSelectModule, NgOption } from '@ng-select/ng-select';
import { PipesModule } from 'src/app/pipes/pipes.module';
import { CustomdatetimeformatPipe } from 'src/app/pipes/customdatetimeformat.pipe';
import { DateTimeFormatePipe } from 'src/app/pipes/date-time-formate.pipe';
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';
import { CurrencyMaskModule } from 'ng2-currency-mask';
import { MyCommonModule } from 'src/app/my-common/my-common.module';


const routes: Routes = [
  {
    path: '',
    component: OrderComponent
  },
  {
    path: 'add',
    component: AddOrderComponent
  },
  {
    path: 'update/:id',
    component: AddOrderComponent
  }
];


@NgModule({
  providers: [CustomdatetimeformatPipe, DateTimeFormatePipe],
  declarations: [OrderComponent, AddOrderComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forChild(routes),
    BsDatepickerModule.forRoot(),
    DatepickerModule.forRoot(),
    TooltipModule.forRoot(),
    NgSelectModule,
    PipesModule,
    NgxExtendedPdfViewerModule,
    CurrencyMaskModule,
    MyCommonModule
  ]
})
export class OrderModule { }
