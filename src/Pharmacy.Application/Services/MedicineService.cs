using Pharmacy.Application.DTOs;
using Pharmacy.Application.Exceptions;
using Pharmacy.Domain.Entities;
using Pharmacy.Domain.Interfaces;

namespace Pharmacy.Application.Services;

public class MedicineService : IMedicineService
{
    private readonly IUnitOfWork _unitOfWork;

    public MedicineService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<MedicineDto>> GetAllAsync()
    {
        var medicines = await _unitOfWork.Medicines.GetAllAsync();
        return medicines.Select(ToDto);
    }

    public async Task<MedicineDto> GetByIdAsync(int id)
    {
        var medicine = await _unitOfWork.Medicines.GetByIdAsync(id)
            ?? throw new NotFoundException($"Медикамент с id={id} не найден");

        return ToDto(medicine);
    }

    public async Task<IEnumerable<MedicineDto>> GetLowStockAsync(int threshold)
    {
        var medicines = await _unitOfWork.Medicines.GetLowStockAsync(threshold);
        return medicines.Select(ToDto);
    }

    public async Task<MedicineDto> CreateAsync(CreateMedicineDto dto)
    {
        if (dto.Price <= 0)
        {
            throw new ArgumentException("Цена должна быть больше нуля");
        }

        var medicine = new Medicine
        {
            Name = dto.Name,
            Manufacturer = dto.Manufacturer,
            Price = dto.Price,
            QuantityInStock = dto.QuantityInStock,
            ExpiryDate = dto.ExpiryDate,
            SupplierId = dto.SupplierId
        };

        await _unitOfWork.Medicines.AddAsync(medicine);
        await _unitOfWork.SaveChangesAsync();

        return ToDto(medicine);
    }

    public async Task<MedicineDto> UpdateAsync(int id, UpdateMedicineDto dto)
    {
        var medicine = await _unitOfWork.Medicines.GetByIdAsync(id)
            ?? throw new NotFoundException($"Медикамент с id={id} не найден");

        medicine.Name = dto.Name;
        medicine.Manufacturer = dto.Manufacturer;
        medicine.Price = dto.Price;
        medicine.ExpiryDate = dto.ExpiryDate;

        _unitOfWork.Medicines.Update(medicine);
        await _unitOfWork.SaveChangesAsync();

        return ToDto(medicine);
    }

    public async Task DeleteAsync(int id)
    {
        var medicine = await _unitOfWork.Medicines.GetByIdAsync(id)
            ?? throw new NotFoundException($"Медикамент с id={id} не найден");

        _unitOfWork.Medicines.Remove(medicine);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<MedicineDto> RegisterStockMovementAsync(StockMovementDto dto)
    {
        var medicine = await _unitOfWork.Medicines.GetByIdAsync(dto.MedicineId)
            ?? throw new NotFoundException($"Медикамент с id={dto.MedicineId} не найден");

        if (!Enum.TryParse<MovementType>(dto.Type, ignoreCase: true, out var movementType))
        {
            throw new ArgumentException($"Неизвестный тип движения: {dto.Type}");
        }

        if (movementType == MovementType.Outgoing && medicine.QuantityInStock < dto.Quantity)
        {
            throw new InsufficientStockException(
                $"На складе недостаточно товара '{medicine.Name}': запрошено {dto.Quantity}, доступно {medicine.QuantityInStock}");
        }

        medicine.QuantityInStock += movementType == MovementType.Incoming
            ? dto.Quantity
            : -dto.Quantity;

        var movement = new StockMovement
        {
            MedicineId = medicine.Id,
            Type = movementType,
            Quantity = dto.Quantity,
            Date = DateTime.UtcNow,
            Note = dto.Note
        };

        await _unitOfWork.StockMovements.AddAsync(movement);
        _unitOfWork.Medicines.Update(medicine);
        await _unitOfWork.SaveChangesAsync();

        return ToDto(medicine);
    }

    private static MedicineDto ToDto(Medicine medicine) => new(
        medicine.Id,
        medicine.Name,
        medicine.Manufacturer,
        medicine.Price,
        medicine.QuantityInStock,
        medicine.ExpiryDate,
        medicine.SupplierId
    );
}
