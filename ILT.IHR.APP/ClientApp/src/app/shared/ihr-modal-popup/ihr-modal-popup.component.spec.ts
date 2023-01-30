import { ComponentFixture, TestBed } from '@angular/core/testing';
import { IHRModalPopupComponent } from './ihr-modal-popup.component';


describe('IHRModalPopupComponent', () => {
  let component: IHRModalPopupComponent;
  let fixture: ComponentFixture<IHRModalPopupComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ IHRModalPopupComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(IHRModalPopupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
