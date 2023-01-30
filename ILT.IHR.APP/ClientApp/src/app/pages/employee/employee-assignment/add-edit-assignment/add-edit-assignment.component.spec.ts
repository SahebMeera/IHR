import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddEditAssignmentComponent } from './add-edit-assignment.component';

describe('AddEditAssignmentComponent', () => {
  let component: AddEditAssignmentComponent;
  let fixture: ComponentFixture<AddEditAssignmentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddEditAssignmentComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddEditAssignmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
