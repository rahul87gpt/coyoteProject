import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PrintSpecialPriceLabelsComponent } from './print-special-price-labels/print-special-price-labels.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';
import { CustomdatetimeformatPipe } from 'src/app/pipes/customdatetimeformat.pipe';
import { DateTimeFormatePipe } from 'src/app/pipes/date-time-formate.pipe';
import { PipesModule } from 'src/app/pipes/pipes.module';
import { NgSelectModule } from '@ng-select/ng-select';

const routes: Routes = [
  {
    path: '',
    component: PrintSpecialPriceLabelsComponent
  }
];

@NgModule({
  providers:[CustomdatetimeformatPipe, DateTimeFormatePipe],
  declarations: [PrintSpecialPriceLabelsComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule.forChild(routes),
    TooltipModule.forRoot(),
    NgSelectModule,
    NgxExtendedPdfViewerModule,
    PipesModule
  ]
})
export class PrintSpecialPriceLabelsModule { }
