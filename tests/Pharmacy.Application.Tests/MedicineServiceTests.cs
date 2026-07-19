using Moq;
using Pharmacy.Application.DTOs;
using Pharmacy.Application.Exceptions;
using Pharmacy.Application.Services;
using Pharmacy.Domain.Entities;
using Pharmacy.Domain.Interfaces;
using Xunit;

namespace Pharmacy.Application.Tests;

public class MedicineServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMedicineRepository> _medicineRepoMock = new();
    private readonly Mock<IRepository<StockMovement>> _stockMovementRepoMock = new();

    private MedicineService CreateService()
    {
        _unitOfWorkMock.Setup(u => u.Medicines).Returns(_medicineRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.StockMovements).Returns(_stockMovementRepoMock.Object);
        return new MedicineService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsDto()
    {
        var medicine = new Medicine { Id = 1, Name = "Парацетамол", Price = 45.5m, QuantityInStock = 10 };
        _medicineRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(medicine);
        var service = CreateService();

        var result = await service.GetByIdAsync(1);

        Assert.Equal("Парацетамол", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_MissingId_ThrowsNotFoundException()
    {
        _medicineRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Medicine?)null);
        var service = CreateService();

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(99));
    }

    [Fact]
    public async Task CreateAsync_InvalidPrice_ThrowsArgumentException()
    {
        var service = CreateService();
        var dto = new CreateMedicineDto("Тест", "Производитель", 0m, 10, DateTime.UtcNow.AddYears(1), 1);

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(dto));
    }

    [Fact]
    public async Task RegisterStockMovement_Incoming_IncreasesQuantity()
    {
        var medicine = new Medicine { Id = 1, Name = "Ибупрофен", QuantityInStock = 10 };
        _medicineRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(medicine);
        var service = CreateService();

        var dto = new StockMovementDto(1, "Incoming", 20, "Поставка");
        var result = await service.RegisterStockMovementAsync(dto);

        Assert.Equal(30, result.QuantityInStock);
    }

    [Fact]
    public async Task RegisterStockMovement_OutgoingMoreThanStock_ThrowsInsufficientStockException()
    {
        var medicine = new Medicine { Id = 1, Name = "Ибупрофен", QuantityInStock = 5 };
        _medicineRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(medicine);
        var service = CreateService();

        var dto = new StockMovementDto(1, "Outgoing", 10, "Продажа");

        await Assert.ThrowsAsync<InsufficientStockException>(
            () => service.RegisterStockMovementAsync(dto));
    }

    [Fact]
    public async Task RegisterStockMovement_UnknownType_ThrowsArgumentException()
    {
        var medicine = new Medicine { Id = 1, Name = "Ибупрофен", QuantityInStock = 5 };
        _medicineRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(medicine);
        var service = CreateService();

        var dto = new StockMovementDto(1, "Teleport", 1, "???");

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.RegisterStockMovementAsync(dto));
    }
}
