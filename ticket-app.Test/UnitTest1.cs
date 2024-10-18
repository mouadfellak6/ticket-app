using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ticket_App.Controllers;
using Ticket_App.Data;
using Ticket_App.Models;
using Ticket_App.Models.DTOs;
using Xunit;

namespace ticket_app.Test
{
    public class TicketsControllerTests
    {
        // This helper method creates a new TicketsController with a fresh in-memory database for each test.
        // It ensures tests do not affect each other by using a unique database instance.
        private TicketsController CreateControllerWithInMemoryDb()
        {
            // Setup the in-memory database options.
            var options = new DbContextOptionsBuilder<TicketDbContext>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())  // Create a unique DB name for each test
                            .Options;

            // Create a new instance of TicketDbContext with the in-memory database.
            var context = new TicketDbContext(options);

            // Seed initial data for testing, creating two tickets in the database.
            context.Tickets.AddRange(new List<Ticket>
            {
                new Ticket { Id = 1, Description = "Test ticket 1", Status = TicketStatus.Open },
                new Ticket { Id = 2, Description = "Test ticket 2", Status = TicketStatus.Closed }
            });
            context.SaveChanges(); // Persist the seeded data.

            // Return a new instance of TicketsController with the in-memory context.
            return new TicketsController(context);
        }

        // ---------------------- GET Tests ----------------------

        [Fact]
        public async Task GetTickets_ReturnsAllTickets()
        {
            // Arrange: Create the controller instance with a fresh in-memory database.
            var controller = CreateControllerWithInMemoryDb();

            // Act: Call the GetTickets method to retrieve tickets.
            var result = await controller.GetTickets();

            // Assert: Check that the result is an ActionResult containing a list of tickets.
            var okResult = Assert.IsType<ActionResult<IEnumerable<Ticket>>>(result);
            var tickets = Assert.IsType<List<Ticket>>(okResult.Value);
            Assert.Equal(2, tickets.Count);  // Verify that 2 tickets were added in the setup.
        }

        [Fact]
        public async Task GetTicket_ReturnsTicket_WhenTicketExists()
        {
            // Arrange: Set up the controller.
            var controller = CreateControllerWithInMemoryDb();

            // Act: Attempt to retrieve a specific ticket by its ID.
            var result = await controller.GetTicket(1);

            // Assert: Verify that the returned ticket matches the expected ticket.
            var okResult = Assert.IsType<ActionResult<Ticket>>(result);
            var ticket = Assert.IsType<Ticket>(okResult.Value);
            Assert.Equal(1, ticket.Id);  // Ensure the correct ticket with ID 1 is returned.
        }

        [Fact]
        public async Task GetTicket_ReturnsNotFound_WhenTicketDoesNotExist()
        {
            // Arrange: Create the controller.
            var controller = CreateControllerWithInMemoryDb();

            // Act: Attempt to retrieve a ticket that does not exist.
            var result = await controller.GetTicket(99);  // ID 99 does not exist.

            // Assert: Check that the result indicates a not found response.
            Assert.IsType<NotFoundResult>(result.Result);
        }

        // ---------------------- POST Tests ----------------------

        [Fact]
        public async Task PostTicket_CreatesNewTicket()
        {
            // Arrange: Set up the controller and a new ticket DTO.
            var controller = CreateControllerWithInMemoryDb();
            var newTicketDto = new TicketCreateDto
            {
                Description = "New test ticket"  // New ticket description.
            };

            // Act: Call the PostTicket method to create a new ticket.
            var result = await controller.PostTicket(newTicketDto);

            // Assert: Verify that the created ticket has the expected properties.
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var ticket = Assert.IsType<Ticket>(createdAtActionResult.Value);
            Assert.Equal("New test ticket", ticket.Description);
            Assert.Equal(TicketStatus.Open, ticket.Status);  // Verify default status is Open.
        }

        // ---------------------- PUT Tests ----------------------

        [Fact]
        public async Task PutTicket_UpdatesTicket_WhenTicketExists()
        {
            // Arrange: Create the controller and the updated ticket.
            var controller = CreateControllerWithInMemoryDb();
            var updatedTicket = new Ticket
            {
                Id = 1,  // ID of the existing ticket.
                Description = "Updated ticket description",  // New description.
                Status = TicketStatus.Closed  // Updated status.
            };

            // Act: Call the PutTicket method to update the ticket.
            var result = await controller.PutTicket(1, updatedTicket);

            // Assert: Verify that the update was successful (No Content response).
            Assert.IsType<NoContentResult>(result);

            // Check the updated ticket properties.
            var getResult = await controller.GetTicket(1);
            var ticket = Assert.IsType<Ticket>(getResult.Value);
            Assert.Equal("Updated ticket description", ticket.Description);
            Assert.Equal(TicketStatus.Closed, ticket.Status);  // Check updated status.
        }

        [Fact]
        public async Task PutTicket_ReturnsNotFound_WhenTicketDoesNotExist()
        {
            // Arrange: Set up the controller and an updated ticket with a non-existent ID.
            var controller = CreateControllerWithInMemoryDb();
            var updatedTicket = new Ticket
            {
                Id = 99,  // Non-existent ticket ID.
                Description = "Non-existent ticket",
                Status = TicketStatus.Closed
            };

            // Act: Call the PutTicket method with the non-existent ID.
            var result = await controller.PutTicket(99, updatedTicket);

            // Assert: Verify that the response indicates the ticket was not found.
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task PutTicket_ReturnsBadRequest_WhenIdDoesNotMatch()
        {
            // Arrange: Create the controller and set up an updated ticket with a mismatched ID.
            var controller = CreateControllerWithInMemoryDb();
            var updatedTicket = new Ticket
            {
                Id = 2,  // ID does not match the ID in the route.
                Description = "Test ticket",
                Status = TicketStatus.Open
            };

            // Act: Attempt to update the ticket with a mismatched ID.
            var result = await controller.PutTicket(1, updatedTicket);  // ID mismatch (1 != 2).

            // Assert: Verify that the response indicates a bad request due to ID mismatch.
            Assert.IsType<BadRequestResult>(result);
        }

        // ---------------------- DELETE Tests ----------------------

        [Fact]
        public async Task DeleteTicket_RemovesTicket_WhenTicketExists()
        {
            // Arrange: Create the controller.
            var controller = CreateControllerWithInMemoryDb();

            // Act: Call the DeleteTicket method to remove the ticket.
            var result = await controller.DeleteTicket(1);

            // Assert: Check for a No Content response indicating successful deletion.
            Assert.IsType<NoContentResult>(result);

            // Verify that the ticket has been removed by trying to retrieve it.
            var getResult = await controller.GetTicket(1);
            Assert.IsType<NotFoundResult>(getResult.Result);  // Should return not found.
        }

        [Fact]
        public async Task DeleteTicket_ReturnsNotFound_WhenTicketDoesNotExist()
        {
            // Arrange: Set up the controller.
            var controller = CreateControllerWithInMemoryDb();

            // Act: Attempt to delete a ticket that does not exist.
            var result = await controller.DeleteTicket(99);  // ID 99 does not exist.

            // Assert: Verify that the response indicates the ticket was not found.
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
