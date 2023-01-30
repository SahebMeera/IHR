import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CalculateLwpComponent } from './calculate-lwp.component';

describe('CalculateLwpComponent', () => {
  let component: CalculateLwpComponent;
  let fixture: ComponentFixture<CalculateLwpComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CalculateLwpComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CalculateLwpComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
