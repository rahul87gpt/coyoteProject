import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RebatesComponent } from './rebates.component';

describe('RebatesComponent', () => {
  let component: RebatesComponent;
  let fixture: ComponentFixture<RebatesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RebatesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RebatesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
