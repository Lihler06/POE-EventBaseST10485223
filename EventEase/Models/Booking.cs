using System.ComponentModel.DataAnnotations.Schema;

namespace EventEase.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        public int VenueId { get; set; }
        public int EventId { get; set; }

        public DateTime BookingDate { get; set; }

        // Navigation properties (make them OPTIONAL)
        [ForeignKey("VenueId")]
        public Venue? Venue { get; set; }

        [ForeignKey("EventId")]
        public Event? Event { get; set; }
    }
}