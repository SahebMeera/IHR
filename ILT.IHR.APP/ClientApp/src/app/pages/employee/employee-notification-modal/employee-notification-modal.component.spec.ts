import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmployeeNotificationModalComponent } from './employee-notification-modal.component';

describe('EmployeeNotificationModalComponent', () => {
  let component: EmployeeNotificationModalComponent;
  let fixture: ComponentFixture<EmployeeNotificationModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EmployeeNotificationModalComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EmployeeNotificationModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
