import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PrintLabelTypesComponent } from './print-label-types.component';

describe('PrintLabelTypesComponent', () => {
  let component: PrintLabelTypesComponent;
  let fixture: ComponentFixture<PrintLabelTypesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PrintLabelTypesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PrintLabelTypesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
