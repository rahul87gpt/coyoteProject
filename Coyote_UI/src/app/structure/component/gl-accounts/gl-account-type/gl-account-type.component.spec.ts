import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { GlAccountTypeComponent } from './gl-account-type.component';

describe('GlAccountTypeComponent', () => {
  let component: GlAccountTypeComponent;
  let fixture: ComponentFixture<GlAccountTypeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ GlAccountTypeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GlAccountTypeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
