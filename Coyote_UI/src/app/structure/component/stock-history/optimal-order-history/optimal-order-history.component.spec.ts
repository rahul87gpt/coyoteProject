import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OptimalOrderHistoryComponent } from './optimal-order-history.component';

describe('OptimalOrderHistoryComponent', () => {
  let component: OptimalOrderHistoryComponent;
  let fixture: ComponentFixture<OptimalOrderHistoryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OptimalOrderHistoryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OptimalOrderHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
