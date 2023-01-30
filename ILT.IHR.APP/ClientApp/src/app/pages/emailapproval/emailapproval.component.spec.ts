import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EmailApprovalComponent } from './emailapproval.component';


describe('LookupComponent', () => {
  let component: LookupComponent;
    let fixture: ComponentFixture<EmailApprovalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
        declarations: [EmailApprovalComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LookupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
