import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddMasterListModuleComponent } from './add-master-list-module.component';

describe('AddMasterListModuleComponent', () => {
  let component: AddMasterListModuleComponent;
  let fixture: ComponentFixture<AddMasterListModuleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddMasterListModuleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddMasterListModuleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
