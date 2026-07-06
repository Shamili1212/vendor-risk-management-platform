using VendorRisk.Api.Domain.Enums;

namespace VendorRisk.Api.Dtos;

public sealed record ContractDto(
    Guid Id,
    Guid VendorId,
    string? VendorName,
    string Title,
    DateOnly StartDate,
    DateOnly EndDate,
    DateOnly RenewalDate,
    decimal Value,
    string Currency,
    ContractStatus Status);

public sealed record CreateContractRequest(
    Guid VendorId,
    string Title,
    DateOnly StartDate,
    DateOnly EndDate,
    DateOnly RenewalDate,
    decimal Value,
    string Currency,
    ContractStatus Status);

public sealed record UpdateContractRequest(
    string Title,
    DateOnly StartDate,
    DateOnly EndDate,
    DateOnly RenewalDate,
    decimal Value,
    string Currency,
    ContractStatus Status);

public sealed record AddDocumentMetadataRequest(
    string FileName,
    string ContentType,
    string StorageUri);
