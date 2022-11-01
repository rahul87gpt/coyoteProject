import { Directive, ElementRef, Input, Renderer2, SimpleChanges } from '@angular/core';

@Directive({
  selector: '[appHightlight]'
})
export class HightlightDirective {

  @Input() search:any;
  constructor(private element:ElementRef,private renderer:Renderer2) {}
  setBackgroundColor(search){
  if(search){
  let value=this.element.nativeElement.innerHTML;
  let newValue=
  isNaN(value)?value.toString():value.toString();
  let newSearch=
  isNaN(search) ? search.toString():search.toString();
  newValue.indexOf(newSearch) > -1 ?this.renderer.setStyle(this.element.nativeElement,'backgroundColor','yellow'):
  this.renderer.setStyle(this.element.nativeElement,'backgroundColor','transparent');
  }
  else{
  this.renderer.setStyle(this.element.nativeElement,'backgroundColor','transparent');
  }
  }
  ngOnChanges(change:SimpleChanges){
  if(change.search){
  this.setBackgroundColor(change.search?.currentValue);
  }}

}
