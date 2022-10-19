import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WagerCatListComponent } from './wager-cat-list.component';

describe('WagerCatListComponent', () => {
  let component: WagerCatListComponent;
  let fixture: ComponentFixture<WagerCatListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WagerCatListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WagerCatListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
