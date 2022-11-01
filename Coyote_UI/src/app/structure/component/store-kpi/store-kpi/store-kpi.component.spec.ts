import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StoreKpiComponent } from './store-kpi.component';

describe('StoreKpiComponent', () => {
  let component: StoreKpiComponent;
  let fixture: ComponentFixture<StoreKpiComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StoreKpiComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StoreKpiComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
