import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddCashiersComponent } from './add-cashiers.component';

describe('AddCashiersComponent', () => {
  let component: AddCashiersComponent;
  let fixture: ComponentFixture<AddCashiersComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddCashiersComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddCashiersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
