import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddEditWfhComponent } from './add-edit-wfh.component';

describe('AddEditWfhComponent', () => {
  let component: AddEditWfhComponent;
  let fixture: ComponentFixture<AddEditWfhComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddEditWfhComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddEditWfhComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
