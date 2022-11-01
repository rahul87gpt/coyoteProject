import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddXeroAccountingComponent } from './add-xero-accounting.component';

describe('AddXeroAccountingComponent', () => {
  let component: AddXeroAccountingComponent;
  let fixture: ComponentFixture<AddXeroAccountingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddXeroAccountingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddXeroAccountingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
