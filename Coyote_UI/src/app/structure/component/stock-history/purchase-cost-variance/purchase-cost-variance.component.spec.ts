import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PurchaseCostVarianceComponent } from './purchase-cost-variance.component';

describe('PurchaseCostVarianceComponent', () => {
  let component: PurchaseCostVarianceComponent;
  let fixture: ComponentFixture<PurchaseCostVarianceComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PurchaseCostVarianceComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PurchaseCostVarianceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
