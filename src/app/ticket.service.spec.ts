import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TicketService } from './ticket.service';

describe('TicketService', () => {
  let service: TicketService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [TicketService]
    });
    service = TestBed.inject(TicketService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify(); // Ensure no outstanding HTTP requests
  });

  it('should retrieve all tickets via GET', () => {
    const mockTickets = [
      { id: 1, description: 'Test ticket 1', status: 'Open', createdDate: new Date() },
      { id: 2, description: 'Test ticket 2', status: 'Closed', createdDate: new Date() }
    ];

    service.getTickets().subscribe((tickets) => {
      expect(tickets.length).toBe(2); // Check if two tickets were returned
      expect(tickets).toEqual(mockTickets); // Ensure the data matches
    });

    const req = httpMock.expectOne('http://localhost:5026/api/tickets');
    expect(req.request.method).toBe('GET'); // Ensure GET request was made
    req.flush(mockTickets); // Simulate response
  });

  it('should create a new ticket via POST', () => {
    const newTicket = { description: 'New test ticket', status: 'Open' };

    service.createTicket(newTicket).subscribe((ticket) => {
      expect(ticket.description).toBe('New test ticket'); // Check if ticket was created
    });

    const req = httpMock.expectOne('http://localhost:5026/api/tickets');
    expect(req.request.method).toBe('POST'); // Ensure POST request was made
    req.flush({ ...newTicket, id: 3, createdDate: new Date() }); // Simulate response
  });

  it('should update an existing ticket via PUT', () => {
    const updatedTicket = { id: 1, description: 'Updated ticket', status: 'Closed' };

    service.updateTicket(1, updatedTicket).subscribe(() => {
      // No return data expected, just check if PUT was called
    });

    const req = httpMock.expectOne('http://localhost:5026/api/tickets/1');
    expect(req.request.method).toBe('PUT'); // Ensure PUT request was made
    expect(req.request.body).toEqual(updatedTicket); // Ensure the body contains the updated ticket
    req.flush({}); // Simulate response
  });

  it('should delete a ticket via DELETE', () => {
    service.deleteTicket(1).subscribe(() => {
      // No return data expected, just check if DELETE was called
    });

    const req = httpMock.expectOne('http://localhost:5026/api/tickets/1');
    expect(req.request.method).toBe('DELETE'); // Ensure DELETE request was made
    req.flush({}); // Simulate response
  });
});
