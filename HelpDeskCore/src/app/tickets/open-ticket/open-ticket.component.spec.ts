import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OpenTicketComponent } from './open-ticket.component';

describe('OpenTicketComponent', () => {
  let component: OpenTicketComponent;
  let fixture: ComponentFixture<OpenTicketComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OpenTicketComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OpenTicketComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
