import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SyncTillListComponent } from './sync-till-list.component';

describe('SyncTillListComponent', () => {
  let component: SyncTillListComponent;
  let fixture: ComponentFixture<SyncTillListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SyncTillListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SyncTillListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
