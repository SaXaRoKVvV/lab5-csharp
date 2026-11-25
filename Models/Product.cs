namespace LR5.Models;

/// <summary>Товар.</summary>
public class Product
{
    public int Article { get; set; }

    public string Department { get; set; }

    public string Name { get; set; }

    public string Unit { get; set; }

    public double QuantityInPack { get; set; }

    public decimal PricePerPack { get; set; }

    public Product(int article, string department, string name,
        string unit, double quantityInPack, decimal pricePerPack)
    {
        Article = article;
        Department = department;
        Name = name;
        Unit = unit;
        QuantityInPack = quantityInPack;
        PricePerPack = pricePerPack;
    }

    public override string ToString()
    {
        return $"Артикул {Article} | {Department} | {Name} | " +
               $"{QuantityInPack} {Unit} в упаковке | " +
               $"{PricePerPack} руб.";
    }
}
