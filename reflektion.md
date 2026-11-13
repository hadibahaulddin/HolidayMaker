# Reflektion

## Datamodellen

Jag har gjort ett konsolprogram i C# för en resebyrå, där en operatör hjälper kunder att boka hotellrum över telefon. All data sparas i en MySQL-databas.
Databasen har 5 tabeller: city, accommodation, room, customer och booking. Varje sak har sin egen tabell eftersom den har sin egen uppgift. Om jag hade använt färre tabeller skulle samma information sparas flera gånger, och det leder till problem. Till exempel om Hadi avbokar och jag tar bort hans rad, så försvinner också allt om Hadi. Eftersom bokningen ligger i en egen tabell försvinner bara bokningen, och Hadi står kvar som kund. Att varje sak sparas på ett ställe kallas normalisering.
Tabellerna kopplas ihop med främmande nycklar. En främmande nyckel är en kolumn som pekar på id:t i en annan tabell, och databasen kontrollerar att det man pekar på finns.
Jag har också lagt in begränsningar så att felaktig data inte kan sparas. NOT NULL gör att viktiga fält inte får vara tomma, UNIQUE gör att två kunder inte kan ha samma mejl och CHECK gör att till exempel omdömet måste vara 1–5 och att utcheckning måste vara efter incheckning. Det gör att databasen skyddar sig själv även om programmet skulle ha en bugg.

## Säkerhet

I SQL-frågorna använder jag parametrar, till exempel @city. Värdet skickas separat, så det användaren skriver behandlas alltid som text och aldrig som kod. Det skyddar mot SQL-injektion.
En brist är att lösenordet till databasen står direkt i koden i Db.cs. Det är inte bra eftersom alla som ser koden också kan se lösenordet och logga in i databasen. Det vore bättre att lägga lösenordet i en miljövariabel, så att det inte finns med i koden.

## Resultatet

Programmet fungerar. Operatören kan välja kund, söka och boka rum, visa bokningar, ändra antal personer och avboka. All sökning, filtrering och sortering görs i SQL-frågan, så databasen skickar bara tillbaka de rum som matchar.
Jag har testat allt för hand och skrivit ner testerna i README-filen. Alla tester fungerade, till exempel att ett bokat rum inte syns och att man kan checka in samma dag som någon annan checkar ut.

## Möjliga förbättringar

Om jag hade haft mer tid skulle jag visa ett tydligt meddelande när orten inte finns. Nu säger programmet bara att inga rum hittades, och det märkte jag själv när jag stavade fel på en ort under testningen. Jag skulle också flytta lösenordet från koden till en miljövariabel.

## Användning av AI

Jag har använt AI som hjälp i projektet. AI hjälpte mig med förslag på tabellerna, SQL-skripten och testdatan, det mesta av C#-koden. Eftersom jag varit sjukskriven i princip hela denna kurs så har jag använt AI mycket till hur jag skulle installera och koppla ihop Docker, DataGrip och VS Code. Jag har använt AI också för att förklara varför jag gör som jag gör, för att förstå vad jag gör.
Själv har jag installerat och ställt in verktygen, kört SQL-skripten, skapat filerna, kört programmet och gjort alla tester.
Jag kontrollerade att det blev rätt genom att testa varje funktion i DataGrip och i programmet och jämföra med vad som borde hända. Till exempel att rum 101 inte syns 10–13 december men syns 13–15 december.
