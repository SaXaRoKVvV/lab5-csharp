using LR5.Models;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Globalization;

namespace LR5;

/// <summary>Чтение и сохранение базы данных в Excel-файл (.xls).</summary>
public static class ExcelService
{
    private const string MovementSheet = "Движение товаров";
    private const string ShopSheet = "Магазин";
    private const string ProductSheet = "Товар";

    /// <summary>Читает все три листа из файла.</summary>
    public static Database Load(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Файл не найден: " + path);
        }

        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        IWorkbook workbook = WorkbookFactory.Create(stream);
        var db = new Database();

        // Лист «Магазин»: ID магазина, Район, Адрес
        ISheet shopSheet = GetSheet(workbook, ShopSheet);
        for (int i = 1; i <= shopSheet.LastRowNum; i++)
        {
            IRow row = shopSheet.GetRow(i);
            if (IsEmpty(row))
            {
                continue;
            }

            db.Shops.Add(new Shop(
                GetText(row, 0),
                GetText(row, 1),
                GetText(row, 2)));
        }

        // Лист «Товар»: Артикул, Отдел, Наименование, Ед. изм., Количество в упаковке, Цена
        ISheet productSheet = GetSheet(workbook, ProductSheet);
        for (int i = 1; i <= productSheet.LastRowNum; i++)
        {
            IRow row = productSheet.GetRow(i);
            if (IsEmpty(row))
            {
                continue;
            }

            db.Products.Add(new Product(
                GetInt(row, 0),
                GetText(row, 1),
                GetText(row, 2),
                GetText(row, 3),
                GetDouble(row, 4),
                (decimal)GetDouble(row, 5)));
        }

        // Лист «Движение товаров»: ID операции, Дата, ID магазина, Артикул, Количество, Тип
        ISheet movementSheet = GetSheet(workbook, MovementSheet);
        for (int i = 1; i <= movementSheet.LastRowNum; i++)
        {
            IRow row = movementSheet.GetRow(i);
            if (IsEmpty(row))
            {
                continue;
            }

            db.Movements.Add(new Movement(
                GetInt(row, 0),
                GetDate(row, 1),
                GetText(row, 2),
                GetInt(row, 3),
                GetInt(row, 4),
                GetText(row, 5)));
        }

        return db;
    }

    /// <summary>Сохраняет базу данных в новый .xls файл.</summary>
    public static void Save(Database db, string path)
    {
        IWorkbook workbook = new HSSFWorkbook();

        ICellStyle dateStyle = workbook.CreateCellStyle();
        dateStyle.DataFormat = workbook.CreateDataFormat().GetFormat("dd.MM.yyyy");

        ISheet movementSheet = workbook.CreateSheet(MovementSheet);
        WriteHeader(movementSheet, "ID операции", "Дата", "ID магазина",
            "Артикул", "Количество упаковок, шт", "Тип операции");
        for (int i = 0; i < db.Movements.Count; i++)
        {
            Movement m = db.Movements[i];
            IRow row = movementSheet.CreateRow(i + 1);
            row.CreateCell(0).SetCellValue(m.OperationId);

            ICell dateCell = row.CreateCell(1);
            dateCell.SetCellValue(m.Date);
            dateCell.CellStyle = dateStyle;

            row.CreateCell(2).SetCellValue(m.ShopId);
            row.CreateCell(3).SetCellValue(m.Article);
            row.CreateCell(4).SetCellValue(m.PackCount);
            row.CreateCell(5).SetCellValue(m.OperationType);
        }

        ISheet productSheet = workbook.CreateSheet(ProductSheet);
        WriteHeader(productSheet, "Артикул", "Отдел", "Наименование товара",
            "Ед_изм", "Количество в упаковке", "Цена за упаковку");
        for (int i = 0; i < db.Products.Count; i++)
        {
            Product p = db.Products[i];
            IRow row = productSheet.CreateRow(i + 1);
            row.CreateCell(0).SetCellValue(p.Article);
            row.CreateCell(1).SetCellValue(p.Department);
            row.CreateCell(2).SetCellValue(p.Name);
            row.CreateCell(3).SetCellValue(p.Unit);
            row.CreateCell(4).SetCellValue(p.QuantityInPack);
            row.CreateCell(5).SetCellValue((double)p.PricePerPack);
        }

        ISheet shopSheet = workbook.CreateSheet(ShopSheet);
        WriteHeader(shopSheet, "ID магазина", "Район", "Адрес");
        for (int i = 0; i < db.Shops.Count; i++)
        {
            Shop s = db.Shops[i];
            IRow row = shopSheet.CreateRow(i + 1);
            row.CreateCell(0).SetCellValue(s.Id);
            row.CreateCell(1).SetCellValue(s.District);
            row.CreateCell(2).SetCellValue(s.Address);
        }

        using var stream = new FileStream(path, FileMode.Create, FileAccess.Write);
        workbook.Write(stream);
    }

    private static ISheet GetSheet(IWorkbook workbook, string name)
    {
        ISheet? sheet = workbook.GetSheet(name);
        if (sheet == null)
        {
            throw new Exception($"В файле нет листа \"{name}\".");
        }

        return sheet;
    }

    private static void WriteHeader(ISheet sheet, params string[] titles)
    {
        IRow row = sheet.CreateRow(0);
        for (int i = 0; i < titles.Length; i++)
        {
            row.CreateCell(i).SetCellValue(titles[i]);
        }
    }

    private static bool IsEmpty(IRow? row)
    {
        if (row == null)
        {
            return true;
        }

        ICell? cell = row.GetCell(0);
        return cell == null || cell.CellType == CellType.Blank;
    }

    private static string GetText(IRow row, int index)
    {
        ICell? cell = row.GetCell(index);
        if (cell == null)
        {
            return "";
        }

        if (cell.CellType == CellType.Numeric)
        {
            return cell.NumericCellValue.ToString();
        }

        return cell.ToString()!.Trim();
    }

    private static double GetDouble(IRow row, int index)
    {
        ICell? cell = row.GetCell(index);
        if (cell == null)
        {
            return 0;
        }

        if (cell.CellType == CellType.Numeric)
        {
            return cell.NumericCellValue;
        }

        double.TryParse(cell.ToString()!.Replace(',', '.'),
            NumberStyles.Float, CultureInfo.InvariantCulture, out double result);
        return result;
    }

    private static int GetInt(IRow row, int index)
    {
        return (int)GetDouble(row, index);
    }

    private static DateTime GetDate(IRow row, int index)
    {
        ICell? cell = row.GetCell(index);
        if (cell == null)
        {
            return DateTime.MinValue;
        }

        if (cell.CellType == CellType.Numeric)
        {
            return DateTime.FromOADate(cell.NumericCellValue);
        }

        DateTime.TryParse(cell.ToString(), CultureInfo.GetCultureInfo("ru-RU"),
            DateTimeStyles.None, out DateTime result);
        return result;
    }
}
