using TestTask.RoomBooking.Models;

namespace TestTask.RoomBooking.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (!db.Rooms.Any())
        {
            db.Rooms.AddRange(
                new Room { Name = "Зал А", Capacity = 50, BaseHourlyPrice = 2000 },
                new Room { Name = "Зал B", Capacity = 100, BaseHourlyPrice = 3500 },
                new Room { Name = "Зал C", Capacity = 30, BaseHourlyPrice = 1500 }
            );
        }

        if (!db.Amenities.Any())
        {
            db.Amenities.AddRange(
                new Amenity { Name = "Проєктор", Price = 500 },
                new Amenity { Name = "Wi-Fi", Price = 300 },
                new Amenity { Name = "Звук", Price = 700 }
            );
        }

        db.SaveChanges();
    }
}