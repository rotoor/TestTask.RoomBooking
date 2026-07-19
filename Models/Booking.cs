namespace TestTask.RoomBooking.Models;

public class Booking
{
    public int Id { get; set; }

    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public decimal TotalCost { get; set; }

    public ICollection<BookingAmenity> BookingAmenities { get; set; } = [];
}