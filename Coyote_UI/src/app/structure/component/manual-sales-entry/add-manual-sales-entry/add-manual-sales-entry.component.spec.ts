import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddManualSalesEntryComponent } from './add-manual-sales-entry.component';

describe('AddManualSalesEntryComponent', () => {
  let component: AddManualSalesEntryComponent;
  let fixture: ComponentFixture<AddManualSalesEntryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddManualSalesEntryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddManualSalesEntryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
