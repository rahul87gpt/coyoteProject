import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TillJournalComponent } from './till-journal.component';

describe('TillJournalComponent', () => {
  let component: TillJournalComponent;
  let fixture: ComponentFixture<TillJournalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TillJournalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TillJournalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
