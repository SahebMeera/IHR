import { ComponentFixture, TestBed } from '@angular/core/testing';

import { I9expiryFormComponent } from './i9expiry-form.component';

describe('I9expiryFormComponent', () => {
  let component: I9expiryFormComponent;
  let fixture: ComponentFixture<I9expiryFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ I9expiryFormComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(I9expiryFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
