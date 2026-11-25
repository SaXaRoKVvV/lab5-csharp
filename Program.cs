using System.Text;
using LR5.Models;

namespace LR5;

public class Program
{
    private const string DefaultFile = "LR5-var10.xls";

    private static Database _db = new();
    private static string _currentPath = DefaultFile;

    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        bool exit = false;

        while (!exit)
        {
            PrintMenu();
            int choice = InputHelper.ReadInt("Ваш выбор: ", 0, 6);
            Console.WriteLine();

            try
            {
                switch (choice)
                {
                    case 1:
                        LoadDatabase();
                        break;
                    case 2:
                        ShowDatabase();
                        break;
                    case 3:
                        DeleteElement();
                        break;
                    case 4:
                        AddElement();
                        break;
                    case 5:
                        RunQueries();
                        break;
                    case 6:
                        SaveDatabase();
                        break;
                    case 0:
                        exit = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка: " + ex.Message);
            }

            if (!exit)
            {
                Console.WriteLine("\nНажмите Enter для продолжения...");
                Console.ReadLine();
            }
        }
    }

    private static void PrintMenu()
    {
        Console.Clear();
        Console.WriteLine("===== БАЗА ДАННЫХ «ПРОДУКТЫ» =====");
        Console.WriteLine("1. Прочитать базу данных из Excel-файла");
        Console.WriteLine("2. Просмотреть базу данных");
        Console.WriteLine("3. Удалить элемент (по ключу)");
        Console.WriteLine("4. Добавить элемент");
        Console.WriteLine("5. Выполнить запрос");
        Console.WriteLine("6. Сохранить изменения в Excel-файл");
        Console.WriteLine("0. Выход");
    }

    private static bool CheckLoaded()
    {
        if (_db.IsLoaded)
        {
            return true;
        }

        Console.WriteLine("Сначала загрузите базу данных (пункт 1).");
        return false;
    }

    private static void LoadDatabase()
    {
        string path = InputHelper.ReadStringOrDefault(
            $"Путь к Excel-файлу (Enter - {DefaultFile}): ", DefaultFile);

        _db = ExcelService.Load(path);
        _currentPath = path;

        Console.WriteLine($"Загружено: движений - {_db.Movements.Count}, " +
                          $"магазинов - {_db.Shops.Count}, товаров - {_db.Products.Count}.");
    }

    private static void SaveDatabase()
    {
        if (!CheckLoaded())
        {
            return;
        }

        string path = InputHelper.ReadStringOrDefault(
            $"Путь для сохранения (Enter - {_currentPath}): ", _currentPath);

        ExcelService.Save(_db, path);
        Console.WriteLine("Файл сохранён: " + path);
    }

    private static void ShowDatabase()
    {
        if (!CheckLoaded())
        {
            return;
        }

        PrintTableMenu();
        int table = InputHelper.ReadInt("Какую таблицу показать: ", 1, 3);
        int count = InputHelper.ReadInt("Сколько первых строк показать (0 - все): ", 0);
        Console.WriteLine();

        if (table == 1)
        {
            PrintRows(_db.Movements, count);
        }
        else if (table == 2)
        {
            PrintRows(_db.Shops, count);
        }
        else
        {
            PrintRows(_db.Products, count);
        }
    }

    private static void PrintRows<T>(List<T> rows, int count)
    {
        IEnumerable<T> part = count == 0 ? rows : rows.Take(count);

        foreach (T row in part)
        {
            Console.WriteLine(row);
        }

        Console.WriteLine($"Показано строк: {part.Count()} из {rows.Count}");
    }

    private static void PrintTableMenu()
    {
        Console.WriteLine("1 - Движение товаров, 2 - Магазины, 3 - Товары");
    }

