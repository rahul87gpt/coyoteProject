import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MasterListModuleComponent } from './master-list-module.component';

describe('MasterListModuleComponent', () => {
  let component: MasterListModuleComponent;
  let fixture: ComponentFixture<MasterListModuleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MasterListModuleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MasterListModuleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
