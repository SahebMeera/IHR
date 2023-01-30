import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddEditAssignmentrateComponent } from './add-edit-assignmentrate.component';

describe('AddEditAssignmentrateComponent', () => {
  let component: AddEditAssignmentrateComponent;
  let fixture: ComponentFixture<AddEditAssignmentrateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddEditAssignmentrateComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddEditAssignmentrateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
