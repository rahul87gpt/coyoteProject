import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StocktakeEntryComponent } from './stocktake-entry/stocktake-entry.component';
import { AddStocktakeEntryComponent } from './add-stocktake-entry/add-stocktake-entry.component';
import { Routes, RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { DatepickerModule, BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { CustomdatetimeformatPipe } from 'src/app/pipes/customdatetimeformat.pipe';
import { DateTimeFormatePipe } from 'src/app/pipes/date-time-formate.pipe';
import { NgSelectModule } from '@ng-select/ng-select';
import { PipesModule } from 'src/app/pipes/pipes.module';
const routes: Routes = [
  {
    path: '',
    component: StocktakeEntryComponent
  },
  {
    path: 'update/:id',
    component: AddStocktakeEntryComponent
  },
  {
    path: 'new',
    component: AddStocktakeEntryComponent
  }

];

@NgModule({
  providers:[CustomdatetimeformatPipe,DateTimeFormatePipe],

  declarations: [StocktakeEntryComponent, AddStocktakeEntryComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    BsDatepickerModule.forRoot(),
    DatepickerModule.forRoot(),
    RouterModule.forChild(routes),
    TooltipModule.forRoot(),
    NgSelectModule,
    PipesModule
  ]
})
export class StocktakeEntryModule { }
