import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TillsComponent } from './tills.component';
import { Routes, RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { NgSelectModule } from '@ng-select/ng-select';
import { MyCommonModule } from 'src/app/my-common/my-common.module';

const routes: Routes = [
  {
    path: '',
    component: TillsComponent
  },

];

@NgModule({
  declarations: [
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule.forChild(routes),
    NgSelectModule,
    TooltipModule.forRoot(),
    MyCommonModule
  ]
})
export class TillsModule { }
