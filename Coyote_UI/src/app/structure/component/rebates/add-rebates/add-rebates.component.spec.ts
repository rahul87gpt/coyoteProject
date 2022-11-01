import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddRebatesComponent } from './add-rebates.component';

describe('AddRebatesComponent', () => {
  let component: AddRebatesComponent;
  let fixture: ComponentFixture<AddRebatesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddRebatesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddRebatesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
