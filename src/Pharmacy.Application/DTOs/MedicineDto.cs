namespace Pharmacy.Application.DTOs;

public record MedicineDto(
    int Id,
    string Name,
    string Manufacturer,
    decimal Price,
    int QuantityInStock,
    DateTime ExpiryDate,
    int SupplierId
);

public record CreateMedicineDto(
    string Name,
    string Manufacturer,
    decimal Price,
    int QuantityInStock,
    DateTime ExpiryDate,
    int SupplierId
);

public record UpdateMedicineDto(
    string Name,
    string Manufacturer,
    decimal Price,
    DateTime ExpiryDate
);

public record StockMovementDto(
    int MedicineId,
    string Type,
    int Quantity,
    string Note
);
