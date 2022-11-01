import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RebatesComponent } from './rebates/rebates.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { BsDatepickerModule, DatepickerModule } from 'ngx-bootstrap/datepicker';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { NgSelectModule } from '@ng-select/ng-select';
import { PipesModule } from 'src/app/pipes/pipes.module';
import { AddRebatesComponent } from './add-rebates/add-rebates.component';
import { MyCommonModule } from 'src/app/my-common/my-common.module';

const routes: Routes = [
  {
    path: '',
    component: RebatesComponent
  },
  {
    path: 'add-rebate',
    component: AddRebatesComponent
  },
  {
    path: 'update-rebate/:id',
    component: AddRebatesComponent
  },
 
];

@NgModule({
  declarations: [RebatesComponent, AddRebatesComponent],
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
    MyCommonModule
  ]
})
export class RebatesModule { }
