import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AutomaticOrdersComponent } from './automatic-orders/automatic-orders.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { BsDatepickerModule, DatepickerModule } from 'ngx-bootstrap/datepicker';
import { NgSelectModule } from '@ng-select/ng-select';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
const routes: Routes = [
  {
    path: '',
    component: AutomaticOrdersComponent
  },
  ];
@NgModule({
  declarations: [AutomaticOrdersComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forChild(routes),
    BsDatepickerModule.forRoot(),
    DatepickerModule.forRoot(),
    TooltipModule.forRoot(),
    NgSelectModule,
    NgMultiSelectDropDownModule
  ]
})
export class AutomaticOrdersModule { }
