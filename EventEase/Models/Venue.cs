using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class Venue
    {
        public int VenueId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Location { get; set; }

        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }

        // This will store the image URL from Azurite
        public string? ImageUrl { get; set; }
    }
}