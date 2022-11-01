import { Directive, ElementRef } from '@angular/core';

@Directive({
  selector: '[appAutofocus]'
})
export class AutofocusDirective {

  constructor(public elementRef: ElementRef) {
    this.autofocus() ;
  }
  // ngOnInit() {
  
  // }

  autofocus(){
    setTimeout(() => {
      this.elementRef.nativeElement.focus();
    });
  }
  

}
