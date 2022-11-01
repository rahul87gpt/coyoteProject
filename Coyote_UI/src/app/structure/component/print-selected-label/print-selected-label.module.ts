import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { DatepickerModule, BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { NgSelectModule, NgOption} from '@ng-select/ng-select';
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';
import { TooltipModule } from 'ngx-bootstrap/tooltip';

import { PrintSelectedListComponent } from './print-selected-list/print-selected-list.component';

const routes: Routes = [
  {
    path: '',
    component: PrintSelectedListComponent
  }
]

@NgModule({
  declarations: [PrintSelectedListComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule.forChild(routes),
    BsDatepickerModule.forRoot(),
    DatepickerModule.forRoot(),
    TooltipModule.forRoot(),
    NgSelectModule,
    NgxExtendedPdfViewerModule
  ]
})
export class PrintSelectedLabelModule { }
