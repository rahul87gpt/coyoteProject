import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PromotionalTieUpComponent } from './promotional-tie-up.component';

describe('PromotionalTieUpComponent', () => {
  let component: PromotionalTieUpComponent;
  let fixture: ComponentFixture<PromotionalTieUpComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PromotionalTieUpComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PromotionalTieUpComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
