namespace Pharmacy.Domain.Entities;

public enum MovementType
{
    Incoming,
    Outgoing
}

public class StockMovement
{
    public int Id { get; set; }
    public int MedicineId { get; set; }
    public Medicine? Medicine { get; set; }
    public MovementType Type { get; set; }
    public int Quantity { get; set; }
    public DateTime Date { get; set; }
    public string Note { get; set; } = string.Empty;
}
