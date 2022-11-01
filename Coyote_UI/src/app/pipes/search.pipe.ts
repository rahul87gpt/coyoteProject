import { Pipe, PipeTransform } from '@angular/core';
import * as _ from 'lodash/fp';
@Pipe({
  name: 'search'
})
export class SearchPipe implements PipeTransform {

  transform(items: any[], searchTerm: string): string[] {
    if (!items) {
      return [];
    }
    if (!searchTerm) {
      return items;
    }

    searchTerm = searchTerm;
    return items.filter(item => {
      return item.desc.includes(searchTerm);
    });
  }

}
// {{store.desc || store.storeName}} {{" " + store ? store.code || store.storeCode : '' }