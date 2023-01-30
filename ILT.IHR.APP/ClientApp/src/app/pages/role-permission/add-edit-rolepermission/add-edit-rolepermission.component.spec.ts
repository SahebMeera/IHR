import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddEditRolepermissionComponent } from './add-edit-rolepermission.component';

describe('AddEditRolepermissionComponent', () => {
  let component: AddEditRolepermissionComponent;
  let fixture: ComponentFixture<AddEditRolepermissionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddEditRolepermissionComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddEditRolepermissionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
