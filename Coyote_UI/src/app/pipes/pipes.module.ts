import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CustomdatetimeformatPipe } from './customdatetimeformat.pipe';
import { DateTimeFormatePipe } from './date-time-formate.pipe';
import { SearchPipe } from './search.pipe';
import { CustomTFdatetimeformatPipe } from './24_date_time.pipe';
import { DateonlyPipe } from './dateonly.pipe';
import { TimeformatePipe } from './timeformate.pipe';
import { DollerCurrencyPipe } from './doller-currency.pipe';

@NgModule({
  declarations: [
    CustomdatetimeformatPipe,
    DateTimeFormatePipe,
    SearchPipe,
    CustomTFdatetimeformatPipe,
    DateonlyPipe,
    TimeformatePipe,
    DollerCurrencyPipe
  ],
  imports: [
    CommonModule
  ],
  exports: [
    CustomdatetimeformatPipe,
    DateTimeFormatePipe,
    SearchPipe,
    CustomTFdatetimeformatPipe,
    DateonlyPipe,
    TimeformatePipe
  ]
})
export class PipesModule {
  static forRoot(): any[] | import("@angular/core").Type<any> | import("@angular/core").ModuleWithProviders<{}> {
    throw new Error('Method not implemented.');
  }
}
