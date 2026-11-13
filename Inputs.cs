using System.Globalization;

// Hjälpmetoder för att läsa in värden från användaren utan att programmet kraschar.
static class Input
{
    public static string ReadText(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var text = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(text))
            {
                return text;
            }
            Console.WriteLine("Fältet får inte vara tomt.");
        }
    }

    public static int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out var value) && value >= min && value <= max)
            {
                return value;
            }
            Console.WriteLine($"Ange ett heltal mellan {min} och {max}.");
        }
    }

    // Returnerar null om användaren lämnar fältet tomt.
    public static int? ReadOptionalInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var text = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }
            if (int.TryParse(text, out var value) && value >= 0)
            {
                return value;
            }
            Console.WriteLine("Ange ett positivt heltal, eller lämna tomt.");
        }
    }

    public static DateTime ReadDate(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var text = Console.ReadLine()?.Trim();
            if (DateTime.TryParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var date))
            {
                return date;
            }
            Console.WriteLine("Ange datum i formatet ÅÅÅÅ-MM-DD, till exempel 2026-12-10.");
        }
    }

    public static bool ReadYesNo(string prompt)
    {
        while (true)
        {
            Console.Write(prompt + " (j/n): ");
            var text = Console.ReadLine()?.Trim().ToLower();
            if (text == "j") return true;
            if (text == "n") return false;
            Console.WriteLine("Svara j eller n.");
        }
    }
}