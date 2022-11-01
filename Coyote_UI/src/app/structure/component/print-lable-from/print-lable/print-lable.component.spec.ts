import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PrintLableComponent } from './print-lable.component';

describe('PrintLableComponent', () => {
  let component: PrintLableComponent;
  let fixture: ComponentFixture<PrintLableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PrintLableComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PrintLableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
