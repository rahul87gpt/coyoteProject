import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddZoneOutletComponent } from './add-zone-outlet.component';

describe('AddZoneOutletComponent', () => {
  let component: AddZoneOutletComponent;
  let fixture: ComponentFixture<AddZoneOutletComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddZoneOutletComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddZoneOutletComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
