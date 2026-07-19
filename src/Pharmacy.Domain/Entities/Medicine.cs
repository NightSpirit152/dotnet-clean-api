namespace Pharmacy.Domain.Entities;

public class Medicine
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int QuantityInStock { get; set; }
    public DateTime ExpiryDate { get; set; }

    public int SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
}
