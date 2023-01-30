import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddEditEndClientComponent } from './add-edit-end-client.component';

describe('AddEditEndClientComponent', () => {
  let component: AddEditEndClientComponent;
  let fixture: ComponentFixture<AddEditEndClientComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddEditEndClientComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddEditEndClientComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
