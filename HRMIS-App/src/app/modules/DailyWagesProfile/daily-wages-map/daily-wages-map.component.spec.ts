import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DailyWagesMapComponent } from './daily-wages-map.component';

describe('DailyWagesMapComponent', () => {
  let component: DailyWagesMapComponent;
  let fixture: ComponentFixture<DailyWagesMapComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DailyWagesMapComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DailyWagesMapComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
