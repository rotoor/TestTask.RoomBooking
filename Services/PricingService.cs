namespace TestTask.RoomBooking.Services
{
    public class PricingService : IPricingService
    {
        // Розраховує загальну вартість бронювання залу з урахуванням тарифних зон
        public decimal CalculateRoomCost(decimal baseHourlyPrice, DateTime start, DateTime end)
        {
            decimal total = 0; // тут накопичуємо підсумкову вартість
            var current = start; // поточний момент часу, з якого починаємо обробку

            
            while (current < end) // Обробляємо бронювання по частинах, поки не дійдемо до кінця
            {
                // Межа наступної повної години від поточного моменту
                // Наприклад: якщо current = 09:30, то nextHourBoundary = 10:00
                var nextHourBoundary = current.Date.AddHours(current.Hour + 1);

                // Кінець поточного сегменту або межа години, або кінець бронювання,
                // залежно від того, що настає раніше
                var segmentEnd = nextHourBoundary < end ? nextHourBoundary : end;

                // Різниця між кінцем сегменту та поточним моментом - це TimeSpan (проміжок часу)
                // .TotalHours перетворює цей проміжок у дробове число годин (тип double)
                // (decimal) - явне приведення типу, бо double не можна множити на decimal напряму
                var hours = (decimal)(segmentEnd - current).TotalHours;

                // Година доби поточного сегменту (0-23) - за нею визначаємо тарифну зону
                var hour = current.Hour;

                decimal rate = baseHourlyPrice; // базова ставка за годину, буде скоригована нижче

                // Перевірка тарифної зони: пікові, вечірні, ранкові години
                // Якщо жодна умова не спрацювала - залишається стандартний тариф
                if (hour >= 12 && hour < 14)
                    rate *= 1.15m; // пікові години (12:00-14:00) - націнка 15%
                else if (hour >= 18 && hour < 23)
                    rate *= 0.80m; // вечірні години (18:00-23:00) - знижка 20%
                else if (hour >= 6 && hour < 9)
                    rate *= 0.90m; // ранкові години (06:00-09:00) - знижка 10%

                
                total += rate * hours; // Додаємо вартість цього сегменту (ставка помножена на частку години)

                
                current = segmentEnd; // Переходимо до наступного сегменту
            }

            return total; // підсумкова вартість бронювання
        }
    }
}