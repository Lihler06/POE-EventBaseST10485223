using System.ComponentModel.DataAnnotations.Schema;

namespace EventEase.Models
{
    public class Event
    {
          public int EventId { get; set; }

       public string EventName { get; set; }

        public DateTime StartDate { get; set; }

          public DateTime EndDate { get; set; }

        public string Description { get; set; }

              public string ImageUrl { get; set; }

        public int EventTypeId { get; set; }

          [ForeignKey("EventTypeId")]
        public EventType EventType { get; set; }
    }
}