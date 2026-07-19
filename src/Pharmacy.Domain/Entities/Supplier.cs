namespace Pharmacy.Domain.Entities;

public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();
}
