import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditTicketComponent } from './edit-ticket.component';

describe('EditTicketComponent', () => {
  let component: EditTicketComponent;
  let fixture: ComponentFixture<EditTicketComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditTicketComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditTicketComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
