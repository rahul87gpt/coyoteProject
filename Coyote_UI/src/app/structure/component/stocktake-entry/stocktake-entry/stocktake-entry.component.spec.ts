import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StocktakeEntryComponent } from './stocktake-entry.component';

describe('StocktakeEntryComponent', () => {
  let component: StocktakeEntryComponent;
  let fixture: ComponentFixture<StocktakeEntryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StocktakeEntryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StocktakeEntryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
