using Pharmacy.Domain.Entities;

namespace Pharmacy.Domain.Interfaces;

public interface IUnitOfWork
{
    IMedicineRepository Medicines { get; }
    IRepository<Supplier> Suppliers { get; }
    IRepository<StockMovement> StockMovements { get; }

    Task<int> SaveChangesAsync();
}
