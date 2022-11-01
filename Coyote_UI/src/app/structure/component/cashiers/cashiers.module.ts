import { NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { CashiersComponent } from './cashiers/cashiers.component';
import { AddCashiersComponent } from './add-cashiers/add-cashiers.component';
import { Routes, RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { DatepickerModule, BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { ImageCropperModule } from 'ngx-image-cropper';
import { NgSelectModule } from '@ng-select/ng-select';
import { MyCommonModule } from 'src/app/my-common/my-common.module';

const routes: Routes = [
  {
    path: '',
    component: CashiersComponent
  },
  {
    path: 'add',
    component: AddCashiersComponent
  },
  {
    path: 'update/:id',
    component: AddCashiersComponent
  }

];

@NgModule({
  declarations: [CashiersComponent, AddCashiersComponent],
  providers:[DatePipe],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    NgSelectModule,
    RouterModule.forChild(routes),
    TooltipModule.forRoot(),
    BsDatepickerModule.forRoot(),
    DatepickerModule.forRoot(),
    ImageCropperModule,
    MyCommonModule
  ]
})
export class CashiersModule { }
