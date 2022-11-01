import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddCommoditiesComponent } from './add-commodities.component';

describe('AddCommoditiesComponent', () => {
  let component: AddCommoditiesComponent;
  let fixture: ComponentFixture<AddCommoditiesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddCommoditiesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddCommoditiesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
