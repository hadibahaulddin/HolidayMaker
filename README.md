# Holiday Maker

Det här är mitt program i C#. Man använder det på en resebyrå för att boka hotellrum åt kunder som ringer. Allt sparas i en MySQL-databas.

## Filerna

database.sql – gör databasen och lägger in testdata.

er-diagram.png – en bild på hur tabellerna hänger ihop.

Program.cs – menyn.

Db.cs – kopplar programmet till databasen.

Inputs.cs – läser det man skriver, så att programmet inte kraschar om man skriver fel.

Customers.cs – lägga till och välja kund.

Rooms.cs – söka lediga rum.

Bookings.cs – boka, visa, ändra och avboka.

## Det här behövs

.NET 8 eller nyare och MySQL 8. Jag kör MySQL i Docker. Paketet MySqlConnector laddas ner av sig självt när man kör programmet.

## Så gör du databasen

Starta MySQL. Öppna database.sql i DataGrip och kör hela filen. Finns databasen redan tas den bort och görs om från början.

## Så startar du programmet

Öppna Db.cs och skriv ditt eget MySQL-lösenord efter "Password=". Skriv sedan dotnet run i terminalen.

## Så använder du programmet

Välj en kund (2) eller lägg till en ny (1). Sök lediga rum (3) genom att skriva ort, datum och hur många ni är. Man kan välja pool, restaurang och hur nära strand och centrum det ska vara, och sortera på pris eller omdöme. Välj ett rum för att boka. Då får man ett bokningsnummer och vad det kostar. Med 4, 5 och 6 kan man se bokningarna, ändra antal personer eller avboka.

Man kan bara boka i december 2026. Man kan checka in samma dag som någon annan checkar ut.

## Testdata

3 orter, 5 hotell, 20 rum, 5 kunder och 25 bokningar.

## Så har jag testat

Jag har testat allt själv i programmet.

Bokat rum syns inte: Ystad 10–13 december. Rum 101 är bokat och syntes inte. Det funkade.

Checka in samma dag som någon checkar ut: Ystad 13–15 december. Rum 101 syntes och gick att boka. Det funkade.

Boka mitt i en annan bokning går inte: Malmö 8–10 december. Rum A1 är bokat 5–20 december och syntes inte. Det funkade.

För många personer: 7 personer i Ystad gav inga rum, eftersom det största rummet är för 6. Det