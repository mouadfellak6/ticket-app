using Microsoft.EntityFrameworkCore;
using Ticket_App.Models;
namespace Ticket_App.Data
{
    public class TicketDbContext : DbContext
    {
        // Constructor that accepts DbContextOptions and passes them to the base class (DbContext).
        public TicketDbContext(DbContextOptions<TicketDbContext> options) : base(options)
        {
            // Initialization logic (if any) can go here.
        }

        // DbSet represents the collection of Ticket entities in the database.
        public DbSet<Ticket> Tickets { get; set; }
    }
}
