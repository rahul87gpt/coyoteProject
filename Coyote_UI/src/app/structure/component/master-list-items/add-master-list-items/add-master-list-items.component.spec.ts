import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddMasterListItemsComponent } from './add-master-list-items.component';

describe('AddMasterListItemsComponent', () => {
  let component: AddMasterListItemsComponent;
  let fixture: ComponentFixture<AddMasterListItemsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddMasterListItemsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddMasterListItemsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
