import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmployeeEmergencyComponent } from './employee-emergency.component';

describe('EmployeeEmergencyComponent', () => {
  let component: EmployeeEmergencyComponent;
  let fixture: ComponentFixture<EmployeeEmergencyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EmployeeEmergencyComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EmployeeEmergencyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
