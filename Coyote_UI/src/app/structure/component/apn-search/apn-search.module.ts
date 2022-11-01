import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ApnSearchComponent } from './apn-search.component';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { MyCommonModule } from 'src/app/my-common/my-common.module';


const routes: Routes = [
  {
    path: '',
    component: ApnSearchComponent
  },
  {
    path: 'apn-search',
    component: ApnSearchComponent
  },
  {
    path: 'product-without-apn',
    component: ApnSearchComponent
  }
];

@NgModule({
  declarations: [
    ApnSearchComponent,
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

export class ApnSearchModule { }
