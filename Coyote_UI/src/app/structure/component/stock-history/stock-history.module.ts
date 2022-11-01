import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { DatepickerModule, BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { StockHistoryComponent } from './stock-history.component';
import { OptimalOrderHistoryComponent } from './optimal-order-history/optimal-order-history.component';
import { NgSelectModule, NgOption} from '@ng-select/ng-select';
import { StockSheetComponent } from './stock-sheet/stock-sheet.component';
import { PurchaseCostVarianceComponent } from './purchase-cost-variance/purchase-cost-variance.component';
import { PdeLoadHistoryComponent } from './pde-load-history/pde-load-history.component';
import { PipesModule } from 'src/app/pipes/pipes.module';
import { CustomdatetimeformatPipe } from 'src/app/pipes/customdatetimeformat.pipe';
import { DateonlyPipe } from 'src/app/pipes/dateonly.pipe';
import { DateTimeFormatePipe } from 'src/app/pipes/date-time-formate.pipe';
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';
import { MyCommonModule } from 'src/app/my-common/my-common.module';
import { InvoicingComponent } from './invoicing/invoicing.component';
import { PromotionalTieUpComponent } from './promotional-tie-up/promotional-tie-up.component';

const routes: Routes = [
  {
    path: '',
    component: OptimalOrderHistoryComponent
  },
  {
    path: 'stock-sheet/:code',
    component: StockSheetComponent
  },
  {
    path: 'purchase-cost-variance',
    component: PurchaseCostVarianceComponent
  },
  {
    path: 'pde-load-history',
    component: PdeLoadHistoryComponent
  },

  {
    path: 'invoicing-history',
    component: InvoicingComponent
  },
  {
    path: 'promotion-tie-up',
    component: PromotionalTieUpComponent
  }
];

@NgModule({
  providers:[CustomdatetimeformatPipe, DateTimeFormatePipe, DateonlyPipe],
  declarations: [
    StockHistoryComponent,
    OptimalOrderHistoryComponent,
    StockSheetComponent,
    PurchaseCostVarianceComponent,
    PdeLoadHistoryComponent,
    InvoicingComponent,
    PromotionalTieUpComponent,
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule.forChild(routes),    
    BsDatepickerModule.forRoot(),
    DatepickerModule.forRoot(),
    TooltipModule.forRoot(),
    NgSelectModule,
    PipesModule,
    NgxExtendedPdfViewerModule,
    MyCommonModule
  ]
})

export class StockHistoryModule { }
