import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PrintPromotionalLabelsComponent } from './print-promotional-labels/print-promotional-labels.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router'
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';
import { DateTimeFormatePipe } from 'src/app/pipes/date-time-formate.pipe';
import { CustomdatetimeformatPipe } from 'src/app/pipes/customdatetimeformat.pipe';
import { PipesModule } from 'src/app/pipes/pipes.module';
import { NgSelectModule } from '@ng-select/ng-select';
import { MyCommonModule } from 'src/app/my-common/my-common.module';

const routes: Routes = [
  {
    path: '',
    component: PrintPromotionalLabelsComponent
  }
];


@NgModule({
  providers:[CustomdatetimeformatPipe, DateTimeFormatePipe],
  declarations: [PrintPromotionalLabelsComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule.forChild(routes),
    NgSelectModule,
    NgxExtendedPdfViewerModule,
    PipesModule,
    MyCommonModule
  ]

})
export class PrintPromotionalLabelsModule { }
