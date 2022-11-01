import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PosMessagingComponent } from './pos-messaging.component';

describe('PosMessagingComponent', () => {
  let component: PosMessagingComponent;
  let fixture: ComponentFixture<PosMessagingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PosMessagingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PosMessagingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
