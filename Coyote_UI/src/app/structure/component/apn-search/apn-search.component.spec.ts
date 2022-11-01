import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ApnSearchComponent } from './apn-search.component';

describe('ApnSearchComponent', () => {
  let component: ApnSearchComponent;
  let fixture: ComponentFixture<ApnSearchComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ApnSearchComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ApnSearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
