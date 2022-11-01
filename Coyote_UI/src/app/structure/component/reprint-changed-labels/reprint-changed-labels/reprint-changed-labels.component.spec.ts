import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReprintChangedLabelsComponent } from './reprint-changed-labels.component';

describe('ReprintChangedLabelsComponent', () => {
  let component: ReprintChangedLabelsComponent;
  let fixture: ComponentFixture<ReprintChangedLabelsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ReprintChangedLabelsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReprintChangedLabelsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
