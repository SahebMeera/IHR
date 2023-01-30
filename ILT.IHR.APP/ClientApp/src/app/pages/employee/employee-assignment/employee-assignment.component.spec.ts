import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmployeeAssignmentComponent } from './employee-assignment.component';

describe('EmployeeAssignmentComponent', () => {
  let component: EmployeeAssignmentComponent;
  let fixture: ComponentFixture<EmployeeAssignmentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EmployeeAssignmentComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EmployeeAssignmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
