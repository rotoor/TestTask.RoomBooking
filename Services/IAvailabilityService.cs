using TestTask.RoomBooking.Models;

namespace TestTask.RoomBooking.Services
{
    public interface IAvailabilityService
    {
        Task<bool> IsRoomAvailableAsync(int roomId, DateTime start, DateTime end);
        Task<List<Room>> GetAvailableRoomsAsync(DateTime start, DateTime end, int minCapacity);
    }
}
