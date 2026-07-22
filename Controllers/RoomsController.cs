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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomResponse>>> GetAll()
        {
            var rooms = await _context.Rooms
                .AsNoTracking()
                .Include(r => r.RoomAmenities)
                .ThenInclude(ra => ra.Amenity)
                .ToListAsync();

            var roomsResponse = rooms.Select(r => new RoomResponse
            {
                Id = r.Id,
                Name = r.Name,
                Capacity = r.Capacity,
                BaseHourlyPrice = r.BaseHourlyPrice,
                Amenities = r.RoomAmenities.Select(ra => new AmenityResponse
                {
                    Id = ra.Amenity.Id,
                    Name = ra.Amenity.Name,
                    Price = ra.Amenity.Price,
                }).ToList()
            }).ToList();

            return Ok(roomsResponse);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomResponse>> GetById(int id)
        {
            var room = await _context.Rooms
                .AsNoTracking()
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
            var (matchedAmenities, invalidIds) = await ValidateAmenityIds(roomRequest.AmenityIds);

            if (invalidIds.Count > 0)
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

        [HttpPut("{id}")]
        public async Task<ActionResult<RoomResponse>> Update(int id, UpdateRoomRequest roomRequest)
        {
            var room = await _context.Rooms
                .Include(r => r.RoomAmenities)
                .ThenInclude(ra => ra.Amenity)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
            {
                return NotFound();
            }

            var (matchedAmenities, invalidIds) = await ValidateAmenityIds(roomRequest.AmenityIds);

            if (invalidIds.Count > 0)
            {
                return BadRequest($"Invalid Amenity IDs: {string.Join(", ", invalidIds)}");
            }

            room.Name = roomRequest.Name;
            room.Capacity = roomRequest.Capacity;
            room.BaseHourlyPrice = roomRequest.BaseHourlyPrice;

            _context.RoomAmenities.RemoveRange(room.RoomAmenities);
            room.RoomAmenities = roomRequest.AmenityIds
                .Select(aid => new RoomAmenity { RoomId = room.Id, AmenityId = aid })
                .ToList();

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

            return Ok(roomResponse);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var room = await _context.Rooms
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
            {
                return NotFound();
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<(List<Amenity> matchedAmenities, List<int> invalidIds)> ValidateAmenityIds(
            ICollection<int> amenityIds)
        {
            var matchedAmenities = await _context.Amenities
                .Where(a => amenityIds.Contains(a.Id))
                .ToListAsync();

            var invalidIds = amenityIds.Except(matchedAmenities.Select(a => a.Id)).ToList();
            return (matchedAmenities, invalidIds);
        }

    }
}

