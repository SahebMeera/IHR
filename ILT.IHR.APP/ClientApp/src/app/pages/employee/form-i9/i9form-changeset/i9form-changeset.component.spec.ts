import { ComponentFixture, TestBed } from '@angular/core/testing';

import { I9formChangesetComponent } from './i9form-changeset.component';

describe('I9formChangesetComponent', () => {
  let component: I9formChangesetComponent;
  let fixture: ComponentFixture<I9formChangesetComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ I9formChangesetComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(I9formChangesetComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
