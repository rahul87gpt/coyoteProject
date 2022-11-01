import { Pipe, PipeTransform } from '@angular/core';
import { DatePipe } from '@angular/common';
import { constant } from 'src/constants/constant';

@Pipe({
  name: 'dateOnlyFormat'
})
export class DateonlyPipe extends DatePipe implements PipeTransform {

  transform(value: any, args?: any): any {
    let date = new Date(value)
    return super.transform(value, constant.WEEKENDING_DATE_FMT);
  }

}
