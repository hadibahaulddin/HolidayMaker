using MySqlConnector;

record AvailableRoom(int RoomId, string Accommodation, string City, string RoomNumber,
                     int MaxGuests, decimal PricePerNight, int Rating);

// Sökning efter lediga rum. All filtrering och sortering görs i SQL.
static class Rooms
{
    public static List<AvailableRoom> Search(string city, DateTime checkIn, DateTime checkOut, int guests,
        bool requirePool, bool requireRestaurant, int? maxBeach, int? maxCenter, bool sortByRating)
    {
        // ORDER BY kan inte skickas som parameter, därför väljer vi mellan två fasta alternativ.
        var orderBy = sortByRating
            ? "a.rating DESC, r.price_per_night ASC"
            : "r.price_per_night ASC, a.rating DESC";

        var sql = $"""
            SELECT r.id, a.name, c.name, r.room_number, r.max_guests, r.price_per_night, a.rating
            FROM room r
            JOIN accommodation a ON a.id = r.accommodation_id
            JOIN city c          ON c.id = a.city_id
            WHERE c.name = @city
              AND r.max_guests >= @guests
              AND (@requirePool = FALSE OR a.has_pool = TRUE)
              AND (@requireRestaurant = FALSE OR a.has_restaurant = TRUE)
              AND (@maxBeach IS NULL OR a.distance_to_beach_m <= @maxBeach)
              AND (@maxCenter IS NULL OR a.distance_to_center_m <= @maxCenter)
              AND NOT EXISTS (
                  SELECT 1 FROM booking b
                  WHERE b.room_id = r.id
                    AND b.check_in  < @checkOut
                    AND b.check_out > @checkIn
              )
            ORDER BY {orderBy}
            """;

        using var connection = Db.Open();
        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@city", city);
        command.Parameters.AddWithValue("@guests", guests);
        command.Parameters.AddWithValue("@requirePool", requirePool);
        command.Parameters.AddWithValue("@requireRestaurant", requireRestaurant);
        command.Parameters.AddWithValue("@maxBeach", maxBeach.HasValue ? maxBeach.Value : (object)DBNull.Value);
        command.Parameters.AddWithValue("@maxCenter", maxCenter.HasValue ? maxCenter.Value : (object)DBNull.Value);
        command.Parameters.AddWithValue("@checkIn", checkIn);
        command.Parameters.AddWithValue("@checkOut", checkOut);

        var rooms = new List<AvailableRoom>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rooms.Add(new AvailableRoom(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                Convert.ToInt32(reader.GetValue(4)),
                reader.GetDecimal(5),
                Convert.ToInt32(reader.GetValue(6))));
        }
        return rooms;
    }
}