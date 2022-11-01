import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PdeLoadHistoryComponent } from './pde-load-history.component';

describe('PdeLoadHistoryComponent', () => {
  let component: PdeLoadHistoryComponent;
  let fixture: ComponentFixture<PdeLoadHistoryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PdeLoadHistoryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PdeLoadHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
