using TestTask.RoomBooking.DTOs.Room;

namespace TestTask.RoomBooking.DTOs.Booking
{
    public class BookingResponse
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string RoomName { get; set; } = "";
        public string CustomerFirstName { get; set; } = "";
        public string CustomerLastName { get; set; } = "";
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal TotalCost { get; set; }
        public ICollection<AmenityResponse> Amenities { get; set; } = [];
    }
}