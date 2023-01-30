import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HelpDocumentationComponent } from './help-documentation.component';

describe('HelpDocumentationComponent', () => {
  let component: HelpDocumentationComponent;
  let fixture: ComponentFixture<HelpDocumentationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ HelpDocumentationComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(HelpDocumentationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
