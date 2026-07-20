namespace TestTask.RoomBooking.DTOs.Room;

public class RoomResponce
{
    public int RoomId { get; set; }
    public string Name { get; set; } = "";
    public int Capacity { get; set; }
    public decimal BaseHourlyPrice { get; set; }
    public ICollection<int> AmenityIds { get; set; } = [];
}