import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PrintLableFromComponent } from './print-lable-from.component';

describe('PrintLableFromComponent', () => {
  let component: PrintLableFromComponent;
  let fixture: ComponentFixture<PrintLableFromComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PrintLableFromComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PrintLableFromComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
