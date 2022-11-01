import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PdeLoadHistoryComponent } from './pde-load-history/pde-load-history.component';
import { RouterModule, Routes } from '@angular/router';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { PipesModule } from 'src/app/pipes/pipes.module';
import { CustomdatetimeformatPipe } from 'src/app/pipes/customdatetimeformat.pipe';
import { DateTimeFormatePipe } from 'src/app/pipes/date-time-formate.pipe';
const routes: Routes = [
  {
    path: '',
    component: PdeLoadHistoryComponent
  }
];
@NgModule({
  providers:[CustomdatetimeformatPipe, DateTimeFormatePipe],
  declarations: [PdeLoadHistoryComponent],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    TooltipModule.forRoot(),
    PipesModule
  ]
})
export class PdeLoadHistoryModule { }
