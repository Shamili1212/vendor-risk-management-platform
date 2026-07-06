using Microsoft.EntityFrameworkCore;
using VendorRisk.Api.Domain.Entities;
using VendorRisk.Api.Domain.Enums;
using VendorRisk.Api.Services;

namespace VendorRisk.Api.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<DocumentMetadata> DocumentMetadata => Set<DocumentMetadata>();
    public DbSet<RiskAssessment> RiskAssessments => Set<RiskAssessment>();
    public DbSet<ApprovalRequest> ApprovalRequests => Set<ApprovalRequest>();
    public DbSet<ApprovalComment> ApprovalComments => Set<ApprovalComment>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(x => x.FullName).HasMaxLength(160).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(240).IsRequired();
            entity.Property(x => x.Role).HasConversion<string>().HasMaxLength(60);
        });

        modelBuilder.Entity<Vendor>(entity =>
        {
            entity.HasIndex(x => x.Name);
            entity.Property(x => x.Name).HasMaxLength(220).IsRequired();
            entity.Property(x => x.Category).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(60);
            entity.Property(x => x.Criticality).HasConversion<string>().HasMaxLength(60);
            entity.Property(x => x.ComplianceStatus).HasConversion<string>().HasMaxLength(60);
            entity.Property(x => x.RiskTier).HasConversion<string>().HasMaxLength(60);
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.Property(x => x.Title).HasMaxLength(220).IsRequired();
            entity.Property(x => x.Value).HasPrecision(18, 2);
            entity.Property(x => x.Currency).HasMaxLength(3).IsRequired();
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(60);
        });

        modelBuilder.Entity<DocumentMetadata>(entity =>
        {
            entity.Property(x => x.FileName).HasMaxLength(260).IsRequired();
            entity.Property(x => x.ContentType).HasMaxLength(120).IsRequired();
            entity.Property(x => x.StorageUri).IsRequired();
        });

        modelBuilder.Entity<RiskAssessment>(entity =>
        {
            entity.Property(x => x.Tier).HasConversion<string>().HasMaxLength(60);
            entity.Property(x => x.Rationale).IsRequired();
        });

        modelBuilder.Entity<ApprovalRequest>(entity =>
        {
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(60);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.Property(x => x.Type).HasConversion<string>().HasMaxLength(60);
            entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Message).IsRequired();
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.Property(x => x.Action).HasMaxLength(120).IsRequired();
            entity.Property(x => x.EntityName).HasMaxLength(120).IsRequired();
            entity.Property(x => x.IpAddress).HasMaxLength(80);
        });

        Seed(modelBuilder);
    }

    private static void Seed(ModelBuilder modelBuilder)
    {
        var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var procurementId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var reviewerId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var auditorId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var vendorId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var contractId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

        modelBuilder.Entity<User>().HasData(
            new User { Id = adminId, FullName = "Admin User", Email = "admin@demo.local", PasswordHash = DemoPasswordHasher.Hash("Admin123!"), Role = UserRole.Admin, CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new User { Id = procurementId, FullName = "Procurement Manager", Email = "procurement@demo.local", PasswordHash = DemoPasswordHasher.Hash("Procure123!"), Role = UserRole.ProcurementManager, CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new User { Id = reviewerId, FullName = "Risk Reviewer", Email = "reviewer@demo.local", PasswordHash = DemoPasswordHasher.Hash("Review123!"), Role = UserRole.Reviewer, CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new User { Id = auditorId, FullName = "Audit Reader", Email = "auditor@demo.local", PasswordHash = DemoPasswordHasher.Hash("Audit123!"), Role = UserRole.Auditor, CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) });

        modelBuilder.Entity<Vendor>().HasData(new Vendor
        {
            Id = vendorId,
            Name = "Northwind Cloud Services",
            Category = "Cloud Infrastructure",
            Status = VendorStatus.Active,
            Criticality = Criticality.Critical,
            ComplianceStatus = ComplianceStatus.ReviewRequired,
            OwnerId = procurementId,
            RiskTier = RiskTier.High,
            IncidentCount = 2,
            CreatedAtUtc = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc)
        });

        modelBuilder.Entity<Contract>().HasData(new Contract
        {
            Id = contractId,
            VendorId = vendorId,
            Title = "Enterprise Cloud Hosting Agreement",
            StartDate = new DateOnly(2026, 1, 1),
            EndDate = new DateOnly(2027, 1, 1),
            RenewalDate = new DateOnly(2026, 11, 15),
            Value = 450000m,
            Currency = "USD",
            Status = ContractStatus.Active,
            CreatedAtUtc = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}
