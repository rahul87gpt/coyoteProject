import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HostChangesSheetComponent } from './host-changes-sheet.component';

describe('HostChangesSheetComponent', () => {
  let component: HostChangesSheetComponent;
  let fixture: ComponentFixture<HostChangesSheetComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HostChangesSheetComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HostChangesSheetComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
