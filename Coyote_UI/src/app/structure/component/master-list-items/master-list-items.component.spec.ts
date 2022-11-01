import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MasterListItemsComponent } from './master-list-items.component';

describe('MasterListItemsComponent', () => {
  let component: MasterListItemsComponent;
  let fixture: ComponentFixture<MasterListItemsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MasterListItemsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MasterListItemsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
