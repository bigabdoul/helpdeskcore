import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TicketsHomeComponent } from './home.component';

describe('HomeComponent', () => {
  let component: TicketsHomeComponent;
  let fixture: ComponentFixture<TicketsHomeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TicketsHomeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TicketsHomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
