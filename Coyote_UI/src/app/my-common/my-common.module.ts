import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UppercaseDirective } from '../directive/uppercase.directive';
import { CurrencyMaskDirective } from '../directive/currency-mask.directive';



@NgModule({
  declarations: [UppercaseDirective],
  exports:[UppercaseDirective],
  imports: [
    CommonModule
  ]
})
export class MyCommonModule {
  constructor(){
  }
 }
