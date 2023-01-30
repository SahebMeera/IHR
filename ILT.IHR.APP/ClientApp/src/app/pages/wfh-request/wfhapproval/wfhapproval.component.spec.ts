import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WFHApprovalComponent } from './wfhapproval.component';

describe('WFHApprovalComponent', () => {
  let component: WFHApprovalComponent;
  let fixture: ComponentFixture<WFHApprovalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ WFHApprovalComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(WFHApprovalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
