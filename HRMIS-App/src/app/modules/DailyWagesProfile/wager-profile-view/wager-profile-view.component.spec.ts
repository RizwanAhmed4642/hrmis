import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WagerProfileViewComponent } from './wager-profile-view.component';

describe('WagerProfileViewComponent', () => {
  let component: WagerProfileViewComponent;
  let fixture: ComponentFixture<WagerProfileViewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WagerProfileViewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WagerProfileViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
