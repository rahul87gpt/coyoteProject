import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HostSettingComponent } from './host-setting/host-setting.component';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { BsDatepickerModule, DatepickerModule } from 'ngx-bootstrap/datepicker';
import { RouterModule, Routes } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgSelectModule } from '@ng-select/ng-select';
import { MyCommonModule } from 'src/app/my-common/my-common.module';

const routes: Routes = [
  {
    path: '',
    component: HostSettingComponent
  }
];

@NgModule({
  declarations: [HostSettingComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule.forChild(routes),
    BsDatepickerModule.forRoot(),
    DatepickerModule.forRoot(),
    TooltipModule.forRoot(),
    NgSelectModule,
    MyCommonModule
  ]
})
export class HostSettingModule { }
