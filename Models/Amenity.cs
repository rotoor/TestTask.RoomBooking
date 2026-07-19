namespace TestTask.RoomBooking.Models;

public class Amenity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }

    public ICollection<RoomAmenity> RoomAmenities { get; set; } = [];
    public ICollection<BookingAmenity> BookingAmenities { get; set; } = [];
}