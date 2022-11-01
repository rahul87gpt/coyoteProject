import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StockAdjustmentEntryDeatailsComponent } from './stock-adjustment-entry-deatails.component';

describe('StockAdjustmentEntryDeatailsComponent', () => {
  let component: StockAdjustmentEntryDeatailsComponent;
  let fixture: ComponentFixture<StockAdjustmentEntryDeatailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StockAdjustmentEntryDeatailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StockAdjustmentEntryDeatailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
