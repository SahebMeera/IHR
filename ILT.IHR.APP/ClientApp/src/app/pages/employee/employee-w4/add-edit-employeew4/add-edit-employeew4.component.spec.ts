import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddEditEmployeew4Component } from './add-edit-employeew4.component';

describe('AddEditEmployeew4Component', () => {
  let component: AddEditEmployeew4Component;
  let fixture: ComponentFixture<AddEditEmployeew4Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddEditEmployeew4Component ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddEditEmployeew4Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
