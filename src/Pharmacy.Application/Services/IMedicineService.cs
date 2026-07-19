using Pharmacy.Application.DTOs;

namespace Pharmacy.Application.Services;

public interface IMedicineService
{
    Task<IEnumerable<MedicineDto>> GetAllAsync();
    Task<MedicineDto> GetByIdAsync(int id);
    Task<IEnumerable<MedicineDto>> GetLowStockAsync(int threshold);
    Task<MedicineDto> CreateAsync(CreateMedicineDto dto);
    Task<MedicineDto> UpdateAsync(int id, UpdateMedicineDto dto);
    Task DeleteAsync(int id);
    Task<MedicineDto> RegisterStockMovementAsync(StockMovementDto dto);
}
