import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LeaveApproveDenyComponent } from './leave-approve-deny.component';

describe('LeaveApproveDenyComponent', () => {
  let component: LeaveApproveDenyComponent;
  let fixture: ComponentFixture<LeaveApproveDenyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LeaveApproveDenyComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LeaveApproveDenyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
