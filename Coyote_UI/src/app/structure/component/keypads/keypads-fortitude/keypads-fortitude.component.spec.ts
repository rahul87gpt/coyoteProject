import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { KeypadsFortitudeComponent } from './keypads-fortitude.component';

describe('KeypadsFortitudeComponent', () => {
  let component: KeypadsFortitudeComponent;
  let fixture: ComponentFixture<KeypadsFortitudeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ KeypadsFortitudeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(KeypadsFortitudeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
