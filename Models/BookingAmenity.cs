namespace TestTask.RoomBooking.Models;

public class BookingAmenity
{
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;

    public int AmenityId { get; set; }
    public Amenity Amenity { get; set; } = null!;

    public decimal PriceAtBookingTime { get; set; }
}