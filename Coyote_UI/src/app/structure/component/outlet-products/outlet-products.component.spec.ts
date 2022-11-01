import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OutletProductsComponent } from './outlet-products.component';

describe('OutletProductsComponent', () => {
  let component: OutletProductsComponent;
  let fixture: ComponentFixture<OutletProductsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OutletProductsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OutletProductsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
