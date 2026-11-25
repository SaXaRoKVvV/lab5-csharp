namespace LR5.Models;

/// <summary>Магазин.</summary>
public class Shop
{
    public string Id { get; set; }

    public string District { get; set; }

    public string Address { get; set; }

    public Shop(string id, string district, string address)
    {
        Id = id;
        District = district;
        Address = address;
    }

    public override string ToString()
    {
        return $"Магазин {Id} | Район: {District} |" +
               $" Адрес: {Address}";
    }
}
