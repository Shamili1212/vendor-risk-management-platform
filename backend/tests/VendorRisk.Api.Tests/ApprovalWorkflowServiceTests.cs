using Microsoft.EntityFrameworkCore;
using VendorRisk.Api.Data;
using VendorRisk.Api.Domain.Entities;
using VendorRisk.Api.Domain.Enums;
using VendorRisk.Api.Services;
using Xunit;

namespace VendorRisk.Api.Tests;

public sealed class ApprovalWorkflowServiceTests
{
    [Fact]
    public async Task SubmitAsync_MovesDraftToSubmitted_AndCreatesReviewerNotification()
    {
        await using var db = CreateDb();
        var audit = new NoopAuditService();
        var service = new ApprovalWorkflowService(db, audit);
        var approval = SeedApproval(db);

        var result = await service.SubmitAsync(approval.Id, CancellationToken.None);

        Assert.Equal(ApprovalStatus.Submitted, result.Status);
        Assert.NotNull(result.SubmittedAtUtc);
        Assert.Single(db.Notifications);
    }

    [Fact]
    public async Task DecideAsync_RejectsDecision_WhenApprovalIsNotSubmitted()
    {
        await using var db = CreateDb();
        var audit = new NoopAuditService();
        var service = new ApprovalWorkflowService(db, audit);
        var approval = SeedApproval(db);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.DecideAsync(approval.Id, ApprovalStatus.Approved, "Looks good.", CancellationToken.None));
    }

    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static ApprovalRequest SeedApproval(AppDbContext db)
    {
        var requestedBy = Guid.NewGuid();
        var reviewer = Guid.NewGuid();
        var contract = new Contract
        {
            Id = Guid.NewGuid(),
            VendorId = Guid.NewGuid(),
            Title = "Test contract",
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(1),
            RenewalDate = DateOnly.FromDateTime(DateTime.UtcNow).AddMonths(10),
            Value = 1000m,
            Currency = "USD"
        };
        var approval = new ApprovalRequest
        {
            Id = Guid.NewGuid(),
            ContractId = contract.Id,
            RequestedById = requestedBy,
            AssignedReviewerId = reviewer,
            Status = ApprovalStatus.Draft
        };

        db.Contracts.Add(contract);
        db.ApprovalRequests.Add(approval);
        db.SaveChanges();
        return approval;
    }

    private sealed class NoopAuditService : IAuditService
    {
        public Task WriteAsync(string action, string entityName, Guid? entityId, string details, CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }
}
