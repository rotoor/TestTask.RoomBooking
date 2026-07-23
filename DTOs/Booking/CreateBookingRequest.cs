using System.ComponentModel.DataAnnotations;

namespace TestTask.RoomBooking.DTOs.Booking
{
    public class CreateBookingRequest
    {
        [Required]
        public int RoomId { get; set; }

        [Required]
        [MaxLength(50)]
        public string CustomerFirstName { get; set; } = "";

        [Required]
        [MaxLength(50)]
        public string CustomerLastName { get; set; } = "";

        [Required]
        [Phone]
        public string CustomerPhone { get; set; } = "";

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public ICollection<int> AmenityIds { get; set; } = [];
    }
}
