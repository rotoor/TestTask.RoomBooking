using Microsoft.EntityFrameworkCore;
using TestTask.RoomBooking.Models;

namespace TestTask.RoomBooking.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Amenity> Amenities => Set<Amenity>();
    public DbSet<RoomAmenity> RoomAmenities => Set<RoomAmenity>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingAmenity> BookingAmenities => Set<BookingAmenity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RoomAmenity>()
            .HasKey(ra => new { ra.RoomId, ra.AmenityId });

        modelBuilder.Entity<RoomAmenity>()
            .HasOne(ra => ra.Room)
            .WithMany(r => r.RoomAmenities)
            .HasForeignKey(ra => ra.RoomId);

        modelBuilder.Entity<RoomAmenity>()
            .HasOne(ra => ra.Amenity)
            .WithMany(a => a.RoomAmenities)
            .HasForeignKey(ra => ra.AmenityId);
        
        modelBuilder.Entity<BookingAmenity>()
            .HasKey(ba => new { ba.BookingId, ba.AmenityId });

        modelBuilder.Entity<BookingAmenity>()
            .HasOne(ba => ba.Booking)
            .WithMany(b => b.BookingAmenities)
            .HasForeignKey(ba => ba.BookingId);

        modelBuilder.Entity<BookingAmenity>()
            .HasOne(ba => ba.Amenity)
            .WithMany(a => a.BookingAmenities)
            .HasForeignKey(ba => ba.AmenityId);
        
        modelBuilder.Entity<Room>().Property(r => r.BaseHourlyPrice).HasPrecision(18, 2);
        modelBuilder.Entity<Amenity>().Property(a => a.Price).HasPrecision(18, 2);
        modelBuilder.Entity<Booking>().Property(b => b.TotalCost).HasPrecision(18, 2);
        modelBuilder.Entity<BookingAmenity>().Property(ba => ba.PriceAtBookingTime).HasPrecision(18, 2);
    }
}