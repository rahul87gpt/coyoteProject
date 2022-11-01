import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OutletSupplierComponent } from './outlet-supplier/outlet-supplier.component';
import { Routes, RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { NgSelectModule } from '@ng-select/ng-select';
import { MyCommonModule } from 'src/app/my-common/my-common.module';

const routes: Routes = [
  {
    path: '',
    component: OutletSupplierComponent
  },

];

@NgModule({
  declarations: [OutletSupplierComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forChild(routes),
    NgSelectModule,
    TooltipModule.forRoot(),
    MyCommonModule
  ]
})
export class OutletSupplierModule { }
