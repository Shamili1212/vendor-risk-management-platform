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
[Route("api/vendors")]
public sealed class VendorsController(AppDbContext db, IAuditService audit, IRiskScoringService riskScoring) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<VendorDto>>> Search(
        [FromQuery] string? search,
        [FromQuery] VendorStatus? status,
        [FromQuery] string? category,
        [FromQuery] Guid? ownerId,
        [FromQuery] RiskTier? riskTier,
        CancellationToken cancellationToken)
    {
        var query = db.Vendors.Include(x => x.Owner).AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.Name.ToLower().Contains(search.ToLower()));
        }

        if (status is not null) query = query.Where(x => x.Status == status);
        if (!string.IsNullOrWhiteSpace(category)) query = query.Where(x => x.Category == category);
        if (ownerId is not null) query = query.Where(x => x.OwnerId == ownerId);
        if (riskTier is not null) query = query.Where(x => x.RiskTier == riskTier);

        return Ok(await query.OrderBy(x => x.Name).Select(x => ToDto(x)).ToListAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<VendorDto>> Get(Guid id, CancellationToken cancellationToken)
    {
        var vendor = await db.Vendors.Include(x => x.Owner).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return vendor is null ? NotFound() : Ok(ToDto(vendor));
    }

    [HttpPost]
    [Authorize(Policy = "ProcurementOrAdmin")]
    public async Task<ActionResult<VendorDto>> Create(CreateVendorRequest request, CancellationToken cancellationToken)
    {
        var validation = ValidateVendor(request.Name, request.Category, request.IncidentCount);
        if (validation is not null) return BadRequest(validation);

        if (!await db.Users.AnyAsync(x => x.Id == request.OwnerId, cancellationToken))
        {
            return BadRequest("Owner user does not exist.");
        }

        var vendor = new Vendor
        {
            Name = request.Name.Trim(),
            Category = request.Category.Trim(),
            Status = request.Status,
            Criticality = request.Criticality,
            ComplianceStatus = request.ComplianceStatus,
            OwnerId = request.OwnerId,
            IncidentCount = request.IncidentCount
        };

        db.Vendors.Add(vendor);
        await db.SaveChangesAsync(cancellationToken);
        await audit.WriteAsync("Create", nameof(Vendor), vendor.Id, $"Vendor {vendor.Name} created.", cancellationToken);

        return CreatedAtAction(nameof(Get), new { id = vendor.Id }, ToDto(vendor));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "ProcurementOrAdmin")]
    public async Task<ActionResult<VendorDto>> Update(Guid id, UpdateVendorRequest request, CancellationToken cancellationToken)
    {
        var vendor = await db.Vendors.FindAsync([id], cancellationToken);
        if (vendor is null) return NotFound();

        var validation = ValidateVendor(request.Name, request.Category, request.IncidentCount);
        if (validation is not null) return BadRequest(validation);

        vendor.Name = request.Name.Trim();
        vendor.Category = request.Category.Trim();
        vendor.Status = request.Status;
        vendor.Criticality = request.Criticality;
        vendor.ComplianceStatus = request.ComplianceStatus;
        vendor.OwnerId = request.OwnerId;
        vendor.IncidentCount = request.IncidentCount;
        vendor.UpdatedAtUtc = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken);
        await audit.WriteAsync("Update", nameof(Vendor), vendor.Id, $"Vendor {vendor.Name} updated.", cancellationToken);
        return Ok(ToDto(vendor));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var vendor = await db.Vendors.FindAsync([id], cancellationToken);
        if (vendor is null) return NotFound();

        db.Vendors.Remove(vendor);
        await db.SaveChangesAsync(cancellationToken);
        await audit.WriteAsync("Delete", nameof(Vendor), id, $"Vendor {vendor.Name} deleted.", cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/risk-assessments")]
    [Authorize(Policy = "ProcurementOrAdmin")]
    public async Task<ActionResult<RiskAssessmentDto>> CalculateRisk(Guid id, [FromQuery] Guid? contractId, CancellationToken cancellationToken)
    {
        var vendor = await db.Vendors.FindAsync([id], cancellationToken);
        if (vendor is null) return NotFound();

        Contract? contract = null;
        if (contractId is not null)
        {
            contract = await db.Contracts.FirstOrDefaultAsync(x => x.Id == contractId && x.VendorId == id, cancellationToken);
            if (contract is null) return BadRequest("Contract does not belong to this vendor.");
        }

        var result = await riskScoring.CalculateAsync(vendor, contract, cancellationToken);
        vendor.RiskTier = result.Tier;

        var assessment = new RiskAssessment
        {
            VendorId = vendor.Id,
            ContractId = contract?.Id,
            Score = result.Score,
            Tier = result.Tier,
            Rationale = result.Rationale,
            CalculatedById = User.GetUserId()
        };

        db.RiskAssessments.Add(assessment);
        await db.SaveChangesAsync(cancellationToken);
        await audit.WriteAsync("RiskCalculation", nameof(Vendor), vendor.Id, result.Rationale, cancellationToken);

        return Ok(new RiskAssessmentDto(assessment.Id, assessment.VendorId, assessment.ContractId, assessment.Score, assessment.Tier, assessment.Rationale, assessment.CalculatedAtUtc));
    }

    private static string? ValidateVendor(string name, string category, int incidentCount)
    {
        if (string.IsNullOrWhiteSpace(name)) return "Vendor name is required.";
        if (string.IsNullOrWhiteSpace(category)) return "Category is required.";
        if (incidentCount < 0) return "Incident count cannot be negative.";
        return null;
    }

    private static VendorDto ToDto(Vendor vendor) =>
        new(vendor.Id, vendor.Name, vendor.Category, vendor.Status, vendor.Criticality, vendor.ComplianceStatus, vendor.OwnerId, vendor.Owner?.FullName, vendor.RiskTier, vendor.IncidentCount);
}
