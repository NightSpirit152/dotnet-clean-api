using Microsoft.EntityFrameworkCore;
using Pharmacy.Domain.Entities;

namespace Pharmacy.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Medicine> Medicines => Set<Medicine>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Medicine>()
            .Property(m => m.Price)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Medicine>()
            .HasOne(m => m.Supplier)
            .WithMany(s => s.Medicines)
            .HasForeignKey(m => m.SupplierId);

        modelBuilder.Entity<StockMovement>()
            .HasOne(sm => sm.Medicine)
            .WithMany(m => m.StockMovements)
            .HasForeignKey(sm => sm.MedicineId);

        modelBuilder.Entity<Supplier>().HasData(
            new Supplier { Id = 1, Name = "ФармДистрибьюшн", ContactPhone = "+7 495 000-00-00", Email = "supply@farmdist.ru" }
        );

        modelBuilder.Entity<Medicine>().HasData(
            new Medicine { Id = 1, Name = "Парацетамол 500мг №20", Manufacturer = "Фармстандарт", Price = 45.5m, QuantityInStock = 120, ExpiryDate = new DateTime(2027, 6, 1), SupplierId = 1 },
            new Medicine { Id = 2, Name = "Ибупрофен 400мг №30", Manufacturer = "Нижфарм", Price = 89.9m, QuantityInStock = 8, ExpiryDate = new DateTime(2026, 12, 1), SupplierId = 1 }
        );
    }
}
