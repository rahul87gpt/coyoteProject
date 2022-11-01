import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CorporateTreeComponent } from './corporate-tree.component';

describe('CorporateTreeComponent', () => {
  let component: CorporateTreeComponent;
  let fixture: ComponentFixture<CorporateTreeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CorporateTreeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CorporateTreeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
