import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ManualSalesEntryComponent } from './manual-sales-entry/manual-sales-entry.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { AddManualSalesEntryComponent } from './add-manual-sales-entry/add-manual-sales-entry.component';
import { BsDatepickerModule, DatepickerModule } from 'ngx-bootstrap/datepicker';
import { PipesModule } from 'src/app/pipes/pipes.module';
import { CurrencyMaskModule } from 'ng2-currency-mask';
import { MyCommonModule } from 'src/app/my-common/my-common.module';
const routes: Routes = [
  {
    path: '',
    component: ManualSalesEntryComponent
  }, 
  {
    path: 'new',
    component: AddManualSalesEntryComponent
  },
  {
    path: 'update/:id',
    component: AddManualSalesEntryComponent
  }, 
];
@NgModule({
  declarations: [ManualSalesEntryComponent, AddManualSalesEntryComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule.forChild(routes),
    TooltipModule.forRoot(),
    BsDatepickerModule.forRoot(),
    DatepickerModule.forRoot(),
    PipesModule,
    CurrencyMaskModule,
    MyCommonModule
  ],
})
export class ManualSalesEntryModule { }
