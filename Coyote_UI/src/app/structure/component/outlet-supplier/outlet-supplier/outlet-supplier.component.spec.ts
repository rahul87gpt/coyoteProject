import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OutletSupplierComponent } from './outlet-supplier.component';

describe('OutletSupplierComponent', () => {
  let component: OutletSupplierComponent;
  let fixture: ComponentFixture<OutletSupplierComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OutletSupplierComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OutletSupplierComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
