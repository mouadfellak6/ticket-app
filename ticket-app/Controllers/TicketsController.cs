using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ticket_App.Data;
using Ticket_App.Models;
using Ticket_App.Models.DTOs;

namespace Ticket_App.Controllers
{
    // This attribute sets up routing for the controller.
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        // Database context used to interact with the tickets data.
        private readonly TicketDbContext _context;

        // Constructor to initialize the controller with the TicketDbContext.
        public TicketsController(TicketDbContext context)
        {
            _context = context;
        }

        // GET: api/Tickets
        // This method retrieves a list of all tickets from the database.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetTickets()
        {
            // Using asynchronous method to get tickets as a list.
            return await _context.Tickets.ToListAsync();
        }

        // GET: api/Tickets/5
        // This method retrieves a specific ticket by its ID.
        [HttpGet("{id}")]
        public async Task<ActionResult<Ticket>> GetTicket(int id)
        {
            // Find the ticket with the specified ID.
            var ticket = await _context.Tickets.FindAsync(id);

            // If the ticket does not exist, return a 404 Not Found response.
            if (ticket == null)
            {
                return NotFound();
            }

            // If the ticket is found, return it.
            return ticket;
        }

        // PUT: api/Tickets/5
        // This method updates an existing ticket based on the provided ID and ticket data.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTicket(int id, Ticket ticket)
        {
            // Check if the ID in the route matches the ID of the ticket to be updated.
            if (id != ticket.Id)
            {
                return BadRequest(); // Return a 400 Bad Request if IDs don't match.
            }

            // Fetch the existing ticket from the database using the ID.
            var existingTicket = await _context.Tickets.FindAsync(id);
            if (existingTicket == null)
            {
                return NotFound(); // Return 404 if the ticket is not found.
            }

            // Update the properties of the existing ticket.
            existingTicket.Description = ticket.Description;
            existingTicket.Status = ticket.Status;

            try
            {
                // Save the changes to the database.
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle concurrency issues where the ticket might have been deleted.
                if (!TicketExists(id))
                {
                    return NotFound(); // Return 404 if the ticket does not exist.
                }
                else
                {
                    throw; // Rethrow the exception for any other errors.
                }
            }

            // Return 204 No Content if the update is successful.
            return NoContent();
        }

        // POST: api/Tickets
        // This method creates a new ticket based on the provided ticket data transfer object (DTO).
        // Protect against overposting attacks by not exposing the entire model.
        [HttpPost]
        public async Task<ActionResult<Ticket>> PostTicket(TicketCreateDto ticketDto)
        {
            // Create a new Ticket object with the provided description.
            var ticket = new Ticket
            {
                Description = ticketDto.Description,
                Status = TicketStatus.Open,  // Set default status as Open.
                CreatedDate = DateTime.Now   // Set the current date as created date.
            };

            // Add the new ticket to the database context.
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync(); // Save changes to the database.

            // Return the created ticket with a 201 Created response.
            return CreatedAtAction("GetTicket", new { id = ticket.Id }, ticket);
        }

        // DELETE: api/Tickets/5
        // This method deletes a ticket by its ID.
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            // Find the ticket to be deleted by its ID.
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound(); // Return 404 if the ticket is not found.
            }

            // Remove the ticket from the database context.
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync(); // Save changes to the database.

            // Return 204 No Content if the deletion is successful.
            return NoContent();
        }

        // Helper method to check if a ticket exists by its ID.
        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.Id == id); // Check if any ticket with the given ID exists.
        }
    }
}
