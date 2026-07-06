namespace VendorRisk.Api.Domain.Entities;

public sealed class DocumentMetadata
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ContractId { get; set; }
    public Contract? Contract { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string StorageUri { get; set; } = string.Empty;
    public Guid UploadedById { get; set; }
    public User? UploadedBy { get; set; }
    public DateTime UploadedAtUtc { get; set; } = DateTime.UtcNow;
}
