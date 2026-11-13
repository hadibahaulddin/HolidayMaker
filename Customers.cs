using MySqlConnector;

record Customer(int Id, string Name);

// Registrera och välja kunder.
static class Customers
{
    public static Customer? Register()
    {
        var firstName = Input.ReadText("Förnamn: ");
        var lastName = Input.ReadText("Efternamn: ");
        var email = Input.ReadText("E-post: ");
        var phone = Input.ReadText("Telefon: ");

        using var connection = Db.Open();
        using var command = new MySqlCommand(
            "INSERT INTO customer (first_name, last_name, email, phone) " +
            "VALUES (@firstName, @lastName, @email, @phone)", connection);
        command.Parameters.AddWithValue("@firstName", firstName);
        command.Parameters.AddWithValue("@lastName", lastName);
        command.Parameters.AddWithValue("@email", email);
        command.Parameters.AddWithValue("@phone", phone);

        try
        {
            command.ExecuteNonQuery();
        }
        catch (MySqlException ex) when (ex.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
        {
            Console.WriteLine("Det finns redan en kund med den e-postadressen.");
            return null;
        }

        var id = (int)command.LastInsertedId;
        Console.WriteLine($"Kunden är registrerad med kundnummer {id}.");
        return new Customer(id, $"{firstName} {lastName}");
    }

    public static Customer? Select()
    {
        using var connection = Db.Open();
        using var command = new MySqlCommand(
            "SELECT id, first_name, last_name, email FROM customer ORDER BY last_name, first_name",
            connection);

        var customers = new List<Customer>();
        using (var reader = command.ExecuteReader())
        {
            Console.WriteLine();
            Console.WriteLine($"{"Nr",-4} {"Namn",-25} E-post");
            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var name = $"{reader.GetString(1)} {reader.GetString(2)}";
                customers.Add(new Customer(id, name));
                Console.WriteLine($"{id,-4} {name,-25} {reader.GetString(3)}");
            }
        }

        if (customers.Count == 0)
        {
            Console.WriteLine("Det finns inga kunder ännu.");
            return null;
        }

        var chosenId = Input.ReadInt("Ange kundnummer: ", 1, int.MaxValue);
        var customer = customers.FirstOrDefault(c => c.Id == chosenId);
        if (customer == null)
        {
            Console.WriteLine("Det finns ingen kund med det numret.");
        }
        return customer;
    }
}