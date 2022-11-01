import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PrintLableFromComponent } from './print-lable-from.component';
import { PrintLableComponent } from './print-lable/print-lable.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { BsDatepickerModule, DatepickerModule } from 'ngx-bootstrap/datepicker';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { NgSelectModule } from '@ng-select/ng-select';
import { PipesModule } from 'src/app/pipes/pipes.module';
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';

const routes: Routes = [
  {
    path: '',
    component: PrintLableFromComponent
  },
  {
    path: 'print-lable-from/:code',
    component: PrintLableComponent
  },
]


@NgModule({
  declarations: [PrintLableFromComponent, PrintLableComponent],
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
    NgxExtendedPdfViewerModule
  ]
})
export class PrintLableFromModule { }
