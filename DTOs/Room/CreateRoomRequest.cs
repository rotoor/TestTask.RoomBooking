using TestTask.RoomBooking.Models;

namespace TestTask.RoomBooking.DTOs.Room;

public class CreateRoomRequest
{
    public string Name { get; set; } = "";
    public int Capacity { get; set; }
    public decimal BaseHourlyPrice { get; set; }
    public ICollection<int> AmenityIds { get; set; } = [];
}