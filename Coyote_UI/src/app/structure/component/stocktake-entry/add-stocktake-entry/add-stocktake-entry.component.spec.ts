import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddStocktakeEntryComponent } from './add-stocktake-entry.component';

describe('AddStocktakeEntryComponent', () => {
  let component: AddStocktakeEntryComponent;
  let fixture: ComponentFixture<AddStocktakeEntryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddStocktakeEntryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddStocktakeEntryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
