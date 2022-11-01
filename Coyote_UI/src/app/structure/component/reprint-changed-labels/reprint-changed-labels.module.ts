import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReprintChangedLabelsComponent } from './reprint-changed-labels/reprint-changed-labels.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';
import { CustomdatetimeformatPipe } from 'src/app/pipes/customdatetimeformat.pipe';
import { DateTimeFormatePipe } from 'src/app/pipes/date-time-formate.pipe';
import { PipesModule } from 'src/app/pipes/pipes.module';
import { NgSelectModule } from '@ng-select/ng-select';

const routes: Routes = [
  {
    path: '',
    component: ReprintChangedLabelsComponent
  }
];


@NgModule({
  providers:[CustomdatetimeformatPipe, DateTimeFormatePipe],
  declarations: [ReprintChangedLabelsComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    NgSelectModule,
    FormsModule,
    RouterModule.forChild(routes),
    NgxExtendedPdfViewerModule,
    PipesModule
  ]
})
export class ReprintChangedLabelsModule { }
