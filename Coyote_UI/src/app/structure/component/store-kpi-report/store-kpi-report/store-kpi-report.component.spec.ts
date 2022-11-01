import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StoreKpiReportComponent } from './store-kpi-report.component';

describe('StoreKpiReportComponent', () => {
  let component: StoreKpiReportComponent;
  let fixture: ComponentFixture<StoreKpiReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StoreKpiReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StoreKpiReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
