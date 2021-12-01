#nullable enable

using System;

using TGS.GameProviders.Domains;

namespace TGS.GameProviders.Core.Repositories.Models.Brand
{
    public record BrandModel(
        Guid BrandId,
        string Code,
        string Name,
        BrandStatus Status,
        string? Description,
        Guid LicenseeId);
}