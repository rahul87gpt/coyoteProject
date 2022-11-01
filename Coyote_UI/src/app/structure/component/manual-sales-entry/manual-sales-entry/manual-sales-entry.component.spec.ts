import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ManualSalesEntryComponent } from './manual-sales-entry.component';

describe('ManualSalesEntryComponent', () => {
  let component: ManualSalesEntryComponent;
  let fixture: ComponentFixture<ManualSalesEntryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ManualSalesEntryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ManualSalesEntryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
