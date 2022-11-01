import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StoreGroupsComponent } from './scheduler.component';

describe('StoreGroupsComponent', () => {
  let component: StoreGroupsComponent;
  let fixture: ComponentFixture<StoreGroupsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StoreGroupsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StoreGroupsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
