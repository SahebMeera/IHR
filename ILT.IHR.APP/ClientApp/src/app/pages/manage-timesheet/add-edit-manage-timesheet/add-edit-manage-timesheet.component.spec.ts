import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddEditManageTimesheetComponent } from './add-edit-manage-timesheet.component';

describe('AddEditManageTimesheetComponent', () => {
  let component: AddEditManageTimesheetComponent;
  let fixture: ComponentFixture<AddEditManageTimesheetComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddEditManageTimesheetComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddEditManageTimesheetComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
