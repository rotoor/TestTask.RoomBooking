namespace TestTask.RoomBooking.DTOs.Room;

public class RoomResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Capacity { get; set; }
    public decimal BaseHourlyPrice { get; set; }
    public ICollection<AmenityResponse> Amenities { get; set; } = [];
}