namespace LR5.Models;

/// <summary>Запись о движении товара (поступление или продажа).</summary>
public class Movement
{
    public int OperationId { get; set; }

    public DateTime Date { get; set; }

    public string ShopId { get; set; }

    public int Article { get; set; }

    public int PackCount { get; set; }

    public string OperationType { get; set; }

    public Movement(int operationId, DateTime date, string shopId,
        int article, int packCount, string operationType)
    {
        OperationId = operationId;
        Date = date;
        ShopId = shopId;
        Article = article;
        PackCount = packCount;
        OperationType = operationType;
    }

    public override string ToString()
    {
        return $"Операция {OperationId} | {Date:dd.MM.yyyy} | Магазин {ShopId} | " +
               $"Артикул {Article} | {OperationType} | {PackCount} уп.";
    }
}
