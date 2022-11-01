import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserCardViewComponent } from './user-card-view.component';

describe('UserCardViewComponent', () => {
  let component: UserCardViewComponent;
  let fixture: ComponentFixture<UserCardViewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserCardViewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserCardViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
