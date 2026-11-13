using MySqlConnector;

// Skapa, visa, ändra och ta bort bokningar.
static class Bookings
{
    // Returnerar bokningsnumret, eller null om bokningen inte kunde göras.
    public static int? Create(int customerId, int roomId, DateTime checkIn, DateTime checkOut, int guests)
    {
        using var connection = Db.Open();
        using var transaction = connection.BeginTransaction();

        // 1. Hämta rummet och lås raden (FOR UPDATE) så att två bokningar av samma rum
        //    inte kan sparas samtidigt.
        using var roomCommand = new MySqlCommand(
            "SELECT max_guests, price_per_night FROM room WHERE id = @roomId FOR UPDATE",
            connection, transaction);
        roomCommand.Parameters.AddWithValue("@roomId", roomId);

        int maxGuests;
        decimal pricePerNight;
        using (var reader = roomCommand.ExecuteReader())
        {
            if (!reader.Read())
            {
                Console.WriteLine("Rummet finns inte.");
                return null;
            }
            maxGuests = Convert.ToInt32(reader.GetValue(0));
            pricePerNight = reader.GetDecimal(1);
        }

        // 2. Kontrollera kapaciteten.
        if (guests > maxGuests)
        {
            Console.WriteLine($"Rummet rymmer bara {maxGuests} personer.");
            return null;
        }

        // 3. Kontrollera tillgängligheten igen precis innan vi sparar.
        using var overlapCommand = new MySqlCommand(
            "SELECT COUNT(*) FROM booking " +
            "WHERE room_id = @roomId AND check_in < @checkOut AND check_out > @checkIn",
            connection, transaction);
        overlapCommand.Parameters.AddWithValue("@roomId", roomId);
        overlapCommand.Parameters.AddWithValue("@checkIn", checkIn);
        overlapCommand.Parameters.AddWithValue("@checkOut", checkOut);

        if (Convert.ToInt32(overlapCommand.ExecuteScalar()) > 0)
        {
            Console.WriteLine("Rummet är redan bokat under hela eller delar av perioden.");
            return null;
        }

        // 4. Spara bokningen.
        using var insertCommand = new MySqlCommand(
            "INSERT INTO booking (customer_id, room_id, check_in, check_out, guests) " +
            "VALUES (@customerId, @roomId, @checkIn, @checkOut, @guests)",
            connection, transaction);
        insertCommand.Parameters.AddWithValue("@customerId", customerId);
        insertCommand.Parameters.AddWithValue("@roomId", roomId);
        insertCommand.Parameters.AddWithValue("@checkIn", checkIn);
        insertCommand.Parameters.AddWithValue("@checkOut", checkOut);
        insertCommand.Parameters.AddWithValue("@guests", guests);
        insertCommand.ExecuteNonQuery();

        var bookingId = (int)insertCommand.LastInsertedId;
        transaction.Commit();

        var nights = (checkOut - checkIn).Days;
        Console.WriteLine();
        Console.WriteLine("Bokningen är klar!");
        Console.WriteLine($"Bokningsnummer: {bookingId}");
        Console.WriteLine($"{nights} nätter × {pricePerNight:0} kr = {nights * pricePerNight:0} kr");
        return bookingId;
    }

    // Visar kundens bokningar och returnerar hur många det finns.
    public static int ShowForCustomer(int customerId)
    {
        using var connection = Db.Open();
        using var command = new MySqlCommand("""
            SELECT b.id, a.name, r.room_number, b.check_in, b.check_out, b.guests,
                   DATEDIFF(b.check_out, b.check_in) * r.price_per_night AS total_price
            FROM booking b
            JOIN room r          ON r.id = b.room_id
            JOIN accommodation a ON a.id = r.accommodation_id
            WHERE b.customer_id = @customerId
            ORDER BY b.check_in
            """, connection);
        command.Parameters.AddWithValue("@customerId", customerId);

        using var reader = command.ExecuteReader();
        var count = 0;
        Console.WriteLine();
        Console.WriteLine($"{"Nr",-5} {"Boende",-22} {"Rum",-6} {"Incheckning",-12} {"Utcheckning",-12} {"Pers",-5} Totalpris");
        while (reader.Read())
        {
            count++;
            Console.WriteLine(
                $"{reader.GetInt32(0),-5} {reader.GetString(1),-22} {reader.GetString(2),-6} " +
                $"{reader.GetDateTime(3):yyyy-MM-dd}   {reader.GetDateTime(4):yyyy-MM-dd}   " +
                $"{Convert.ToInt32(reader.GetValue(5)),-5} {reader.GetDecimal(6):0} kr");
        }

        if (count == 0)
        {
            Console.WriteLine("Kunden har inga bokningar.");
        }
        return count;
    }

    public static void ChangeGuests(int customerId, int bookingId, int newGuests)
    {
        using var connection = Db.Open();

        // Hämta rummets kapacitet. customer_id i villkoret gör att man bara kan ändra
        // den valda kundens egna bokningar.
        using var checkCommand = new MySqlCommand(
            "SELECT r.max_guests FROM booking b JOIN room r ON r.id = b.room_id " +
            "WHERE b.id = @bookingId AND b.customer_id = @customerId", connection);
        checkCommand.Parameters.AddWithValue("@bookingId", bookingId);
        checkCommand.Parameters.AddWithValue("@customerId", customerId);

        var result = checkCommand.ExecuteScalar();
        if (result == null)
        {
            Console.WriteLine("Kunden har ingen bokning med det numret.");
            return;
        }

        var maxGuests = Convert.ToInt32(result);
        if (newGuests > maxGuests)
        {
            Console.WriteLine($"Rummet rymmer bara {maxGuests} personer. Ingen ändring gjordes.");
            return;
        }

        using var updateCommand = new MySqlCommand(
            "UPDATE booking SET guests = @guests WHERE id = @bookingId AND customer_id = @customerId",
            connection);
        updateCommand.Parameters.AddWithValue("@guests", newGuests);
        updateCommand.Parameters.AddWithValue("@bookingId", bookingId);
        updateCommand.Parameters.AddWithValue("@customerId", customerId);
        updateCommand.ExecuteNonQuery();

        Console.WriteLine($"Antal personer är ändrat till {newGuests}.");
    }

    public static void Cancel(int customerId, int bookingId)
    {
        using var connection = Db.Open();
        using var command = new MySqlCommand(
            "DELETE FROM booking WHERE id = @bookingId AND customer_id = @customerId", connection);
        command.Parameters.AddWithValue("@bookingId", bookingId);
        command.Parameters.AddWithValue("@customerId", customerId);

        var rows = command.ExecuteNonQuery();
        Console.WriteLine(rows == 1
            ? "Bokningen är avbokad. Rummet kan nu bokas igen."
            : "Kunden har ingen bokning med det numret.");
    }
}