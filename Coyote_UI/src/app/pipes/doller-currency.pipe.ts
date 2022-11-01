import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'dollerCurrency'
})
export class DollerCurrencyPipe implements PipeTransform {

  transform(value: unknown, ...args: unknown[]): unknown {
    return null;
  }

}
