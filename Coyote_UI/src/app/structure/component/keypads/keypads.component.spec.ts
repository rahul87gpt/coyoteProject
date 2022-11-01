import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { KeypadsComponent } from './keypads.component';

describe('KeypadsComponent', () => {
  let component: KeypadsComponent;
  let fixture: ComponentFixture<KeypadsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ KeypadsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(KeypadsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
