import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DirectDepositComponent } from './direct-deposit.component';

describe('DirectDepositComponent', () => {
  let component: DirectDepositComponent;
  let fixture: ComponentFixture<DirectDepositComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DirectDepositComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DirectDepositComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
