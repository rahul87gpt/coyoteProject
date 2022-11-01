import { Pipe, PipeTransform } from '@angular/core';
import { DatePipe } from '@angular/common';
import { constant } from 'src/constants/constant';

@Pipe({
  name: 'customdatetimeformat'
})
export class CustomdatetimeformatPipe extends DatePipe implements PipeTransform {

  transform(value: any, args?: any): any {
    let date = new Date(value)
    // return date.toISOString()
    return super.transform(value, constant.DATE_FMT);
  }

}
