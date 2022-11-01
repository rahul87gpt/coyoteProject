import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NewStoreGroupComponent } from './new-store-group.component';

describe('NewStoreGroupComponent', () => {
  let component: NewStoreGroupComponent;
  let fixture: ComponentFixture<NewStoreGroupComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NewStoreGroupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NewStoreGroupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
