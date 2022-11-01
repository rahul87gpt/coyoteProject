import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DeactivateProductsComponent } from './deactivate-products/deactivate-products.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { NgSelectModule } from '@ng-select/ng-select';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';

const routes: Routes = [
  {
    path: '',
    component: DeactivateProductsComponent
  },

];

@NgModule({
  declarations: [DeactivateProductsComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    NgSelectModule,
    NgMultiSelectDropDownModule,
    FormsModule,
    RouterModule.forChild(routes),
    TooltipModule.forRoot()
  ]
})
export class DeactivateProductsModule { }
