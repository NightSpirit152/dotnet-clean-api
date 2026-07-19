using Pharmacy.Domain.Entities;
using Pharmacy.Domain.Interfaces;
using Pharmacy.Infrastructure.Data;

namespace Pharmacy.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Medicines = new MedicineRepository(_context);
        Suppliers = new Repository<Supplier>(_context);
        StockMovements = new Repository<StockMovement>(_context);
    }

    public IMedicineRepository Medicines { get; }
    public IRepository<Supplier> Suppliers { get; }
    public IRepository<StockMovement> StockMovements { get; }

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
}
