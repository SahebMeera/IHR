import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WizardDataFlowComponent } from './wizard-data-flow.component';

describe('WizardDataFlowComponent', () => {
  let component: WizardDataFlowComponent;
  let fixture: ComponentFixture<WizardDataFlowComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ WizardDataFlowComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(WizardDataFlowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
