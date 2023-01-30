import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ApprovalDenyTimesheetComponent } from './approval-deny-timesheet.component';

describe('ApprovalDenyTimesheetComponent', () => {
  let component: ApprovalDenyTimesheetComponent;
  let fixture: ComponentFixture<ApprovalDenyTimesheetComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ApprovalDenyTimesheetComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ApprovalDenyTimesheetComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
