using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendorRisk.Api.Data;
using VendorRisk.Api.Domain.Entities;
using VendorRisk.Api.Domain.Enums;
using VendorRisk.Api.Dtos;
using VendorRisk.Api.Services;

namespace VendorRisk.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/contracts")]
public sealed class ContractsController(AppDbContext db, IAuditService audit) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ContractDto>>> Search(
        [FromQuery] Guid? vendorId,
        [FromQuery] ContractStatus? status,
        [FromQuery] DateOnly? renewalBefore,
        [FromQuery] DateOnly? renewalAfter,
        CancellationToken cancellationToken)
    {
        var query = db.Contracts.Include(x => x.Vendor).AsNoTracking();
        if (vendorId is not null) query = query.Where(x => x.VendorId == vendorId);
        if (status is not null) query = query.Where(x => x.Status == status);
        if (renewalBefore is not null) query = query.Where(x => x.RenewalDate <= renewalBefore);
        if (renewalAfter is not null) query = query.Where(x => x.RenewalDate >= renewalAfter);

        return Ok(await query.OrderBy(x => x.RenewalDate).Select(x => ToDto(x)).ToListAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ContractDto>> Get(Guid id, CancellationToken cancellationToken)
    {
        var contract = await db.Contracts.Include(x => x.Vendor).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return contract is null ? NotFound() : Ok(ToDto(contract));
    }

    [HttpPost]
    [Authorize(Policy = "ProcurementOrAdmin")]
    public async Task<ActionResult<ContractDto>> Create(CreateContractRequest request, CancellationToken cancellationToken)
    {
        var validation = ValidateContract(request.Title, request.StartDate, request.EndDate, request.RenewalDate, request.Value, request.Currency);
        if (validation is not null) return BadRequest(validation);
        if (!await db.Vendors.AnyAsync(x => x.Id == request.VendorId, cancellationToken)) return BadRequest("Vendor does not exist.");

        var contract = new Contract
        {
            VendorId = request.VendorId,
            Title = request.Title.Trim(),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            RenewalDate = request.RenewalDate,
            Value = request.Value,
            Currency = request.Currency.Trim().ToUpperInvariant(),
            Status = request.Status
        };

        db.Contracts.Add(contract);
        await db.SaveChangesAsync(cancellationToken);
        await audit.WriteAsync("Create", nameof(Contract), contract.Id, $"Contract {contract.Title} created.", cancellationToken);

        return CreatedAtAction(nameof(Get), new { id = contract.Id }, ToDto(contract));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "ProcurementOrAdmin")]
    public async Task<ActionResult<ContractDto>> Update(Guid id, UpdateContractRequest request, CancellationToken cancellationToken)
    {
        var contract = await db.Contracts.FindAsync([id], cancellationToken);
        if (contract is null) return NotFound();

        var validation = ValidateContract(request.Title, request.StartDate, request.EndDate, request.RenewalDate, request.Value, request.Currency);
        if (validation is not null) return BadRequest(validation);

        contract.Title = request.Title.Trim();
        contract.StartDate = request.StartDate;
        contract.EndDate = request.EndDate;
        contract.RenewalDate = request.RenewalDate;
        contract.Value = request.Value;
        contract.Currency = request.Currency.Trim().ToUpperInvariant();
        contract.Status = request.Status;
        contract.UpdatedAtUtc = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken);
        await audit.WriteAsync("Update", nameof(Contract), contract.Id, $"Contract {contract.Title} updated.", cancellationToken);
        return Ok(ToDto(contract));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var contract = await db.Contracts.FindAsync([id], cancellationToken);
        if (contract is null) return NotFound();

        db.Contracts.Remove(contract);
        await db.SaveChangesAsync(cancellationToken);
        await audit.WriteAsync("Delete", nameof(Contract), id, $"Contract {contract.Title} deleted.", cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/documents")]
    [Authorize(Policy = "ProcurementOrAdmin")]
    public async Task<IActionResult> AddDocument(Guid id, AddDocumentMetadataRequest request, CancellationToken cancellationToken)
    {
        if (!await db.Contracts.AnyAsync(x => x.Id == id, cancellationToken)) return NotFound();
        if (string.IsNullOrWhiteSpace(request.FileName) || string.IsNullOrWhiteSpace(request.StorageUri)) return BadRequest("File name and storage URI are required.");

        db.DocumentMetadata.Add(new DocumentMetadata
        {
            ContractId = id,
            FileName = request.FileName.Trim(),
            ContentType = request.ContentType.Trim(),
            StorageUri = request.StorageUri.Trim(),
            UploadedById = User.GetUserId()
        });

        await db.SaveChangesAsync(cancellationToken);
        await audit.WriteAsync("Create", nameof(DocumentMetadata), id, $"Document metadata added for contract {id}.", cancellationToken);
        return NoContent();
    }

    private static string? ValidateContract(string title, DateOnly startDate, DateOnly endDate, DateOnly renewalDate, decimal value, string currency)
    {
        if (string.IsNullOrWhiteSpace(title)) return "Contract title is required.";
        if (endDate <= startDate) return "End date must be after start date.";
        if (renewalDate < startDate || renewalDate > endDate) return "Renewal date must be inside the contract term.";
        if (value <= 0) return "Contract value must be greater than zero.";
        if (string.IsNullOrWhiteSpace(currency) || currency.Trim().Length != 3) return "Currency must be a three-letter code.";
        return null;
    }

    private static ContractDto ToDto(Contract contract) =>
        new(contract.Id, contract.VendorId, contract.Vendor?.Name, contract.Title, contract.StartDate, contract.EndDate, contract.RenewalDate, contract.Value, contract.Currency, contract.Status);
}
