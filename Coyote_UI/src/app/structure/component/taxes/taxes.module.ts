import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

import { TaxesComponent } from './taxes.component';
import { AddTaxComponent } from './add-tax/add-tax.component';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { MyCommonModule } from 'src/app/my-common/my-common.module';

const routes: Routes = [
  {
    path: '',
    component: TaxesComponent
  },
  {
    path: 'add-tax',
    component: AddTaxComponent
  },
  {
    path: 'update-tax/:id',
    component: AddTaxComponent
  }
];

@NgModule({
  declarations: [
	TaxesComponent,
	AddTaxComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule.forChild(routes),
    TooltipModule.forRoot(),
    MyCommonModule
  ]
})

export class TaxesModule { }
