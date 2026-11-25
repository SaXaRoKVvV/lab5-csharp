using System.Globalization;

namespace LR5;

/// <summary>Ввод данных с клавиатуры с проверкой.</summary>
public static class InputHelper
{
    public static string ReadString(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? text = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(text))
            {
                return text.Trim();
            }

            Console.WriteLine("Ошибка: строка не должна быть пустой.");
        }
    }

    /// <summary>Читает строку; если нажат Enter, возвращает значение по умолчанию.</summary>
    public static string ReadStringOrDefault
        (string prompt, string defaultValue)
    {
        Console.Write(prompt);
        string? text = Console.ReadLine();

        return string.IsNullOrWhiteSpace(text) ? defaultValue : text.Trim();
    }

    public static int ReadInt(string prompt, int min = 1, int max = int.MaxValue)
    {
        while (true)
        {
            Console.Write(prompt);

            if (int.TryParse(Console.ReadLine(), out int value) &&
                value >= min && value <= max)
            {
                return value;
            }

            Console.WriteLine($"Ошибка: введите целое число от {min} до {max}.");
        }
    }

    public static double ReadPositiveDouble(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string text = (Console.ReadLine() ?? "").Replace(',', '.');

            if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double value) && value > 0)
            {
                return value;
            }

            Console.WriteLine("Ошибка: введите положительное число.");
        }
    }

    public static DateTime ReadDate(string prompt)
    {
        while (true)
        {
            Console.Write(prompt + " (дд.мм.гггг): ");

            if (DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime value))
            {
                return value;
            }

            Console.WriteLine("Ошибка: неверный формат даты.");
        }
    }

    public static string ReadOperationType()
    {
        while (true)
        {
            Console.Write("Тип операции (1 - Поступление, 2 - Продажа): ");
            string? text = Console.ReadLine();

            if (text == "1")
            {
                return "Поступление";
            }

            if (text == "2")
            {
                return "Продажа";
            }

            Console.WriteLine("Ошибка: введите 1 или 2.");
        }
    }
}
