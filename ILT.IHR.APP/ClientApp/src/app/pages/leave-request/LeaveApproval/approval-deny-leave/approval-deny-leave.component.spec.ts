import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ApprovalDenyLeaveComponent } from './approval-deny-leave.component';

describe('ApprovalDenyLeaveComponent', () => {
  let component: ApprovalDenyLeaveComponent;
  let fixture: ComponentFixture<ApprovalDenyLeaveComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ApprovalDenyLeaveComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ApprovalDenyLeaveComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
