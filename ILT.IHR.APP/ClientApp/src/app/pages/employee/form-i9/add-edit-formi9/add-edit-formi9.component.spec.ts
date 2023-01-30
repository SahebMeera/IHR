import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddEditFormi9Component } from './add-edit-formi9.component';

describe('AddEditFormi9Component', () => {
  let component: AddEditFormi9Component;
  let fixture: ComponentFixture<AddEditFormi9Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddEditFormi9Component ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddEditFormi9Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
