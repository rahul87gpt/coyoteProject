import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { PrintLabelTypesComponent } from './print-label-types.component';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { MyCommonModule } from 'src/app/my-common/my-common.module';


const routes: Routes = [
  {
    path: '',
    component: PrintLabelTypesComponent
  }
];

@NgModule({
  declarations: [
    PrintLabelTypesComponent,
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

export class PrintLabelTypesModule { }
