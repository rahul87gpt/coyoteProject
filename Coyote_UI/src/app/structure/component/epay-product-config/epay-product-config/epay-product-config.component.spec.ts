import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EpayProductConfigComponent } from './epay-product-config.component';

describe('EpayProductConfigComponent', () => {
  let component: EpayProductConfigComponent;
  let fixture: ComponentFixture<EpayProductConfigComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EpayProductConfigComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EpayProductConfigComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
