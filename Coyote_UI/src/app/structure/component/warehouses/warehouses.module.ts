import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { WarehousesComponent } from './warehouses.component';
import { AddWarehouseComponent } from './add-warehouse/add-warehouse.component';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { NgKnifeModule } from 'ng-knife';
import { NgSelectModule } from '@ng-select/ng-select';
import { MyCommonModule } from 'src/app/my-common/my-common.module';
const routes: Routes = [
  {
    path: '',
    component: WarehousesComponent
  },
  {
    path: 'add-warehouse',
    component: AddWarehouseComponent
  },
  {
    path: 'update-warehouse/:id',
    component: AddWarehouseComponent
  }
];

@NgModule({
  declarations: [
    WarehousesComponent,
    AddWarehouseComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule.forChild(routes),
    TooltipModule.forRoot(),
    NgKnifeModule,
    NgSelectModule,
    MyCommonModule
  ]
})

export class WarehousesModule { }


