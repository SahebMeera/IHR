import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmployeeW4Component } from './employee-w4.component';

describe('EmployeeW4Component', () => {
  let component: EmployeeW4Component;
  let fixture: ComponentFixture<EmployeeW4Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EmployeeW4Component ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EmployeeW4Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
