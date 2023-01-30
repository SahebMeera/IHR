import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddEditDependentComponent } from './add-edit-dependent.component';

describe('AddEditDependentComponent', () => {
  let component: AddEditDependentComponent;
  let fixture: ComponentFixture<AddEditDependentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddEditDependentComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddEditDependentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
