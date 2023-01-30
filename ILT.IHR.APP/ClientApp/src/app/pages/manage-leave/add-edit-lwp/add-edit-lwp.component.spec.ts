import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddEditLwpComponent } from './add-edit-lwp.component';

describe('AddEditLwpComponent', () => {
  let component: AddEditLwpComponent;
  let fixture: ComponentFixture<AddEditLwpComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddEditLwpComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddEditLwpComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
