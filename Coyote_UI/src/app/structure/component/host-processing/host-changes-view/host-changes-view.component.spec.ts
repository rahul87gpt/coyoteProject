import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HostChangesViewComponent } from './host-changes-view.component';

describe('HostChangesViewComponent', () => {
  let component: HostChangesViewComponent;
  let fixture: ComponentFixture<HostChangesViewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HostChangesViewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HostChangesViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
