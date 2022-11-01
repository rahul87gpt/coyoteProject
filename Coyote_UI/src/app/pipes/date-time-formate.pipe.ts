import { DatePipe } from '@angular/common';
import { Pipe, PipeTransform } from '@angular/core';
import { constant } from 'src/constants/constant';

@Pipe({
  name: 'dateTimeFormate'
})
export class DateTimeFormatePipe extends DatePipe implements PipeTransform {

  transform(value: any, args?: any): any {
    let date = new Date(value)
    // return date.toISOString()
    return super.transform(value, constant.PRODUCT_DATE_TIME_FMT);
  }

}
