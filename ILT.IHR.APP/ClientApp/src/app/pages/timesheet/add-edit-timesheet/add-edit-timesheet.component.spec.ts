import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddEditTimesheetComponent } from './add-edit-timesheet.component';

describe('AddEditTimesheetComponent', () => {
  let component: AddEditTimesheetComponent;
  let fixture: ComponentFixture<AddEditTimesheetComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddEditTimesheetComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddEditTimesheetComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
