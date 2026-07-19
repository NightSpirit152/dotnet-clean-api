using Pharmacy.Domain.Entities;

namespace Pharmacy.Domain.Interfaces;

public interface IMedicineRepository : IRepository<Medicine>
{
    Task<IEnumerable<Medicine>> GetLowStockAsync(int threshold);
    Task<IEnumerable<Medicine>> SearchByNameAsync(string namePart);
}
