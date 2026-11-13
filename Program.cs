using MySqlConnector;

// Bokningsbar period enligt uppgiften: december 2026.
// Sista utcheckningsdag är 1 januari, så att natten till nyårsdagen kan bokas.
var firstDate = new DateTime(2026, 12, 1);
var lastCheckOut = new DateTime(2027, 1, 1);

Customer? currentCustomer = null;

while (true)
{
    Console.WriteLine();
    Console.WriteLine("===== HOLIDAY MAKER =====");
    Console.WriteLine($"Vald kund: {currentCustomer?.Name ?? "ingen"}");
    Console.WriteLine("1. Registrera ny kund");
    Console.WriteLine("2. Välj befintlig kund");
    Console.WriteLine("3. Sök lediga rum och boka");
    Console.WriteLine("4. Visa kundens bokningar");
    Console.WriteLine("5. Ändra antal personer i en bokning");
    Console.WriteLine("6. Avboka");
    Console.WriteLine("0. Avsluta");

    var choice = Input.ReadInt("Välj: ", 0, 6);
    if (choice == 0)
    {
        break;
    }

    try
    {
        switch (choice)
        {
            case 1:
                currentCustomer = Customers.Register() ?? currentCustomer;
                break;
            case 2:
                currentCustomer = Customers.Select() ?? currentCustomer;
                break;
            case 3:
                SearchAndBook();
                break;
            case 4:
                if (RequireCustomer()) Bookings.ShowForCustomer(currentCustomer!.Id);
                break;
            case 5:
                ChangeGuests();
                break;
            case 6:
                Cancel();
                break;
        }
    }
    catch (MySqlException ex)
    {
        // Databasfel ska inte krascha programmet.
        Console.WriteLine($"Ett databasfel inträffade: {ex.Message}");
    }
}

Console.WriteLine("Hej då!");

bool RequireCustomer()
{
    if (currentCustomer == null)
    {
        Console.WriteLine("Välj eller registrera en kund först (menyval 1 eller 2).");
        return false;
    }
    return true;
}

void SearchAndBook()
{
    var city = Input.ReadText("Ort: ");

    DateTime checkIn;
    DateTime checkOut;
    while (true)
    {
        checkIn = Input.ReadDate("Incheckning (ÅÅÅÅ-MM-DD): ");
        checkOut = Input.ReadDate("Utcheckning (ÅÅÅÅ-MM-DD): ");

        if (checkOut <= checkIn)
        {
            Console.WriteLine("Utcheckningen måste vara efter incheckningen.");
        }
        else if (checkIn < firstDate || checkOut > lastCheckOut)
        {
            Console.WriteLine("Det går bara att boka i december 2026.");
        }
        else
        {
            break;
        }
    }

    var guests = Input.ReadInt("Antal personer: ", 1, 20);

    var requirePool = Input.ReadYesNo("Måste ha pool?");
    var requireRestaurant = Input.ReadYesNo("Måste ha restaurang?");
    var maxBeach = Input.ReadOptionalInt("Max avstånd till strand i meter (tomt = inget krav): ");
    var maxCenter = Input.ReadOptionalInt("Max avstånd till centrum i meter (tomt = inget krav): ");

    Console.WriteLine("Sortering: 1 = pris (lägst först), 2 = omdöme (högst först)");
    var sortByRating = Input.ReadInt("Välj sortering: ", 1, 2) == 2;

    var rooms = Rooms.Search(city, checkIn, checkOut, guests,
        requirePool, requireRestaurant, maxBeach, maxCenter, sortByRating);

    if (rooms.Count == 0)
    {
        Console.WriteLine("Inga lediga rum matchar sökningen.");
        return;
    }

    Console.WriteLine();
    Console.WriteLine($"{"Nr",-4} {"Boende",-22} {"Ort",-10} {"Rum",-6} {"Kapacitet",-10} {"Omdöme",-7} Pris/natt");
    for (var i = 0; i < rooms.Count; i++)
    {
        var room = rooms[i];
        Console.WriteLine(
            $"{i + 1,-4} {room.Accommodation,-22} {room.City,-10} {room.RoomNumber,-6} " +
            $"{room.MaxGuests,-10} {room.Rating,-7} {room.PricePerNight:0} kr");
    }

    if (!Input.ReadYesNo("Vill du boka ett av rummen?"))
    {
        return;
    }
    if (!RequireCustomer())
    {
        return;
    }

    var number = Input.ReadInt("Ange nummer i listan: ", 1, rooms.Count);
    var chosen = rooms[number - 1];
    Bookings.Create(currentCustomer!.Id, chosen.RoomId, checkIn, checkOut, guests);
}

void ChangeGuests()
{
    if (!RequireCustomer()) return;
    if (Bookings.ShowForCustomer(currentCustomer!.Id) == 0) return;

    var bookingId = Input.ReadInt("Bokningsnummer att ändra: ", 1, int.MaxValue);
    var newGuests = Input.ReadInt("Nytt antal personer: ", 1, 20);
    Bookings.ChangeGuests(currentCustomer!.Id, bookingId, newGuests);
}

void Cancel()
{
    if (!RequireCustomer()) return;
    if (Bookings.ShowForCustomer(currentCustomer!.Id) == 0) return;

    var bookingId = Input.ReadInt("Bokningsnummer att avboka: ", 1, int.MaxValue);
    if (Input.ReadYesNo($"Är du säker på att du vill avboka bokning {bookingId}?"))
    {
        Bookings.Cancel(currentCustomer!.Id, bookingId);
    }
}