import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IhrTableComponent } from './ihr-table.component';

describe('IhrTableComponent', () => {
  let component: IhrTableComponent;
  let fixture: ComponentFixture<IhrTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ IhrTableComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(IhrTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
