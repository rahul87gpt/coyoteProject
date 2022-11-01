import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StockAdjustmentEntryComponent } from './stock-adjustment-entry.component';

describe('StockAdjustmentEntryComponent', () => {
  let component: StockAdjustmentEntryComponent;
  let fixture: ComponentFixture<StockAdjustmentEntryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StockAdjustmentEntryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StockAdjustmentEntryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
