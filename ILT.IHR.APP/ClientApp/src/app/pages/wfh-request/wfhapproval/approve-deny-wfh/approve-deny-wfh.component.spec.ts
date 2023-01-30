import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ApproveDenyWfhComponent } from './approve-deny-wfh.component';

describe('ApproveDenyWfhComponent', () => {
  let component: ApproveDenyWfhComponent;
  let fixture: ComponentFixture<ApproveDenyWfhComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ApproveDenyWfhComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ApproveDenyWfhComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
