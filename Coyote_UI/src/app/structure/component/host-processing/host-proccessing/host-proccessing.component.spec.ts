import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HostProccessingComponent } from './host-proccessing.component';

describe('HostProccessingComponent', () => {
  let component: HostProccessingComponent;
  let fixture: ComponentFixture<HostProccessingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HostProccessingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HostProccessingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
