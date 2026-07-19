namespace TestTask.RoomBooking.Models;

public class Room
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Capacity { get; set; }
    public decimal BaseHourlyPrice { get; set; }

    public ICollection<RoomAmenity> RoomAmenities { get; set; } = [];
    public ICollection<Booking> Bookings { get; set; } = [];
}