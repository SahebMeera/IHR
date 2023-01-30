import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AssetChangesetComponent } from './asset-changeset.component';

describe('AssetChangesetComponent', () => {
  let component: AssetChangesetComponent;
  let fixture: ComponentFixture<AssetChangesetComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AssetChangesetComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AssetChangesetComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
