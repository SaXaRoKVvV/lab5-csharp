using LR5.Models;

namespace LR5;

/// <summary>База данных «Продукты»: три таблицы, хранящиеся в памяти.</summary>
public class Database
{
    public List<Movement> Movements { get; set; } = new();

    public List<Shop> Shops { get; set; } = new();

    public List<Product> Products { get; set; } = new();

    public bool IsLoaded => Movements.Count > 0 ||
                            Shops.Count > 0 || Products.Count > 0;
}
