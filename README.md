# Conference Room Booking API

A REST API for managing conference rooms, searching availability, and booking with automatic rental cost calculation. Built as a test assignment solution.

## Tech stack

- **.NET 8 / ASP.NET Core Web API** — controller-based style (not Minimal API)
- **Entity Framework Core 8** (SQL Server / LocalDB)
- **Swagger / Swashbuckle** — auto-generated API documentation

## Architecture

The project follows a layered structure:

```
Controllers/    — HTTP endpoints, request validation, response shaping
Services/       — business logic (pricing, availability checks)
DTOs/           — request/response contracts, decoupled from DB models
Models/         — EF Core entities
Data/           — DbContext, initial data seeding
Migrations/     — EF Core migrations
```

DTOs are intentionally different from the EF Core entities — only the data the client actually needs is exposed, avoiding circular references and internal EF Core details.

### Data model

- **Room** — a conference room (name, capacity, base hourly price)
- **Amenity** — a service (Projector, Wi-Fi, Sound)
- **RoomAmenity** — which amenities are available in a given room (many-to-many)
- **Customer** — a client, identified by phone number, no registration/login required
- **Booking** — a booking record
- **BookingAmenity** — which amenities were selected for a specific booking, storing a **price snapshot** at booking time (so a later price change for an amenity does not affect already placed bookings)

## Getting started

1. Make sure SQL Server or LocalDB is installed (check with `sqllocaldb info`)
2. If needed, update the connection string in `appsettings.Development.json`:
   ```
   Server=(localdb)\mssqllocaldb;Database=RoomBookingDb;Trusted_Connection=True;TrustServerCertificate=True;
   ```
3. Run the project (`dotnet run` or via your IDE) — migrations are applied and seed data (3 rooms, 3 amenities) is loaded **automatically** on startup, no need to run `dotnet ef database update` manually
4. Open `https://localhost:{port}/swagger` to explore and test the API

## API

### Rooms

| Method | Route | Description |
|---|---|---|
| GET | `/api/rooms` | List all rooms |
| GET | `/api/rooms/{id}` | Get a room by ID |
| POST | `/api/rooms` | Create a new room |
| PUT | `/api/rooms/{id}` | Update a room (full replace) |
| DELETE | `/api/rooms/{id}` | Delete a room |
| GET | `/api/rooms/available?startTime=&endTime=&minCapacity=` | Search rooms available in a given time range and matching a minimum capacity |

### Bookings

| Method | Route | Description |
|---|---|---|
| POST | `/api/bookings` | Create a booking with automatic total cost calculation |

When creating a booking, the client provides a first name, last name and phone number. An existing `Customer` is looked up by phone number; if none is found, a new one is created (no registration/password involved).

## Cost calculation

Per the assignment, the rental price depends on the hour of the booking:

- 09:00–18:00 — base rate
- 18:00–23:00 — 20% discount
- 06:00–09:00 — 10% discount
- 12:00–14:00 — 15% surcharge

The booking interval is split into hourly segments, each priced according to the matching rate zone; when a segment crosses a zone boundary, the **exact fraction of an hour** is used rather than rounding to full hours (e.g. a `09:30–14:15` booking correctly prices 30 minutes and 15 minutes as partial hours instead of rounding up to whole hours). The cost of selected amenities is added on top of the room rental cost.

## Security and robustness

- Input validation via Data Annotations (`[Required]`, `[Range]`, `[MaxLength]`) — invalid requests (negative price/capacity, empty name) are rejected at the model level, before reaching business logic
- Amenity ID existence is validated before creating/updating a room and before booking — prevents data integrity issues
- Booking interval overlap is checked (`AvailabilityService`) to prevent double-booking a room
- Explicit HTTP status codes (`400`, `404`, `201`, `204`) with descriptive error messages

**Authorization is not implemented.** Room management endpoints (create/update/delete) would, in a real system, be restricted to company staff and should be protected (e.g. via an API key or JWT with an Admin role). This was intentionally left out given the time constraints of the assignment, but is a known, planned direction for further work.

## Possible future improvements (scalability)

- Authorization for admin endpoints (JWT/API key)
- Reports and analytics (room utilization by period, revenue, top customers) — the data model (price snapshot, a separate Customer entity) was designed so such reports could be added without schema changes
- More flexible room updates (PATCH instead of a full PUT replace)
- Extracting the duplicated amenity-ID validation logic (currently present in two controllers) into a shared service

## Seed data

On first run, the following data is created automatically:

- Room A — 50 people, 2000 UAH/hour
- Room B — 100 people, 3500 UAH/hour
- Room C — 30 people, 1500 UAH/hour
- Amenities: Projector (500 UAH), Wi-Fi (300 UAH), Sound (700 UAH), linked to rooms
