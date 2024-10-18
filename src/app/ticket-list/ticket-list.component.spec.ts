import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TicketListComponent } from './ticket-list.component';
import { TicketService } from '../ticket.service';
import { of } from 'rxjs';

describe('TicketListComponent', () => {
  let component: TicketListComponent;
  let fixture: ComponentFixture<TicketListComponent>;
  let ticketServiceMock: any;

  beforeEach(() => {
    ticketServiceMock = jasmine.createSpyObj('TicketService', ['getTickets', 'createTicket', 'updateTicket', 'deleteTicket']);

    TestBed.configureTestingModule({
      declarations: [TicketListComponent],
      providers: [
        { provide: TicketService, useValue: ticketServiceMock }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TicketListComponent);
    component = fixture.componentInstance;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should load tickets on init', () => {
    const mockTickets = [
      { id: 1, description: 'Test Ticket 1', status: 'Open', createdDate: new Date() },
      { id: 2, description: 'Test Ticket 2', status: 'Closed', createdDate: new Date() }
    ];
    
    ticketServiceMock.getTickets.and.returnValue(of(mockTickets));
    component.ngOnInit();
    expect(component.tickets.length).toBe(2);
  });



it('should edit an existing ticket', () => {
  const ticketToEdit = { id: 1, description: 'Edited Ticket', status: 'Open' };
  ticketServiceMock.updateTicket.and.returnValue(of({}));
  ticketServiceMock.getTickets.and.returnValue(of([ticketToEdit]));

  // Set up the edit with just the description and status
  component.editTicketId = 1;
  component.newTicket = { description: 'Edited Ticket', status: 'Open' }; // Do not include id
  component.editTicket();

  // Check the service call
  expect(ticketServiceMock.updateTicket).toHaveBeenCalledWith(1, { description: 'Edited Ticket', status: 'Open' });
  expect(ticketServiceMock.getTickets).toHaveBeenCalled();
});
  it('should delete a ticket', () => {
    ticketServiceMock.deleteTicket.and.returnValue(of({}));
    ticketServiceMock.getTickets.and.returnValue(of([])); // Return an empty array after deletion

    component.deleteTicket(1);

    expect(ticketServiceMock.deleteTicket).toHaveBeenCalledWith(1);
    expect(ticketServiceMock.getTickets).toHaveBeenCalled();
  });

  it('should show form for adding a ticket', () => {
    component.showForm();
    expect(component.showPopupForm).toBeTrue();
    expect(component.isEditMode).toBeFalse();
  });

  it('should hide form', () => {
    component.hideForm();
    expect(component.showPopupForm).toBeFalse();
  });

  it('should prepare form for editing a ticket', () => {
    const ticketToEdit = { id: 1, description: 'Existing Ticket', status: 'Open' };
    component.editForm(ticketToEdit);
    expect(component.newTicket).toEqual(ticketToEdit);
    expect(component.isEditMode).toBeTrue();
    expect(component.showPopupForm).toBeTrue();
  });
});
