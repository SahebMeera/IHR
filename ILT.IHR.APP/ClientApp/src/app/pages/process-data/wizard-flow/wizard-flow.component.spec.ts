import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WizardFlowComponent } from './wizard-flow.component';

describe('WizardFlowComponent', () => {
  let component: WizardFlowComponent;
  let fixture: ComponentFixture<WizardFlowComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ WizardFlowComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(WizardFlowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
