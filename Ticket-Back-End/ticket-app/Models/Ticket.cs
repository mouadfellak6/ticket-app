using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Ticket_App.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Description { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TicketStatus Status { get; set; } 
        public DateTime CreatedDate { get; set; }
    }
}
