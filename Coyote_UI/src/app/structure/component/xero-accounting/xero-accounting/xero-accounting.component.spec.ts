import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { XeroAccountingComponent } from './xero-accounting.component';

describe('XeroAccountingComponent', () => {
  let component: XeroAccountingComponent;
  let fixture: ComponentFixture<XeroAccountingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ XeroAccountingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(XeroAccountingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
