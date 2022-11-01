import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PrintPromotionalLabelsComponent } from './print-promotional-labels.component';

describe('PrintPromotionalLabelsComponent', () => {
  let component: PrintPromotionalLabelsComponent;
  let fixture: ComponentFixture<PrintPromotionalLabelsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PrintPromotionalLabelsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PrintPromotionalLabelsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
