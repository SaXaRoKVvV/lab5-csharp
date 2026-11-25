using LR5.Models;

namespace LR5;

/// <summary>Вспомогательный класс с LINQ-запросами к базе данных.</summary>
public static class QueryService
{
    private const string Sale = "Продажа";

    /// <summary>
    /// Запрос 1 (одна таблица, перечень).
    /// Магазины, расположенные в указанном районе города.
    /// </summary>
    public static List<Shop> GetShopsByDistrict(Database db, string district)
    {
        return db.Shops
            .Where(s => s.District.Equals(district,
                StringComparison.OrdinalIgnoreCase))
            .OrderBy(s => s.Address)
            .ToList();
    }

    /// <summary>
    /// Запрос 2 (две таблицы, одно значение).
    /// Общее количество упаковок товаров, проданных в магазинах указанного района.
    /// </summary>
    public static int GetSoldPacksInDistrict(Database db, string district)
    {
        return db.Movements
            .Where(m => m.OperationType == Sale)
            .Join(db.Shops,
                m => m.ShopId,
                s => s.Id,
                (m, s) => new { m.PackCount, s.District })
            .Where(x => x.District.Equals(district, StringComparison.OrdinalIgnoreCase))
            .Sum(x => x.PackCount);
    }

    /// <summary>
    /// Запрос 3 (три таблицы, перечень).
    /// Товары указанного отдела, проданные в магазинах указанного района,
    /// и общее количество проданных упаковок каждого товара.
    /// </summary>
    public static List<string> GetSoldProductsByDepartment(
        Database db, string department, string district)
    {
        return db.Movements
            .Where(m => m.OperationType == Sale)
            .Join(db.Shops,
                m => m.ShopId,
                s => s.Id,
                (m, s) => new { m.Article, m.PackCount, s.District })
            .Where(x => x.District.Equals(district, StringComparison.OrdinalIgnoreCase))
            .Join(db.Products,
                x => x.Article,
                p => p.Article,
                (x, p) => new { p.Name, p.Department, x.PackCount })
            .Where(x => x.Department.Equals(department, StringComparison.OrdinalIgnoreCase))
            .GroupBy(x => x.Name)
            .Select(g => $"{g.Key}: {g.Sum(x => x.PackCount)} уп.")
            .OrderBy(text => text)
            .ToList();
    }

    /// <summary>
    /// Запрос 4 (три таблицы, одно значение) - пример из задания.
    /// Общий объём (в литрах) товаров, в названии которых есть указанный текст
    /// (например, «Шампунь»), проданных в магазинах на указанной улице
    /// за период с даты from по дату to включительно.
    /// </summary>
    public static double GetSoldVolumeInLiters(Database db, string productName,
        string street, DateTime from, DateTime to)
    {
        return db.Movements
            .Where(m => m.OperationType == Sale &&
                        m.Date.Date >= from.Date &&
                        m.Date.Date <= to.Date)
            .Join(db.Shops.Where(s => s.Address.Contains(street, StringComparison.OrdinalIgnoreCase)),
                m => m.ShopId,
                s => s.Id,
                (m, s) => new { m.Article, m.PackCount })
            .Join(db.Products.Where(p => p.Unit == "мл" &&
                                         p.Name.Contains(productName, StringComparison.OrdinalIgnoreCase)),
                x => x.Article,
                p => p.Article,
                (x, p) => x.PackCount * p.QuantityInPack)
            .Sum() / 1000;
    }
}
