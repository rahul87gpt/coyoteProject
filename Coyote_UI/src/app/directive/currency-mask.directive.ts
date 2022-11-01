import { CurrencyPipe } from '@angular/common';
import { Directive, ElementRef, HostListener, OnInit } from '@angular/core';

@Directive({
  selector: '[appCurrencyMask]'
})
export class CurrencyMaskDirective implements OnInit  {

  private regexString(max?: number) {
    const maxStr = max ? `{0,${max}}` : `+`;
    return `^(\\d${maxStr}(\\.\\d{0,2})?|\\.\\d{0,2})$`
  }
  private el: HTMLInputElement;
  private digitRegex: RegExp;
  private setRegex(maxDigits?: number) {
    this.digitRegex = new RegExp(this.regexString(maxDigits), 'g')
  }

  constructor(
    private elementRef: ElementRef,
    private currencyPipe: CurrencyPipe
  ) {
    this.el = this.elementRef.nativeElement;
    this.setRegex();
  }

  ngOnInit() {
    this.el.value = this.currencyPipe.transform(this.el.value, 'USD');
    console.log(this.el.value,'value123')
  }

  @HostListener("focus", ["$event.target.value"])
  onFocus(value) {
    // on focus remove currency formatting
    this.el.value = value.replace(/[^0-9.]+/g, '')
  }

  @HostListener("blur", ["$event.target.value"])
  onBlur(value) {
    // on blur, add currency formatting
    this.el.value = this.currencyPipe.transform(value, 'USD');
  }

  // variable to store last valid input
  private lastValid = '';
  @HostListener('input', ['$event'])
  onInput(event) {
    // on input, run regex to only allow certain characters and format
    const cleanValue = (event.target.value.match(this.digitRegex) || []).join('')
    if (cleanValue || !event.target.value)
      this.lastValid = cleanValue
    this.el.value = cleanValue || this.lastValid
  }

}
