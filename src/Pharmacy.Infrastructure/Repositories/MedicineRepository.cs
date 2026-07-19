using Microsoft.EntityFrameworkCore;
using Pharmacy.Domain.Entities;
using Pharmacy.Domain.Interfaces;
using Pharmacy.Infrastructure.Data;

namespace Pharmacy.Infrastructure.Repositories;

public class MedicineRepository : Repository<Medicine>, IMedicineRepository
{
    public MedicineRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Medicine>> GetLowStockAsync(int threshold) =>
        await DbSet.Where(m => m.QuantityInStock <= threshold).ToListAsync();

    public async Task<IEnumerable<Medicine>> SearchByNameAsync(string namePart) =>
        await DbSet.Where(m => EF.Functions.Like(m.Name, $"%{namePart}%")).ToListAsync();
}
