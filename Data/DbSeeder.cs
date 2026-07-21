using TestTask.RoomBooking.Models;

namespace TestTask.RoomBooking.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.Rooms.Any())
        {
            return;
        }

        var roomA = new Room { Name = "Зал А", Capacity = 50, BaseHourlyPrice = 2000 };
        var roomB = new Room { Name = "Зал B", Capacity = 100, BaseHourlyPrice = 3500 };
        var roomC = new Room { Name = "Зал C", Capacity = 30, BaseHourlyPrice = 1500 };

        db.Rooms.AddRange(roomA, roomB, roomC);

        var projector = new Amenity { Name = "Проєктор", Price = 500 };
        var wifi = new Amenity { Name = "Wi-Fi", Price = 300 };
        var sound = new Amenity { Name = "Звук", Price = 700 };

        db.Amenities.AddRange(projector, wifi, sound);

        db.SaveChanges();

        db.RoomAmenities.AddRange(
            new RoomAmenity { RoomId = roomA.Id, AmenityId = projector.Id },
            new RoomAmenity { RoomId = roomA.Id, AmenityId = wifi.Id },
            new RoomAmenity { RoomId = roomB.Id, AmenityId = wifi.Id },
            new RoomAmenity { RoomId = roomB.Id, AmenityId = sound.Id },
            new RoomAmenity { RoomId = roomC.Id, AmenityId = wifi.Id }
        );

        db.SaveChanges();
    }
}