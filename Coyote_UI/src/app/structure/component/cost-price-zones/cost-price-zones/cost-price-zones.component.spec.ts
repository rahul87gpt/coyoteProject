import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CostPriceZonesComponent } from './cost-price-zones.component';

describe('CostPriceZonesComponent', () => {
  let component: CostPriceZonesComponent;
  let fixture: ComponentFixture<CostPriceZonesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CostPriceZonesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CostPriceZonesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
