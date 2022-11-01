import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DeactivateProductsComponent } from './deactivate-products.component';

describe('DeactivateProductsComponent', () => {
  let component: DeactivateProductsComponent;
  let fixture: ComponentFixture<DeactivateProductsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DeactivateProductsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DeactivateProductsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
