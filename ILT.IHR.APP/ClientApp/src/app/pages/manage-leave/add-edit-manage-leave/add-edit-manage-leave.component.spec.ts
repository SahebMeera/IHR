import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddEditManageLeaveComponent } from './add-edit-manage-leave.component';

describe('AddEditManageLeaveComponent', () => {
  let component: AddEditManageLeaveComponent;
  let fixture: ComponentFixture<AddEditManageLeaveComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddEditManageLeaveComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddEditManageLeaveComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
