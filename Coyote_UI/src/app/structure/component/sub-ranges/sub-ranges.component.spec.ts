import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SubRangesComponent } from './sub-ranges.component';

describe('SubRangesComponent', () => {
  let component: SubRangesComponent;
  let fixture: ComponentFixture<SubRangesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SubRangesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SubRangesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
