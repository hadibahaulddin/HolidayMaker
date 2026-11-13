using MySqlConnector;

// Ansvarar för anslutningen till databasen.
static class Db
{
    private const string ConnectionString =
        "Server=localhost;Port=3306;Database=holiday_maker;User ID=root;Password=kurslosenord;";

    // Öppnar en ny anslutning. Anroparen stänger den med "using".
    public static MySqlConnection Open()
    {
        var connection = new MySqlConnection(ConnectionString);
        connection.Open();
        return connection;
    }
}