import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MassPriceUpdateComponent } from './mass-price-update.component';

describe('MassPriceUpdateComponent', () => {
  let component: MassPriceUpdateComponent;
  let fixture: ComponentFixture<MassPriceUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MassPriceUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MassPriceUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
