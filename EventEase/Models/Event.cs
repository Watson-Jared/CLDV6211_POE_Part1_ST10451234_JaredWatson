using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{

    public class Event
    {
        public int EventID { get; set; }

        [Required]
        public string EventName { get; set; } = "";

        [Required]
        public string EventDescription { get; set; } = "";

        public DateTime EventStartDate { get; set; }

        public DateTime EventEndDate { get; set; }
    }
}