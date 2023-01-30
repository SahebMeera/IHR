import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddEditDirectDepositComponent } from './add-edit-direct-deposit.component';

describe('AddEditDirectDepositComponent', () => {
  let component: AddEditDirectDepositComponent;
  let fixture: ComponentFixture<AddEditDirectDepositComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddEditDirectDepositComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddEditDirectDepositComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
