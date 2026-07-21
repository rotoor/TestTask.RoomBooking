using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestTask.RoomBooking.Data;
using TestTask.RoomBooking.DTOs.Room;
using TestTask.RoomBooking.Models;


namespace TestTask.RoomBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RoomsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomResponse>> GetById(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.RoomAmenities)
                .ThenInclude(ra => ra.Amenity)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
            {
                return NotFound();
            }

            var roomResponse = new RoomResponse
            {
                Id = room.Id,
                Name = room.Name,
                Capacity = room.Capacity,
                BaseHourlyPrice = room.BaseHourlyPrice,
                Amenities = room.RoomAmenities.Select(ra => new AmenityResponse
                {
                    Id = ra.Amenity.Id,
                    Name = ra.Amenity.Name,
                    Price = ra.Amenity.Price,
                }).ToList()
            };

            return roomResponse;
        }

        [HttpPost]
        public async Task<ActionResult<RoomResponse>> Create(CreateRoomRequest roomRequest)
        {
            var matchedAmenities = await _context.Amenities
                    .Where(a => roomRequest.AmenityIds.Contains(a.Id))
                    .ToListAsync();

            var invalidIds = roomRequest.AmenityIds.Except(matchedAmenities.Select(a => a.Id));
            
            if (invalidIds.Any())
            {
                return BadRequest($"Invalid Amenity IDs: {string.Join(", ", invalidIds)}");
            }

            var room = new Room
            {
                Name = roomRequest.Name,
                Capacity = roomRequest.Capacity,
                BaseHourlyPrice = roomRequest.BaseHourlyPrice,
                RoomAmenities = roomRequest.AmenityIds.Select(aid => new RoomAmenity { AmenityId = aid }).ToList()
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            var roomResponse = new RoomResponse
            {
                Id = room.Id,
                Name = room.Name,
                Capacity = room.Capacity,
                BaseHourlyPrice = room.BaseHourlyPrice,
                Amenities = matchedAmenities.Select(a => new AmenityResponse
                {
                    Id = a.Id,
                    Name = a.Name,
                    Price = a.Price
                }).ToList()
            };

            return CreatedAtAction(nameof(GetById), new { id = room.Id }, roomResponse);
        }
    }
}
