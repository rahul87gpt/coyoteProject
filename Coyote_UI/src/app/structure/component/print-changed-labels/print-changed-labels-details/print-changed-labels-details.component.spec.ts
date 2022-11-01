import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PrintChangedLabelsDetailsComponent } from './print-changed-labels-details.component';

describe('PrintChangedLabelsDetailsComponent', () => {
  let component: PrintChangedLabelsDetailsComponent;
  let fixture: ComponentFixture<PrintChangedLabelsDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PrintChangedLabelsDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PrintChangedLabelsDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
