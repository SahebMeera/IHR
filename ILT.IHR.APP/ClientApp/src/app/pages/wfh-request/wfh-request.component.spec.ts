import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WFHRequestComponent } from './wfh-request.component';

describe('WFHRequestComponent', () => {
  let component: WFHRequestComponent;
  let fixture: ComponentFixture<WFHRequestComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ WFHRequestComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(WFHRequestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
