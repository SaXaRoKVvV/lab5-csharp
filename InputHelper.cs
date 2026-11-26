using System.Globalization;

namespace LR5;

/// <summary>Ввод данных с клавиатуры с проверкой.</summary>
public static class InputHelper
{
    public static string ReadString(string prompt)
    {
        string? text = null;
        bool isCorrect = false;

        while (!isCorrect)
        {
            Console.Write(prompt);
            text = Console.ReadLine();
            isCorrect = !string.IsNullOrWhiteSpace(text);

            if (!isCorrect)
            {
                Console.WriteLine("Ошибка: строка не должна быть пустой.");
            }
        }

        return text!.Trim();
    }

    /// <summary>Читает строку; если нажат Enter, возвращает значение по умолчанию.</summary>
    public static string ReadStringOrDefault(string prompt, string defaultValue)
    {
        Console.Write(prompt);
        string? text = Console.ReadLine();

        return string.IsNullOrWhiteSpace(text) ? defaultValue : text.Trim();
    }

    public static int ReadInt(string prompt, int min = 1, int max = int.MaxValue)
    {
        int value = 0;
        bool isCorrect = false;

        while (!isCorrect)
        {
            Console.Write(prompt);
            isCorrect = int.TryParse(Console.ReadLine(), out value) &&
                        value >= min && value <= max;

            if (!isCorrect)
            {
                Console.WriteLine($"Ошибка: введите целое число от {min} до {max}.");
            }
        }

        return value;
    }

    public static double ReadPositiveDouble(string prompt)
    {
        string text;
        double value = 0;
        bool isCorrect = false;

        while (!isCorrect)
        {
            Console.Write(prompt);
            text = (Console.ReadLine() ?? "").Replace(',', '.');
            isCorrect = double.TryParse(text, NumberStyles.Float,
                            CultureInfo.InvariantCulture, out value) && value > 0;

            if (!isCorrect)
            {
                Console.WriteLine("Ошибка: введите положительное число.");
            }
        }

        return value;
    }

    public static DateTime ReadDate(string prompt)
    {
        DateTime value = DateTime.MinValue;
        bool isCorrect = false;

        while (!isCorrect)
        {
            Console.Write(prompt + " (дд.мм.гггг): ");
            isCorrect = DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out value);

            if (!isCorrect)
            {
                Console.WriteLine("Ошибка: неверный формат даты.");
            }
        }

        return value;
    }

    public static string ReadOperationType()
    {
        string? text;
        string result = "";

        while (result == "")
        {
            Console.Write("Тип операции (1 - Поступление, 2 - Продажа): ");
            text = Console.ReadLine();

            if (text == "1")
            {
                result = "Поступление";
            }
            else if (text == "2")
            {
                result = "Продажа";
            }
            else
            {
                Console.WriteLine("Ошибка: введите 1 или 2.");
            }
        }

        return result;
    }
}
