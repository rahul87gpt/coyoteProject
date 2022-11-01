import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExternalStocktakeFileComponent } from './external-stocktake-file.component';

describe('ExternalStocktakeFileComponent', () => {
  let component: ExternalStocktakeFileComponent;
  let fixture: ComponentFixture<ExternalStocktakeFileComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ExternalStocktakeFileComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ExternalStocktakeFileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
