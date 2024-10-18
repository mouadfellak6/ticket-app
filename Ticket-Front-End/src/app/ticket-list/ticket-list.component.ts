import { Component, OnInit } from '@angular/core';
import { TicketService } from '../ticket.service';

// Define an interface for Ticket. This helps in making sure we know what a ticket should look like.
interface Ticket {
  id?: number;                // Ticket ID (optional because it might not be set when creating a new ticket)
  description: string;       // Description of the ticket
  status?: string;           // Status of the ticket (e.g., 'Open' or 'Closed')
  createdDate?: Date;        // Date when the ticket was created (optional)
}

// The component decorator defines this class as a component and specifies its metadata.
@Component({
  selector: 'app-ticket-list',          // The name used in HTML to reference this component
  templateUrl: './ticket-list.component.html',  // The HTML template for this component
  styleUrls: ['./ticket-list.component.css']    // The styles for this component
})
export class TicketListComponent implements OnInit {

  tickets: Ticket[] = [];                // Array to hold the list of tickets
  newTicket: Ticket = { description: '', status: 'Open' };  // Default values for a new ticket
  showPopupForm: boolean = false;        // Controls the visibility of the form for adding/editing tickets
  isEditMode: boolean = false;           // Indicates whether we are in edit mode or not
  editTicketId: number | undefined;      // Holds the ID of the ticket currently being edited (if any)

  constructor(private ticketService: TicketService) { }  // Inject the TicketService to use its methods

  ngOnInit(): void {
    this.loadTickets();  // Load tickets when the component initializes
  }

  // Method to fetch and load tickets from the server
  loadTickets(): void {
    this.ticketService.getTickets().subscribe((data) => {
      this.tickets = data;  // Store the fetched tickets in the component's tickets array
    });
  }

  // Method to add a new ticket
  addTicket(): void {
    this.ticketService.createTicket(this.newTicket).subscribe(() => {
      this.loadTickets();  // Reload tickets to include the new one
      this.newTicket = { description: '', status: 'Open' };  // Reset the new ticket form
      this.hideForm();  // Hide the form after submission
    });
  }

  // Method to populate the form with the selected ticket's data for editing
  editForm(ticket: Ticket): void {
    this.newTicket = { ...ticket }; // Fill the form with the ticket details
    this.editTicketId = ticket.id;  // Store the ID of the ticket being edited
    this.isEditMode = true;         // Set the mode to edit
    this.showPopupForm = true;      // Show the form for editing
  }

  // Method to update an existing ticket
  editTicket(): void {
    if (this.editTicketId !== undefined) {  // Check if there's a ticket to edit
      // Call the update method from the service and pass the ticket ID and updated ticket data
      this.ticketService.updateTicket(this.editTicketId, this.newTicket).subscribe(() => {
        this.loadTickets();  // Reload the tickets after the update
        this.newTicket = { description: '', status: 'Open' };  // Reset the form
        this.hideForm();             // Hide the form after submission
        this.isEditMode = false;     // Switch back to Add mode
      });
    }
  }

  // Method to delete a ticket by ID
  deleteTicket(id: number): void {
    this.ticketService.deleteTicket(id).subscribe(() => {
      this.loadTickets();  // Reload tickets after deletion to get the latest list
    });
  }

  // Method to show the form for adding a new ticket
  showForm(): void {
    this.isEditMode = false;  // Ensure the form is in Add mode
    this.newTicket = { description: '', status: 'Open' };  // Reset the form values
    this.showPopupForm = true;  // Show the form
  }

  // Method to hide the form
  hideForm(): void {
    this.showPopupForm = false;  // Hide the form
  }
}
