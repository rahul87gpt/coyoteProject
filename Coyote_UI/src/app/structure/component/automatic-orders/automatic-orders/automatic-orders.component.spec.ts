import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AutomaticOrdersComponent } from './automatic-orders.component';

describe('AutomaticOrdersComponent', () => {
  let component: AutomaticOrdersComponent;
  let fixture: ComponentFixture<AutomaticOrdersComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AutomaticOrdersComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AutomaticOrdersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