    private static void DeleteElement()
    {
        if (!CheckLoaded())
        {
            return;
        }

        PrintTableMenu();
        int table = InputHelper.ReadInt("Из какой таблицы удалить: ", 1, 3);
        int removed;

        if (table == 1)
        {
            int id = InputHelper.ReadInt("ID операции: ");
            removed = _db.Movements.RemoveAll(m => m.OperationId == id);
        }
        else if (table == 2)
        {
            string id = InputHelper.ReadString("ID магазина (например, M5): ");

            if (_db.Movements.Any(m => m.ShopId.Equals(id, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Нельзя удалить: на этот магазин есть ссылки в таблице движений.");
                return;
            }

            removed = _db.Shops.RemoveAll(s => s.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }
        else
        {
            int article = InputHelper.ReadInt("Артикул: ");

            if (_db.Movements.Any(m => m.Article == article))
            {
                Console.WriteLine("Нельзя удалить: на этот товар есть ссылки в таблице движений.");
                return;
            }

            removed = _db.Products.RemoveAll(p => p.Article == article);
        }

        Console.WriteLine(removed > 0 ? "Элемент удалён." : "Элемент с таким ключом не найден.");
    }

    private static void AddElement()
    {
        if (!CheckLoaded())
        {
            return;
        }

        PrintTableMenu();
        int table = InputHelper.ReadInt("В какую таблицу добавить: ", 1, 3);

        if (table == 1)
        {
            AddMovement();
        }
        else if (table == 2)
        {
            AddShop();
        }
        else
        {
            AddProduct();
        }
    }

    private static void AddMovement()
    {
        int id = InputHelper.ReadInt("ID операции: ");
        if (_db.Movements.Any(m => m.OperationId == id))
        {
            Console.WriteLine("Такой ID операции уже существует.");
            return;
        }

        DateTime date = InputHelper.ReadDate("Дата");

        string shopId = InputHelper.ReadString("ID магазина (например, M5): ");
        Shop? shop = _db.Shops.FirstOrDefault(
            s => s.Id.Equals(shopId, StringComparison.OrdinalIgnoreCase));
        if (shop == null)
        {
            Console.WriteLine("Магазина с таким ID нет.");
            return;
        }

        int article = InputHelper.ReadInt("Артикул: ");
        if (!_db.Products.Any(p => p.Article == article))
        {
            Console.WriteLine("Товара с таким артикулом нет.");
            return;
        }

        int count = InputHelper.ReadInt("Количество упаковок: ");
        string type = InputHelper.ReadOperationType();

        _db.Movements.Add(new Movement(id, date, shop.Id, article, count, type));
        Console.WriteLine("Элемент добавлен.");
    }

    private static void AddShop()
    {
        string id = InputHelper.ReadString("ID магазина (например, M19): ");
        if (_db.Shops.Any(s => s.Id.Equals(id, StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine("Такой ID уже существует.");
            return;
        }

        string district = InputHelper.ReadString("Район: ");
        string address = InputHelper.ReadString("Адрес: ");

        _db.Shops.Add(new Shop(id, district, address));
        Console.WriteLine("Элемент добавлен.");
    }

    private static void AddProduct()
    {
        int article = InputHelper.ReadInt("Артикул: ");
        if (_db.Products.Any(p => p.Article == article))
        {
            Console.WriteLine("Такой артикул уже существует.");
            return;
        }

        string department = InputHelper.ReadString("Отдел: ");
        string name = InputHelper.ReadString("Наименование: ");
        string unit = InputHelper.ReadString("Единица измерения (мл, грамм, шт): ");
        double quantity = InputHelper.ReadPositiveDouble("Количество в упаковке: ");
        decimal price = (decimal)InputHelper.ReadPositiveDouble("Цена за упаковку: ");

        _db.Products.Add(new Product(article, department, name, unit, quantity, price));
        Console.WriteLine("Элемент добавлен.");
    }

    private static void RunQueries()
    {
        if (!CheckLoaded())
        {
            return;
        }

        Console.WriteLine("1. Магазины района (перечень, 1 таблица)");
        Console.WriteLine("2. Количество проданных упаковок в районе (значение, 2 таблицы)");
        Console.WriteLine("3. Проданные товары отдела в районе (перечень, 3 таблицы)");
        Console.WriteLine("4. Объём проданных жидких товаров на улице за период (значение, 3 таблицы)");
        int query = InputHelper.ReadInt("Номер запроса: ", 1, 4);
        Console.WriteLine();

        if (query == 1)
        {
            string district = InputHelper.ReadString("Район (Северный, Южный, Центральный): ");
            List<Shop> shops = QueryService.GetShopsByDistrict(_db, district);

            Console.WriteLine($"Найдено магазинов: {shops.Count}");
            foreach (Shop shop in shops)
            {
                Console.WriteLine(shop);
            }
        }
        else if (query == 2)
        {
            string district = InputHelper.ReadString("Район: ");
            int packs = QueryService.GetSoldPacksInDistrict(_db, district);

            Console.WriteLine($"Продано упаковок: {packs}");
        }
        else if (query == 3)
        {
            string department = InputHelper.ReadString(
                "Отдел (Бытовая химия, Гигиена, Красота и здоровье): ");
            string district = InputHelper.ReadString("Район: ");
            List<string> products = QueryService.GetSoldProductsByDepartment(
                _db, department, district);

            Console.WriteLine($"Найдено товаров: {products.Count}");
            foreach (string product in products)
            {
                Console.WriteLine(product);
            }
        }
        else
        {
            string name = InputHelper.ReadString("Часть названия товара (например, Шампунь): ");
            string street = InputHelper.ReadString("Улица (например, Гагарина): ");
            DateTime from = InputHelper.ReadDate("Дата начала");
            DateTime to = InputHelper.ReadDate("Дата окончания");

            if (from > to)
            {
                Console.WriteLine("Дата начала не может быть позже даты окончания.");
                return;
            }

            double liters = QueryService.GetSoldVolumeInLiters(_db, name, street, from, to);
            Console.WriteLine($"Общий объём: {(int)liters} л");
        }
    }
}
