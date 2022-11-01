import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MinOnHandComponent } from './min-on-hand.component';

describe('MinOnHandComponent', () => {
  let component: MinOnHandComponent;
  let fixture: ComponentFixture<MinOnHandComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MinOnHandComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MinOnHandComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
