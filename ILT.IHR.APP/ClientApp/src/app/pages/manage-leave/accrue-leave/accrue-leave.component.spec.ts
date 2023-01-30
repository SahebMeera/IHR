import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AccrueLeaveComponent } from './accrue-leave.component';

describe('AccrueLeaveComponent', () => {
  let component: AccrueLeaveComponent;
  let fixture: ComponentFixture<AccrueLeaveComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AccrueLeaveComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AccrueLeaveComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
