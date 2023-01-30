import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddEditLookupComponent } from './add-edit-lookup.component';

describe('AddEditLookupComponent', () => {
  let component: AddEditLookupComponent;
  let fixture: ComponentFixture<AddEditLookupComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddEditLookupComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddEditLookupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
