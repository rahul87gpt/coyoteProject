import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

import { ProductsComponent } from './products.component';
import { AddProductComponent } from './add-product/add-product.component';

import { DatepickerModule, BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { NgSelectModule} from '@ng-select/ng-select';
import { CustomdatetimeformatPipe } from 'src/app/pipes/customdatetimeformat.pipe';
import { DateTimeFormatePipe } from 'src/app/pipes/date-time-formate.pipe';
import { PipesModule } from 'src/app/pipes/pipes.module';
import { defineLocale } from 'ngx-bootstrap/chronos';
import { enGbLocale } from 'ngx-bootstrap/locale';
import { CurrencyMaskModule } from 'ng2-currency-mask';
import { MyCommonModule } from 'src/app/my-common/my-common.module';
import { OnlyNumberDirective } from 'src/app/directive/only-number.directive';
defineLocale('en-gb',enGbLocale);
/// import { ProductWithoutApnComponent } from './product-without-apn/product-without-apn.component';

const routes: Routes = [
  {
    path: '',
    component: ProductsComponent
  },
  {
    path: 'add-product',
    component: AddProductComponent
  },
  {
    path: 'update-product/:id',
    component: AddProductComponent
  },
  {
    path: 'keypads/update-product/:id',
    component: AddProductComponent
  },
  {
    path: 'clone-product/:id',
    component: AddProductComponent
  },
  {
    path: 'outlet-product/:id',
    component: AddProductComponent
  },
  {
    path: 'apn-update/:id',
    component: AddProductComponent
  },
  {
    path: 'product-without-apn',
    component: ProductsComponent
  }
];

@NgModule({
  providers:[CustomdatetimeformatPipe,DateTimeFormatePipe],
  declarations: [ProductsComponent, AddProductComponent,OnlyNumberDirective],
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
    CurrencyMaskModule,
    MyCommonModule
  ]
})

export class ProductsModule { }

