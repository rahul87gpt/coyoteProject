import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StoreDashboardComponent } from './store-dashboard/store-dashboard.component';
import { Routes, RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { BsDatepickerModule, DatepickerModule } from 'ngx-bootstrap/datepicker';
import { PipesModule } from 'src/app/pipes/pipes.module';
import { NgSelectModule } from '@ng-select/ng-select';
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';
const routes: Routes = [
  {
    path: '',
    component: StoreDashboardComponent
  },
  
];


@NgModule({
  declarations: [StoreDashboardComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule.forChild(routes),
    TooltipModule.forRoot(),
    BsDatepickerModule.forRoot(),
    DatepickerModule.forRoot(),
    PipesModule,
    NgSelectModule,
    NgxExtendedPdfViewerModule
  ]
})
export class StoreDashboardModule { }
