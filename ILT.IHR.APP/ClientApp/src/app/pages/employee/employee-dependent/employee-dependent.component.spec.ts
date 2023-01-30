import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmployeeDependentComponent } from './employee-dependent.component';

describe('EmployeeDependentComponent', () => {
  let component: EmployeeDependentComponent;
  let fixture: ComponentFixture<EmployeeDependentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EmployeeDependentComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EmployeeDependentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
