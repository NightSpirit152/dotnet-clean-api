using Microsoft.AspNetCore.Mvc;
using Pharmacy.Application.DTOs;
using Pharmacy.Application.Exceptions;
using Pharmacy.Application.Services;

namespace Pharmacy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicinesController : ControllerBase
{
    private readonly IMedicineService _medicineService;

    public MedicinesController(IMedicineService medicineService)
    {
        _medicineService = medicineService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MedicineDto>>> GetAll()
        => Ok(await _medicineService.GetAllAsync());

    [HttpGet("low-stock")]
    public async Task<ActionResult<IEnumerable<MedicineDto>>> GetLowStock([FromQuery] int threshold = 10)
        => Ok(await _medicineService.GetLowStockAsync(threshold));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MedicineDto>> GetById(int id)
    {
        try
        {
            return Ok(await _medicineService.GetByIdAsync(id));
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<MedicineDto>> Create(CreateMedicineDto dto)
    {
        try
        {
            var created = await _medicineService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<MedicineDto>> Update(int id, UpdateMedicineDto dto)
    {
        try
        {
            return Ok(await _medicineService.UpdateAsync(id, dto));
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _medicineService.DeleteAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPost("stock-movements")]
    public async Task<ActionResult<MedicineDto>> RegisterStockMovement(StockMovementDto dto)
    {
        try
        {
            return Ok(await _medicineService.RegisterStockMovementAsync(dto));
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InsufficientStockException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
