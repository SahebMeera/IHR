import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DummayComponent } from './dummay.component';

describe('DummayComponent', () => {
  let component: DummayComponent;
  let fixture: ComponentFixture<DummayComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DummayComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DummayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
