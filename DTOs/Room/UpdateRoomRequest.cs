using System.ComponentModel.DataAnnotations;

namespace TestTask.RoomBooking.DTOs.Room
{
    public class UpdateRoomRequest
    {
        [Required(ErrorMessage = "Room name is required")]
        [MaxLength(100)]
        public string Name { get; set; } = "";

        [Range(1, 1000, ErrorMessage = "{0} must be between {1} and {2}")]
        public int Capacity { get; set; }

        [Range(0.01, 100000, ErrorMessage = "{0} must be between {1} and {2}")]
        public decimal BaseHourlyPrice { get; set; }

        public ICollection<int> AmenityIds { get; set; } = [];
    }
}
