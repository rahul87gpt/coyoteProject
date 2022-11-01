import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PrintChangedLabelsComponent } from './print-changed-labels.component';
import { PrintChangedLabelsDetailsComponent } from './print-changed-labels-details/print-changed-labels-details.component';
import { RouterModule, Routes } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';
import { NgSelectModule } from '@ng-select/ng-select';

const routes: Routes = [
  {
    path: '',
    component: PrintChangedLabelsComponent
  }
];
@NgModule({
  declarations: [PrintChangedLabelsComponent, PrintChangedLabelsDetailsComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    NgSelectModule,
    FormsModule,
    RouterModule.forChild(routes),
    TooltipModule.forRoot(),
    NgxExtendedPdfViewerModule
  ]
})
export class PrintChangedLabelsModule { }
