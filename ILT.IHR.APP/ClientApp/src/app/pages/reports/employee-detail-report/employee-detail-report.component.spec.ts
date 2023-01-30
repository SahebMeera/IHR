import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmployeeDetailReportComponent } from './employee-detail-report.component';

describe('EmployeeDetailReportComponent', () => {
  let component: EmployeeDetailReportComponent;
  let fixture: ComponentFixture<EmployeeDetailReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EmployeeDetailReportComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EmployeeDetailReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
