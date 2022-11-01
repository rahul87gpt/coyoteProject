import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PrintChangedLabelsComponent } from './print-changed-labels.component';

describe('PrintChangedLabelsComponent', () => {
  let component: PrintChangedLabelsComponent;
  let fixture: ComponentFixture<PrintChangedLabelsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PrintChangedLabelsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PrintChangedLabelsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
