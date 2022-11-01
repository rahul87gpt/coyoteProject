import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { KeypadsComponent } from './keypads.component';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { KeypadsFortitudeComponent } from './keypads-fortitude/keypads-fortitude.component';
import { ColorPickerModule } from 'ngx-color-picker';
import { NgSelectModule, NgOption } from '@ng-select/ng-select';
import { MyCommonModule } from 'src/app/my-common/my-common.module';

const routes: Routes = [
  {
    path: '',
    component: KeypadsComponent
  },
  {
    path: 'keypads-fortitude/:id',
    component: KeypadsFortitudeComponent
  },
  {
    path: 'keypads-fortitude',
    component: KeypadsFortitudeComponent
  }
];

@NgModule({
  declarations: [KeypadsComponent, KeypadsFortitudeComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    NgSelectModule,
    ColorPickerModule,
    RouterModule.forChild(routes),
    TooltipModule.forRoot(),
    MyCommonModule
  ]
})
export class KeypadsModule { }
