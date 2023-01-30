import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TimesheetApprovalDenyComponent } from './timesheet-approval-deny.component';

describe('TimesheetApprovalDenyComponent', () => {
  let component: TimesheetApprovalDenyComponent;
  let fixture: ComponentFixture<TimesheetApprovalDenyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TimesheetApprovalDenyComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TimesheetApprovalDenyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
