import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PrintSpecialPriceLabelsComponent } from './print-special-price-labels.component';

describe('PrintSpecialPriceLabelsComponent', () => {
  let component: PrintSpecialPriceLabelsComponent;
  let fixture: ComponentFixture<PrintSpecialPriceLabelsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PrintSpecialPriceLabelsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PrintSpecialPriceLabelsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
