using Microsoft.EntityFrameworkCore;
using TestTask.RoomBooking.Data;
using TestTask.RoomBooking.Models;

namespace TestTask.RoomBooking.Services
{
    public class AvailabilityService : IAvailabilityService
    {
        private readonly AppDbContext _context;

        public AvailabilityService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime start, DateTime end)
        {
            var hasConflictingBooking = await _context.Bookings
                .AnyAsync(b =>
                    b.RoomId == roomId &&
                    b.StartTime < end &&
                    b.EndTime > start);

            return !hasConflictingBooking;
        }

        public async Task<List<Room>> GetAvailableRoomsAsync(
            DateTime start,
            DateTime end,
            int minCapacity)
        {
            return await _context.Rooms
                .Include(r => r.RoomAmenities)
                .ThenInclude(ra => ra.Amenity)
                .Where(r => r.Capacity >= minCapacity)
                .Where(r => !_context.Bookings.Any(b =>
                    b.RoomId == r.Id &&
                    b.StartTime < end &&
                    b.EndTime > start))
                .ToListAsync();
        }
    }
}
