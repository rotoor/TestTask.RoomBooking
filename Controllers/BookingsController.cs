using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestTask.RoomBooking.Data;
using TestTask.RoomBooking.DTOs.Booking;
using TestTask.RoomBooking.DTOs.Room;
using TestTask.RoomBooking.Models;
using TestTask.RoomBooking.Services;

namespace TestTask.RoomBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPricingService _pricingService;
        private readonly IAvailabilityService _availabilityService;

        public BookingsController(AppDbContext context, IPricingService pricingService, IAvailabilityService availabilityService)
        {
            _context = context;
            _pricingService = pricingService;
            _availabilityService = availabilityService;
        }


        [HttpPost]
        public async Task<ActionResult<BookingResponse>> CreateBooking(CreateBookingRequest bookingRequest)
        {
            var room = await _context.Rooms.FindAsync(bookingRequest.RoomId);

            if (room == null)
            {
                return NotFound("Room not found.");
            }

            var (matchedAmenities, invalidIds) = await ValidateAmenityIds(bookingRequest.AmenityIds);

            if (invalidIds.Count > 0)
            {
                return BadRequest($"Invalid Amenity IDs: {string.Join(", ", invalidIds)}");
            }

            var isAvailable = await _availabilityService.IsRoomAvailableAsync(
                bookingRequest.RoomId,
                bookingRequest.StartTime,
                bookingRequest.EndTime);

            if (!isAvailable)
            {
                return BadRequest("The room is not available for the selected time range.");
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.PhoneNumber == bookingRequest.CustomerPhone);

            if (customer == null)
            {
                customer = new Customer
                {
                    FirstName = bookingRequest.CustomerFirstName,
                    LastName = bookingRequest.CustomerLastName,
                    PhoneNumber = bookingRequest.CustomerPhone
                };
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
            }

            var roomCost = _pricingService.CalculateRoomCost(room.BaseHourlyPrice, bookingRequest.StartTime, bookingRequest.EndTime);
            var amenityCost = matchedAmenities.Sum(a => a.Price);

            var booking = new Booking
            {
                RoomId = bookingRequest.RoomId,
                CustomerId = customer.Id,
                StartTime = bookingRequest.StartTime,
                EndTime = bookingRequest.EndTime,
                TotalCost = roomCost + amenityCost,
                BookingAmenities = matchedAmenities.Select(a => new BookingAmenity { AmenityId = a.Id, PriceAtBookingTime = a.Price }).ToList()
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();


            var bookingResponse = new BookingResponse
            {
                Id = booking.Id,
                RoomId = booking.RoomId,
                RoomName = room.Name,
                CustomerFirstName = customer.FirstName,
                CustomerLastName = customer.LastName,
                StartTime = booking.StartTime,
                EndTime= booking.EndTime,
                TotalCost = booking.TotalCost,
                Amenities = matchedAmenities
                    .Select(a => new AmenityResponse
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Price = a.Price 
                    }).ToList()
            };

            return Created($"/api/bookings/{booking.Id}", bookingResponse);
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
