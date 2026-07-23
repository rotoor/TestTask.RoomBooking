namespace TestTask.RoomBooking.Services
{
    public interface IPricingService
    {
        decimal CalculateRoomCost(decimal baseHourlyPrice, DateTime start, DateTime end);
    }
}
