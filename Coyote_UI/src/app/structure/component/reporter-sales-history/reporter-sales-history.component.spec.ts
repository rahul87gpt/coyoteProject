import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReporterSalesHistoryComponent } from './reporter-sales-history.component';

describe('ReporterSalesHistoryComponent', () => {
  let component: ReporterSalesHistoryComponent;
  let fixture: ComponentFixture<ReporterSalesHistoryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ReporterSalesHistoryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReporterSalesHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
